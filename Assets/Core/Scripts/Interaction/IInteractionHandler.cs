using UnityEngine;
using UnityEngine.InputSystem;

namespace Cinderwild.Core
{
    public interface IInteractionHandler
    {
        void Enable(InteractionTarget target, Vector2 mousePos);
        void Disable();
        void OnMouseMoved(Vector2 pos, Vector2 diff);
        void OnLeftDown(InteractionTarget target);
        void OnLeftHeld(Vector2 diff, float time);
        void OnLeftUp(Vector2 diff, float time);
        void OnRightDown(InteractionTarget target);
        void OnRightHeld(Vector2 pos, Vector2 diff, float time);
        void OnRightUp(Vector2 diff, float time);
        void OnKeyPressed(Key key);
        void OnKeyReleased(Key key);
        void OnMoveInput(Vector2 move);
    }
}
