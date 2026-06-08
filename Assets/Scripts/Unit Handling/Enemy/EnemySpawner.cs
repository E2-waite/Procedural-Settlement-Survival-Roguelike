using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyUnit SpawnEnemy(EnemyObject enemy, GridTile tile)
    {
        GameObject enemyObj = Instantiate(enemy.prefab, tile.worldPosition, Quaternion.identity);
        EnemyUnit enemyUnit = enemyObj.GetComponent<EnemyUnit>();



        return enemyUnit;
    }
}
