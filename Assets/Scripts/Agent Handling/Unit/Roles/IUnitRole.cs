using UnityEngine;

public interface IUnitRole
{
    IUnitRole New();
    void Init(GameContext context, Unit unit);

    void Tick();

    void HandleWorkStates();
    void HandleCombatStates();

    void OnReachedTarget();

    void OnHit(float damage, Destructable source);

    #region Commanding

    bool Command(GridTile tile, Vector3 pos);

    bool Command(Agent agent);

    bool Command(Building building);
    #endregion
}
