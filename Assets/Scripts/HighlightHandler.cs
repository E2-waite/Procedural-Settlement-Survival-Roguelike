using UnityEngine;
using UnityEngine.UIElements;
using static CommandSystem;
using static GlobalDefs;
public class HighlightHandler
{
    CommandSystem commandSystem;

    public void Init(GameContext context)
    {
        commandSystem = context.commandSystem;
    }

    public void Clear(HoverTarget target)
    {
        if (target == null) return;

        if (target.IsAgent)
        {
            target.Agent.Highlight(false);
        }
        else if (target.IsBuilding)
        {
            target.Building.Highlight(false);
        }
    }

    public void Set(HoverTarget target)
    {
        if (target == null) return;

        if (target.IsAgent)
        {
            target.Agent.Highlight(true);
        }
        else if (target.IsBuilding)
        {
            bool highlight = false;

            if (commandSystem.IsCommanding)
            {
                if (target.Building is ConvertBuilding) highlight = true;

                if (commandSystem.State == CommandState.Fighter && target.Building.Owner == Faction.Enemy)
                    highlight = true;

                if (commandSystem.State == CommandState.Worker && target.Building.Owner == Faction.Unit)
                    highlight = true;
            }

            target.Building.Highlight(highlight);
        }
    }
}
