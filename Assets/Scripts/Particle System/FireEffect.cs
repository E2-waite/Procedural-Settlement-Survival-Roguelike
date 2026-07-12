using System.Collections;
using UnityEngine;

public class FireEffect : MonoBehaviour
{
    public Light fireLight;
    public ParticleSystem flameParticle;
    public ParticleSystem sparkParticle;
    public ParticleSystem smokeParticle;


    float intensity = 1f;

    [Header("Intensity")]
    [SerializeField] float baseIntensity = 1.5f;
    [SerializeField] float intensityVariation = 0.3f;

    [Header("Flicker")]
    [SerializeField] float flickerSpeed = 5f;
    private float lightIntensity = 0;
    float noiseOffset;
    float fireLevel = 0;
    float smokeDuration = 10f;

    void Awake()
    {
        noiseOffset = Random.Range(0f, 1000f);
        lightIntensity = baseIntensity;
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);

        fireLight.intensity =
            lightIntensity + (noise - 0.5f) * 2f * intensityVariation;
    }

    public void UpdateEffect(float level, float max)
    {
        fireLevel = max > 0f? Mathf.Clamp01(level / max) : 0f;

        ParticleSystem.MainModule main = flameParticle.main;
        ParticleSystem.EmissionModule flameEmission = flameParticle.emission;

        main.startSize = Mathf.Lerp(0, 1, fireLevel);
        main.startSpeed = Mathf.Lerp(0, .1f, fireLevel);
        main.startLifetime = Mathf.Lerp(0, 1, fireLevel);

        flameEmission.rateOverTime = Mathf.Lerp(0, 100, fireLevel);
        lightIntensity = Mathf.Lerp(0, baseIntensity, fireLevel);

        ParticleSystem.EmissionModule sparkEmission = sparkParticle.emission;
        ParticleSystem.MainModule sparkMain = sparkParticle.main;

        sparkMain.startLifetime = Mathf.Lerp(0, 1, fireLevel);
        sparkEmission.rateOverTime = Mathf.Lerp(0, 4, fireLevel);

        ParticleSystem.EmissionModule smokeEmission = smokeParticle.emission;
        smokeEmission.rateOverTime = Mathf.Lerp(20, 2, fireLevel);
    }

    public void Extinguish()
    {
        StartCoroutine(ExtinguishRoutine());
    }

    IEnumerator ExtinguishRoutine()
    {
        float elapsed = 0f;

        ParticleSystem.EmissionModule smokeEmission = smokeParticle.emission;
        float startRate = smokeEmission.rateOverTime.constant;

        while (elapsed < smokeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / smokeDuration);
            smokeEmission.rateOverTime = Mathf.Lerp(startRate, 0, progress);
            yield return null;
        }

        smokeEmission.rateOverTime = 0;
    }
}
