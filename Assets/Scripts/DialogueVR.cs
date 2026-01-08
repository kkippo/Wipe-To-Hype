using System;
using TMPro;
using UnityEngine;

public class DialogueVR : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    [TextArea]
    public string[] phrases;

    public GameObject nextButton;
    public GameObject prevButton;

    private int index = 0;

    // ===== Task counters =====
    private int totalCoins;
    private int totalCubes;

    private int collectedCoins = 0;
    private int collectedCubes = 0;

    // ===== Bottle break system =====
    private int brokenBottles = 0;
    private bool isBottleBroken = false;
    private int totalBreakPieces = 0;
    private int cleanedBreakPieces = 0;
    public Material witchNormalFace;
    public Material witchScaredFace;
    public Renderer witchFaceRenderer;
    private int normalMaterialIndex = 0;
    public AudioClip bottleBreakSound;
    private AudioSource audioSource;

    // Шаблон финальной фразы при разбитии (добавим динамически в конец массива)
    private string breakEventPhraseTemplate = "АААА! Ты разбил, убери.\nОсколки: {cleanedBreak} / {totalBreak}";

    void Start()
    {
        // Добавляем финальную фразу в конец массива
        System.Array.Resize(ref phrases, phrases.Length + 1);
        phrases[phrases.Length - 1] = breakEventPhraseTemplate;

        // Аудио для звука разбития
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        CountTotalTasks();
        index = 0;
        UpdateDialogue();
    }

    // ================= Dialogue =================

    public void NextPhrase()
    {
        if (index < phrases.Length - 1)
        {
            index++;
            UpdateDialogue();
        }
    }

    public void PrevPhrase()
    {
        if (index > 0)
        {
            index--;
            UpdateDialogue();
        }
    }

    private void UpdateDialogue()
    {
        string text = phrases[index];

        text = text.Replace("{coins}", $"{collectedCoins} / {totalCoins}");
        text = text.Replace("{cubes}", $"{collectedCubes} / {totalCubes}");
        text = text.Replace("{broken}", $"{brokenBottles}");
        text = text.Replace("{cleanedBreak}", $"{cleanedBreakPieces}");
        text = text.Replace("{totalBreak}", $"{totalBreakPieces}");

        dialogueText.text = text;

        prevButton.SetActive(index > 0);
        nextButton.SetActive(index < phrases.Length - 1);
    }

    // ================= Tasks =================

    private void CountTotalTasks()
    {
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        totalCubes = GameObject.FindGameObjectsWithTag("Cube").Length;
    }

    public void OnCoinCollected()
    {
        if (collectedCoins < totalCoins)
        {
            collectedCoins++;
            UpdateDialogue();
        }
    }

    public void OnCubeCollected()
    {
        if (collectedCubes < totalCubes)
        {
            collectedCubes++;
            UpdateDialogue();
        }
    }

    // ================= Bottle Break System =================

    public void OnBottleBreak()
    {
        brokenBottles++;
        isBottleBroken = true;
        cleanedBreakPieces = 0;
        totalBreakPieces = GameObject.FindGameObjectsWithTag("Break").Length;

        // Звук разбития
        if (audioSource != null && bottleBreakSound != null)
        {
            audioSource.PlayOneShot(bottleBreakSound);
        }

        // Change witch face to scared
        if (witchFaceRenderer != null && witchScaredFace != null)
        {
            Material[] mats = witchFaceRenderer.materials;
            mats[normalMaterialIndex] = witchScaredFace;
            witchFaceRenderer.materials = mats;
        }

        // Jump to the last phrase (break message)
        index = phrases.Length - 1;
        UpdateDialogue();
    }

    // Явный вызов при уборке осколков/колбы
    public void OnBottleCleaned()
    {
        if (!isBottleBroken)
            return;

        cleanedBreakPieces++;

        // Если все осколки убраны — вернуть лицо
        if (cleanedBreakPieces >= totalBreakPieces && totalBreakPieces > 0)
        {
            isBottleBroken = false;

            if (witchFaceRenderer != null && witchNormalFace != null)
            {
                Material[] mats = witchFaceRenderer.materials;
                mats[normalMaterialIndex] = witchNormalFace;
                witchFaceRenderer.materials = mats;
            }
        }

        UpdateDialogue();
    }
}
