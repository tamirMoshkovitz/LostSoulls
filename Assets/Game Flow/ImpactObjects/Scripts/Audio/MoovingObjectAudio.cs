using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Audio
{
    public class MoovingObjectAudio
    {
        private AudioSource _objectAudioSource;
        private AudioClip _sound;

        public MoovingObjectAudio(AudioSource objectAudioSource, AudioClip draggingSound)
        {
            _objectAudioSource = objectAudioSource;
            _sound = draggingSound;
            _objectAudioSource.clip = _sound;
            _objectAudioSource.loop = true;
        }
        
        public void PlaySound()
        {
            if (!_objectAudioSource.isPlaying)
            {
                _objectAudioSource.Play();
            }
        }
        
        public void StopSound()
        {
            if (_objectAudioSource.isPlaying)
            {
                _objectAudioSource.Stop();
            }
        }
    }
}