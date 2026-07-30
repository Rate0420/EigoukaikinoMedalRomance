using UnityEngine;

public class TitleVoice : MonoBehaviour
{
    [SerializeField] private AudioClip[] startVoice;
    [SerializeField] private AudioSource voiceSource;

    public void StartVoice()
    {
        int index = Random.Range(0, startVoice.Length);
        voiceSource.clip = startVoice[index];
        voiceSource.Play();
    }
}
