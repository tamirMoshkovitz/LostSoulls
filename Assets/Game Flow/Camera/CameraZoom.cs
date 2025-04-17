using UnityEngine;
using UnityEngine.InputSystem;

// allows player to zoom in the FOV when holding a button down
namespace Game_Flow.Camera
{
	[RequireComponent (typeof (UnityEngine.Camera))]
	public class CameraZoom : MonoBehaviour
	{
		[SerializeField] private float zoomFOV = 30.0f;
		[SerializeField] private float zoomSpeed = 9f;
	
		private float _targetFOV;
		private float _baseFOV;
		private UnityEngine.Camera _camera;
		private InputSystem_Actions _inputActions;

	
		
		void Awake()
		{
			_inputActions = new InputSystem_Actions();
		}

		void OnEnable()
		{
			if (_camera == null)
				_camera = GetComponent<UnityEngine.Camera>();

			_baseFOV = _camera.fieldOfView;
			_targetFOV = _baseFOV;
			_inputActions.Player.Enable();
			_inputActions.Player.Zoom.performed += OnZoomPerformed;
			_inputActions.Player.Zoom.canceled += OnZoomCanceled;
		}

		private void OnZoomPerformed(InputAction.CallbackContext context)
		{
			_targetFOV = zoomFOV;
			Debug.Log("Zoom performed");
		}

		private void OnZoomCanceled(InputAction.CallbackContext context)
		{
			_targetFOV = _baseFOV;
		}
	
		void Update()
		{
			UpdateZoom();
		}
	
		private void UpdateZoom()
		{
			_camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFOV, zoomSpeed * Time.deltaTime);
		}

		void OnDisable()
		{
			_inputActions.Player.Zoom.performed -= OnZoomPerformed;
			_inputActions.Player.Zoom.canceled -= OnZoomCanceled;
			_inputActions.Player.Disable();
		}
	}
}
