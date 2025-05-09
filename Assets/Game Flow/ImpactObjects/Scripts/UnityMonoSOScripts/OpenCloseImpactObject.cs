﻿using DG.Tweening;
using Game_Flow.ImpactObjects.Scripts.Audio;
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
        [SerializeField] private bool isLetter;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Color highlightColor;
        [SerializeField] private float width;
        [SerializeField] private float scale;
        
        [Header("Audio")]
        [SerializeField] private AudioSource objectAudioSource;
        [SerializeField] private AudioClip audioClip;
        
        private bool _isOpen;
        private Animator _animator;
        private OpenCloseObjectAudio _objectAudio;
        
        public bool IsLocked { get; set; }
        
        public bool IsLetter => isLetter;
        
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
            
            _objectAudio = new OpenCloseObjectAudio(objectAudioSource, audioClip);
        }

        public void OpenImpactObject()
        {
            _isOpen = true;
            _animator.SetTrigger("IsOpen");
            //_animator.SetBool("IsOpen", true);
            Debug.Log("OpenImpactObject");
            _objectAudio.PlaySound();
        }
        
        public void CloseImpactObject()
        {
            _isOpen = false;
            _animator.SetTrigger("IsClose");
            //_animator.SetBool("IsOpen", false);
            Debug.Log("CloseImpactObject");
            _objectAudio.PlaySound();
        }
        
        public void PlayLockedAnimation()
        {
            transform.DOShakePosition(duration, strength, vibrato, 90, false, true);
        }

        public void HighlightObject()
        {
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                var material = renderer.material;
                if (material == null) continue;
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 1);
                }
                material.EnableKeyword("DR_OUTLINE_ON");
                if (material.HasProperty("_OutlineColor"))
                {
                    material.SetColor("_OutlineColor", highlightColor);
                }
                if (material.HasProperty("_OutlineWidth"))
                {
                    material.SetFloat("_OutlineWidth", width);
                }
                if (material.HasProperty("_OutlineScale"))
                {
                    material.SetFloat("_OutlineScale", scale);
                }
            }
            
        }
        
        public void UnHighlightObject()
        {
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                var material = renderer.material;
                if (material == null) continue;
                if (material.HasProperty("_OutlineEnabled"))
                {
                    material.SetInt("_OutlineEnabled", 0);
                }
                material.DisableKeyword("DR_OUTLINE_ON");
            }
        }

        private void StopSound()
        {
            _objectAudio.StopSound();
        }
    }
}