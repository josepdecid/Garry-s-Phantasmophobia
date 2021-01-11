using UnityEngine;
 
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;
 
    // Access singleton instance through this propriety.
    public static T Instance {
        get { 
            lock (m_Lock) {
                if (m_Instance == null) {

                    // Search for existing instance.
                    m_Instance = (T)FindObjectOfType(typeof(T));
                    
                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null) {
                        
                        // Need to create a new GameObject to attach the singleton to.
                        GameObject singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
 
                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                if (m_ShuttingDown) {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed. Returning new instance.");
                }
 
                return m_Instance;
            }
        }
    }
 
    private void OnApplicationQuit() {
        m_ShuttingDown = true;
        m_Instance = null;
    }
 
 
    private void OnDestroy() {
        m_ShuttingDown = true;
        m_Instance = null;
    }
}