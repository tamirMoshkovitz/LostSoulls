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
        [SerializeField] private GameObject ceiling;
        [SerializeField] private ParticleSystem dollParticles;

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
            EventManager.OnDollPlaced += ToggleViewByDoll;
            EventManager.OnPlayerZoneChanged += HandleZoneChange;
        }

        private void OnDisable()
        {
            _inputActions.Player.SwitchView.performed -= ToggleView;
            EventManager.OnPlayerZoneChanged -= HandleZoneChange;
            EventManager.OnDollPlaced -= ToggleViewByDoll;
            _inputActions.Player.Disable();
        }

        private void ToggleView(InputAction.CallbackContext ctx)
        {
            if (!_canSwitchView || _cinemachineBrain.IsBlending) return;
            StartCoroutine(DelayedToggleView());
        }

        private void ToggleViewByDoll()
        {
            StartCoroutine(DelayedToggleView());
        }

        private IEnumerator DelayedToggleView()
        {
            ChangeColorToRed();
            yield return new WaitForSeconds(2f);
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
                StartCoroutine(SwitchToPerspective());
            }
        }
        
        private IEnumerator SwitchToOrtho()
        {
            PlayerController.Instance.IsMovementLocked = true;
            yield return new WaitForSeconds(_cinemachineBrain.DefaultBlend.BlendTime * .18f);
            ceiling.gameObject.SetActive(false);
            ObjectController.Instance.ChangeState(new TopDownState());
            
        }
        
        private IEnumerator SwitchToPerspective()
        {
            mainCamera.fieldOfView = 60f;
            ObjectController.Instance.ChangeState(new FPState());
            yield return new WaitForSeconds(_cinemachineBrain.DefaultBlend.BlendTime * .82f);
            ceiling.gameObject.SetActive(true);
        }
        
        public void ChangeColorToRed()
        {
            Debug.Log("Change Color to Red");
            var colorOverLifetime = dollParticles.colorOverLifetime;
            colorOverLifetime.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0.0f),
                    new GradientColorKey(Color.red, 1.0f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 1.0f)
                }
            );

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
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