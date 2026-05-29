using UnityEngine;

[System.Serializable]
public struct SaveVector3
{
    float x;
    float y;
    float z;

    public SaveVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToPublic2Int()
    {
        return new Vector3(x, y, z);
    }
}
