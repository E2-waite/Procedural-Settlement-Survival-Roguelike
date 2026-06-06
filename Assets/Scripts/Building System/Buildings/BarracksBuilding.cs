using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BarracksBuilding : Building
{
    public GameObject fighterPrefab;
    [SerializeField] public float convertTime = 5f; // Seconds
    [SerializeField] private int maxCapacity = 3;

    private int activeConversions = 0;
    private FollowerSystem followerSystem;
    public void Init(FollowerSystem followerSystem)
    {
        this.followerSystem = followerSystem;
    }

    public bool Convert(WorkerUnit worker)
    {
        if (activeConversions < maxCapacity)
        {
            activeConversions++;
            StartCoroutine(ConvertWorker(worker));
            return true;
        }

        return false;
    }

    IEnumerator ConvertWorker(WorkerUnit worker)
    {
        // Disable worker
        followerSystem.RemoveUnit(worker);
        worker.gameObject.SetActive(false);
        worker.chunk.RemoveUnit(worker);
        yield return new WaitForSeconds(convertTime);

        if (worker == null)
        {
            activeConversions--;
            yield break;
        }
        else
        {
            GameObject fighterObj = Instantiate(fighterPrefab, transform.position, Quaternion.identity);
            FighterUnit newFighter = fighterObj.GetComponent<FighterUnit>();

            if (newFighter != null)
            {
                newFighter.Init(worker);
            }

            followerSystem.AddUnit(newFighter);
            Destroy(worker.gameObject);
            activeConversions--;
        }
    }
}
