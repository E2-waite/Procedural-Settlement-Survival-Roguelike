using UnityEngine;


public class MainFireBuilding : FireBuilding
{
    protected override void Start()
    {
        base.Start();
        fire.Light();
    }
}
