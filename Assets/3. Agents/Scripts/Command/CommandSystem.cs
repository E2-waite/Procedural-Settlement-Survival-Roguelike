using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;
using static UnityEngine.GraphicsBuffer;
public class CommandSystem
{
    public enum CommandState
    {
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
    private Player player;
    private AgentSquad currentSquad;
    public bool IsCommanding => currentSquad != null && currentSquad.Size > 0;
    public AgentSquad CurrentSquad => currentSquad;
    List<AgentSquad> squads = new List<AgentSquad>();
    GameContext gameContext;
    Quaternion lookRot;

    public CommandSystem(GameContext context)
    {
        player = context.player;
        gameContext = context;
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
        if (target.IsEnemy)
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

        bool recruiting = false;

        if (unit.Faction == Faction.Neutral) // Try to recruit a unit if we have space
        {
            recruiting = true;
            if (!unit.Recruit())
                return;
        }

        if (unit.Role == null) SetState(CommandState.Unit);
        else if (unit.Role is WorkerRole) SetState(CommandState.Worker);
        else if (unit.Role is FighterRole) SetState(CommandState.Fighter);

        // If we have a different squad selected, stop commanding it
        if (unit.HasSquad && currentSquad != null && unit.Squad != currentSquad)
        {
            StopCommanding();
        }

        
        if (unit.HasSquad && !recruiting) // Start commanding existing squad if unit has one
        {
            List<Agent> squadAgents = new List<Agent>(unit.Squad.Agents);
            foreach (Unit squadUnit in squadAgents)
            {
                squadUnit.StartCommanding(player);
            }
            currentSquad = unit.Squad;
        }
        else // Create new squad or add unit to current squad
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
    }
    public void CommandFollow()
    {
        currentSquad?.CommandFollow();
    }

    public void CommandMove(HoverTarget target)
    {
        currentSquad?.CommandMove(lookRot);
    }

    AgentSquad GetSquad(Agent agent)
    {
        if (agent.HasSquad && agent.Squad.Faction == Faction.Friendly)
        {
            return agent.Squad;
        }
        else
        {
            AgentSquad squad = new AgentSquad();
            squad.Init(gameContext, Faction.Friendly);
            squads.Add(squad);
            return squad;
        }
    }

    public void UpdateLookRot(Quaternion rot)
    {
        lookRot = rot;
        currentSquad?.UpdateFollowDir(rot);
    }

    public void UpdateLookDist(float dist)
    {
        currentSquad?.UpdateCommandDist(dist);
    }

    public void AimFormation(bool commanding)
    {
        currentSquad?.UpdateMarkers(commanding, lookRot);
    }


}
