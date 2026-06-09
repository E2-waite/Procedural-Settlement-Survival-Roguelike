using System.Collections.Generic;
using UnityEngine;
using static CommandSystem;
[CreateAssetMenu]
public class CommandTypeCatalog : ScriptableObject
{
    public CommandTypeObject Move;
    public CommandTypeObject Build;
    public CommandTypeObject Attack;
    public CommandTypeObject Defend;
    public CommandTypeObject Gather;
    public CommandTypeObject Convert;

    public CommandTypeObject GetObject(CommandType type)
    {
        CommandTypeObject commandType = Move;

        switch (type)
        {
            case CommandType.Attack:
                commandType = Attack;
                break;
            case CommandType.Defend:
                commandType = Defend;
                break;
            case CommandType.Build:
                commandType = Build;
                break;
            case CommandType.Gather:
                commandType = Gather;
                break;
            case CommandType.Convert:
                commandType = Convert;
                break;
        }

        return commandType;
    }
}
