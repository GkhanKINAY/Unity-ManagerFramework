/*
* TRIFLES GAMES
* www.triflesgames.com
*
* Developed by Gökhan KINAY.
* www.gokhankinay.com.tr
*
* Contact,
* info@triflesgames.com
* info@gokhankinay.com.tr
*/

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ManagerActorFramework
{
    [ExecutionOrder(-32766)]
    public abstract class Manager<TManager> : MonoBehaviourExecutor where TManager : Manager<TManager>
    {
        internal bool _IsRegistered;

        internal bool _IsSingletonManager = true;

        private List<Actor<TManager>> _sActors = new List<Actor<TManager>>();

        private Dictionary<Type, List<object>> _actors = new Dictionary<Type, List<object>>();

        private Dictionary<Type, object[]> _actorsCache = new Dictionary<Type, object[]>();

        private Dictionary<ManagerEvents, List<Subscription<TManager>>> _lowSubscriptions = new Dictionary<ManagerEvents, List<Subscription<TManager>>>();

        private Dictionary<ManagerEvents, Subscription<TManager>[]> _lowSubscriptionsCache = new Dictionary<ManagerEvents, Subscription<TManager>[]>();

        private Dictionary<ManagerEvents, List<Subscription<TManager>>> _mediumSubscriptions = new Dictionary<ManagerEvents, List<Subscription<TManager>>>();

        private Dictionary<ManagerEvents, Subscription<TManager>[]> _mediumSubscriptionsCache = new Dictionary<ManagerEvents, Subscription<TManager>[]>();

        private Dictionary<ManagerEvents, List<Subscription<TManager>>> _highSubscriptions = new Dictionary<ManagerEvents, List<Subscription<TManager>>>();

        private Dictionary<ManagerEvents, Subscription<TManager>[]> _highSubscriptionsCache = new Dictionary<ManagerEvents, Subscription<TManager>[]>();

        public static TManager Instance
        {
            get;
            private set;
        }

        public Actor<TManager>[] Actors
        {
            get;
            private set;
        }

        internal override bool _Awake()
        {
            TManager val = this as TManager;
            if (val == null)
            {
                Object.Destroy(this);
                return false;
            }
            if (!CoreManager._Instance._RegisterManager(val))
            {
                Object.Destroy(this);
                return false;
            }
            Instance = val;
            Object.DontDestroyOnLoad(Instance);
            return true;
        }

        internal override void _OnDestroy()
        {
            if (_IsRegistered && CoreManager.IsInitialized)
            {
                CoreManager._Instance._UnregisterManager(Instance);
                Instance = (TManager)null;
            }
        }

        internal override void _Start()
        {
            Listen(true);
            CoreManager.Log(LogLevel.Info, "Listen status: True");
        }

        internal override void _Listen(bool status)
        {
            if (status)
            {
                SceneManager.activeSceneChanged += ChangedActiveScene;
            }
            else
            {
                SceneManager.activeSceneChanged -= ChangedActiveScene;
            }
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            if (_IsSingletonManager)
            {
                return;
            }

            CoreManager.Log(LogLevel.Warning, "_IsSingletonManager is false!");
            Destroy();
        }

        protected void Destroy()
        {
            CoreManager.Log(LogLevel.Warning, "Manager destroying...");
            _Destroy();
        }

        internal void _Destroy()
        {
            Listen(false);
            CoreManager.Log(LogLevel.Info, "Listen status: False");

            Object.Destroy(this);
        }

        protected void DestroyActor(Actor<TManager> actor)
        {
            actor._Destroy();
        }

        public TActor[] GetActors<TActor>() where TActor : Actor<TManager>
        {
            Type typeFromHandle = typeof(TActor);
            return _actors.ContainsKey(typeFromHandle) ? Array.ConvertAll(_actorsCache[typeFromHandle], (object x) => x as TActor) : new TActor[0];
        }

        internal void _RegisterActor<TActor>(TActor actor) where TActor : Actor<TManager>
        {
            Type type = ((object)actor).GetType();
            if (!_actors.ContainsKey(type))
            {
                _actors.Add(type, new List<object>());
            }
            if (!_actorsCache.ContainsKey(type))
            {
                _actorsCache.Add(type, null);
            }
            _actors[type].Add(actor);
            _actorsCache[type] = _actors[type].ToArray();
            _sActors.Add(actor);
            Actors = _sActors.ToArray();
            actor._IsRegistered = true;

            CoreManager.Log(LogLevel.Info, $"{type} Registered!");
        }

        internal void _UnregisterActor<TActor>(TActor actor) where TActor : Actor<TManager>
        {
            Type type = ((object)actor).GetType();
            if (!_actors.ContainsKey(type))
            {
                return;
            }
            _actors[type].Remove(actor);
            _actorsCache[type] = _actors[type].ToArray();
            _sActors.Remove(actor);
            Actors = _sActors.ToArray();
            actor._IsRegistered = false;
            actor._Destroy();

            CoreManager.Log(LogLevel.Info, $"{type} Unregistered!");
        }

        public void Subscribe(ManagerEvents managerEvent, Subscription<TManager> subscription, Priority priority = Priority.Medium)
        {
            switch (priority)
            {
                default:
                    if (!_mediumSubscriptions.ContainsKey(managerEvent))
                    {
                        _mediumSubscriptions[managerEvent] = new List<Subscription<TManager>>();
                    }
                    _mediumSubscriptions[managerEvent].Add(subscription);
                    _mediumSubscriptionsCache[managerEvent] = _mediumSubscriptions[managerEvent].ToArray();
                    break;
                case Priority.Low:
                    if (!_lowSubscriptions.ContainsKey(managerEvent))
                    {
                        _lowSubscriptions[managerEvent] = new List<Subscription<TManager>>();
                    }
                    _lowSubscriptions[managerEvent].Add(subscription);
                    _lowSubscriptionsCache[managerEvent] = _lowSubscriptions[managerEvent].ToArray();
                    break;
                case Priority.High:
                    if (!_highSubscriptions.ContainsKey(managerEvent))
                    {
                        _highSubscriptions[managerEvent] = new List<Subscription<TManager>>();
                    }
                    _highSubscriptions[managerEvent].Add(subscription);
                    _highSubscriptionsCache[managerEvent] = _highSubscriptions[managerEvent].ToArray();
                    break;
            }

            CoreManager.Log(LogLevel.Info, $"{subscription.Target} Subscribed! ManagerEvent: \"{managerEvent}\" Priority: {priority.ToString()}");
        }

        public void Unsubscribe(ManagerEvents managerEvent, Subscription<TManager> subscription, Priority priority = Priority.Medium)
        {
            switch (priority)
            {
                default:
                    if (_mediumSubscriptions.ContainsKey(managerEvent))
                    {
                        _mediumSubscriptions[managerEvent].Remove(subscription);
                        _mediumSubscriptionsCache[managerEvent] = _mediumSubscriptions[managerEvent].ToArray();
                    }
                    break;
                case Priority.Low:
                    if (_lowSubscriptions.ContainsKey(managerEvent))
                    {
                        _lowSubscriptions[managerEvent].Remove(subscription);
                        _lowSubscriptionsCache[managerEvent] = _lowSubscriptions[managerEvent].ToArray();
                    }
                    break;
                case Priority.High:
                    if (_highSubscriptions.ContainsKey(managerEvent))
                    {
                        _highSubscriptions[managerEvent].Remove(subscription);
                        _highSubscriptionsCache[managerEvent] = _highSubscriptions[managerEvent].ToArray();
                    }
                    break;
            }

            CoreManager.Log(LogLevel.Info, $"{subscription.Target} Unsubscribed! ManagerEvent: \"{ managerEvent}\" Priority: {priority.ToString()}");
        }

        protected void Publish(ManagerEvents managerEvent, params object[] arguments)
        {
            _Publish(managerEvent, arguments);
        }

        protected virtual bool OnPull(Actor<TManager> actor, ManagerEvents managerEvent, object[] arguments)
        {
            _Publish(managerEvent, arguments);
            return true;
        }

        internal bool _OnPull(Actor<TManager> actor, ManagerEvents managerEvent, object[] arguments)
        {
            return OnPull(actor, managerEvent, arguments);
        }

        private void _Publish(ManagerEvents managerEvent, object[] arguments)
        {
            if (_highSubscriptionsCache.ContainsKey(managerEvent))
            {
                Subscription<TManager>[] array = _highSubscriptionsCache[managerEvent];
                foreach (Subscription<TManager> subscription in array)
                {
                    subscription(arguments);
                }
            }
            if (_mediumSubscriptionsCache.ContainsKey(managerEvent))
            {
                Subscription<TManager>[] array2 = _mediumSubscriptionsCache[managerEvent];
                foreach (Subscription<TManager> subscription2 in array2)
                {
                    subscription2(arguments);
                }
            }
            if (_lowSubscriptionsCache.ContainsKey(managerEvent))
            {
                Subscription<TManager>[] array3 = _lowSubscriptionsCache[managerEvent];
                foreach (Subscription<TManager> subscription3 in array3)
                {
                    subscription3(arguments);
                }
            }

            CoreManager.Log(LogLevel.Info, $"Published! ManagerEvent: \"{managerEvent}\"", arguments);
        }
    }
}
