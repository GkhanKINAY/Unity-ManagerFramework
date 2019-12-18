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
using Object = UnityEngine.Object;

namespace ManagerActorFramework
{
    public abstract class Actor<TManager> : MonoBehaviourExecutor where TManager : Manager<TManager>
    {
        internal bool _IsRegistered;

        public static TManager Manager
        {
            get;
            private set;
        }

        internal override bool _Awake()
        {
            if (Manager == null)
            {
                Manager = CoreManager._Instance._GetManager<TManager>();
            }
            if (Manager == null)
            {
                Object.Destroy(this);
                return false;
            }
            TManager manager = Manager;
            manager._RegisterActor(this);

            Listen(true);
            CoreManager.Log(LogLevel.Info, "Listen status: True");

            return true;
        }

        internal override void _OnDestroy()
        {
            if (_IsRegistered && Manager<TManager>.Instance != null)
            {
                TManager manager = Manager;
                manager._UnregisterActor(this);
            }
        }

        protected void Destroy()
        {
            CoreManager.Log(LogLevel.Warning, "Actor destroying...");
            _Destroy();
        }

        internal void _Destroy()
        {
            Listen(false);
            CoreManager.Log(LogLevel.Info, "Listen status: False");

            Object.Destroy(this);
        }

        protected bool Push(ManagerEvents managerEvent, params object[] arguments)
        {
            TManager manager = Manager;

            bool published = manager._OnPull(this, managerEvent, arguments);
            CoreManager.Log(LogLevel.Info, $"Push status: {published} , ManagerEvent: \"{managerEvent}\"", arguments);
            return published;
        }
    }
}