using UnityEngine;

public class FinalConvoTrigger : MonoBehaviour
{
    public FinalDialogue dialogueBox;
    public DialogueLine[] conversation;

    [Header("Final Choice")]
    public bool isFinalChoice = false;

    public string continueEndingScene;
    public string dumpEndingScene;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (isFinalChoice)
        {
            dialogueBox.StartFinalChoiceDialogue(conversation, continueEndingScene, dumpEndingScene);
        }
        else
        {
            dialogueBox.StartDialogue(conversation, gameObject);
        }
    }
}