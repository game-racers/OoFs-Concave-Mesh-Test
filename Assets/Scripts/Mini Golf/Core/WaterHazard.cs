using gameracers.MiniGolf.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHazard : MonoBehaviour
{
    [SerializeField] float punishmentDelay = 3f;
    [SerializeField] GameObject waterHazard;
    [SerializeField] LayerMask badMask;

    float timer = Mathf.Infinity;
    MiniGolfPlayerController pc;

    private void OnCollisionEnter(Collision collision)
    {
        if ((badMask & (1 << gameObject.layer)) != 0)
        {
            if (collision.gameObject.tag == "Player")
            {
                pc = collision.transform.GetComponent<MiniGolfPlayerController>();
                if (pc.GetHazard()) return;

                pc.SetHazard();
                waterHazard.SetActive(true);
                timer = Time.time;
                pc = collision.transform.GetComponent<MiniGolfPlayerController>();

                AudioManager.am.WaterHazardSound(collision.transform.position);
            }
        }
    }

    private void Update()
    {
        if (pc != null)
        {
            if (Time.time - timer > punishmentDelay)
            {
                pc.AddSwing(true);
                pc.ResetHazard();
                pc = null;
                waterHazard.SetActive(false);
            }
        }
    }
}