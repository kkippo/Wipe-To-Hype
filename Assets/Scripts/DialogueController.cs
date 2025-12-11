using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    // Use TMP_Text so both TextMeshPro (3D) and TextMeshProUGUI (UI) are supported
    public TMP_Text output;
    public string[] lines;
    private int index = 0;

    // Optional: if true, this component will call NextLine when something enters its trigger.
    // Leave false if you call NextLine from another script or via UnityEvent in the inspector.
    public bool triggerOnEnter = false;
    // Tag to filter which collider triggers the dialogue (default: Player). Empty = no filter.
    public string triggerTag = "Player";

    public void NextLine()
    {
        Debug.Log($"DialogueController.NextLine called (index={index}, lines={(lines==null?0:lines.Length)})");

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("DialogueController: lines array is empty or null.");
            return;
        }

        if (output == null)
        {
            // Try to find a TMP_Text in children to reduce inspector setup mistakes
            output = GetComponentInChildren<TMP_Text>();
            if (output == null)
            {
                Debug.LogWarning("DialogueController: output (TMP_Text) is not assigned and none found in children.");
                return;
            }
        }

        if (index < lines.Length)
        {
            output.text = lines[index];
            index++;
        }
        else
        {
            Debug.Log("DialogueController: reached end of lines.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnter)
            return;

        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag))
            return;

        NextLine();
    }
}
