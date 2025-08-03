using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class ChecklistManager : MonoBehaviour
{
    public CanvasGroup checklistCanvasGroup; // Assign the CanvasGroup of ChecklistPanel
    public Transform checklistContent;       // Assign ChecklistContent (the Vertical Layout Group)
    public GameObject checklistItemPrefab;   // Assign the ChecklistItem prefab (with TextMeshProUGUI)

    private Dictionary<string, TextMeshProUGUI> itemToText = new Dictionary<string, TextMeshProUGUI>();
    private InputAction toggleAction;

    void Awake()
    {
        toggleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/v");
        toggleAction.performed += ctx => ToggleChecklist();
        toggleAction.Enable();

        Debug.Log("ChecklistManager initialized. Press 'V' to toggle checklist visibility.");
    }

    void OnDestroy()
    {
        toggleAction.Disable();
    }

    void ToggleChecklist()
    {
        bool show = checklistCanvasGroup.alpha == 0;
        checklistCanvasGroup.alpha = show ? 1 : 0;
        checklistCanvasGroup.interactable = show;
        checklistCanvasGroup.blocksRaycasts = show;
    }

    // Called by RandomItemSpawner after spawning items
    public void PopulateChecklist(GameObject[] items)
    {
        foreach (Transform child in checklistContent)
            Destroy(child.gameObject);
        itemToText.Clear();

        foreach (GameObject item in items)
        {
            GameObject entry = Instantiate(checklistItemPrefab, checklistContent);
            TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();
            text.text = item.name;
            itemToText[item.name] = text;
        }
    }

    // Call this when an item is picked up
    public void CrossOffItem(string itemName)
    {
        if (itemToText.TryGetValue(itemName, out TextMeshProUGUI text))
        {
            text.text = $"<s>{itemName}</s>"; // Strikethrough using TMP rich text
            text.color = Color.gray;
        }
    }

    public void RegisterAndCrossOff(string itemName)
    {
        // If not already on the checklist, add it
        if (!itemToText.ContainsKey(itemName))
        {
            GameObject entry = Instantiate(checklistItemPrefab, checklistContent);
            TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();
            text.text = itemName;
            itemToText[itemName] = text;
        }
        // Cross it off
        CrossOffItem(itemName);
    }
}