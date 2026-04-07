using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;
    public Sprite portrait;

}

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Image portraitImage;
    public float textSpeed = 0.03f;

    private DialogueLine[] lines;
    private int index;
    private Coroutine typingCoroutine;
    private GameObject objectToDestroyAfter;
    
    [Header("Final Choice UI")]
    public GameObject choicePanel;
    public UnityEngine.UI.Button continueButton;
    public UnityEngine.UI.Button dumpButton;

    private string continueScene;
    private string dumpScene;
    private bool isFinalChoiceDialogue = false;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueLine[] newLines, GameObject destroyAfter = null)
    {
        if (GameManager.Instance != null)
        GameManager.Instance.SetDialogueRunning(true);
        
        lines = newLines;
        index = 0;
        objectToDestroyAfter = destroyAfter;

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

        StartDialogue(newLines, null);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (lines == null || lines.Length == 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            // If the line is still typing, finish it instantly
            if (textComponent.text != lines[index].text)
            {
                StopTyping();
                textComponent.text = lines[index].text;
                return;
            }

            // If the line is already finished, advance
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

    void EndDialogue()
    {
        gameObject.SetActive(false);

        if (isFinalChoiceDialogue)
        {
            ShowChoices();
            isFinalChoiceDialogue = false;
            return;
        }

        if (objectToDestroyAfter != null)
        {
            Destroy(objectToDestroyAfter);
        }
    }
    void ShowChoices()
    {
        choicePanel.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        dumpButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(continueScene);
        });

        dumpButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(dumpScene);
        });
    }
}