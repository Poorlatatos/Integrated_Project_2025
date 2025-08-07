using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class ChecklistManager : MonoBehaviour
{
    public CanvasGroup checklistCanvasGroup; // Assign the CanvasGroup of ChecklistPanel
    public Transform checklistContent;       // Assign ChecklistContent (the Vertical Layout Group)
    public GameObject checklistItemPrefab;   // Assign the ChecklistItem prefab (with TextMeshProUGUI)
    public bool IsChecklistOpen => checklistCanvasGroup != null && checklistCanvasGroup.alpha > 0.5f;

    private Dictionary<string, TextMeshProUGUI> itemToText = new Dictionary<string, TextMeshProUGUI>();
    private InputAction toggleAction;

    // For sliding animation
    public RectTransform checklistPanelRect; // Assign the RectTransform of ChecklistPanel
    public Vector2 hiddenPosition = new Vector2(0, -400); // Off-screen (adjust as needed)
    public Vector2 shownPosition = new Vector2(0, 0);     // On-screen (adjust as needed)
    public float slideDuration = 0.4f;
    private Coroutine slideCoroutine;

    void Awake()
    {
        toggleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/v");
        toggleAction.performed += ctx => ToggleChecklist();
        toggleAction.Enable();

        Debug.Log("ChecklistManager initialized. Press 'V' to toggle checklist visibility.");

        // Hide checklist UI at startup
        if (checklistCanvasGroup != null)
        {
            checklistCanvasGroup.alpha = 0;
            checklistCanvasGroup.interactable = false;
            checklistCanvasGroup.blocksRaycasts = false;
        }
    }

    void OnDestroy()
    {
        toggleAction.Disable();
    }

    void ToggleChecklist()
    {
        bool show = checklistCanvasGroup.alpha == 0;
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideChecklist(show));
        checklistCanvasGroup.interactable = show;
        checklistCanvasGroup.blocksRaycasts = show;
    }

    private IEnumerator SlideChecklist(bool show)
    {
        checklistCanvasGroup.alpha = 1; // Always visible during animation
        Vector2 start = checklistPanelRect.anchoredPosition;
        Vector2 end = show ? shownPosition : hiddenPosition;
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            checklistPanelRect.anchoredPosition = Vector2.Lerp(start, end, elapsed / slideDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        checklistPanelRect.anchoredPosition = end;
        if (!show)
            checklistCanvasGroup.alpha = 0; // Hide after sliding out
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

    public bool AreAllItemsCollected()
    {
        foreach (var text in itemToText.Values)
        {
            // If the text is not crossed off (not gray), return false
            if (text.color != Color.gray)
                return false;
        }
        return true;
    }
}