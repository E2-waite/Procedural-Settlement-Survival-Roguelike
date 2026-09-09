using Cinderwild.Core;
using Cinderwild.World.Runtime;
using UnityEngine;

namespace Cinderwild.Gameplay.Agents
{
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

        [SerializeField] private Transform body;
        public Transform Body => body;
        private bool selected = false;

        public virtual void Init(Context context)
        {
            Controller = GetComponent<AgentController>();
            Sprite = GetComponent<AgentSprites>();
            States = GetComponent<AgentStates>();
            Tracking = GetComponent<AgentTracking>();
            Controller?.Init(this, context.World.System);
            Sprite?.Init(context.Camera, this);
            States?.Init(this);
            Tracking?.Init(this, context.World.System);
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