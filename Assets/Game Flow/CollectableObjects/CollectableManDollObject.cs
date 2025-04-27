using System;
using System.Collections;
using Game_Flow.UI;
using UnityEngine;

namespace Game_Flow.CollectableObjects
{
    public class CollectableManDollObject : MonoBehaviour
    {
        [Header("Assign a disabled DollAnimationScript in the scene")]
        [SerializeField] private DollAnimationScript dollAnimationScript;
        [SerializeField] private ItemsUpdater itemsUpdater;

        /// <summary>
        /// Call this when the player collects.
        /// </summary>
        public void OnCollect(Action callback)
        {
            StartCoroutine(MakeAnimationOfDoll(callback));
        }

        private IEnumerator MakeAnimationOfDoll(Action callback)
        {
            // 1) Freeze time (and thus the camera)
            Time.timeScale = 0f;

            // 2) Clear UI icons
            itemsUpdater.ClearAll();

            // 3) Activate the existing doll object and start its animation
            dollAnimationScript.gameObject.SetActive(true);
            dollAnimationScript.PlayAnimation(); 

            // 4) Wait 2 real-time seconds (ignores timeScale)
            yield return new WaitForSecondsRealtime(2f);

            // 5) Unfreeze time
            Time.timeScale = 1f;

            // 6) Invoke the callback for getting to top down state
            callback?.Invoke();
        }
    }
}