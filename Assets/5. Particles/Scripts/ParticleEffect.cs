using UnityEngine;

[CreateAssetMenu]
public class ParticleEffect : ScriptableObject
{
    public Particle prefab;
    public int poolSize = 10;
    public Vector3 offset = Vector3.zero;
}
