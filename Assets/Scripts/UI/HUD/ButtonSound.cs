using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void PlayClickSound()
    {
        Debug.Log("Button clicked");

        audioSource.PlayOneShot(clickSound);
    }
}
