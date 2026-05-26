using System.Collections;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class Unit : PathAgent
{
    private Camera cam;
    public float currentHealth, maxHealth = 100;
    protected virtual void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);

        currentHealth = maxHealth;
    }

    // Returns true if target is dead
    public virtual bool Hit(float damage, Unit source)
    {
        if (currentHealth <= 0) return true; // Already dead

        Debug.Log(name + " hit by " + source.name + "(" + damage + "dmg)");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            StartCoroutine(DeathRoutine());
            return true;
        }
        return false;
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(name + " should die");
        Destroy(gameObject);
    }

    protected virtual void SetState(int newState)
    {
    }

    protected virtual int GetState()
    {
        return 0;
    }

    protected Vector2Int GridPos()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

    }

    protected virtual void Update()
    {
        HandleStates();
    }

    protected virtual bool HandleStates()
    {
        bool handled = false;
        return handled;
    }

}
