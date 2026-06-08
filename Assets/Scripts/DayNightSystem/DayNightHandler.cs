using System.Collections;
using UnityEngine;
using static DayNightSystem;

public class DayNightHandler : MonoBehaviour
{
    public Light worldLight;

    public float dayDuration = 30f, nightDuration = 30f;
    bool running = false;
    DayNightSystem dayNightSystem;

    public void Init(DayNightSystem dayNightSystem)
    {
        this.dayNightSystem = dayNightSystem;
        running = true;
        StartCoroutine(CycleRoutine());
    }

    IEnumerator CycleRoutine()
    {
        while (running)
        {
            yield return DayRoutine();
            yield return NightRoutine();
        }
    }

    IEnumerator DayRoutine()
    {
        dayNightSystem.SetPhase(Phase.Day);
        worldLight.intensity = .5f;
        RenderSettings.ambientIntensity = 0.3f;
        for (float time = dayDuration; time > 0; time -= Time.deltaTime)
        {
            Debug.Log("Day left: " + time);
            yield return null;
        }
    }

    IEnumerator NightRoutine()
    {
        dayNightSystem.SetPhase(Phase.Night);
        worldLight.intensity = .0f;
        RenderSettings.ambientIntensity = 0;
        for (float time = nightDuration; time > 0; time -= Time.deltaTime)
        {
            Debug.Log("Night left: " + time);

            yield return null;
        }
    }
}
