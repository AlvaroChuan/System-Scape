using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum ClipEnum
    {
        Button,
        Footsteps,
        MainTheme,
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
            InitializeDictionaries();

			//SaveSystem.LoadData();
			//SetMusicVolume(SaveSystem.saveData.musicVolume);
			//SetSfxVolume(SaveSystem.saveData.sfxVolume);

			//PlayMusic(ClipEnum.MainTheme);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionaries()
    {
        foreach (var clip in musicClips) _musicDictionary[clip.clip] = clip.soundFile;
        foreach (var clip in sfxClips) _sfxDictionary[clip.clip] = clip.soundFile;
    }

    public void PlayMusic(ClipEnum clip, bool loop = true)
    {
        if (_musicDictionary.TryGetValue(clip, out var sound))
        {
            musicSource.clip = sound;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else Debug.LogWarning($"Music clip '{clip}' not found!");
    }

    public void PlaySfx(ClipEnum clip)
    {
        if (_sfxDictionary.TryGetValue(clip, out var sound))
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            newSource.PlayOneShot(sound);
            StartCoroutine(DestroySource(newSource, sound.length));
        }
        else Debug.LogWarning($"SFX clip '{clip}' not found!");
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    IEnumerator DestroySource(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        Destroy(source);
    }

    public void SetMusicVolume(float volume)
    {
        if(volume < 0.001) volume = 0.001f;
        mixer.SetFloat("MusicVolume", MathF.Log10(volume) * 20f);
    }

    public void SetSfxVolume(float volume)
    {
        if(volume < 0.001) volume = 0.001f;
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
