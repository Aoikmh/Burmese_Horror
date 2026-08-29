using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public AudioClip voice;
    }

    public DialogueLine[] dialogueLines;
    public TMP_Text dialogueText;
    public AudioSource audioSource;

    public ForkSequence forkSequence;
    public GameObject choiceCanvas;

    private int currentLine = 0;
    private bool dialogueActive = false;

    void Start()
    {
        // Dialogue does NOT start automatically.
        // Timeline Signal will call StartDialogue().
    }

    public void StartDialogue()
    {
        currentLine = 0;
        dialogueActive = true;

        // Make sure the dialogue UI is visible
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(true);
        }

        ShowLine();
    }

    void Update()
    {
        // Ignore mouse clicks until dialogue has actually started
        if (!dialogueActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = dialogueLines[currentLine].text;

        if (dialogueLines[currentLine].voice != null)
        {
            audioSource.clip = dialogueLines[currentLine].voice;
            audioSource.Play();
        }
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueActive = false;

        dialogueText.text = "";

        // Start the fork sequence after the conversation ends
        if (forkSequence != null)
        {
            forkSequence.StartForkSequence();
        }
    }
}