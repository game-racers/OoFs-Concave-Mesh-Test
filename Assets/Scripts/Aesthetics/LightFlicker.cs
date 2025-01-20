using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] float flickerIntensity = .2f;
    [SerializeField] float flickersPerSecond = 3.0f;
    [SerializeField] float speedRandomness = 1f;

    float time;
    float startingIntensity;
    Light lightBulb;

    void Awake() 
    {
        lightBulb = GetComponent<Light>();
        startingIntensity = lightBulb.intensity;
    }

    void Update() 
    {
        time += Time.deltaTime * (1 - Random.Range(-speedRandomness, speedRandomness)) * Mathf.PI;
        lightBulb.intensity = startingIntensity + Mathf.Sin(time * flickersPerSecond) * flickerIntensity;
    }
}
