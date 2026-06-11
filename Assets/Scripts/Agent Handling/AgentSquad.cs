using System.Collections.Generic;
using UnityEngine;

public class AgentSquad
{
    private List<Agent> agents = new List<Agent>();
    public Color color = Color.white;
    int id;
    public IEnumerable<Agent> Agents => agents;
    public int Size => agents.Count;

    List < Color> squadColors = new List<Color>()
    {
       Color.green,
       Color.blue,
       Color.red,
       Color.yellow,
       Color.purple,
       Color.orange,
       Color.pink
    };


    public void Init(int index)
    {
        id = index;
        color = squadColors[id];
    }

    public void AddAgent(Agent agent)
    {
        if (!agents.Contains(agent))
        {
            Debug.Log("Adding " + agent.name + " to squad");
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

}
