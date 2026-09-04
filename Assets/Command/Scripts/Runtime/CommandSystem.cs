using Cinderwild.Gameplay.Agents;
using UnityEngine;

namespace Cinderwild.Command.Runtime
{
    public enum CommandType
    {
        None,
        MoveTo,
        Attack,
        Gather
    }

    public static class CommandSystem
    {
        private static Agent selected = null; // The currently selected agent

        public static void SelectAgent(Agent agent)
        {
            selected = agent;
        }

        public static void ClearSelection()
        {
            selected = null;
        }


    }
}