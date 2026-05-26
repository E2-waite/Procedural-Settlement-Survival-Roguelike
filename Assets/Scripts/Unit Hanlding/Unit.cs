using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class Unit : PathAgent
{
    private Camera cam;

    protected virtual void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
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
