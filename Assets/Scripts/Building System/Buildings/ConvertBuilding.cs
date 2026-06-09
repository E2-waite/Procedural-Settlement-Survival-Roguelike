using System.Collections;
using UnityEngine;

public class ConvertBuilding : Building
{
    [SerializeField] public float convertTime = 5f; // Seconds
    [SerializeField] private int maxCapacity = 3;
    protected int activeConversions = 0;
    private CommandSystem commandSystem;
    protected virtual IUnitRole Role => null;
    public virtual bool SameType(IUnitRole unitRole) => false;

    public override void Init(GameContext context) 
    {
        commandSystem = context.commandSystem;
    }

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
        commandSystem.StopCommanding(unit);
        unit.gameObject.SetActive(false);
        yield return new WaitForSeconds(convertTime);
        unit.SetRole(Role.New());
        unit.gameObject.SetActive(true);
        unit.SetState(Unit.State.Idle);
        activeConversions--;
    }
}
