using UnityEngine;

public interface IUnitRole
{
    void Init(GameContext context, Unit unit);

    void Tick();

    void HandleStates();

    void OnReachedTarget();

    void OnHit(float damage, Destructable source);

    #region Commanding

    bool Command(GridTile tile);

    bool Command(Agent agent);

    bool Command(Building building);
    #endregion
}
