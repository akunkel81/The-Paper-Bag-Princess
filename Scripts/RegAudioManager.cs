using UnityEngine;

public class RegAudioManager : MonoBehaviour
{
[Header("Audio Source")]
[SerializeField] AudioSource musicSource;

[Header("Audio Clips")]
[SerializeField] AudioClip background;

void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
}