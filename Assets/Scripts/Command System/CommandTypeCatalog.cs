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

    public CommandTypeObject GetObject(InteractType type)
    {
        CommandTypeObject commandType = Move;

        switch (type)
        {
            case InteractType.Attack:
                commandType = Attack;
                break;
            case InteractType.Defend:
                commandType = Defend;
                break;
            case InteractType.Build:
                commandType = Build;
                break;
            case InteractType.Gather:
                commandType = Gather;
                break;
            case InteractType.Convert:
                commandType = Convert;
                break;
        }

        return commandType;
    }
}
