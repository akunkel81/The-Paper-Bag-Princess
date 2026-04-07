using UnityEngine;
using UnityEngine.SceneManagement;

public class DumpSceneSwitch : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Dump");
    }

}