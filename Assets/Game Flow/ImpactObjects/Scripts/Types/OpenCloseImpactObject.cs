using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OpenCloseImpactObject : MonoBehaviour
    {
        private bool _isOpen;
        private Animator _animator;
        
        public bool IsOpen => _isOpen;

        void Start()
        {
            _isOpen = false;
            _animator = gameObject.GetComponent<Animator>();
        }

        public void OpenImpactObject()
        {
            _isOpen = true;
            _animator.SetBool("IsOpen", true);
            Debug.Log("OpenImpactObject");
        }
        
        public void CloseImpactObject()
        {
            _isOpen = false;
            _animator.SetBool("IsOpen", false);
            Debug.Log("CloseImpactObject");
        }
    }
}