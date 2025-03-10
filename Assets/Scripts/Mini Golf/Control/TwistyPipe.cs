using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.MiniGolf.Control
{
    public class TwistyPipe : MonoBehaviour
    {
        [SerializeField] float dur;
        [SerializeField] float power;
        [SerializeField] Vector3 ejectDir;

        Vector3 ejectPos;

        MiniGolfPlayerController pc;

        float timer;

        void Start()
        {
            ejectPos = transform.GetChild(0).position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                timer = Time.time;
                pc = other.transform.GetComponent<MiniGolfPlayerController>();
                pc.FreezeCharacter(true);
                pc.transform.DOMove(ejectPos, dur);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (pc != null)
            {
                if (Time.time - timer > dur)
                {
                    pc.FreezeCharacter(false);
                    pc.transform.GetComponent<Rigidbody>().AddForce(ejectDir * power);
                    pc = null;
                }
            }
        }
    }
}