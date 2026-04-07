using UnityEngine;
using UnityEngine.SceneManagement;

public class RescueSceneSwitch : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Rescue");
    }

}
