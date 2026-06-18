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
    private AgentFormation agentFormation;
    WorldGrid grid;
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
        agentFormation = context.formationCatalog.wedgeFormation;
        World world = context.world;
        grid = world.Context.grid;
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

    public Vector2Int GetFormationPos(Agent agent)
    {
        if (agentFormation != null && agents.Contains(agent))
        {
            List<AgentFormation.Slot> formationSlots = agentFormation.GetSlots();

            if (agent.SquadSlotIndex >= formationSlots.Count) return Vector2Int.zero;
            else return formationSlots[agent.SquadSlotIndex].pos;
        }
        return Vector2Int.zero;
    }

    public void CommandMove(GridTile targetTile)
    {
        foreach (Unit unit in agents)
        {
            Vector2Int offset = GetFormationPos(unit);
            Vector2Int gridPos = targetTile.position + offset;

            GridTile slotTile = grid.GetTile(gridPos);

            if (slotTile != null && slotTile.IsEmpty)
            {
                unit.RequestPath(slotTile);
            }
            else
            {
                unit.RequestPath(targetTile);
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
