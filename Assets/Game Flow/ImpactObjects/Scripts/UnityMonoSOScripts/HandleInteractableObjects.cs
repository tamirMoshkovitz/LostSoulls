using System;
using Game_Flow.ImpactObjects.Scripts.Types;
using UnityEngine;

namespace Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts
{
    public class HandleInteractableObjects : MonoBehaviour
    {
        [SerializeField] private OpenCloseImpactObject[] animatedObjects;
        
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
    }
}