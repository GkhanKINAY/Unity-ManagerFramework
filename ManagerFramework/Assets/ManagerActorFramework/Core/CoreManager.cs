using UnityEngine;
using System.Collections.Generic;
using System;

namespace ManagerActorFramework
{
    [ExecutionOrder(-32767)]
    public sealed class CoreManager : Singleton<CoreManager>
    {
        private List<Type> _Managers;

        private void Awake()
        {
            _Managers = new List<Type>();
        }

        internal void RegisterManager<TManager>(TManager manager) where TManager : Manager<TManager>
        {
            _Managers.Add(typeof(TManager));
        }

        internal void UnregisterManager<TManager>(TManager manager) where TManager : Manager<TManager>
        {
            _Managers.Remove(typeof(TManager));
        }
    }


}