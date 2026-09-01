using Cinderwild.Core.Data;
using UnityEngine;
using Cinderwild.SpriteSytem.Runtime;

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
            Sprite?.Init(this);
            States?.Init(this);
            Tracking?.Init(this);
        }

        protected virtual void Update()
        {
            UpdateChunk();
        }

        // Updates chunk state (adds unit to new chunk and removes unit from old chunk)
        void UpdateChunk()
        {
            //if (chunkTimer <= 0 && grid != null)
            //{
            //    chunkTimer = chunkInterval;

            //    // Chunk membership powers local enemy/friendly queries for combat and swarming.
            //    Chunk newChunk = grid.ChunkFromGridPos(GridPos);
            //    if (newChunk != null && newChunk != chunk)
            //    {
            //        if (chunk != null) chunk.Data.RemoveAgent(this);

            //        newChunk.Data.AddAgent(this);

            //        chunk = newChunk;
            //    }
            //}
            //if (chunkTimer <= 0 && grid != null)
            //{
            //    chunkTimer = chunkInterval;

            //    // Chunk membership powers local enemy/friendly queries for combat and swarming.
            //    Chunk newChunk = grid.ChunkFromGridPos(GridPos);
            //    if (newChunk != null && newChunk != chunk)
            //    {
            //        if (chunk != null) chunk.Data.RemoveAgent(this);

            //        newChunk.Data.AddAgent(this);

            //        chunk = newChunk;
            //    }
            //}
        }
    }
}