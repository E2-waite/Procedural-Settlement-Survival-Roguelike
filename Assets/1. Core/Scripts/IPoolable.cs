using UnityEngine;

public interface IPoolable
{
    public void Init();
    public void OnGet();
    public void OnReturn();
}
