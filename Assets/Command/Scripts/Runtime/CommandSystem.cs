using Cinderwild.Core;
using Cinderwild.Gameplay.Agents;
using UnityEngine;

namespace Cinderwild.Command.Runtime
{
    public enum CommandType
    {
        MoveTo,
        Attack,
        Gather,
        None // No command type (max)
    }

    public class CommandSystem
    {
        private Agent selected = null; // The currently selected agent

        public void SelectAgent(Agent agent)
        {
            selected = agent;
            agent.Select();
            Debug.Log("Selected " + agent.name);
        }

        public void Deselect()
        {
            selected?.Deselect();
            selected = null;
        }

        public void ExecuteCommand(CommandType commandType, InteractionTarget target)
        {
            if (selected == null) return;

            switch (commandType)
            {
                case CommandType.MoveTo:
                    if (target.Type == TargetType.Tile)
                        selected.Controller?.MoveTo(target.Tile);
                    break;
            }
        }
    }
}