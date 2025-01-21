using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Aesthetics
{
    public class NPCBobble : MonoBehaviour
    {
        [SerializeField] float clothingBob = 5f;
        [SerializeField] float duration = 3f;

        void Start()
        {
            transform.DORotate(Vector3.left * clothingBob, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }
}