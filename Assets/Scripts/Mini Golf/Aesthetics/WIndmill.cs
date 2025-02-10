using DG.Tweening;
using UnityEngine;

namespace gameracers.MiniGolf.Aesthetics
{
    public class WIndmill : MonoBehaviour
    {
        [SerializeField] bool isVert = false;
        [SerializeField] float dur = 5f;

        void Start()
        {
            if (isVert == false)
                transform.DOLocalRotate(Vector3.forward * 360, dur, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            else
                transform.DOLocalRotate(Vector3.up * 360, dur, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }
    }
}