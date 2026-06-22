using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MenuUserInterface : MonoBehaviour
{
    public Image image;
    public Sprite[] logoFrames;
    [SerializeField] private float framesPerSecond = 12f, interval = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayLogoAnim());
    }


    IEnumerator PlayLogoAnim()
    {
        float delay = 1f / framesPerSecond;

        while (true)
        {
            for (int i = 0; i < logoFrames.Length; i++)
            {
                image.sprite = logoFrames[i];
                yield return new WaitForSecondsRealtime(delay);
            }
            Debug.Log("Finished frames, waiting 10 seconds");
            yield return new WaitForSecondsRealtime(interval);
            Debug.Log("Wait finished, replaying");
        }
    }
}
