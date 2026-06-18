using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Formations/Grid Formation")]
public class AgentFormation : ScriptableObject
{
    public int[] slots;
    public int width = 5, height = 5;
    public int spacing = 1;

    public struct Slot
    {
        public int weight;
        public Vector2Int pos;
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

        int  xOffset = (int)((width - 1) * spacing * 0.5f);
        int zOffset = (int)((height - 1) * spacing * 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int weight = GetSlotWeight(x, y);
                if (weight == 0) continue;

                Slot slot = new Slot()
                {
                    weight = weight,
                    pos = new Vector2Int(
                            x * spacing - xOffset,
                            y * spacing - zOffset)
                };

                result.Add(slot);
            }
        }

        result.Sort((a, b) => b.weight.CompareTo(a.weight));

        return result;
    }
}
