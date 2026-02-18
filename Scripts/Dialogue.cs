using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialogue : MonoBehaviour
{


    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.03f;

    private int index;

    void Start()
    {


        textComponent.text = string.Empty;
        index = 0;

        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
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
            textComponent.text = string.Empty;

            StartCoroutine(TypeLine());
        }
        else
        {
            HideDialogue();
        }
    }

    void HideDialogue()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

}
