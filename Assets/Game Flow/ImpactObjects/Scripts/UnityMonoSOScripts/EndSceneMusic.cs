using Core.Audio;
using Game_Flow.ImpactObjects.Scripts.Audio;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class EndSceneMusic: MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;
        private MoovingObjectAudio _objectAudio;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _objectAudio =  new MoovingObjectAudio(audioSource, audioClip);
        }


        public void PlayMusic()
        {
            _objectAudio.PlaySound();
        }

        public void StopMusic()
        {
            _objectAudio.StopSound();
        }
        
    }
}