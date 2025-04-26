using Core.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
using Game_Flow.DotVisual.Scripts;
using Game_Flow.DotVisual.Scripts.States;
using Game_Flow.Player.Scripts;
using WaitForSeconds = UnityEngine.WaitForSeconds;

namespace Game_Flow.Camera
{
    public enum ViewMode { FirstPerson, TopDown }
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField] private CinemachineCamera topDownCamera;
        [SerializeField] private UnityEngine.Camera mainCamera;

        private InputSystem_Actions _inputActions;
        
        private bool _isTopDown = false;
        private bool _canSwitchView = false;
        private ViewMode _currentViewMode = ViewMode.FirstPerson;
        private CinemachineBrain _cinemachineBrain;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            _cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            _inputActions.Player.SwitchView.performed += ToggleView;
            EventManager.OnPlayerZoneChanged += HandleZoneChange;
        }

        private void OnDisable()
        {
            _inputActions.Player.SwitchView.performed -= ToggleView;
            EventManager.OnPlayerZoneChanged -= HandleZoneChange;
            _inputActions.Player.Disable();
        }

        private void ToggleView(InputAction.CallbackContext ctx)
        {
            if (!_canSwitchView || _cinemachineBrain.IsBlending) return; 
            _isTopDown = !_isTopDown;
            firstPersonCamera.gameObject.SetActive(!_isTopDown);
            topDownCamera.gameObject.SetActive(_isTopDown);
            ChangeViewMode();
            EventManager.ViewModeChanged(_currentViewMode);
        }

        private void ChangeViewMode()
        {
            if (_isTopDown)
            {
                _currentViewMode = ViewMode.TopDown;
                StartCoroutine(SwitchToOrtho());
            }
            else
            {
                SwitchToPerspective();
            }
        }
        
        private IEnumerator SwitchToOrtho()
        {
            PlayerController.Instance.IsMovementLocked = true;
            yield return new WaitForSeconds(_cinemachineBrain.DefaultBlend.BlendTime * 9/10);
            mainCamera.orthographic = true;
            topDownCamera.Lens.OrthographicSize = 2.75f;
            mainCamera.orthographicSize = 2.5f;
            ObjectController.Instance.ChangeState(new TopDownState());
        }
        
        private void SwitchToPerspective()
        {
            mainCamera.orthographic = false;
            mainCamera.fieldOfView = 60f;
            ObjectController.Instance.ChangeState(new FPState());
        }
        
        private void HandleZoneChange(bool canSwitch)
        {
            _canSwitchView = canSwitch;
            if (!_canSwitchView && _isTopDown)
            {
                _isTopDown = false;
                firstPersonCamera.gameObject.SetActive(true);
                topDownCamera.gameObject.SetActive(false);
                _currentViewMode = ViewMode.FirstPerson;
            }
        }
    }
}