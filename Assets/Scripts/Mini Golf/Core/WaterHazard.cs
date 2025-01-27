using gameracers.MiniGolf.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHazard : MonoBehaviour
{
    [SerializeField] static float punishmentDelay = 3f;
    [SerializeField] GameObject waterHazard;

    float timer = Mathf.Infinity;
    MiniGolfPlayerController pc;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            waterHazard.SetActive(true);
            timer = Time.time;
            pc = collision.transform.GetComponent<MiniGolfPlayerController>();
            // play spash sound
        }
    }

    private void Update()
    {
        if (pc != null)
        {
            if (Time.time - timer > punishmentDelay)
            {
                pc.AddSwing(true);
                pc = null;
                waterHazard.SetActive(false);
            }
        }
    }
}