/*
* Developed by Gökhan KINAY.
* www.gokhankinay.com.tr
*
* Contact,
* info@gokhankinay.com.tr
*/

using UnityEngine;

namespace ManagerActorFramework
{
    public class MonoBehaviourExecutor : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        internal bool m_ShuttingDown;

        public MonoBehaviourExecutor()
        {
        }

        protected virtual void MB_Listen(bool status)
        {
        }

        protected virtual void MB_Awake()
        {
        }

        protected virtual void MB_FixedUpdate()
        {
        }

        protected virtual void MB_LateUpdate()
        {
        }

        protected virtual void MB_OnApplicationQuit()
        {
        }

        protected virtual void MB_OnDestroy()
        {
        }

        protected virtual void MB_OnDisable()
        {
        }

        protected virtual void MB_OnEnable()
        {
        }

        protected virtual void MB_Start()
        {
        }

        protected virtual void MB_Update()
        {

        }

        internal virtual void _Listen(bool status)
        {
        }

        internal virtual bool _Awake()
        {
            return true;
        }

        internal virtual void _FixedUpdate()
        {
        }

        internal virtual void _LateUpdate()
        {
        }

        internal virtual void _OnApplicationQuit()
        {
        }

        internal virtual void _OnDestroy()
        {
        }

        internal virtual void _OnDisable()
        {
        }

        internal virtual void _OnEnable()
        {
        }

        internal virtual void _Start()
        {
        }

        internal virtual void _Update()
        {
        }

        internal void Listen(bool status)
        {
            if (m_ShuttingDown)
            {
                return;
            }

            _Listen(status);
            MB_Listen(status);
        }

        private void Awake()
        {
            if (_Awake())
            {
                MB_Awake();
            }
        }

        private void FixedUpdate()
        {
            _FixedUpdate();
            MB_FixedUpdate();
        }

        private void LateUpdate()
        {
            _LateUpdate();
            MB_LateUpdate();
        }

        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;

            _OnApplicationQuit();
            MB_OnApplicationQuit();
        }

        private void OnDestroy()
        {
            _OnDestroy();
            MB_OnDestroy();
        }

        private void OnDisable()
        {
            _OnDisable();
            MB_OnDisable();
        }

        private void OnEnable()
        {
            _OnEnable();
            MB_OnEnable();
        }

        private void Start()
        {
            _Start();
            MB_Start();
        }

        private void Update()
        {
            _Update();
            MB_Update();
        }
    }
}