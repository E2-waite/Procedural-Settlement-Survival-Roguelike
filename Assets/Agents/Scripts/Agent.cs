using UnityEngine;
using Cinderwild.Core;

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

        public virtual void Init(Context context)
        {
            Controller = GetComponent<AgentController>();
            Sprite = GetComponent<AgentSprites>();
            States = GetComponent<AgentStates>();
            Tracking = GetComponent<AgentTracking>();
            Controller?.Init(this);
            Sprite?.Init(context.Camera, this);
            States?.Init(this);
            Tracking?.Init(this);
        }
    }
}