using UnityEngine;

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
    private ObjectPool<PooledParticle> hitEffectPool;
    private ObjectPool<PooledParticle> chopEffectPool;

    public void Init()
    {
        // Initialize the particle pools for each effect
        hitEffectPool = new ObjectPool<PooledParticle>(hitEffect.prefab, hitEffect.poolSize, transform);
        chopEffectPool = new ObjectPool<PooledParticle>(chopEffect.prefab, chopEffect.poolSize, transform);

        hitEffectPool.PreWarm();
        chopEffectPool.PreWarm();
    }

    public void PlayEffect(ParticleType type, Vector3 position)
    {
        PooledParticle particle = null;
        ObjectPool<PooledParticle> pool = type switch
        {
            ParticleType.Hit => hitEffectPool,
            ParticleType.Chop => chopEffectPool,
            _ => null
        };

        if (pool == null) return;
        particle = pool.Get();
        if (particle == null) return;

        particle.transform.position = position;
        particle.Play(() => pool.Return(particle));
    }
}
