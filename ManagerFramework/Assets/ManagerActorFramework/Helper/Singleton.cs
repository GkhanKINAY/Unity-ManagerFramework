// Docs : http://wiki.unity3d.com/index.php/Singleton

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object Lock = new object();

    public static T Instance
    {
        get
        {
            if (ApplicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");

                        return _instance;
                    }


                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T);

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                            _instance.gameObject.name);

                        //---------- Trifles Games ----------//
                        // Sonradan ekledik.
                        DontDestroyOnLoad(_instance.gameObject);
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


    /* 
    //---------- Trifles Games ----------//
    // SocketIO paketi yüzünden böyle bir yama ekledik!
    private void Awake()
    {
        _instance = Instance;
    }
    */
}