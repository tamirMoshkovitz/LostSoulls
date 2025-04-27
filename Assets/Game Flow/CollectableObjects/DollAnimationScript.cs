using UnityEngine;
using DG.Tweening;

namespace Game_Flow.CollectableObjects
{
    public class DollAnimationScript : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;
        [SerializeField] private float strength = 0.2f;
        [SerializeField] private int vibrato = 10;
        

        public void PlayAnimation()
        {
            transform.DOShakePosition(duration, strength, vibrato, 90, false, true);
        }
    }
}