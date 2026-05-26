using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoSingleton<BuildPanel>
{
    public Transform contentTransform;
    public GameObject buttonPrefab;
    public List<BuildingButton> buildingButtons = new List<BuildingButton>();
    public float slideSpeed = 10f;
    RectTransform rect;
    bool visible = false;
    Coroutine moveRoutine;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void UpdateDisplay(List<BuildingObject> buildings)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingObject building = buildings[i];

            GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);

            BuildingButton button = buttonObj.GetComponent<BuildingButton>();

            buildingButtons.Add(button);

            button.Setup(building, i);
        }
    }

    public void ToggleVisibility()
    {
        visible = !visible;

        if (moveRoutine != null)
           StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(ShowRoutine(visible));
    }

    IEnumerator ShowRoutine(bool show)
    {
        Vector2 target;
        if (show)
            target = new Vector2(0, 0);
        else
            target = new Vector2(rect.sizeDelta.x, 0);

        while (rect.anchoredPosition != target)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target, slideSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
