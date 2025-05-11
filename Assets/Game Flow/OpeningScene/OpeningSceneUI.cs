using System.Collections;
using Core.Managers;
using UnityEngine;

namespace Game_Flow.OpeningScene
{
    public class OpeningSceneUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup parentCanvasGroup;
        [SerializeField] private CanvasGroup childCanvasGroup;
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float fadeOutDuration = .5f;
        [SerializeField] private float delayBeforeFirstFade = 2f;
        [SerializeField] private float delayBetweenFades = 0.5f;

        private void Start()
        {
            parentCanvasGroup.gameObject.SetActive(true);
            parentCanvasGroup.alpha = 0;
            childCanvasGroup.alpha = 0;

            StartCoroutine(FadeInSequence());
        }

        private void OnEnable()
        {
            EventManager.EnterRoom += DisableCanvas;
        }

        private void OnDisable()
        {
            EventManager.EnterRoom -= DisableCanvas;
        }

        private IEnumerator FadeInSequence()
        {
            yield return new WaitForSeconds(delayBeforeFirstFade);
            yield return StartCoroutine(FadeCanvasGroup(parentCanvasGroup, 0f, 1f, fadeInDuration));
            yield return new WaitForSeconds(delayBetweenFades);
            yield return StartCoroutine(FadeCanvasGroup(childCanvasGroup, 0f, 1f, fadeInDuration));
        }

        private IEnumerator FadeOutSequence()
        {
            yield return StartCoroutine(FadeCanvasGroup(childCanvasGroup, 1f, 0f, fadeOutDuration));
            yield return new WaitForSeconds(delayBetweenFades);
            yield return StartCoroutine(FadeCanvasGroup(parentCanvasGroup, 1f, 0f, fadeOutDuration));
            parentCanvasGroup.gameObject.SetActive(false);
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            cg.alpha = to;
        }

        private void DisableCanvas()
        {
            StartCoroutine(FadeOutSequence());
        }
    }
}