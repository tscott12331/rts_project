using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour 
    where T : MonoBehaviourSingleton<T>
{
    private static T _instance;
    public static T Instance 
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            throw new System.Exception("An instance of this singleton already exists.");
        }
        else
        {
            _instance = this as T;
        }
    }
}
