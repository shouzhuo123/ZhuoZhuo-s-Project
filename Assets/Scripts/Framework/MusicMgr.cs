using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource bkMusic;
    private float bkVolume = 1f;
    private float soundVolume = 1f;
    private GameObject soundObj;
    private List<AudioSource> soundList = new List<AudioSource>();

    public float BkVolume
    {
        get => bkVolume;
        set
        {
            bkVolume = value;
            if (bkMusic != null) bkMusic.volume = value;
        }
    }

    public float SoundVolume
    {
        get => soundVolume;
        set
        {
            soundVolume = value;
            for (int i = 0; i < soundList.Count; i++)
                soundList[i].volume = value;
        }
    }

    public MusicMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    public void PlayBKMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject("BKMusic");
            bkMusic = obj.AddComponent<AudioSource>();
        }
        ResMgr.GetInstance().LoadAsync<AudioClip>("Music/bk/" + name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkVolume;
            bkMusic.Play();
        });
    }

    public void PauseBKMusic()
    {
        bkMusic?.Pause();
    }

    public void StopBKMusic()
    {
        bkMusic?.Stop();
    }

    public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callback = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject("Sounds");
            GameObject.DontDestroyOnLoad(soundObj);
        }
        AudioSource source = soundObj.AddComponent<AudioSource>();
        ResMgr.GetInstance().LoadAsync<AudioClip>("Music/Sounds/" + name, (clip) =>
        {
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundVolume;
            source.Play();
            soundList.Add(source);
            callback?.Invoke(source);
        });
    }

    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }

    public void StopAllSound()
    {
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            soundList[i].Stop();
            GameObject.Destroy(soundList[i]);
        }
        soundList.Clear();
    }
}
