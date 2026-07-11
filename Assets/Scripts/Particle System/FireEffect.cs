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

    float noiseOffset;

    void Awake()
    {
        noiseOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);

        fireLight.intensity =
            baseIntensity + (noise - 0.5f) * 2f * intensityVariation;
    }
}
