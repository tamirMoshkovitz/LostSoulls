using Game_Flow.ImpactObjects.Scripts.Decorator_Interface;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;
using Unity.VisualScripting;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Types
{
    public class OpenCloseImpactObject : ImpactObjectDecorator
    {
        private bool _isOpen;
        private Animator _animator;

        public OpenCloseImpactObject(IImpactObject inner, MonoImpactObject mono, ImpactObjectStats stats, bool isOpen) : base(inner, mono, stats)
        {
            _isOpen = isOpen;
            _animator = mono.GetComponent<Animator>();
        }

        public void OpenImpactObject()
        {
            _isOpen = true;
            _animator.SetBool("IsOpen", true);
        }
        
        public void CloseImpactObject()
        {
            _isOpen = false;
            _animator.SetBool("IsOpen", false);
        }
    }
}