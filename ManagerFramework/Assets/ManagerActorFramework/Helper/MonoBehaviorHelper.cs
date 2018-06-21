using UnityEngine;

public class MonoBehaviorHelper<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object Lock = new object();

    public static T Instance
    {
        get
        {
            if (ApplicationIsQuitting)
            {
                return null;
            }

            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[MonoBehaviorHelper] Something went really wrong " +
                            " - there should never be more than 1 same script!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }
                }

                return _instance;
            }
        }
    }

    public static bool ApplicationIsQuitting { get; set; }

    protected virtual void OnDestroy()
    {
        ApplicationIsQuitting = true;
    }
}