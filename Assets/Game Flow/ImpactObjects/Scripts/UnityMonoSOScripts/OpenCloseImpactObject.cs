using DG.Tweening;
using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OpenCloseImpactObject : MonoBehaviour
    {
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private float strength = 0.2f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private bool isShowcase;
        
        private bool _isOpen;
        private Animator _animator;
        
        public bool IsLocked { get; set; }
        
        public bool IsOpen => _isOpen;

        void Start()
        {
            _isOpen = false;
            _animator = gameObject.GetComponent<Animator>();
            if (isShowcase)
            {
                IsLocked = true;
            }
            else
            {
                IsLocked = false;
            }
        }

        public void OpenImpactObject()
        {
            _isOpen = true;
            _animator.SetTrigger("IsOpen");
            //_animator.SetBool("IsOpen", true);
            Debug.Log("OpenImpactObject");
        }
        
        public void CloseImpactObject()
        {
            _isOpen = false;
            _animator.SetTrigger("IsClose");
            //_animator.SetBool("IsOpen", false);
            Debug.Log("CloseImpactObject");
        }
        
        public void PlayLockedAnimation()
        {
            transform.DOShakePosition(duration, strength, vibrato, 90, false, true);
        }
    }
}