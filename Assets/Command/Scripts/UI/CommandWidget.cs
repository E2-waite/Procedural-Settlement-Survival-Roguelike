using UnityEngine;
using UnityEngine.UI;

namespace Cinderwild.Command.UI
{
    public class CommandWidget : MonoBehaviour
    {
        [SerializeField] private Sprite defaultSelector; // The default selector sprite
        [SerializeField] private Sprite snappedSelector; // The sprite shown when in snapped position
        [SerializeField] private Image selector;
        [SerializeField] private float snapThresh = 1f;
        [SerializeField] private float bounds = 80f;
        [SerializeField] private float defaultSelectorSize = 50f;
        private CommandWidgetSlot[] slots = new CommandWidgetSlot[4];
        private RectTransform rect;
        private bool hasSnapped = false; // For hiding selector after snapping when not snapped
        CommandWidgetSlot selectedSlot = null;

        private void Start()
        {
            rect = GetComponent<RectTransform>();

            for (int i = 0; i < 4; i++)
            {
                Transform child = transform.GetChild(i);
                slots[i] = child?.GetComponent<CommandWidgetSlot>();
            }

            gameObject.SetActive(false);
        }

        public void Show(Vector2 position)
        {
            gameObject.SetActive(true);
            rect.position = position;
            selector.rectTransform.position = position;
            selector.gameObject.SetActive(true);
        }

        public void Hide()
        {
            selector.sprite = defaultSelector;
            selector.rectTransform.sizeDelta = new Vector2(defaultSelectorSize, defaultSelectorSize);
            selectedSlot = null;
            hasSnapped = false;
            gameObject.SetActive(false);
        }

        public void UpdateSelector(Vector2 delta)
        {
            // TODO: make unsnapping easier
            selectedSlot = null;
            Vector2 center = rect.position;
            Vector2 selectorPos = selector.rectTransform.position;
            selectorPos += delta;
            Vector2 offset = selectorPos - center;


            // Restrict movement to horizontal or vertical
            if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            {
                offset.y = 0f;
            }
            else if (Mathf.Abs(offset.x) < Mathf.Abs(offset.y))
            {
                offset.x = 0f;
            }

            // Clamp position within bounds
            if (offset.magnitude > bounds)
                offset = offset.normalized * bounds;

            Vector2 targetPos = center + offset;

            // Clamp position within bounds
            if (offset.magnitude > bounds)
                offset = offset.normalized * bounds;

            // Handle snapping and selecting specific slots
            foreach (CommandWidgetSlot slot in slots)
            {
                if (slot == null) continue;

                float dist = Vector2.Distance(slot.Rect.position, targetPos);

                if (dist <= snapThresh)
                {
                    selectedSlot = slot;
                }
            }

            if (selectedSlot == null)
            {
                // Move within bounds
                selector.rectTransform.position = targetPos;
                selector.rectTransform.sizeDelta = new Vector2(defaultSelectorSize, defaultSelectorSize);

                if (hasSnapped)
                {
                    selector.gameObject.SetActive(false);
                }
                else
                {
                    selector.sprite = defaultSelector;
                }
            }
            else
            {
                // Snap to slot
                selector.gameObject.SetActive(true);
                selector.rectTransform.position = selectedSlot.Rect.position;
                selector.rectTransform.sizeDelta = new Vector2(100, 100);
                selector.sprite = snappedSelector;
                hasSnapped = true;
            }
        }
    }
}
