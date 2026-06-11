using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    public enum CommandState
    {
        None,
        Unit,
        Worker,
        Fighter
    }

    // Currently just for the command widget 
    public enum InteractType
    {
        Move,
        Build,
        Attack,
        Defend,
        Gather,
        Convert
    }

    public CommandState commandState = CommandState.Unit;
    public CommandState State => commandState;
    // Units in this list respond to player right-click commands.
    //private List<Unit> commanding = new List<Unit>();
    private Player player;
    private AgentSquad currentSquad;
    public bool IsCommanding => currentSquad != null && currentSquad.Size > 0;
    List<AgentSquad> squads = new List<AgentSquad>();
    

    public CommandSystem(GameContext context)
    {
        player = context.player;

    }

    private void SetState(CommandState state)
    {
        if (state != commandState)
        {
            commandState = state;
        }
    }

    // Commands all following units to interact with the target
    public void Command(HoverTarget target)
    {
        if (target.IsTile)
        {
            foreach (Unit unit in currentSquad.Agents)
            {
                unit.Command(target.Tile);
            }
        }
        else if (target.IsEnemy)
        {
            foreach (Unit unit in currentSquad.Agents)
            {
                unit.Command(target.Enemy);
            }
        }
        else if (target.IsBuilding)
        {
            foreach (Unit unit in currentSquad.Agents)
            {
                unit.Command(target.Building);
            }
        }
    }
    
    public void StartCommanding(Unit unit)
    {
        if (unit == null) return;

        if (unit.Role == null) SetState(CommandState.Unit);
        else if (unit.Role is WorkerRole) SetState(CommandState.Worker);
        else if (unit.Role is FighterRole) SetState(CommandState.Fighter);

        // If we have a different squad selected, stop commanding it
        if (unit.HasSquad && currentSquad != null && unit.Squad != currentSquad)
        {
            StopCommanding();
        }

        
        if (unit.HasSquad) // Start commanding existing squad if unit has one
        {
            foreach (Unit squadUnit in unit.Squad.Agents)
            {
                squadUnit.StartCommanding(player);
            }
            currentSquad = unit.Squad;
        }
        else if (!unit.HasSquad) // Create new squad or add unit to current squad
        {
            if (currentSquad == null)
                currentSquad = GetSquad(unit);
            unit.SetSquad(currentSquad);
            currentSquad.AddAgent(unit);
            unit.StartCommanding(player);
        }
    }

    // Stops commanding a specific unit - removes unit from their squad
    public void StopCommanding(Unit unit)
    {
        if (currentSquad == null) return;

        if (currentSquad.HasAgent(unit))
        {
            currentSquad.RemoveAgent(unit);
        }
        unit.StopCommanding();

        if (currentSquad.Size == 0)
        {
            currentSquad.ClearAgents();
            squads.Remove(currentSquad);
            currentSquad = null; // Clear current squad when no units are being commanded
            SetState(CommandState.None);
        }
    }

    // Stops commanding all units - units should keep their squad 
    public void StopCommanding()
    {
        if (currentSquad == null) return;

        foreach (Unit unit in currentSquad.Agents)
        {
            unit.StopCommanding();
        }

        if (currentSquad.Size <= 1) // If the squad only has 1 or less agents clear it
        {
            currentSquad.ClearAgents();
            squads.Remove(currentSquad);
        }
            
        currentSquad = null; // Clear current squad when no units are being commanded
        SetState(CommandState.None);
    }
    public void CommandFollow()
    {
        if (currentSquad == null) return;
        foreach (Unit unit in currentSquad.Agents)
        {
            unit.StartFollowing();
        }
    }

    AgentSquad GetSquad(Agent agent)
    {
        if (agent.HasSquad)
        {
            return agent.Squad;
        }
        else
        {
            Debug.Log("New squad");
            AgentSquad squad = new AgentSquad();
            squad.Init(squads.Count);
            squads.Add(squad);
            return squad;
        }
    }
}
