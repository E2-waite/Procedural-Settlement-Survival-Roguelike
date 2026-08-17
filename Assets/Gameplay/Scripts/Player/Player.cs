using UnityEngine;
using Cinderwild.Core.Data;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }

        public void Init(Context context)
        {
            Controller = GetComponent<PlayerController>();
            health.Fill();
        }

        void Update()
        {
        }

        public void OnMove(Vector2 moveInput)
        {
            Debug.Log("Player");
            Controller?.MovePlayer(moveInput);
        }
    }
}
