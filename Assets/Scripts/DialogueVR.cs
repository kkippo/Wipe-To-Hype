using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueVR : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    [TextArea]
    public string[] phrases;

    public GameObject nextButton;
    public GameObject prevButton;

    // ===== Victory UI =====
    public GameObject dialogueWindow;
    public GameObject victoryWindow;

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

    private bool breakPhraseAdded = false;
    private string breakEventPhraseTemplate = "АААА! Ты разбил, убери.\nОсколки: {cleanedBreak} / {totalBreak}";

    // ===== Witch Face =====
    public Material witchNormalFace;
    public Material witchScaredFace;
    public Renderer witchFaceRenderer;
    private int normalMaterialIndex = 2;
    public Animator witchFaceAnimator;

    public AudioClip bottleBreakSound;
    public AudioClip witchSpeechSound;
    private AudioSource audioSource;

    private bool hasShowedVictory = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        CountTotalTasks();
        index = 0;
        UpdateDialogue();

        // Проверяем победу сразу на старте
        CheckAndShowVictory();
    }

    // ================= Dialogue =================

    public void NextPhrase()
    {
        if (hasShowedVictory) return;

        if (index < phrases.Length - 1)
        {
            index++;
            UpdateDialogue();
            PlayTalkingAnimation();
        }
    }

    public void PrevPhrase()
    {
        if (hasShowedVictory) return;

        if (index > 0)
        {
            index--;
            UpdateDialogue();
            PlayTalkingAnimation();
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
            CheckAndShowVictory();
        }
    }

    public void OnCubeCollected()
    {
        if (collectedCubes < totalCubes)
        {
            collectedCubes++;
            UpdateDialogue();
            CheckAndShowVictory();
        }
    }

    // ================= Bottle Break System =================

    public void OnBottleBreak()
    {
        brokenBottles++;
        isBottleBroken = true;
        cleanedBreakPieces = 0;
        totalBreakPieces = GameObject.FindGameObjectsWithTag("Break").Length;

        if (!breakPhraseAdded)
        {
            string[] newPhrases = new string[phrases.Length + 1];
            phrases.CopyTo(newPhrases, 0);
            newPhrases[newPhrases.Length - 1] = breakEventPhraseTemplate;
            phrases = newPhrases;
            breakPhraseAdded = true;
        }

        if (audioSource != null && bottleBreakSound != null)
            audioSource.PlayOneShot(bottleBreakSound);

        if (witchFaceRenderer != null && witchScaredFace != null)
        {
            Material[] mats = witchFaceRenderer.materials;
            mats[normalMaterialIndex] = witchScaredFace;
            witchFaceRenderer.materials = mats;
        }

        if (witchFaceAnimator != null)
            witchFaceAnimator.SetBool("IsScared", true);

        index = phrases.Length - 1;
        UpdateDialogue();
    }

    public void OnBottleCleaned()
    {
        if (!isBottleBroken)
            return;

        cleanedBreakPieces++;

        if (cleanedBreakPieces >= totalBreakPieces && totalBreakPieces > 0)
        {
            isBottleBroken = false;

            if (witchFaceAnimator != null)
                witchFaceAnimator.SetBool("IsScared", false);

            if (witchFaceRenderer != null && witchNormalFace != null)
            {
                Material[] mats = witchFaceRenderer.materials;
                mats[normalMaterialIndex] = witchNormalFace;
                witchFaceRenderer.materials = mats;
            }
        }

        UpdateDialogue();
        CheckAndShowVictory();
    }

    // ================= Talking Animation =================

    private void PlayTalkingAnimation()
    {
        if (witchFaceAnimator != null)
        {
            witchFaceAnimator.SetTrigger("IsTalking");
            StartCoroutine(ResetTalkingCoroutine());
        }
    }

    private IEnumerator ResetTalkingCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (witchFaceAnimator != null && !isBottleBroken)
            witchFaceAnimator.ResetTrigger("IsTalking");
    }

    // ================= Victory =================

    private void CheckAndShowVictory()
    {
        if (hasShowedVictory) return;

        if (AreAllItemsCleaned())
        {
            hasShowedVictory = true;

            // Сразу показываем Win-панель
            if (dialogueWindow != null)
                dialogueWindow.SetActive(false);

            if (victoryWindow != null)
                victoryWindow.SetActive(true);

            if (audioSource != null && witchSpeechSound != null)
                audioSource.PlayOneShot(witchSpeechSound);
        }
    }

    public bool AreAllItemsCleaned()
    {
        bool coinsDone = collectedCoins >= totalCoins;
        bool cubesDone = collectedCubes >= totalCubes;

        // Если бутылка не была разбита, считаем осколки выполненными
        bool breakDone = !breakPhraseAdded || cleanedBreakPieces >= totalBreakPieces;

        return coinsDone && cubesDone && breakDone;
    }
}
