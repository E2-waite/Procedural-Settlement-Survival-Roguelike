using UnityEngine;

[System.Serializable]
public class UnitCombat
{
    private Unit unit;

    public float attackDist = 1f, attackDamage = 10f;
    protected float attackInterval = 0.5f, attackTimer = 0;
    public  Unit target;

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    public void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public void SetTarget(Unit unit)
    {
        target = unit;
    }

    public Vector2Int TargetPos()
    {
        return target.GridPos();
    }

    public bool HasTarget()
    {
        return target != null;
    }

    public bool InRange()
    {
        float dist = Vector3.Distance(unit.transform.position, target.transform.position);

        return dist < attackDist;
    }

    public bool AttackTarget()
    {
        if (target == null) return false;
        if (attackTimer > 0) return false;

        float dist = Vector3.Distance(unit.transform.position, target.transform.position);

        if (dist < attackDist)
        {
            attackTimer = attackInterval;
            if (target.Hit(attackDamage, unit))
            {
                Unit nearbyUnit = unit.ScanForUnits(true);

                if (nearbyUnit == null)
                {
                    // Become idle if no nearby valid units
                    unit.SetIdle();
                }
                else
                {
                    // Target unit if nearby unit was found
                    SetTarget(nearbyUnit);
                }
            }

            return true;
        }

        return false;
    }
}
