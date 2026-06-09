using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy SpawnEnemy(EnemyObject enemy, GridTile tile)
    {
        GameObject enemyObj = Instantiate(enemy.prefab, tile.worldPosition, Quaternion.identity);
        Enemy enemyUnit = enemyObj.GetComponent<Enemy>();



        return enemyUnit;
    }
}
