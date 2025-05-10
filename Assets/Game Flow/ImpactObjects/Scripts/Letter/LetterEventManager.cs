using System.Collections;
using System.Linq;
using Core.Managers;
using Game_Flow.ImpactObjects.Scripts.Types;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

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
        
        private Image image;
        
        private void Awake()
        {
            image = letterContent.GetComponent<Image>();
        }
        
        
        public override void OpenImpactObject()
        {
            // turn off the collider
            GetComponent<BoxCollider>().enabled = false;
            
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

        public IEnumerator ExitLetterState()
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

            yield return new WaitForSeconds(2f);

            letterContent.SetActive(false);
            EventManager.ExitPainting();
        }

        public void ShowLetterContentAndPlaySound()
        {
            letterContent.SetActive(true);
            StartCoroutine(FadeCanvasImage(1f, 1.5f));
            StartCoroutine(PlayNarrator());
        }

        private IEnumerator PlayNarrator()
        {
            yield return new WaitForSeconds(2f);
            PlaySound();
        }

        public IEnumerator HideLetterContent(float readingDuration)
        {
            var clips = pictureAnimator.runtimeAnimatorController.animationClips;
            var paintingClip = clips.FirstOrDefault(c => c.name == "Picture Sprite Animation");
            float pictureAnimationLength = paintingClip != null ? paintingClip.length : 0f;
            yield return new WaitForSeconds(readingDuration - pictureAnimationLength);
            StartCoroutine(FadeCanvasImage(0f, 1f));
            PlayPaintingAnimation();
            StartCoroutine(ExitLetterState());
        }

        private IEnumerator FadeCanvasImage(float targetAlpha, float duration)
        {
            float startAlpha = image.color.a;
            float time = 0f;

            Color startColor = image.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                image.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            image.color = endColor; // Ensure exact target at end
        }
    }
}