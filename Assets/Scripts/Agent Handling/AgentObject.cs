using UnityEngine;
using static GlobalDefs;
[CreateAssetMenu(fileName = "RoleObject", menuName = "Scriptable Objects/Agent")]
public class AgentObject : ScriptableObject
{
    public Sprite upRight;
    public Sprite upLeft;
    public Sprite downRight;
    public Sprite downLeft;
    public Sprite eyesLeft;
    public Sprite eyesRight;
    public Sprite GetAgentSprite(AgentDir dir)
    {
        if (dir == AgentDir.UpLeft) return upLeft;
        else if (dir == AgentDir.UpRight) return upRight;
        else if (dir == AgentDir.DownLeft) return downLeft;
        else if (dir == AgentDir.DownRight) return downRight;
        else return null;
    }

    public Sprite GetAgentEyes(AgentDir dir)
    {
        if (dir == AgentDir.DownLeft) return eyesLeft;
        else if (dir == AgentDir.DownRight) return eyesRight;
        else return null;
    }
}