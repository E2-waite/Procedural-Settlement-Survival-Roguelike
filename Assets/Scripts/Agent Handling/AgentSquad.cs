using System.Collections.Generic;
using UnityEngine;
using static AgentObject;

public class AgentSquad
{
    private List<Agent> agents = new List<Agent>(); // List of agents of all types
    private List<Agent>[] agentTypes = new List<Agent>[(int)RoleType.Max]; // Lists of agents of a specific type

    public Color color = Color.white;
    int id;
    public IEnumerable<Agent> Agents => agents;
    public int Size => agents.Count;
    private AgentFormation[] commandFormation = new AgentFormation[(int)RoleType.Max];
    private AgentFormation[] followFormation = new AgentFormation[(int)RoleType.Max];
    public AgentFormation CommandFormation(RoleType agentType) => commandFormation[(int)agentType];
    public AgentFormation FollowFormation(RoleType agentType) => followFormation[(int)agentType];

    WorldGrid grid;
    private float commandDist = 7.5f; // The distance from the player that squads are commanded to

    private Vector3 facing = Vector3.zero;
    public Vector3 FacingDir
    {
        get { return facing; }
        set { facing = value; }
    }
    private Quaternion rotation = Quaternion.identity;
    public Quaternion FacingRot
    {
        get { return rotation; }
        set { rotation = value; }
    }

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
        commandFormation[(int)RoleType.Melee] = context.formationCatalog.meleeCommand;
        followFormation[(int)RoleType.Melee] = context.formationCatalog.meleeFollow;
        commandFormation[(int)RoleType.Ranged] = context.formationCatalog.rangedCommand;
        followFormation[(int)RoleType.Ranged] = context.formationCatalog.rangedFollow;

        for (int i = 0; i < (int)RoleType.Max; i++)
        {
            agentTypes[i] = new List<Agent>();
        }
        World world = context.world;
        grid = world.Context.grid;
    }

    public void AddAgent(Agent agent)
    {
        if (!agents.Contains(agent))
        {
            Debug.Log("Adding " + agent.name + " to squad");
            agent.SquadIndex = agents.Count;
            agents.Add(agent);
            agent.SquadTypeIndex = agentTypes[(int)agent.AgentType].Count;
            agentTypes[(int)agent.AgentType].Add(agent);
            agent.SetSquad(this);

            agent.FormationPos = (facing * commandDist) + (rotation * CommandFormationPos(agent));
        }
    }

    public void RemoveAgent(Agent agent)
    {
        Debug.Log("Removing " + agent.name + " from squad");

        agents.Remove(agent);
        agentTypes[(int)agent.AgentType].Remove(agent);
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
            List<AgentFormation.Slot> formationSlots = followFormation[(int)agent.AgentType]?.GetSlots();

            if (formationSlots == null || agent.SquadTypeIndex >= formationSlots.Count) return Vector3.zero;
            else return formationSlots[agent.SquadTypeIndex].pos;
        }
        return Vector3.zero;
    }

    public Vector3 CommandFormationPos(Agent agent)
    {
        if (commandFormation != null && agents.Contains(agent))
        {
            List<AgentFormation.Slot> formationSlots = commandFormation[(int)agent.AgentType]?.GetSlots();

            if (formationSlots == null || agent.SquadTypeIndex >= formationSlots.Count) return Vector3.zero;
            else return formationSlots[agent.SquadTypeIndex].pos;
        }
        return Vector3.zero;
    }

    public void CommandMove(GridTile targetTile, Vector3 playerPos)
    {
        foreach (Unit unit in agents)
        {
            Vector3 targetPos = playerPos + unit.FormationPos;

            Vector2Int gridPos = new Vector2Int(
                    Mathf.FloorToInt(targetPos.x),
                    Mathf.FloorToInt(targetPos.z));

            GridTile slotTile = grid.GetTile(gridPos);
            unit.LookTo(facing);

            if (slotTile != null && slotTile.IsEmpty)
            {
                unit.Command(slotTile, targetPos);
            }
            else
            {
                unit.Command(targetTile, targetPos);
            }
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

    public void SetFormationDir(Quaternion rot)
    {
        facing = rot * Vector3.forward;
        rotation = rot;

        foreach (Agent agent in agents)
        {
            agent.FormationPos = (facing * commandDist) + (rotation * CommandFormationPos(agent));
        }
    }

    public void UpdateMarkers(bool commanding, Vector3 playerPos)
    {
        foreach (Unit unit in agents)
        {
            if (unit == null) continue;

            if (commanding)
            {
                Vector3 targetPos = playerPos + unit.FormationPos;
                unit.UpdateMarkerPos(targetPos);
            }
            else
            {
                unit.SetMarkerUpdating();
            }
        }
    }
}