using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FinalDialogueLine
{
    [TextArea(2, 5)]
    public string text;
    public Sprite portrait;
}

public class FinalDialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Image portraitImage;
    public float textSpeed = 0.03f;

    [Header("Final Choice UI")]
    public GameObject choicePanel;
    public Button continueButton;
    public Button dumpButton;

    private string continueScene;
    private string dumpScene;
    private bool isFinalChoiceDialogue = false;

    private DialogueLine[] lines;
    private int index;
    private Coroutine typingCoroutine;
    private GameObject objectToDestroyAfter;

    void Awake()
    {
        gameObject.SetActive(false);

        // Ensure choice panel starts OFF
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    public void StartDialogue(DialogueLine[] newLines, GameObject destroyAfter = null)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueRunning(true);

        lines = newLines;
        index = 0;
        objectToDestroyAfter = destroyAfter;

        // Always reset this
        isFinalChoiceDialogue = false;

        gameObject.SetActive(true);

        StopTyping();
        ApplyPortrait();
        textComponent.text = "";

        typingCoroutine = StartCoroutine(TypeLine());
    }

    public void StartFinalChoiceDialogue(DialogueLine[] newLines, string continueSceneName, string dumpSceneName)
    {
        continueScene = continueSceneName;
        dumpScene = dumpSceneName;

        isFinalChoiceDialogue = true;

        // Start dialogue WITHOUT resetting the flag
        lines = newLines;
        index = 0;
        objectToDestroyAfter = null;

        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueRunning(true);

        gameObject.SetActive(true);

        StopTyping();
        ApplyPortrait();
        textComponent.text = "";

        typingCoroutine = StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (lines == null || lines.Length == 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text != lines[index].text)
            {
                StopTyping();
                textComponent.text = lines[index].text;
                return;
            }

            NextLine();
        }
    }

    IEnumerator TypeLine()
    {
        textComponent.text = "";

        string line = lines[index].text ?? "";
        foreach (char c in line)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            ApplyPortrait();

            StopTyping();
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            FinishDialogue();
        }
    }

    void FinishDialogue()
    {
        StopTyping();

        if (GameManager.Instance != null)
            GameManager.Instance.SetDialogueRunning(false);

        gameObject.SetActive(false);

        // THIS is where the panel gets enabled
        if (isFinalChoiceDialogue)
        {
            ShowChoices();
            isFinalChoiceDialogue = false;
            return;
        }

        if (objectToDestroyAfter != null)
        {
            Destroy(objectToDestroyAfter);
            objectToDestroyAfter = null;
        }
    }

    void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    void ApplyPortrait()
    {
        if (portraitImage == null) return;

        Sprite p = lines[index].portrait;
        portraitImage.sprite = p;
        portraitImage.enabled = (p != null);
    }

    void ShowChoices()
    {
        if (choicePanel == null)
        {
            Debug.LogError("Choice panel not assigned.");
            return;
        }

        choicePanel.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        dumpButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(continueScene);
        });

        dumpButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(dumpScene);
        });
    }
}