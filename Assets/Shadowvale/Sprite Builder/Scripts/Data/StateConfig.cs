using UnityEngine;

namespace Shadowvale.SpriteSytem.Data
{
    [System.Serializable]
    public class StateConfig
    {
        public string name;
        public StateConfig(string name)
        {
            this.name = name;
        }
    }
}
