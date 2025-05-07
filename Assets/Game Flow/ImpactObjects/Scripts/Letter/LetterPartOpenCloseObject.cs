using System.Collections;
using Game_Flow.ImpactObjects.Scripts.Types;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.Letter
{
    public class LetterPartOpenCloseObject: OpenCloseImpactObject
    {
        [Header("Letter Part")]
        [SerializeField] private LetterEventManager letterMain;
        
        private void OpenLetterBottom()
        {
            letterMain.OpenLetterBottom();
        }

        private void StopLetterAudio()
        {
            letterMain.StopLetterAudio();
        }
        
        private void ShowLetterContentAndPlaySound()
        {
            letterMain.ShowLetterContentAndPlaySound();
            float letterDuration = letterMain.GetComponent<AudioSource>().clip.length;
            StartCoroutine(HideLetterContent(letterDuration));
        }
        
        private IEnumerator HideLetterContent(float duration)
        {
            yield return new WaitForSeconds(duration);
            letterMain.HideLetterContent();
            letterMain.PlayPaintingAnimation();
            StartCoroutine(letterMain.ExitPaintingCamera());
        }
    }
}