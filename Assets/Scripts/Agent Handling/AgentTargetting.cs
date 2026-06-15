using System.Collections.Generic;
using UnityEngine;

public class AgentTargetting
{
    List<TargetCandidate> targetCandidates = new List<TargetCandidate>();
    public List<TargetCandidate> Candidates => targetCandidates;

    public Destructable currentTarget;
    public Destructable Current => currentTarget;

    protected Agent agent;

    // Returns true if we have a target
    public bool HasTarget => currentTarget != null;

    public void Init(Agent agent)
    {
        this.agent = agent;
    }

    public virtual void Tick()
    {
        if (agent.IsDead) return;

        CheckTargets();
    }

    public void Target(Destructable target)
    {
        AddTarget(target, 100f);
        currentTarget = target;
    }

    // Returns the target's grid position
    public Vector2Int TargetPos()
    {
        return currentTarget != null ? currentTarget.GridPos : Vector2Int.zero;
    }

    // Add threat to the threat candidate associated with the target
    public void AddTarget(Destructable target, float threat)
    {
        if (target == null) return;

        TargetCandidate targetCandidate = GetCandidate(target);
        if (targetCandidate == null)
        {
            Candidates.Add(NewCandidate(target, threat));
        }
    }

    public void AddThreat(Destructable target, float threat)
    {
        TargetCandidate targetCandidate = GetCandidate(target);

        if (targetCandidate == null)
        {
            Candidates.Add(NewCandidate(target, threat));
        }
        else
        {
            targetCandidate.threat += threat;
        }
    }

    private TargetCandidate NewCandidate(Destructable target, float threat) =>
        new TargetCandidate()
        {
            target = target,
            baseThreat = threat
        };

    public TargetCandidate GetCandidate(Destructable target)
    {
        // Check if we already have this target
        foreach (TargetCandidate existing in Candidates)
        {
            if (existing.target == target)
            {
                return existing;
            }
        }
        return null;
    }

    // Gets the candidate with the highest threat and targets it
    private void CheckTargets()
    {
        UpdateThreat();

        TargetCandidate highestThreat = HighestThreat();

        if (highestThreat != null && highestThreat.target != Current)
        {
            // TODO: have a threshold to ensure it doesn't continuously switch targets when threat is close
            Target(highestThreat.target);
        }
    }

    private void UpdateThreat()
    {
        // Iterate backwards so candidates can be removed while scanning.
        for (int i = Candidates.Count - 1; i >= 0; i--)
        {
            TargetCandidate candidate = Candidates[i];
            if (candidate == null || candidate.target == null) // Remove null (dead) candidates
            {
                Candidates.RemoveAt(i);
                continue;
            }
            else if (candidate.target.isActiveAndEnabled)
            {
                float dist = Vector3.Distance(agent.transform.position, candidate.target.transform.position);
                if (dist <= 10)
                {
                    float threat = 10 - dist;
                    candidate.threat = threat;
                }
            }
        }
    }

    // Returns the threat candidate that currently has the highest threat
    TargetCandidate HighestThreat()
    {
        float highestVal = 0;
        TargetCandidate highestThreat = null;
        for (int i = Candidates.Count - 1; i >= 0; i--)
        {
            TargetCandidate candidate = Candidates[i];
            float threat = candidate.baseThreat + candidate.threat;
            if (threat > highestVal)
            {
                highestVal = threat;
                highestThreat = candidate;
            }
        }

        return highestThreat;
    }

    //public void AddThreat(Destructable target, float threat)
    //{
    //    TargetCandidate targetCandidate = GetCandidate(target);
    //    if (targetCandidate == null)
    //    {
    //        // If no candidate with target exists, set threat and add to candidates list.
    //        TargetCandidate newCandidate = new TargetCandidate()
    //        {
    //            target = target,
    //            baseThreat = threat
    //        };

    //        Candidates.Add(newCandidate);
    //    }
    //    else
    //    {
    //        targetCandidate.threat += threat;
    //    }
    //}

    
}
