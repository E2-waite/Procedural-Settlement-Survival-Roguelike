using UnityEngine;

public interface IInteractionHandler
{
    void Enable();
    void Disable();

    void OnHover(HoverTarget target);
    void OnMouseMoved(Vector2 pos, Vector2 diff);
    void OnLeftDown();
    void OnLeftHeld(Vector2 diff, float time);
    void OnLeftUp(Vector2 diff, float time);
    void OnRightDown();
    void OnRightHeld(Vector2 diff, float time);
    void OnRightUp(Vector2 diff, float time);
    void OnEscape();
    void OnFKey();
    void OnMoveInput(Vector2 move);
    void OnLookChanged(Quaternion rot);
    void OnLookDistChanged(float dist);

}
