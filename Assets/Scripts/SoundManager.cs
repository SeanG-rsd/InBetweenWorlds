using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource airSouce;

    private string MASTER_VOLUME_KEY = "MASTER_VOLUME";
    private string MUSIC_VOLUME_KEY = "MUSIC_VOLUME";
    private string EFFECTS_VOLUME_KEY = "EFFECTS_VOLUME";

    [SerializeField] private Slider masterVolumSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;

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
        }

        Initialize();
    }

    private void Initialize()
    {
        if (PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
        {
            masterVolumSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, 0.5f);
            masterVolumSlider.value = 0.5f;
        }

        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, 0.5f);
            musicVolumeSlider.value = 0.5f;
        }

        if (PlayerPrefs.HasKey(EFFECTS_VOLUME_KEY))
        {
            effectsVolumeSlider.value = PlayerPrefs.GetFloat(EFFECTS_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(EFFECTS_VOLUME_KEY, 0.5f);
            effectsVolumeSlider.value = 0.5f;
        }

        AudioListener.volume = masterVolumSlider.value;
        musicSource.volume = musicVolumeSlider.value;
        effectsSource.volume = effectsVolumeSlider.value;
    }

    public void PlaySound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
    }

    public void ChangeMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
    }

    public void ChangeEffectsVolume(float value)
    {
        airSouce.volume = value;
        effectsSource.volume = value;
        PlayerPrefs.SetFloat(EFFECTS_VOLUME_KEY, value);
    }

    public void AirSound(bool play)
    {
        if (airSouce.isPlaying && !play)
        {
            airSouce.Stop();
        }
        else if (play && !airSouce.isPlaying)
        {
            airSouce.Play();
        }
    }
}
