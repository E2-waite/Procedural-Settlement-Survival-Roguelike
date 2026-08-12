using System.Collections.Generic;
using UnityEngine;
using static CommandSystem;
[CreateAssetMenu]
public class ActionTypeCatalog : ScriptableObject
{
    public ActionTypeObject Move;
    public ActionTypeObject Build;
    public ActionTypeObject Attack;
    public ActionTypeObject Defend;
    public ActionTypeObject Gather;
    public ActionTypeObject Convert;

    public ActionTypeObject GetObject(InteractType type)
    {
        ActionTypeObject commandType = Move;

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
