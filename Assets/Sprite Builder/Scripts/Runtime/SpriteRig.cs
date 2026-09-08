using Cinderwild.SpriteSytem.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.SpriteSytem.Runtime
{
    public class SpriteRig : MonoBehaviour
    {
        public Transform body;
        public SpriteConfig config;
        [SerializeField] private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        private void LateUpdate()
        {
            Camera cam = Camera.main;

            if (cam == null) return;

            Vector3 direction = cam.transform.position - transform.position;
            //direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = cam.transform.rotation;
            }
        }

        public void Init()
        {
            config?.Init();
        }

        public void Build()
        {
            Clear();
            for (int i = 0; i < config.LayerCount; i++)
            {
                Layer layer = config.GetLayer(i);
                if (layer == null) continue;

                GameObject layerObj = new GameObject(layer.name);
                layerObj.transform.parent = transform;
                layerObj.transform.position = Vector3.zero;
                layerObj.transform.rotation = Quaternion.identity;
                layerObj.transform.localScale = Vector3.one;

                SpriteRenderer rend = layerObj.AddComponent<SpriteRenderer>();
                rend.sprite = config.GetSprite(i);
                rend.color = config.GetColor(i);
                rend.sortingOrder = config.LayerCount - i;
                renderers.Add(rend);

                layerObj.SetActive(layer.visible);
            }
        }

        public void Clear()
        {
            for (int i = renderers.Count - 1; i >= 0; i--)
            {
                if (renderers[i] != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(renderers[i].gameObject);
                    else
#endif
                        Destroy(renderers[i].gameObject);
                }
            }
            renderers.Clear();
        }

        public void UpdateDirection(Direction dir)
        {
            for (int i = 0; i < config.LayerCount; i++)
            {
                Layer layer = config.GetLayer(i);
                if (layer == null) continue;

                SpriteRenderer rend = renderers[i];

                if (rend != null)
                    rend.sprite = config.GetSprite(i, dir);
            }
        }

        // Enable/disable layers with the passed tag
        public void Display(bool enable, string tag = "")
        {
            if (tag == "")
            {
                foreach (SpriteRenderer rend in renderers)
                {
                    rend.gameObject.SetActive(enable);
                }
            }
            else
            {
                List<Layer> layers = config?.GetLayers(tag);

                foreach (Layer layer in layers)
                {
                    int id = layer.id;
                    SpriteRenderer rend = renderers[id];
                    rend.gameObject.SetActive(enable);
                }
            }


        }

        public void SetColor(Color color, string tag = "")
        {
            if (tag == "")
            {
                foreach (SpriteRenderer rend in renderers)
                {
                    rend.color = color;
                }
            }
            else
            {
                List<Layer> layers = config?.GetLayers(tag);

                foreach (Layer layer in layers)
                {
                    int id = layer.id;
                    SpriteRenderer rend = renderers[id];
                    rend.color = color;
                }
            }
        }
    }
}
