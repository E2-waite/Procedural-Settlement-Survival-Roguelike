using Cinderwild.Core.Data;
using Cinderwild.WorldBuilder.Data;
using System.Collections;
using UnityEngine;
using Cinderwild.SpriteSytem.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(AgentController), typeof(SpriteController))]
    public class Agent : Destructable
    {
        private SpriteController spriteController;
        private AgentController controller;

        public float chunkInterval = 1f, chunkTimer = 0f;
        [SerializeField] private Transform body;
        public Transform Body => body;
        public virtual void Init(Context context)
        {

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