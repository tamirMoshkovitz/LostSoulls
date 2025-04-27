using UnityEngine;

namespace Game_Flow.Player.Scripts
{
    public enum WalkingSurface
    {
        ConcreteFloor,
        WoodenFloor,
        WoodenStairs
    }
    
    public class PlayerAudio
    {
        private AudioSource _BGAudioSource;
        private AudioSource _StepsAudioSource;
        private AudioClip _whiteNoise;
        private AudioClip _walkingOnConcreteFloor;
        private AudioClip _walkingOnWoodenFloor;
        private AudioClip _walkingOnWoodenStairs;
        private WalkingSurface _currentSurfaceType;
        
        public PlayerAudio(AudioSource stepsAudioSource, AudioSource bgAudioSource, AudioClip whiteNoise, AudioClip walkingOnConcreteFloor, AudioClip walkingOnWoodenFloor, AudioClip walkingOnWoodenStairs)
        {
            _StepsAudioSource = stepsAudioSource;
            _BGAudioSource = bgAudioSource;
            _walkingOnWoodenFloor = walkingOnWoodenFloor;
            _walkingOnConcreteFloor = walkingOnConcreteFloor;
            _walkingOnWoodenStairs = walkingOnWoodenStairs;
            _whiteNoise = whiteNoise;
            _StepsAudioSource.loop = true;
            _currentSurfaceType = WalkingSurface.ConcreteFloor;
        }
        
        public void PlayWhiteNoise()
        {
            _BGAudioSource.clip = _whiteNoise;
            _BGAudioSource.loop = true;
            _BGAudioSource.volume = 0.1f;
            _BGAudioSource.Play();
        }
        
        public void PlayFootstepSound(WalkingSurface surfaceType)
        {
            if (_StepsAudioSource.isPlaying && _currentSurfaceType.Equals(surfaceType))
                return;

            AudioClip clipToPlay;

            switch (surfaceType)
            {
                case WalkingSurface.ConcreteFloor:
                    clipToPlay = _walkingOnConcreteFloor;
                    break;
                case WalkingSurface.WoodenFloor:
                    clipToPlay = _walkingOnWoodenFloor;
                    break;
                case WalkingSurface.WoodenStairs:
                    clipToPlay = _walkingOnWoodenStairs;
                    break;
                default:
                    Debug.LogWarning("Unknown surface type: " + surfaceType);
                    return;
            }

            if (clipToPlay != null)
            {
                _StepsAudioSource.clip = clipToPlay;
                _StepsAudioSource.Play();
            }
            _currentSurfaceType = surfaceType;
        }

        public void StopFootstepSound()
        {
            if (_StepsAudioSource.isPlaying)
            {
                _StepsAudioSource.Stop();
            }
        }
    }
}