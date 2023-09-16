using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidController : MonoBehaviour
{
    public bool enableSkid;
    public bool enableParticles;

    float particleEmissionRate = 0;

    CarController car;
    TrailRenderer trail;
    ParticleSystem particles;
    ParticleSystem.EmissionModule emission;

    // Start is called before the first frame update
    void Awake()
    {
        car = GetComponentInParent<CarController>();

        trail = GetComponent<TrailRenderer>();
        particles = GetComponent<ParticleSystem>();
        emission = particles.emission;

        trail.emitting = false;
        emission.rateOverTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableSkid)
            trail.emitting = car.IsTireSkid(out float lateralVelocity, out bool isBraking);

        if (enableParticles)
        {
            bool doParticles = car.IsTireSkid(out float lateralVelocity, out bool isBraking);

            particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5.0f);
            emission.rateOverTime = particleEmissionRate;

            particleEmissionRate = (doParticles) ? (isBraking) ? 30 : Mathf.Abs(lateralVelocity * 2) : particleEmissionRate;
        }
    }
}
