using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void playSFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}