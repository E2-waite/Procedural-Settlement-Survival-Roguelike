using System.Collections;
using UnityEngine;

public class Particle : MonoBehaviour, IPoolable
{
    ParticleSystem ps;
    public void Init()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Method to play the particle system and invoke a callback when it finishes
    public void Play(System.Action onComplete)
    {
        if (ps != null)
        {
            ps.Play();
            StartCoroutine(WaitForCompletion(ps, onComplete));
        }
    }

    // Method to play the particle system and invoke a callback when it finishes
    public void Emit(System.Action onComplete, int count)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Emit(count);
            StartCoroutine(WaitForCompletion(ps, onComplete));
        }
    }

    // Coroutine to wait for the particle system to finish playing
    IEnumerator WaitForCompletion(ParticleSystem ps, System.Action onComplete)
    {
        if (ps == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        yield return new WaitUntil(() => !ps.IsAlive(true));
        onComplete?.Invoke();
    }

    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnReturn()
    { 
        gameObject.SetActive(false); 
    }
}
