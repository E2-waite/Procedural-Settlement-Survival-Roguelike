using Cinderwild.Core;
using Cinderwild.Gameplay.Agents;
using System.Collections;
using UnityEngine;
using Cinderwild.SpriteSytem.Data;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float followSpeed = 10f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float rotationSpeed = 1f;

    private Vector3 velocity;

    Vector3 currentVelocity;
    Player player;
    public float minScroll = 10;
    public float maxScroll = 50;

    private Vector3 followOffset = new Vector3(-10, 10, -10);
    private bool initialized = false;
    private Coroutine rotateRoutine = null;
    public Direction Facing => facingDir;
    private Direction facingDir = Direction.North;

    public void Init(Context context)
    {
        player = context.Player;

        Vector3 targetPos = player.transform.position + followOffset;
        targetPos.y = transform.position.y;
        transform.position = targetPos;

        initialized = true;
    }

    void Update()
    {
        if (initialized)
        {
            FollowPlayer();
            currentVelocity = Vector3.zero;
        }
    }

    public void Rotate(int dir)
    {
        if (rotateRoutine != null) return;

        rotateRoutine = StartCoroutine(RotateRoutine(dir));
    }

    private IEnumerator RotateRoutine(int dir)
    {
        Vector3 desiredVec = transform.rotation.eulerAngles + new Vector3(0, dir > 0 ? 90 : -90, 0);
        Quaternion desiredRot = Quaternion.Euler(desiredVec);

        while (Quaternion.Angle(transform.rotation, desiredRot) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotationSpeed * Time.deltaTime);

            facingDir = GetDirectionFromAngle(transform.rotation.eulerAngles.y);

            yield return null;
        }

        transform.rotation = desiredRot;
        rotateRoutine = null;
    }

    private Direction GetDirectionFromAngle(float angle)
    {
        angle = Mathf.Repeat(angle, 360f);

        int index = Mathf.RoundToInt(angle / 45f) % 8;

        return (Direction)index;
    }

    void FollowPlayer()
    {
        if (player == null) return;

        Vector3 targetPosition = player.transform.position;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
