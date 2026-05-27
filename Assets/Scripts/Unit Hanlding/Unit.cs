using System.Collections;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class Unit : PathAgent
{
    private Camera cam;
    public float currentHealth, maxHealth = 100;
    public float chunkInterval = 1f, chunkTimer = 0f;
    protected Chunk chunk;

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

        Debug.Log(name + " hit by " + source.name + "(" + damage + " dmg)");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected virtual void Die()
    {
        if (chunk != null)
        {
            chunk.RemoveUnit(this);
        }
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(name + " should die");
        Destroy(gameObject);
    }

    // TODO: handle targetting targets in range

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
        if (currentHealth <= 0) return; // Dead

        HandleStates();

        UpdateChunk();
    }

    void UpdateChunk()
    {
        if (chunkTimer <= 0)
        {
            chunkTimer = chunkInterval;

            Chunk newChunk = Grid.Instance.ChunkFromGridPos(GridPos());
            if (newChunk != null && newChunk != chunk)
            {
                if (chunk != null) chunk.RemoveUnit(this);

                newChunk.AddUnit(this);

                chunk = newChunk;
            }
        }
    }

    protected virtual bool HandleStates()
    {
        bool handled = false;
        return handled;
    }

}
