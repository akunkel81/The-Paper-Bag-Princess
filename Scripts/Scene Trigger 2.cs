using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTrigger2 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("TrollFight");
        }
    }

}