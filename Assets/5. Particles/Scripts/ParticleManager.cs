using UnityEngine;
using UnityEngine.UIElements;

public enum ParticleType
{
    Hit,
    Chop
}

// Class to manage particle effects using object pooling
public class ParticleManager : MonoBehaviour
{
    [Header("Particle Effect Objects")]
    public ParticleEffect hitEffect;
    public ParticleEffect chopEffect;
    private ObjectPool<Particle> hitEffectPool;
    private ObjectPool<Particle> chopEffectPool;

    public void Init()
    {
        // Initialize the particle pools for each effect
        hitEffectPool = new ObjectPool<Particle>(hitEffect.prefab, hitEffect.poolSize, transform, hitEffect.offset);
        chopEffectPool = new ObjectPool<Particle>(chopEffect.prefab, chopEffect.poolSize, transform, hitEffect.offset);

        hitEffectPool.PreWarm();
        chopEffectPool.PreWarm();
    }

    void GetParticle(ParticleType type, ref Particle particle, ref ObjectPool<Particle> pool)
    {
        pool = type switch
        {
            ParticleType.Hit => hitEffectPool,
            ParticleType.Chop => chopEffectPool,
            _ => null
        };

        if (pool == null) return;
        particle = pool.Get();
        if (particle == null) return;
    }

    public void PlayEffect(ParticleType type, Vector3 position)
    {
        Particle particle = null;
        ObjectPool<Particle> pool = null;
        GetParticle(type, ref particle, ref pool);

        if (pool == null || particle == null) return;

        particle.transform.position = position + pool.Offset;
        particle.Play(() => pool.Return(particle));
    }

    public void PlayHitEffect(ParticleType type, Vector3 position, float damage)
    {
        Particle particle = null;
        ObjectPool<Particle> pool = null;
        GetParticle(type, ref particle, ref pool);
        if (pool == null || particle == null) return;

        int numParticles = Mathf.RoundToInt(damage * 0.1f + 1);
        numParticles = Random.Range(numParticles - 1, numParticles + 1);

        particle.transform.position = position + pool.Offset;
        particle.Emit(() => pool.Return(particle), numParticles);
    }
}
