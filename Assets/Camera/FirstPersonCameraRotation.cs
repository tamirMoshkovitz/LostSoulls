using UnityEngine;

namespace Camera
{
    public class FirstPersonCameraRotation : MonoBehaviour
    {
        [SerializeField] private Transform playerBody;
        [SerializeField] private float sensitivity = 2f;

        private InputSystem_Actions _inputActions;
        private Vector2 _lookInput;
        private float _xRotation = 0f;

        void Awake()
        {
            _inputActions = new InputSystem_Actions();
        }

        void OnEnable()
        {
            _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
            _inputActions.Enable();
        }

        void OnDisable()
        {
            _inputActions.Disable();
        }

        void Update()
        {
            float mouseX = _lookInput.x * sensitivity * Time.deltaTime;
            float mouseY = _lookInput.y * sensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}