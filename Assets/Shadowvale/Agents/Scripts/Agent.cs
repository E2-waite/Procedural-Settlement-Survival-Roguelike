using Shadowvale.Pathfinding.Runtime;
using Shadowvale.World.Runtime;
using UnityEngine;

namespace Shadowvale.Gameplay.Agents
{
    /// <summary>
    /// Represents an agent entity and acts as a point of entry for interacting with its components
    /// </summary>
    [RequireComponent(typeof(AgentController))]
    [RequireComponent(typeof(AgentSprites))]
    [RequireComponent(typeof(AgentStates))]
    [RequireComponent(typeof(AgentTracking))]
    public class Agent : Destructable
    {
        public AgentController Controller { get; private set; }
        public AgentSprites Sprite {  get; private set; }
        public AgentStates States { get; private set; }
        public AgentTracking Tracking { get; private set; }
        public Transform Body => body;
        [SerializeField] private Transform body;
        private bool selected = false;

        public virtual void Init(WorldManager world, CameraController camera, PathfindingManager pathfinding)
        {
            Controller = GetComponent<AgentController>();
            Sprite = GetComponent<AgentSprites>();
            States = GetComponent<AgentStates>();
            Tracking = GetComponent<AgentTracking>();
            Controller?.Init(this, world, pathfinding);
            Sprite?.Init(camera, this);
            States?.Init(this);
            Tracking?.Init(this, world);
        }

        public void Highlight()
        {
            Sprite?.Highlight(Color.white);
        }

        public void ClearHighlight()
        {
            if (selected)
            {
                Sprite?.Highlight(Color.green);
            }
            else
            {
                Sprite?.ClearHighlight();
            }
        }

        public void Select()
        {
            Sprite?.Highlight(Color.green);
            selected = true;
        }

        public void Deselect()
        {
            Sprite?.ClearHighlight();
            selected = false;
        }

        private void Update()
        {
            // TODO: remove update from agent 
            if (Controller != null)
                Sprite?.UpdateDirection(Controller.Facing);
        }
    }
}