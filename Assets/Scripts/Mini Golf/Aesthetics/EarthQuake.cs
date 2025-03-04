using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.MiniGolf.Aesthetics
{
    public class EarthQuake : MonoBehaviour
    {
        [SerializeField] float speed = 5f;
        [SerializeField] float intensity = 0.1f;
        [SerializeField] float slowDownDur = 1f;

        void Update()
        {
            transform.GetChild(0).localPosition = intensity * new Vector3(
                Mathf.PerlinNoise(speed * Time.time, 1),
                Mathf.PerlinNoise(speed * Time.time, 2),
                Mathf.PerlinNoise(speed * Time.time, 3));
        }

        public void StopQuake()
        {
            DOTween.To(() => intensity, x => intensity = x, 0f, slowDownDur);
        }
    }
}
