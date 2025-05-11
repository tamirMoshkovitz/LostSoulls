using Core.Input_System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Core.Managers
{
    public class GameRestart : MonoBehaviour
    {
        private InputSystem_Actions _inputActions;
        
        void Awake()
        {
            _inputActions = InputSystemBuffer.Instance.InputSystem;
            _inputActions.EndingScene.Enable();
        }
        
        private void OnEnable()
        {
            _inputActions.EndingScene.Restart.performed += OnRestartPerformed;
        }
        
        private void OnDisable()
        {
            _inputActions.EndingScene.Restart.performed -= OnRestartPerformed;
            _inputActions.EndingScene.Disable();
        }
        
        private void OnRestartPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Restarting Game");
            _inputActions.EndingScene.Disable();
            SceneManager.LoadScene("Sketch room");
        }
    }
}