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
        if (lines == null || lines.Length == 0)
        {
            return;
        }

        if (output == null)
        {
            // Try to find a TMP_Text in children to reduce inspector setup mistakes
            output = GetComponentInChildren<TMP_Text>();
            if (output == null)
            {
                return;
            }
        }

        if (index < lines.Length)
        {
            output.text = lines[index];
            index++;
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
