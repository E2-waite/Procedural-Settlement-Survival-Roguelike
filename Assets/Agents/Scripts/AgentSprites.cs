using Cinderwild.Core.Data;
using Cinderwild.SpriteSytem.Data;
using Cinderwild.SpriteSytem.Runtime;
using System.Collections;
using UnityEngine;

namespace Cinderwild.Gameplay.Agents
{
    /// <summary>
    /// Controls sprite direction 
    /// </summary>
    public class AgentSprites : MonoBehaviour
    {
        private Direction facing = Direction.South;
        [SerializeField] private SpriteRig spriteRig;
        private CameraController camera;
        private int lastCamIndex = -1, lastPlayerIndex = -1;
        Coroutine rotateRoutine = null;
        private Agent agent = null;
        public void Init(Agent agent)
        {
            this.agent = agent;
            spriteRig?.Init();
            camera = GameManager.Context.Camera;
        }

        public void Init()
        {
            spriteRig?.Init();
            camera = GameManager.Context.Camera;
        }

        public void UpdateDirection(Vector3 facingVec)
        {
            if (facingVec != Vector3.zero)
            {
                Direction newFacing = GetDirection(facingVec);

                if (newFacing != facing)
                {
                    if (rotateRoutine == null)
                    {
                        rotateRoutine = StartCoroutine(TransitionRoutine(newFacing));
                    }
                }
            }
        }

        private void Update()
        {
            UpdateSpriteRig();
        }

        private IEnumerator TransitionRoutine(Direction desired)
        {
            int delta = ((int)desired - (int)facing + 8) % 8;

            if (delta > 4)
                delta -= 8;

            while (facing != desired)
            {
                Direction next = facing;

                // Wrap around increment/decrement
                if (delta > 0) next = (Direction)(((int)facing + 1) % 8);
                else if (delta < 0) next = (Direction)(((int)facing - 1 + 8) % 8);
                else break;

                yield return new WaitForSeconds(0.05f);

                facing = next;
            }

            rotateRoutine = null;
        }

        private static Direction GetDirection(Vector3 forward)
        {
            if (forward.sqrMagnitude < 0.0001f)
                return Direction.Null;

            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            angle = Mathf.Repeat(angle, 360f);

            int index = Mathf.RoundToInt(angle / 45f) % 8;

            return (Direction)index;
        }

        public void UpdateSpriteRig()
        {
            if (camera == null) return;

            if (facing == Direction.Null ||
                camera.Facing == Direction.Null)
                return;

            int playerIndex = (int)facing;
            int cameraIndex = (int)camera.Facing;

            if (playerIndex == lastPlayerIndex && cameraIndex == lastCamIndex) return;

            lastPlayerIndex = playerIndex;
            lastCamIndex = cameraIndex;

            int relativeIndex = (playerIndex - cameraIndex) % 8;

            if (relativeIndex < 0)
                relativeIndex += 8;

            Direction dir = (Direction)relativeIndex;

            spriteRig.UpdateDirection(dir);
        }
    }
}
