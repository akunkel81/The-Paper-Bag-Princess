using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
    public Dialogue dialogueBox;
    public DialogueLine[] conversation;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        dialogueBox.StartDialogue(conversation, gameObject);
    }
}