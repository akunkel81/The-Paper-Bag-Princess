using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null.");
        }
    }
}