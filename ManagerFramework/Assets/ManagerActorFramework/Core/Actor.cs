using UnityEngine;

namespace ManagerActorFramework
{
    public abstract class Actor<TManager> : MonoBehaviorHelper<TManager> where TManager : Manager<TManager>
    {
        public static TManager Manager
        {
            get { return Instance; }
        }

        protected virtual void Awake()
        {
            Init();
        }

        protected void Init()
        {
            if (Manager == null)
            {
                Debug.LogError("*" + typeof(TManager) + "* -> Bulunamadığı için sahipsiz actor yok edildi!");
                Destroy(gameObject);
                return;
            }

            Manager.RegisterActor(this);
        }

        internal void Destroy()
        {
            Manager.UnregisterActor(this);
            Destroy(gameObject);
        }

        protected void Push(ManagerEvents managerEvent, params object[] arguments)
        {
            bool canPublish = Manager.OnPull(managerEvent, arguments);
            if (canPublish)
            {
                Manager.Publish(managerEvent, arguments);
            }
        }
    }
}
