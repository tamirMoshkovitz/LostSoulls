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
            ChangeParticleSize();
            ChangeParticleVelocity();
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

        public void ChangeParticleSize()
        {
            Debug.Log("Change Particle Size");
            var sizeOverLifetime = dollParticles.sizeOverLifetime;
            sizeOverLifetime.enabled = true;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.5f);     // Start at half size
            curve.AddKey(0.1f, 0.8f);     // Quick initial growth
            curve.AddKey(0.2f, 1.2f);     // Continue growing
            curve.AddKey(0.3f, 2.0f);     // Accelerate growth
            curve.AddKey(0.4f, 3.0f);     // Keep growing
            curve.AddKey(0.5f, 4.0f);     // Half way point
            curve.AddKey(1.0f, 5.0f);    // End at 5x size

            // Make the curve smoother
            for (int i = 0; i < curve.length; i++)
            {
                curve.SmoothTangents(i, 0.5f);
            }

            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
        }

        public void ChangeParticleVelocity()
        {
            Debug.Log("Change Particle Velocity");
            var velocityOverLifetime = dollParticles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;

            // Create curves for all velocity components
            AnimationCurve curveX = new AnimationCurve();
            curveX.AddKey(0.0f, 0.0f);    // No X velocity
            curveX.AddKey(1.0f, 0.0f);

            AnimationCurve curveY = new AnimationCurve();
            curveY.AddKey(0.0f, 0.0f);    // No Y velocity
            curveY.AddKey(1.0f, 8.0f);

            AnimationCurve curveZ = new AnimationCurve();
            curveZ.AddKey(0.0f, 0.0f);   // -2 Z velocity
            curveZ.AddKey(1.0f, 0.0f);

            // Apply the curves to all velocity components
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(1.0f, curveX);
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(1.0f, curveY);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(1.0f, curveZ);
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
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