using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DialogueVR : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    [TextArea]
    public string[] phrases;

    private int index = 0;

    void Start()
    {
        dialogueText.text = phrases[index];
    }

    public void NextPhrase()
    {
        index++;

        if (index < phrases.Length)
        {
            dialogueText.text = phrases[index];
        }
        else
        {
            gameObject.SetActive(false); // скрыть диалог
        }
    }
}