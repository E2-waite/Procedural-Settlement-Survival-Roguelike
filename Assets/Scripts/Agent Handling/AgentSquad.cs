using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AgentSquad
{
    private List<Agent> agents = new List<Agent>();
    public Color color = Color.white;
    int id;
    public IEnumerable<Agent> Agents => agents;
    public int Size => agents.Count;
    private AgentFormation commandFormation;

    private AgentFormation followFormation;

    WorldGrid grid;
    Player player;
    List <Color> squadColors = new List<Color>()
    {
       Color.green,
       Color.blue,
       Color.red,
       Color.yellow,
       Color.purple,
       Color.orange,
       Color.pink
    };


    public void Init(GameContext context, int index)
    {
        id = index;
        color = squadColors[id];
        commandFormation = context.formationCatalog.wedgeFormation;
        followFormation = context.formationCatalog.ringFormation;

        World world = context.world;
        grid = world.Context.grid;
        player = context.player;
    }

    public void AddAgent(Agent agent)
    {
        if (!agents.Contains(agent))
        {
            Debug.Log("Adding " + agent.name + " to squad");
            agent.SquadSlotIndex = agents.Count;
            agents.Add(agent);
            agent.SetSquad(this);
        }
    }

    public void RemoveAgent(Agent agent)
    {
        Debug.Log("Removing " + agent.name + " from squad");

        agents.Remove(agent);
        agent.ClearSquad();
    }

    public void ClearAgents()
    {
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            RemoveAgent(agents[i]);
        }
    }

    public bool HasAgent(Agent agent)
    {
        return agents.Contains(agent);
    }

    public Vector3 CenterPos()
    {
        Vector3 total = Vector3.zero;
        foreach(Agent agent in agents)
        {
            total += agent.transform.position;
        }
        return total / agents.Count;
    }

    public Vector3 FollowFormationPos(Agent agent)
    {
        if (followFormation != null && agents.Contains(agent))
        {
            List<AgentFormation.Slot> formationSlots = followFormation.GetSlots();

            if (agent.SquadSlotIndex >= formationSlots.Count) return Vector3.zero;
            else return formationSlots[agent.SquadSlotIndex].pos;
        }
        return Vector3.zero;
    }

    public Vector3 CommandFormationPos(Agent agent)
    {
        if (commandFormation != null && agents.Contains(agent))
        {
            List<AgentFormation.Slot> formationSlots = commandFormation.GetSlots();

            if (agent.SquadSlotIndex >= formationSlots.Count) return Vector3.zero;
            else return formationSlots[agent.SquadSlotIndex].pos;
        }
        return Vector3.zero;
    }

    public void CommandMove(GridTile targetTile, Vector3 worldPos)
    {
        Vector3 lookDir = (player.transform.position - worldPos).normalized;
        lookDir.y = 0;

        foreach (Unit unit in agents)
        {
            Vector3 offset = unit.SnappedRotation(lookDir) * CommandFormationPos(unit);

            Vector3 targetPos = worldPos + offset;

            Vector2Int gridPos = new Vector2Int(
                    Mathf.FloorToInt(targetPos.x),
                    Mathf.FloorToInt(targetPos.z));

            GridTile slotTile = grid.GetTile(gridPos);

            if (slotTile != null && slotTile.IsEmpty)
            {
                unit.RequestPath(slotTile, targetPos);
            }
            else
            {
                unit.RequestPath(targetTile, targetPos);
            }

            unit.SetState(Unit.State.Moving);
        }
    }

    // Commands agents
    public virtual void Command(GridTile tile)
    {
        
    }

    // Commands unit to interact with an agent
    public virtual void Command(Agent agent)
    {

    }

    // Commands unit to interact with a building
    public virtual void Command(Building building)
    {
        
    }
}
