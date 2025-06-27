using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum ClipEnum
    {
        ClosePad,
        OpenPad,
        CRT,
        FadeIn,
        FadeOut,
        Hit,
        Jetpack,
        LoadingBar,
        MaterailBreak,
        Oxygen,
        Prohibited,
        SpaceShipEngine,
        Sword,
        Upgrade,
        Rifle,
        Footsteps,
        Interface,
        MainTheme,
        MercumTheme,
        ColisTheme,
        PhobosTheme,
        RegioTheme,
        PlatumTheme,
        SpaceTheme,
    }


    [Serializable]
	struct ClipStruct
	{
		public ClipEnum clip;
		public AudioClip soundFile;
	}
	public static SoundManager instance;

    [Header("Audio Sources")][SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")][SerializeField] List<ClipStruct> musicClips;
	[SerializeField] AudioMixer mixer;
	[SerializeField] List<ClipStruct> sfxClips;
    private readonly Dictionary<ClipEnum, AudioClip> _musicDictionary = new();
    private readonly Dictionary<ClipEnum, AudioClip> _sfxDictionary = new();

	private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionaries();

            if (PlayerPrefs.HasKey("musicVolume")) LoadMusicVolume();
            else SetMusicVolume(1f);

            if (PlayerPrefs.HasKey("sfxVolume")) LoadSFXVolume();
            else SetSfxVolume(1f);

            PlayMusic(ClipEnum.MainTheme);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadMusicVolume()
    {
        float volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        SetMusicVolume(volume);
    }

    private void LoadSFXVolume()
    {
        float volume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        SetSfxVolume(volume);
    }

    private void InitializeDictionaries()
    {
        foreach (var clip in musicClips) _musicDictionary[clip.clip] = clip.soundFile;
        foreach (var clip in sfxClips) _sfxDictionary[clip.clip] = clip.soundFile;
    }

    public void PlayMusic(ClipEnum clip, bool loop = true, float volume = 1f)
    {
        if (_musicDictionary.TryGetValue(clip, out var sound))
        {
            musicSource.clip = sound;
            musicSource.loop = loop;
            musicSource.volume = volume;
            musicSource.Play();
        }
        else Debug.LogWarning($"Music clip '{clip}' not found!");
    }

    public void PlaySfx(ClipEnum clip, bool loop = false, float volume = 1f)
    {
        if (_sfxDictionary.TryGetValue(clip, out var sound))
        {
            if (!loop)
            {
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
                newSource.PlayOneShot(sound, volume);
                StartCoroutine(DestroySource(newSource, sound.length));
            }
            else
            {
                AudioSource newSource = gameObject.AddComponent<AudioSource>();
                newSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
                newSource.clip = sound;
                newSource.loop = true;
                newSource.Play();
            }
        }
        else Debug.LogWarning($"SFX clip '{clip}' not found!");
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopSfx(ClipEnum clip)
    {
        if (_sfxDictionary.TryGetValue(clip, out var sound))
        {
            AudioSource[] sources = gameObject.GetComponents<AudioSource>();
            foreach (var source in sources)
            {
                if (source.clip == sound)
                {
                    source.Stop();
                    Destroy(source);
                }
            }
        }
    }

    IEnumerator DestroySource(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        Destroy(source);
    }

    public void SetMusicVolume(float volume)
    {
        if(volume < 0.001) volume = 0.001f;
        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
        mixer.SetFloat("MusicVolume", MathF.Log10(volume) * 20f);
    }

    public void SetSfxVolume(float volume)
    {
        if(volume < 0.001) volume = 0.001f;
        PlayerPrefs.SetFloat("sfxVolume", volume);
        PlayerPrefs.Save();
        mixer.SetFloat("SFXVolume", MathF.Log10(volume) * 20f);
    }

    public float GetMusicVolume()
    {
        float volume;
        mixer.GetFloat("MusicVolume", out volume);
        return volume;
    }

    public float GetSFXVolume()
    {
        float volume;
        mixer.GetFloat("SFXVolume", out volume);
        return volume;
    }
}
