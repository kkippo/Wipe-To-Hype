using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop(); // останавливаем текущий
        }

        audioSource.Play(); // запускаем заново
    }
}
