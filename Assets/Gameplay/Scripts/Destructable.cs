using System.Collections;
using UnityEngine;

namespace Cinderwild.Gameplay
{
    public class Destructable : MonoBehaviour
    {
        public enum TargetType
        {
            Agent,
            Building,
            Fire,
            Player,
            Max
        }
        public virtual TargetType Type => TargetType.Agent;
        [SerializeField] protected Health health = new Health();
        public Health Health => health;

        public float deathTime = 0.5f;
        private bool dead = false;
        public bool IsDead => dead;
        public virtual Vector2Int GridPos => new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

        public virtual Vector3 WorldPos => transform.position;


        // Handles receiving hits from units. Returns true if target is dead
        public virtual bool OnHit(Destructable source, float damage, Vector3 dir)
        {
            if (dead) return true; // Already dead
            else
                Debug.Log(name + " hit by " + source.name + "(" + damage + " dmg)");

            health.Damage(damage);

            StartCoroutine(HitRoutine(dir));

            if (!dead && health.IsEmpty)
            {
                Debug.Log(name + " SHOULD DIE!!!");
                dead = true;
                StartCoroutine(DeathRoutine());
                return true;
            }

            return false;
        }


        protected virtual IEnumerator HitRoutine(Vector3 dir)
        {
            yield return null;
        }

        // Handle death
        protected virtual void OnDeathStart()
        {
        }

        protected virtual void OnDeathFinish()
        {
            Destroy(gameObject);
        }

        // Delayed death
        protected virtual IEnumerator DeathRoutine()
        {
            OnDeathStart();
            yield return new WaitForSeconds(deathTime);
            OnDeathFinish();
        }
    }
}