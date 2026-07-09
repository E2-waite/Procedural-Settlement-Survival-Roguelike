using UnityEngine;

[CreateAssetMenu]
public class ParticleEffect : ScriptableObject
{
    public PooledParticle prefab;
    public int poolSize = 10;
}
