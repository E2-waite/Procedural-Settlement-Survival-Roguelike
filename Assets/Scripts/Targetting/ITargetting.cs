using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public interface ITargetting
{
    List<TargetCandidate> Candidates => null;

    void Tick()
    {

    }
}
