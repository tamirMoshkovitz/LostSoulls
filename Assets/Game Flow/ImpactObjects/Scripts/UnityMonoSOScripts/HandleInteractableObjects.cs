using System;
using Game_Flow.ImpactObjects.Scripts.Types;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class HandleInteractableObjects : MonoBehaviour
    {
        [SerializeField] private OpenCloseImpactObject[] animatedObjects;

        public bool InHighlightZone { get; private set; }
        public bool InLetterZone { get; private set; }

        public void CloseAllOpenedObjects()
        {
            foreach (var animatedObject in animatedObjects)
            {
                var openable = animatedObject.GetComponentInChildren<OpenCloseImpactObject>();
                if (openable != null && openable.IsOpen)
                {
                    openable.CloseImpactObject();
                    openable.UnHighlightObject();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HighlightZone"))
            {
                InHighlightZone = true;
                Debug.Log("HighlightZone");
                foreach (var openable in animatedObjects)
                {
                    if (openable != null && !openable.IsLetter)
                    {
                        Debug.Log(openable.name);
                        openable.HighlightObject();
                    }
                }
            }
            else if (other.CompareTag("LetterZone"))
            {
                InLetterZone = true;
                Debug.Log("LetterZone");
                foreach (var openable in animatedObjects)
                {
                    if (openable != null && openable.IsLetter)
                    {
                        Debug.Log(openable.name);
                        openable.HighlightObject();
                    }
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("HighlightZone"))
            {
                InHighlightZone = false;
                Debug.Log("Exit HighlightZone");
                foreach (var openable in animatedObjects)
                {
                    if (openable != null && !openable.IsLetter)
                    {
                        openable.UnHighlightObject();
                    }
                }
            }
            else if (other.CompareTag("LetterZone"))
            {
                InLetterZone = false;
                Debug.Log("Exit LetterZone");
                foreach (var openable in animatedObjects)
                {
                    if (openable != null && openable.IsLetter)
                    {
                        openable.UnHighlightObject();
                    }
                }
            }
        }
    }
}