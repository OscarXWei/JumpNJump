using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landOnPlatformSound;
    [SerializeField] private AudioClip shatterSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource added to GameObject");
        }

        // 检查并报告 AudioClip 的状态
        Debug.Log($"Jump Sound: {(jumpSound != null ? "Set" : "Not set")}");
        Debug.Log($"Land On Platform Sound: {(landOnPlatformSound != null ? "Set" : "Not set")}");
        Debug.Log($"Shatter Sound: {(shatterSound != null ? "Set" : "Not set")}");
    }

    public void PlayJumpSound()
    {
        PlaySound(jumpSound);
        Debug.Log("Jump sound played");
    }

    public void PlayLandOnPlatformSound()
    {
        PlaySound(landOnPlatformSound);
        Debug.Log("Land on platform sound played");
    }

    public void PlayShatterSound()
    {
        PlaySound(shatterSound);
        Debug.Log("Shatter sound played");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Audio clip is not set!");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource is not set!");
            return;
        }

        audioSource.PlayOneShot(clip);
        Debug.Log($"Playing sound: {clip.name}");
    }
}