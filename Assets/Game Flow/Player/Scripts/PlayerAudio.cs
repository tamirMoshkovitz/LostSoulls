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
        private AudioSource _audioSource;
        private AudioClip _walkingOnConcreteFloor;
        private AudioClip _walkingOnWoodenFloor;
        private AudioClip _walkingOnWoodenStairs;
        private WalkingSurface _currentSurfaceType;
        
        public PlayerAudio(AudioSource audioSource, AudioClip walkingOnConcreteFloor, AudioClip walkingOnWoodenFloor, AudioClip walkingOnWoodenStairs)
        {
            _audioSource = audioSource;
            _walkingOnWoodenFloor = walkingOnWoodenFloor;
            _walkingOnConcreteFloor = walkingOnConcreteFloor;
            _walkingOnWoodenStairs = walkingOnWoodenStairs;
            _audioSource.loop = true;
            _currentSurfaceType = WalkingSurface.ConcreteFloor;
        }
        
        public void PlayFootstepSound(WalkingSurface surfaceType)
        {
            if (_audioSource.isPlaying && _currentSurfaceType.Equals(surfaceType))
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
                _audioSource.clip = clipToPlay;
                _audioSource.Play();
            }
            _currentSurfaceType = surfaceType;
        }

        public void StopFootstepSound()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}