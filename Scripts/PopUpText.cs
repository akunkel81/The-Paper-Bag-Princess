using UnityEngine;
using System.Collections;

public class PopUpText : MonoBehaviour
{
    public GameObject popupMessage;

    void Start()
    {
        StartCoroutine(DisplayForSeconds());
    }

    IEnumerator DisplayForSeconds()
    {
        popupMessage.SetActive(true);
        yield return new WaitForSeconds(10f);
        popupMessage.SetActive(false);
    }
}