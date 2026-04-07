using UnityEngine;

public class GameQuit : MonoBehaviour
{
    public void ExitGame()
    {
        // Debug message to confirm the button is working
        Debug.Log("Game is exiting...");

        #if UNITY_EDITOR
            // This line quits the game while running in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // This line quits the game in a standalone build (Windows/Mac/Linux)
            Application.Quit();
        #endif
    }
}
