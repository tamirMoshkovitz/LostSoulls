using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game_Flow.Camera
{
    public enum ViewMode { FirstPerson, TopDown }
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera firstPersonCamera;
        [SerializeField] private UnityEngine.Camera topDownCamera;

        private InputSystem_Actions inputActions;
        
        private bool isTopDown = false;
        private bool canSwitchView = false;
        private ViewMode currentViewMode = ViewMode.FirstPerson;

        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.SwitchView.performed += ToggleView;
            EventManager.OnPlayerZoneChanged += HandleZoneChange;
        }

        private void OnDisable()
        {
            inputActions.Player.SwitchView.performed -= ToggleView;
            EventManager.OnPlayerZoneChanged -= HandleZoneChange;
            inputActions.Player.Disable();
        }

        private void ToggleView(InputAction.CallbackContext ctx)
        {
            if (!canSwitchView) return; 
            isTopDown = !isTopDown;
            firstPersonCamera.gameObject.SetActive(!isTopDown);
            topDownCamera.gameObject.SetActive(isTopDown);
            currentViewMode = isTopDown ? ViewMode.TopDown : ViewMode.FirstPerson;
            EventManager.ViewModeChanged(currentViewMode);
        }
        
        private void HandleZoneChange(bool canSwitch)
        {
            canSwitchView = canSwitch;
            if (!canSwitchView && isTopDown)
            {
                isTopDown = false;
                firstPersonCamera.gameObject.SetActive(true);
                topDownCamera.gameObject.SetActive(false);
                currentViewMode = ViewMode.FirstPerson;
            }
        }
    }
}