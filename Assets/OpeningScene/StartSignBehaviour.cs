using DG.Tweening;
using UnityEngine;

namespace OpeningScene
{
    public class StartSignBehaviour : MonoBehaviour
    {
        public void OnStartPressed()
        {
            Transform signTransform = gameObject.transform;
            Vector3 originalPos = signTransform.position;
            Sequence fallSequence = DOTween.Sequence();
            fallSequence.Append(signTransform.DOMoveY(originalPos.y - 2f, 1.5f)
                .SetEase(Ease.InQuad));
            signTransform.DORotate(new Vector3(
                    signTransform.eulerAngles.x,
                    signTransform.eulerAngles.y,
                    signTransform.eulerAngles.z + UnityEngine.Random.Range(-45f, 45f)
                ), 1.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad);
            fallSequence.Append(signTransform.DOShakeRotation(0.5f, new Vector3(0, 0, 10f), 10, 90f));
            fallSequence.OnComplete(() =>
            {
                Destroy(gameObject);
            });
            fallSequence.Play();
        }
    }
}