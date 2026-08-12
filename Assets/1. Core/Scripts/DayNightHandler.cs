using System.Collections;
using UnityEngine;
using static DayNightSystem;

public class DayNightHandler : MonoBehaviour
{
    public Light worldLight;

    public float dayDuration = 30f, nightDuration = 30f, thresh = 5f;
    public Color dayColor = Color.white, nightColor = Color.blue;
    bool running = false;
    DayNightSystem dayNightSystem;
    public void Init(DayNightSystem dayNightSystem)
    {
        if (running) return;

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
        dayNightSystem.SetPhase(DayPhase.Day);
        for (float time = dayDuration; time > 0; time -= Time.deltaTime)
        {
            if (time <= thresh)
            {
                float t = Mathf.Clamp01(time / thresh);
                t = Mathf.SmoothStep(1f, 0f, t);
                worldLight.intensity = Mathf.Lerp(0.5f, 0.0f, t);
                worldLight.color = Color.Lerp(dayColor, nightColor, t);
                RenderSettings.ambientIntensity = Mathf.Lerp(0.3f, 0.0f, t);
            }
            yield return null;
        }
    }

    IEnumerator NightRoutine()
    {
        dayNightSystem.SetPhase(DayPhase.Night);
        for (float time = nightDuration; time > 0; time -= Time.deltaTime)
        {
            if (time <= thresh)
            {
                float t = Mathf.Clamp01(time / thresh);
                t = Mathf.SmoothStep(0f, 1f, t);
                worldLight.intensity = Mathf.Lerp(0.5f, 0.0f, t);
                worldLight.color = Color.Lerp(dayColor, nightColor, t);
                RenderSettings.ambientIntensity = Mathf.Lerp(0.3f, 0.0f, t);
            }
            yield return null;
        }
    }
}
