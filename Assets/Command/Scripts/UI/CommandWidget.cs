using Cinderwild.Command.Data;
using Cinderwild.Command.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cinderwild.Command.UI
{
    public class CommandWidget : MonoBehaviour
    {
        [SerializeField] private CommandWidgetSlot slotPrefab;
        [SerializeField] private Sprite defaultSelector; // The default selector sprite
        [SerializeField] private Sprite snappedSelector; // The sprite shown when in snapped position
        [SerializeField] private Image selector;
        [SerializeField] private float snapThresh = 1f;
        [SerializeField] private float bounds = 80f;
        [SerializeField] private float slotDist = 80f;
        [SerializeField] private float defaultSelectorSize = 50f;
        [SerializeField] private int numSlots = 1;
        private List<CommandWidgetSlot> slots = new List<CommandWidgetSlot>();
        private RectTransform rect;
        CommandWidgetSlot selectedSlot = null;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public void Show(Vector2 position, List<CommandOption> options)
        {
            if (numSlots == 0) return;

            gameObject.SetActive(true);
            rect.position = position;
            selector.rectTransform.position = position;
            UpdateSlots(options);
        }

        public CommandType Hide()
        {
            CommandType commandType = selectedSlot ? selectedSlot.CommandType : CommandType.None;
            selector.sprite = defaultSelector;
            selector.rectTransform.sizeDelta = new Vector2(defaultSelectorSize, defaultSelectorSize);
            selectedSlot = null;
            gameObject.SetActive(false);
            return commandType;
        }

        private void UpdateSlots(List<CommandOption> options)
        {
            // Enable/instantiate required slots
            for (int i = 0; i < options.Count; i++)
            {
                if (i >= slots.Count)
                {
                    CommandWidgetSlot slot = Instantiate(slotPrefab, transform);
                    slot.Init();
                    slots.Add(slot);
                }

                slots[i].gameObject.SetActive(true);
                slots[i].SetOption(options[i]);
            }

            // Disable non-required slots
            for (int i = options.Count; i < slots.Count; i++)
            {
                slots[i].gameObject.SetActive(false);
            }

            // Update slot position based on number of options
            float angleStep = 360f / options.Count;
            for (int i = 0; i < options.Count; i++)
            {
                float angle = (i * angleStep) + 90f;

                Vector2 direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad));

                Vector2 widgetPos = rect.position;
                slots[i].Rect.position = widgetPos + direction * slotDist;
            }
        }

        public void UpdateSelector(Vector2 delta)
        {
            selectedSlot = null;
            Vector2 center = rect.position;
            Vector2 selectorPos = selector.rectTransform.position;
            selectorPos += delta;
            Vector2 offset = selectorPos - center;

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
                selector.sprite = defaultSelector;
            }
            else
            {
                // Snap to slot
                selector.rectTransform.position = selectedSlot.Rect.position;
                selector.rectTransform.sizeDelta = new Vector2(100, 100);
                selector.sprite = snappedSelector;
            }
        }
    }
}
