using System;
using System.Collections;
using Core.Audio;
using Game_Flow.ImpactObjects.Scripts.Audio;
using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.UI;
using UnityEngine;

namespace Game_Flow.CollectableObjects
{
    public class CollectableKeyObject : MonoBehaviour
    {
        [SerializeField] private OpenCloseImpactObject showcaseDoor1;
        [SerializeField] private OpenCloseImpactObject showcaseDoor2;
        [SerializeField] private ItemsUpdater itemsUpdater;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;
        
        private OpenCloseObjectAudio _objectAudio;

        private void Start()
        {
            _objectAudio = new OpenCloseObjectAudio(audioSource, audioClip);
        }

        public void OnCollect()
        {
            showcaseDoor1.IsLocked = false;
            showcaseDoor2.IsLocked = false;
            itemsUpdater.AddItem(2);
            _objectAudio.PlaySound();
            StartCoroutine(DelayDestroy());
        }
        
        private IEnumerator DelayDestroy()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}