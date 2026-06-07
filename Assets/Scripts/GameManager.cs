using UnityEngine;

public class GameManager : MonoBehaviour
{
    private EnemySystem enemySystem;
    private bool initialized = false;

    public void Init(GameContext context)
    {
        enemySystem = context.enemySystem;

        initialized = true;
    }

    private void Update()
    {
        if (initialized)
        {
            enemySystem.Tick();
        }
    }
}

