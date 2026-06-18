using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Formations/Grid Formation")]
public class AgentFormation : ScriptableObject
{
    public enum Type
    {
        Command,
        Follow
    };

    public Type type = Type.Command;
    public int[] slots;
    public int width = 5, height = 5;
    public float spacing = 1.5f;

    public struct Slot
    {
        public int weight;
        public Vector3 pos;
    }


    private void OnValidate()
    {
        int size = width * height;

        if (slots == null || slots.Length != size)
            slots = new int[size];
    }

    //public bool IsSlotEnabled(int x, int y)
    //{
    //    return slots[y * width + x] > 0;
    //}

    public int GetSlotWeight(int x, int y)
    {
        return slots[y * width + x];
    }   
    
    public List<Slot> GetSlots()
    {
        List<Slot> result = new();

        float  xOffset = (int)((width - 1) * spacing * 0.5f);
        float zOffset = (int)((height - 1) * spacing * 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int weight = GetSlotWeight(x, y);
                if (weight == 0) continue;

                Slot slot = new Slot()
                {
                    weight = weight,
                    pos = new Vector3(
                            x * spacing - xOffset,
                            0,
                            y * spacing - zOffset)
                };

                result.Add(slot);
            }
        }

        result.Sort((a, b) => b.weight.CompareTo(a.weight));

        return result;
    }
}
