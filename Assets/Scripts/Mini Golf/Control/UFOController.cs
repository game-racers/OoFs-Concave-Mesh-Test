using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.MiniGolf.Control
{
    public class UFOController : MonoBehaviour
    {
        Transform player;
        Transform alien;

        [SerializeField] float ufoRotSpeed = 5f;
        [SerializeField] Transform moveSpots;

        int health = 3;
        [SerializeField] float moveDur = 2.5f;
        [SerializeField] ParticleSystem smoke;
        [SerializeField] AudioSource thudSFX;

        void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            alien = transform.Find("Alien");

            transform.Find("UFO").DOLocalRotate(new Vector3(0, 360, 0), ufoRotSpeed, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
            MoveToPos();
        }

        void Update()
        {
            // rotate alien head to look at player

            alien.LookAt(player.position, Vector3.up);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.tag == "Player")
            {
                health -= 1;
                MoveToPos();
                thudSFX.Play();
            }
        }

        private void MoveToPos()
        {
            var emission = smoke.emission;
            emission.rateOverTime = 3 - health;

            if (health == 0)
            {
                transform.DOLocalMove(Vector3.up * 2f, moveDur).SetEase(Ease.OutQuad);
                player.GetComponent<MiniGolfPlayerController>().StopInput(true);

                return;
            }

            Debug.Log(moveSpots.GetChild(3 - health).name);
            transform.DOMove(moveSpots.GetChild(3 - health).position, moveDur).SetEase(Ease.OutQuad);
        }
    }
}
