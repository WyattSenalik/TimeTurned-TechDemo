using UnityEngine;
// Original Authors - Wyatt Senalik

/// <summary>
/// Very similar to <see cref="SingletonMonoBehaviour{T}"/> but
/// will dynamically create itself if there is not one in the scene already.
/// </summary>
public class DynamicSingletonMonoBehaviour<T> : MonoBehaviour where T :
    DynamicSingletonMonoBehaviour<T>
{
    /// <summary>
    /// Reference to the current instance of this class.
    /// </summary>
    public static T instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject temp_singletonObj = new GameObject(typeof(T).Name);
                m_instance = temp_singletonObj.AddComponent<T>();
            }
            return m_instance;
        }
    }
    private static T m_instance = null;


    // Domestic initialization
    protected virtual void Awake()
    {
        // Set the instance to this unless it already exists.
        if (m_instance != null)
        {
            Debug.Log($"Multiple {GetType().Name} exist in the scene. " +
                $"Destroying the other");
            Destroy(gameObject);
            return;
        }

        m_instance = this as T;
    }
}
