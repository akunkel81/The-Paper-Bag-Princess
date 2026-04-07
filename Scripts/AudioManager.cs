using UnityEngine;

public class AudioManager : MonoBehaviour
{
[Header("Audio Source")]
[SerializeField] AudioSource musicSource;

[Header("Audio Clips")]
[SerializeField] AudioClip battleMusic;

void Start()
    {
        musicSource.clip = battleMusic;
        musicSource.Play();
    }
}
