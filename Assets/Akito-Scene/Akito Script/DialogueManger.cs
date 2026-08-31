using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 4)]
        public string text;
        public AudioClip voice;

        [Header("Speaker (optional — leave empty for narration)")]
        public Animator speakerAnimator;
        public string animationName;

        [Header("Typing")]
        [Tooltip("Characters per second. 0 = use manager default.")]
        public float typeSpeedOverride = 0f;

        [Header("Events")]
        [Tooltip("Fires the instant this line STARTS showing. Use this for jumpscare SFX, camera shake, lights, etc.")]
        public UnityEvent onLineStart;
    }

    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;

    [Header("UI / Output")]
    public TMP_Text dialogueText;
    public AudioSource audioSource;

    [Header("Typing Settings")]
    public float defaultCharsPerSecond = 40f;

    [Header("On Dialogue End")]
    [Tooltip("Hook up ForkSequence, a choice canvas, or anything else here — no code edits needed per scene.")]
    public UnityEvent onDialogueEnd;

    private int currentLine = 0;
    private bool dialogueActive = false;
    private bool isTyping = false;
    private Coroutine typeRoutine;

    void Update()
    {
        if (!dialogueActive)
            return;

        // Keyboard AND mouse both advance — more accessible, easier to test in editor.
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // First click: snap-finish the current line instantly.
                CompleteLineInstantly();
            }
            else
            {
                NextLine();
            }
        }
    }

    // Called by Timeline Signal
    public void StartDialogue()
    {
        currentLine = 0;
        dialogueActive = true;
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines[currentLine];

        // Per-line hook — jumpscare SFX, camera shake, light flicker, whatever you assign in Inspector.
        line.onLineStart?.Invoke();

        // Voice
        audioSource.Stop();
        if (line.voice != null)
        {
            audioSource.clip = line.voice;
            audioSource.Play();
        }

        // Animation — now works for ANY character, not just "wife"
        if (line.speakerAnimator != null && !string.IsNullOrEmpty(line.animationName))
        {
            line.speakerAnimator.Play(line.animationName, 0, 0f);
        }

        // Typewriter
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        float speed = line.typeSpeedOverride > 0f ? line.typeSpeedOverride : defaultCharsPerSecond;
        typeRoutine = StartCoroutine(TypeText(line.text, speed));
    }

    IEnumerator TypeText(string text, float charsPerSecond)
    {
        isTyping = true;
        dialogueText.text = "";
        float delay = 1f / Mathf.Max(charsPerSecond, 1f);

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    void CompleteLineInstantly()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        dialogueText.text = dialogueLines[currentLine].text;
        isTyping = false;
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
        audioSource.Stop();

        // Decoupled — hook up ForkSequence, a choice canvas, next Timeline, whatever this scene needs.
        onDialogueEnd?.Invoke();
    }
}