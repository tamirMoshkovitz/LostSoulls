using System;
using UnityEngine;

namespace Core.Managers
{
    public class EventManager : MonoBehaviour
    {
        
        public static EventManager Instance {get; private set;}
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public static event Action<bool> OnLockStateChanged;

        public static void LockStateChanged(bool isLocked)
        {
            OnLockStateChanged?.Invoke(isLocked);
        }
    }
}