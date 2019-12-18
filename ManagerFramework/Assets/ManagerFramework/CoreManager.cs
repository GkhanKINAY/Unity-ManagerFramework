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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ManagerActorFramework
{
    [ExecutionOrder(-32767)]
    public sealed class CoreManager : MonoBehaviour
    {
        private static CoreManager _instance;

        private static FileStream _logFile;

        private Dictionary<Type, object> _managers = new Dictionary<Type, object>();

        public static bool IsInitialized
        {
            [CompilerGenerated]
            get
            {
                return _instance != null;
            }
        }

        public static string LogFile
        {
            get;
            private set;
        }

        internal static CoreManager _Instance
        {
            get
            {
                //IL_0048: Unknown result type (might be due to invalid IL or missing references)
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = Object.FindObjectOfType<CoreManager>();
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = new GameObject("[CoreManager]").AddComponent<CoreManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                Object.DontDestroyOnLoad(this);
            }
            else if (_instance != this)
            {
                Object.Destroy(this);
                return;
            }

            string filePath = Path.Combine(Application.persistentDataPath, "Logs");
            filePath = Path.Combine(filePath, _GetDateTimeString() + ".md");

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
           
            _logFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            byte[] bytes = Encoding.UTF8.GetBytes($"# {_GetDateTimeString()}{Environment.NewLine}");
            _logFile.Write(bytes, 0, bytes.Length);
            _logFile.Flush();
            Log(LogLevel.Info, "Initialized.");
        }

        private void OnDestroy()
        {
            if (!(_instance != this))
            {
                Log(LogLevel.Info, "Shutdown.");
                _logFile.Close();
            }
        }

        internal TManager _GetManager<TManager>() where TManager : Manager<TManager>
        {
            Type typeFromHandle = typeof(TManager);
            if (!_managers.ContainsKey(typeFromHandle))
            {
                return (TManager)null;
            }
            return _managers[typeFromHandle] as TManager;
        }

        internal bool _RegisterManager<TManager>(TManager manager) where TManager : Manager<TManager>
        {
            Type typeFromHandle = typeof(TManager);
            if (_managers.ContainsKey(typeFromHandle))
            {
                return false;
            }
            _managers[typeFromHandle] = manager;
            manager._IsRegistered = true;
            return true;
        }

        internal void _UnregisterManager<TManager>(TManager manager) where TManager : Manager<TManager>
        {
            Type typeFromHandle = typeof(TManager);
            if (_managers.ContainsKey(typeFromHandle))
            {
                _managers.Remove(typeFromHandle);
                manager._IsRegistered = false;
                manager._Destroy();
            }
        }

        private static string _GetDateTimeString()
        {
            return "D" + DateTime.Now.ToString("s").Replace(":", ".");
        }

        public static void Log(LogLevel level, string message, object argument = null)
        {
            MethodBase method = new StackFrame(1).GetMethod();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"### {level}: {message}").Append(Environment.NewLine);
            stringBuilder.Append($"From `({method.MemberType}) {method.DeclaringType}.{method.Name}` at _{_GetDateTimeString()}_").Append(Environment.NewLine);
            if (argument != null)
            {
                stringBuilder.Append(string.Format("```{1}{0}{1}```{1}", argument, Environment.NewLine));
            }
            stringBuilder.Append("***").Append(Environment.NewLine);
            string text = stringBuilder.ToString();
            switch (level)
            {
                default:
                    Debug.Log((object)text);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning((object)text);
                    break;
                case LogLevel.Error:
                case LogLevel.Exception:
                    Debug.LogError((object)text);
                    break;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            _logFile.Write(bytes, 0, bytes.Length);
            _logFile.Flush();
        }
    }
}