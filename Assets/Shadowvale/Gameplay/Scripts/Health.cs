using UnityEngine;

namespace Shadowvale.Gameplay
{
    [System.Serializable]
    public class Health
    {
        [SerializeField] protected float current = 0, max = 100;

        public void Set(float val)
        {
            current = val;
        }

        public void Fill()
        {
            current = max;
        }

        public void Damage(float val)
        {
            current = Mathf.Clamp(current - val, 0, max);
        }

        public void Heal(float val)
        {
            current = Mathf.Clamp(current + val, 0, max);
        }

        public bool Full()
        {
            return current >= max;
        }

        public bool IsEmpty { get { return current <= 0; } }
    }
}
