using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] bgmTracks;
    public AudioClip clickSfx;
    public AudioClip coinsSfx;
    public AudioClip plowSfx;
    public AudioClip waterSfx;
    public AudioClip walkSfx;
    public AudioClip plantSfx;
    public AudioClip harvestSfx;

    private int lastBgmIndex = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayRandomBGM();
    }

    private void Update()
    {
        if (bgmSource != null && !bgmSource.isPlaying && Time.timeScale > 0)
        {
            // Pastikan ada lagu di dalam list sebelum mencoba memutarnya
            if (bgmTracks.Length > 0)
            {
                Debug.Log("Lagu selesai, memainkan lagu berikutnya...");
                PlayRandomBGM();
            }
        }
    }

    public void PlayRandomBGM()
    {
        if (bgmTracks.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, bgmTracks.Length);
        } while (bgmTracks.Length > 1 && index == lastBgmIndex);

        lastBgmIndex = index;
        bgmSource.clip = bgmTracks[index];
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}