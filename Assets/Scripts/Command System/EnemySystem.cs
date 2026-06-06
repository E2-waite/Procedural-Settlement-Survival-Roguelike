using System.Collections.Generic;
using UnityEngine;

public class EnemySystem : MonoSingleton<EnemySystem>
{
    public List<EnemyUnit> enemies = new List<EnemyUnit>();



    public void AddEnemy(EnemyUnit enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(EnemyUnit enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    void SpawnEnemy()
    {

    }
}
