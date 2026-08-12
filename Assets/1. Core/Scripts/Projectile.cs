using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Destructable source, target;
    private float damage = 10;
    private float moveSpeed = 10f;
    public void Launch(Destructable from, Destructable to, float damage)
    {
        source = from;
        target = to;
        this.damage = damage;
        StartCoroutine(MoveRoutine());
    }
     
    IEnumerator MoveRoutine()
    {
        float dist = float.MaxValue;
        Vector3 hitDir = (target.transform.position - transform.position).normalized;
        Vector3 moveDir = Vector3.zero;
        while (dist > .1f)
        {
            if (target != null)
            {
                moveDir = target.transform.position - transform.position;
                dist = Vector3.Distance(transform.position, target.transform.position);
            }
                
            transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDir, moveSpeed * Time.deltaTime);

            yield return null;
        }

        target?.OnHit(source, damage, hitDir);

        Destroy(gameObject);
    }
}
