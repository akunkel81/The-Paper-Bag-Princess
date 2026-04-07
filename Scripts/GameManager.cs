using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName;

    public string SceneName
    {
        get { return sceneName; }
    }

#if UNITY_EDITOR
    public void UpdateSceneName()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool dialogueRunning;

    [Header("Scene Order")]
    public List<SceneReference> sceneOrder = new List<SceneReference>();

    [Header("Scenes")]
    public SceneReference gameOverScene;
    public SceneReference mainMenuScene;

    private int currentSceneIndex = 0;

    // Stores the scene the player was in before Game Over
    private string lastGameplaySceneName = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateCurrentSceneIndex();
    }

    void UpdateCurrentSceneIndex()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < sceneOrder.Count; i++)
        {
            if (sceneOrder[i] != null && sceneOrder[i].SceneName == currentScene)
            {
                currentSceneIndex = i;
                return;
            }
        }

        Debug.LogWarning("Current scene not found in sceneOrder list.");
    }

    public void SetDialogueRunning(bool isRunning)
    {
        dialogueRunning = isRunning;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        // Remember where the player lost
        lastGameplaySceneName = SceneManager.GetActiveScene().name;

        if (gameOverScene != null && !string.IsNullOrEmpty(gameOverScene.SceneName))
        {
            SceneManager.LoadScene(gameOverScene.SceneName);
        }
        else
        {
            Debug.LogWarning("Game Over scene is not assigned.");
        }
    }

    public void Restart()
    {
        // If coming from Game Over, go back to the scene where the player lost
        if (!string.IsNullOrEmpty(lastGameplaySceneName))
        {
            SceneManager.LoadScene(lastGameplaySceneName);
            return;
        }

        // Otherwise start from the first scene in the queue
        if (sceneOrder.Count > 0 && !string.IsNullOrEmpty(sceneOrder[0].SceneName))
        {
            SceneManager.LoadScene(sceneOrder[0].SceneName);
        }
        else
        {
            Debug.LogWarning("No first scene found in sceneOrder.");
        }
    }

    public void Begin()
    {
        if (sceneOrder.Count > 0 && !string.IsNullOrEmpty(sceneOrder[0].SceneName))
        {
            SceneManager.LoadScene(sceneOrder[0].SceneName);
        }
        else
        {
            Debug.LogWarning("No first scene found in sceneOrder.");
        }
    }

    public void LoadMainMenu()
    {
        if (mainMenuScene != null && !string.IsNullOrEmpty(mainMenuScene.SceneName))
        {
            SceneManager.LoadScene(mainMenuScene.SceneName);
        }
        else
        {
            Debug.LogWarning("Main Menu scene is not assigned.");
        }
    }

    public void LoadNextScene()
    {
        UpdateCurrentSceneIndex();

        int nextIndex = currentSceneIndex + 1;

        if (nextIndex < sceneOrder.Count && !string.IsNullOrEmpty(sceneOrder[nextIndex].SceneName))
        {
            SceneManager.LoadScene(sceneOrder[nextIndex].SceneName);
        }
        else
        {
            Debug.Log("No next scene in queue.");
        }
    }

    public void LoadPreviousScene()
    {
        UpdateCurrentSceneIndex();

        int prevIndex = currentSceneIndex - 1;

        if (prevIndex >= 0 && !string.IsNullOrEmpty(sceneOrder[prevIndex].SceneName))
        {
            SceneManager.LoadScene(sceneOrder[prevIndex].SceneName);
        }
        else
        {
            Debug.Log("No previous scene in queue.");
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (sceneOrder != null)
        {
            foreach (SceneReference sceneRef in sceneOrder)
            {
                if (sceneRef != null)
                {
                    sceneRef.UpdateSceneName();
                }
            }
        }

        if (gameOverScene != null)
        {
            gameOverScene.UpdateSceneName();
        }

        if (mainMenuScene != null)
        {
            mainMenuScene.UpdateSceneName();
        }
    }
#endif
}