using UnityEngine;

namespace Shadowvale.SpriteSytem.Data
{
    [System.Serializable]
    public class SpriteData
    {
        public UnityEngine.Sprite sprite = null;

        public SpriteData()
        {

        }

        public SpriteData(SpriteData other)
        {
            sprite = other.sprite;
        }
    }
}
