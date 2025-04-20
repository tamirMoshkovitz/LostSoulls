using UnityEngine;

namespace Core.Managers
{
    /// <summary>
    /// A generic Singleton class for MonoBehaviours.
    /// Example usage: public class GameManager : MonoSingleton<GameManager>
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
    
        private static bool _isQuitting;


        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                if (_isQuitting) 
                    return null;
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    var singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject); // Don't destroy the object when loading a new scene
                }

                return _instance;
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        // Ensure no other instances can be created by having the constructor as protected
        protected MonoSingleton() { }
    }
}