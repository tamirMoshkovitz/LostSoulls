using System.Collections;
using Core.Audio;
using Game_Flow.ImpactObjects.Scripts.Audio;
using UnityEngine;

namespace Game_Flow.CollectableObjects
{
    public class SwitchObject:MonoBehaviour
    {
        [SerializeField] private GameObject objectToSwitch;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clip;
        
        private OpenCloseObjectAudio _objectAudio;
        
        private void Start()
        {
            _objectAudio = new OpenCloseObjectAudio(audioSource, clip);
            _objectAudio.SetVolume(0.3f);
        }
        public void ControlLights()
        {
            objectToSwitch.SetActive(!objectToSwitch.activeSelf);
            StartCoroutine(PlaySound());
        }

        private IEnumerator PlaySound()
        {
            _objectAudio.PlaySound();
            yield return new WaitForSeconds(.5f);
            _objectAudio.StopSound();
        }
    }
}