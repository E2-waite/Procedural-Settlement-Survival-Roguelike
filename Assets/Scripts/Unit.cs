using UnityEngine;

public class Unit : MonoBehaviour
{
    enum UnitState
    {
        Idle,
        Moving
    }

    public float moveSpeed = 10f;
    UnitState state;
    Vector3 targetPos;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = UnitState.Idle;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == UnitState.Moving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                state = UnitState.Idle;
            }
        }
    }

    public void MoveTo(Vector3 pos)
    {
        targetPos = pos;

        state = UnitState.Moving;
    }


}
