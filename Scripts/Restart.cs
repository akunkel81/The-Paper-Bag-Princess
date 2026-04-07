using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Restart();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null.");
        }
    }
}