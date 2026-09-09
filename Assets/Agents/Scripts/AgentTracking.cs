using Cinderwild.Gameplay.Agents;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;
using UnityEngine;

public class AgentTracking : MonoBehaviour
{
    Vector3 lastPos = Vector3.zero;
    private Agent agent;
    WorldSystem world;
    public void Init(Agent agent, WorldSystem worldSystem)
    {
        this.agent = agent;
        this.world = worldSystem;
    }

    void Update()
    {
        UpdateChunk();
    }

    private void UpdateChunk()
    {
        if ((transform.position - lastPos).sqrMagnitude < 0.1f) return;
        lastPos = transform.position;

        Chunk chunk = world.GetChunk(transform.position);
    }
}
