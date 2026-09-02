using UnityEngine;
using UnityEngine.UI;

namespace Cinderwild.Command.UI
{
    public class CommandWidget : MonoBehaviour
    {
        [SerializeField] private Image selector;
        [SerializeField] private float snapThresh = 1f;
        [SerializeField] private float bounds = 80f;
        private CommandWidgetSlot[] slots = new CommandWidgetSlot[4];
        private RectTransform rect;
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
        }

        public void Hide()
        {
            selectedSlot = null;
            gameObject.SetActive(false);
        }

        public void UpdateSelector(Vector2 delta)
        {
            // TODO: make unsnapping easier
            Vector2 center = rect.position;
            Vector2 selectorPos = selector.rectTransform.position;
            selectorPos += delta;

            Vector2 offset = selectorPos - center;

            // Clamp position within bounds
            if (offset.magnitude > bounds)
                offset = offset.normalized * bounds;

            Vector2 targetPos = center + offset;

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
                selector.rectTransform.sizeDelta = new Vector2(75, 75);
            }
            else
            {
                // Snap to slot
                selector.rectTransform.position = selectedSlot.Rect.position;
                selector.rectTransform.sizeDelta = new Vector2(130, 130);
            }
        }
    }
}
