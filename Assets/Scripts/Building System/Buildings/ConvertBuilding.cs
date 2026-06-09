using System.Collections;
using UnityEngine;

public class ConvertBuilding : Building
{
    [SerializeField] public float convertTime = 5f; // Seconds
    [SerializeField] private int maxCapacity = 3;
    protected int activeConversions = 0;

    protected virtual IUnitRole Role => null;
    public virtual bool SameType(IUnitRole unitRole) => false;

    public virtual void Convert(Unit unit)
    {
        if (Role == null) return;

        if (activeConversions < maxCapacity)
        {
            activeConversions++;
            StartCoroutine(ConvertWorker(unit));
        }
    }
    protected virtual IEnumerator ConvertWorker(Unit unit)
    {
        Debug.Log("Converting unit");
        if (unit == null)
        {
            activeConversions--;
            yield break;
        }

        // Disable worker
        unit.gameObject.SetActive(false);
        yield return new WaitForSeconds(convertTime);
        unit.SetRole(Role.New());
        unit.gameObject.SetActive(true);
        unit.SetState(Unit.State.None);
        activeConversions--;
    }
}
