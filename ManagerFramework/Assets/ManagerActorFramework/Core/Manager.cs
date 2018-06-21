using UnityEngine;
using System.Collections.Generic;
using System;

namespace ManagerActorFramework
{
    [ExecutionOrder(-32766)]
    public abstract class Manager<TManager> : Singleton<TManager> where TManager : Manager<TManager>
    {
        //---------- Variables ----------//
        #region Variables

        private List<Actor<TManager>> _Actors;
        private Dictionary<ManagerEvents, List<Action<object[]>>> _Subscriptions;

        #endregion

        protected virtual void Awake()
        {
            Init();
        }

        protected void Init()
        {
            _Actors = new List<Actor<TManager>>();
            _Subscriptions = new Dictionary<ManagerEvents, List<Action<object[]>>>();

            CoreManager.Instance.RegisterManager(Instance);
        }

        internal void RegisterActor<TActor>(TActor actor) where TActor : Actor<TManager>
        {
            _Actors.Add(actor);
        }

        internal void UnregisterActor<TActor>(TActor actor) where TActor : Actor<TManager>
        {
            _Actors.Remove(actor);
        }

        protected void DestroyActor(Actor<TManager> actor)
        {
            actor.Destroy();
        }

        protected virtual void Destroy()
        {
            foreach (var actor in _Actors)
            {
                actor.Destroy();
            }

            CoreManager.Instance.UnregisterManager(Instance);
            Destroy(gameObject);
        }

        public List<Actor<TManager>> GetActors<TActor>() where TActor : Actor<TManager>
        {
            return _Actors;
        }

        internal virtual bool OnPull(ManagerEvents managerEvent, object[] arguments)
        {
            return true;
        }

        internal void Publish(ManagerEvents managerEvent, params object[] arguments)
        {
            if (!_Subscriptions.ContainsKey(managerEvent))
            {
                return;
            }

            foreach (var subs in _Subscriptions[managerEvent])
            {
                subs(arguments);
            }
        }

        public void Subscribe(ManagerEvents managerEvent, Action<object[]> subscription)
        {
            if (_Subscriptions.ContainsKey(managerEvent))
            {
                _Subscriptions[managerEvent].Add(subscription);
            }
            else
            {
                _Subscriptions.Add(managerEvent, new List<Action<object[]>> { subscription });
            }
        }

        public void Unsubscribe(ManagerEvents managerEvent, Action<object[]> subscription)
        {
            if (!_Subscriptions.ContainsKey(managerEvent))
            {
                return;
            }

            if (!_Subscriptions[managerEvent].Contains(subscription))
            {
                return;
            }

            _Subscriptions[managerEvent].Remove(subscription);
        }

    }
}