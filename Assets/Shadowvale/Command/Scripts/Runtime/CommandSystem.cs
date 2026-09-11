using Shadowvale.Core;
using Shadowvale.Gameplay.Agents;
using UnityEngine;

namespace Shadowvale.Command.Runtime
{
    public enum CommandType
    {
        MoveTo,
        Attack,
        Gather,
        None // No command type (max)
    }

    /// <summary>
    /// Handles agent selection and command execution
    /// </summary>
    public class CommandSystem
    {
        private Agent selected = null; // The currently selected agent

        public void SelectAgent(Agent agent)
        {
            selected = agent;
            agent.Select();
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