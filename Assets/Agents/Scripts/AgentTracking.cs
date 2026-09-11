using Cinderwild.World.Runtime;
using UnityEngine;

namespace Cinderwild.Gameplay.Agents
{
    /// <summary>
    /// Tracks an agent's state within the world
    /// </summary>
    public class AgentTracking : MonoBehaviour
    {
        Vector3 lastPos = Vector3.zero;
        private Agent agent;
        WorldManager world;
        public void Init(Agent agent, WorldManager world)
        {
            this.agent = agent;
            this.world = world;
        }

        //void Update()
        //{
        //    UpdateChunk();
        //}

        //private void UpdateChunk()
        //{
        //    if ((transform.position - lastPos).sqrMagnitude < 0.1f) return;
        //    lastPos = transform.position;

        //    position /= world.Properties.chunkSize;
        //    Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

        //    Chunk chunk = world.Data.GetChunk(transform.position);
        //}
    }
}
