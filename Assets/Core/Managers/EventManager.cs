using System;
using Game_Flow.Camera;
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
        
        public static event Action<bool> OnPlayerZoneChanged;

        public static void TriggerPlayerZoneChanged(bool canSwitchView)
        {
            OnPlayerZoneChanged?.Invoke(canSwitchView);
        }
        
        public static event Action<ViewMode> OnViewModeChanged;

        public static void ViewModeChanged(ViewMode newMode)
        {
            OnViewModeChanged?.Invoke(newMode);
        }
    }
}