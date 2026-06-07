using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public enum TargetType
    {
        Unit,
        Building,
        Fire,
        Player,
        Max
    }

    public virtual TargetType Type => TargetType.Unit;
    [SerializeField] protected Health health = new Health();
    public Health Health => health;

    public float deathTime = 0.5f;
    private bool dead = false;
    public bool IsDead => dead;
    public Vector2Int GridPos => new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

    // Handles receiving hits from units. Returns true if target is dead
    public virtual bool Hit(float damage, Damageable source)
    {
        if (dead) return true; // Already dead
        else
            Debug.Log(name + " hit by " + source.name + "(" + damage + " dmg)");

        health.Damage(damage);

        if (!dead && health.IsEmpty)
        {
            Debug.Log(name + " SHOULD DIE!!!");
            dead = true;
            StartCoroutine(DeathRoutine());
            return true;
        }
        else
        {
            StartCoroutine(HitCoroutine());
        }
            
        return false;
    }


    protected virtual IEnumerator HitCoroutine()
    {
        yield return null;
    }

    // Handle death
    protected virtual void OnDeathStart()
    {
    }

    protected virtual void OnDeathFinish()
    {
        Destroy(gameObject);
    }

    // Delayed death
    protected virtual IEnumerator DeathRoutine()
    {
        OnDeathStart();
        yield return new WaitForSeconds(deathTime);
        OnDeathFinish();
    }
}
