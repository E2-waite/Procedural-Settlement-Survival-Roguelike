using System.Collections;
using UnityEngine;

public class PooledParticle : MonoBehaviour
{
    // Method to play the particle system and invoke a callback when it finishes
    public void Play(System.Action onComplete)
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
            StartCoroutine(WaitForCompletion(particleSystem, onComplete));
        }
    }

    // Coroutine to wait for the particle system to finish playing
    IEnumerator WaitForCompletion(ParticleSystem particleSystem, System.Action onComplete)
    {
        if (particleSystem == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        yield return new WaitUntil(() => !particleSystem.IsAlive(true));
        onComplete?.Invoke();
    }
}
