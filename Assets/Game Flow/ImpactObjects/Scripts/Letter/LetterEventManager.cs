using System.Collections;
using Core.Managers;
using Game_Flow.ImpactObjects.Scripts.Types;
using NUnit.Framework;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Letter
{
    [RequireComponent(typeof(BoxCollider))]
    public class LetterEventManager: OpenCloseImpactObject
    {
        private static readonly int Play = Animator.StringToHash("Play");

        [SerializeField] private OpenCloseImpactObject letterTop;
        [SerializeField] private OpenCloseImpactObject letterBottom;
        [SerializeField] private GameObject letterContent;
        [SerializeField] private Animator pictureAnimator;
        public override void OpenImpactObject()
        {
            //show painting
            SwitchToPaintingCamera();
            
            // open letter
            letterTop.OpenImpactObject();
            // letter bottom opens with a animator event
            // letter content shows with a animator event
            // letter content hides with a animator event
            
            // play painting animation
            
            // switch back to first person with a animator event
        }

        public void OpenLetterBottom()
        {
            letterBottom.OpenImpactObject();
        }
        
        public void PlayPaintingAnimation()
        {
            pictureAnimator.SetTrigger(Play);
        }
        
        public void StopLetterAudio()
        {
            letterTop.StopSound();
        }

        private void SwitchToPaintingCamera()
        {
            EventManager.ShowPainting();
        }

        public IEnumerator ExitPaintingCamera()
        {
            // Wait until the Animator has transitioned to the desired state
            while (!pictureAnimator.GetCurrentAnimatorStateInfo(0).IsName("Picture Sprite Animation"))
            {
                yield return null;
            }

            // Wait until the animation has finished playing
            while (pictureAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            EventManager.ExitPainting();
        }

        public void ShowLetterContentAndPlaySound()
        {
            letterContent.SetActive(true);
            this.PlaySound();
        }

        public void HideLetterContent()
        {
            letterContent.SetActive(false);
        }
    }
}