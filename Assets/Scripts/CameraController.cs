using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float followSpeed = 10f;
    public float edgeSize = 20f;
    public float buildRange = 200f;

    Vector3 currentVelocity;
    Camera camera;
    PlayerController player;
    public float minScroll = 10;
    public float maxScroll = 50;

    private Vector3 followOffset = new Vector3(-10, 10, -10);

    private void Start()
    {
        camera = GetComponent<Camera>();
    }


    void Update()
    {
        if (player == null)
        {
            player = GameManager.Instance.player;
            InitCamera();
        }
        else
        {
            if (InteractionManager.Instance.state == InteractionManager.GameState.Build)
            {
                MoveCamera();
            }
            else
            {
                FollowPlayer();
                currentVelocity = Vector3.zero;
            }
        }
    }

    void InitCamera()
    {
        Vector3 targetPos = player.transform.position + followOffset;

        targetPos.y = transform.position.y;
        transform.position = targetPos;
    }

    void FollowPlayer()
    {
        Vector3 targetPos =
            player.transform.position + followOffset;

        targetPos.y = transform.position.y;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }

    void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (mousePos.x < 0 || mousePos.x > Screen.width ||
            mousePos.y < 0 || mousePos.y > Screen.height)
        {
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = Vector3.zero;

        if (mousePos.y >= Screen.height - edgeSize)
        {
            float t = 1f - ((Screen.height - mousePos.y) / edgeSize);
            move += forward * t;
        }

        if (mousePos.y <= edgeSize)
        {
            float t = 1f - (mousePos.y / edgeSize);
            move -= forward * t;
        }

        if (mousePos.x >= Screen.width - edgeSize)
        {
            float t = 1f - ((Screen.width - mousePos.x) / edgeSize);
            move += right * t;
        }

        if (mousePos.x <= edgeSize)
        {
            float t = 1f - (mousePos.x / edgeSize);
            move -= right * t;
        }

        move = Vector3.ClampMagnitude(move, 1f);

        currentVelocity = Vector3.Lerp(
            currentVelocity,
            move * moveSpeed,
            10f * Time.deltaTime
        );

        transform.position += currentVelocity * Time.deltaTime;

        Vector3 playerPos = player.transform.position + followOffset;
        playerPos.y = transform.position.y;

        Vector3 offset = transform.position - playerPos;
        offset.y = 0f;

        if (offset.magnitude > buildRange)
        {
            transform.position = playerPos + offset.normalized * buildRange;
        }
    }


}
