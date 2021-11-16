using System;
using Spenve;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Xml;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Aud : SingletonCreateDontDestroy<Aud>
{
    Dictionary<string, AudioClip> allBgm = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> allSfx = new Dictionary<string, AudioClip>();
    AudioSource bgmAudioSource;
    List<AudioSource> sfxAudioSource = new List<AudioSource>();
    float bgmVolume = 1.0f;
    float sfxVolume = 1.0f;

    private int enableBgm;
    private int enableSfx;
    
    private void OnEnable()
    {
        enableBgm = PlayerPrefs.GetInt("enableBgm", 1);
        enableSfx = PlayerPrefs.GetInt("enableSfx", 1);
    }

    //设置背景音乐状态
    public static bool EnableBgm
    {
        set
        {
            Instance.enableBgm = value ? 1 : 0;
            PlayerPrefs.SetInt("enableBgm", Instance.enableBgm);
            if (value)
            {
                UnpauseBgm();
            }
            else
            {
                PauseBgm();
            }
        }
        get
        {
            return Instance.enableBgm == 1;
        }
    }

    //设置音效状态
    public static bool EnableSfx
    {
        set
        {
            Instance.enableSfx = value ? 1 : 0;
            PlayerPrefs.SetInt("enableSfx", Instance.enableSfx);
            if (!value)
            {
                StopAllSfx();
            }
        }
        get
        {
            return Instance.enableSfx == 1;
        }
    }
    
    //背景音乐
    private AudioSource BgmAudioSource
    {
        get
        {
            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
                bgmAudioSource.volume = bgmVolume;
            }
            return bgmAudioSource;
        }
    }

    public static void PlayBgmRandom()
    {
        string[] randomName = Instance.allBgm.Keys.ToArray<string>();
        PlayBgm(randomName[UnityEngine.Random.Range(0, randomName.Length)]);
    }

    public static void PlayBgm(string name, bool loop = true)
    {
        AudioClip si;
        if (!Instance.allBgm.TryGetValue(name, out si))
        {
            si = ResLoader.Global.LoadAsset<AudioClip>(name);
            if (si == null)
            {
                Debug.LogError("没有找到音乐：" + name);
                return;
            }

            Instance.allBgm.Add(name, si);
        }
        AudioSource auds = Instance.BgmAudioSource;
        auds = Instance.BgmAudioSource;
        auds.loop = loop;
        auds.clip = si;
        auds.volume = 0;
        DOTween.To(() => auds.volume, x => auds.volume = x, Instance.bgmVolume, 1.0f).SetEase(Ease.OutQuad);
        auds.Play();
        
        if(Instance.enableBgm == 0)
        {
            PauseBgm();
        }
    }

    public static void SetBgmVolume(float v)
    {
        Instance.bgmVolume = Mathf.Clamp01(v);
        Instance.BgmAudioSource.volume = Instance.bgmVolume;
    }

    public static void PauseBgm()
    {
        Instance.BgmAudioSource.Pause();
    }

    public static void UnpauseBgm()
    {
        Instance.BgmAudioSource.UnPause();
    }

    public static void StopBgm()
    {
        Instance.BgmAudioSource.Stop();
    }

    //音效

    public static void PlaySfx(string name, bool forceLoop = false, float delayTime = 0)
    {
        if (delayTime == 0)
        {
            Instance._PlaySfx(name, forceLoop);
        }
        else
        {
            Instance.StartCoroutine(Instance._PlayDelay(name, forceLoop, delayTime));
        }
    }

    IEnumerator _PlayDelay(string name, bool forceLoop, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _PlaySfx(name, forceLoop);
    }

    public static void SetSfxVolume(float v)
    {
        Instance.sfxVolume = Mathf.Clamp01(v);

        for (int i = 0; i < Instance.sfxAudioSource.Count; i++)
        {
            Instance.sfxAudioSource[i].volume = Instance.sfxVolume;
        }
    }

    public static void StopAllSfx()
    {
        for (int i = 0; i < Instance.sfxAudioSource.Count; i++)
        {
            Instance.sfxAudioSource[i].Stop();
        }
    }

    public static void StopSfx(string name)
    {
        for (int i = 0; i < Instance.sfxAudioSource.Count; i++)
        {
            if (Instance.sfxAudioSource[i].clip != null && Instance.sfxAudioSource[i].clip.name.Equals(name) && Instance.sfxAudioSource[i].isPlaying)
            {
                Instance.sfxAudioSource[i].Stop();
            }
        }
    }

    private AudioSource _PlaySfx(string name, bool forceLoop = false)
    {
        if(Instance.enableSfx == 0)
            return null;

        AudioClip si;
        if (!allSfx.TryGetValue(name, out si))
        {
            si = ResLoader.Global.LoadAsset<AudioClip>(name);
            if (si == null)
            {
                Debug.LogError("没有找到音效：" + name);
                return null;
            }
            allSfx.Add(name, si);
        }

        AudioSource sfxAS = GetSfxAudioSource(name);
        if (sfxAS == null)
        {
            return null;
        }
        sfxAS.clip = si;
        sfxAS.spatialBlend = 0;
        sfxAS.loop = forceLoop;
        sfxAS.Play();
        return sfxAS;
    }

    private AudioSource GetSfxAudioSource(string name)
    {
        AudioSource a = null;
        float maxt = -1;
        int count = 0;
        for (int i = 0; i < sfxAudioSource.Count; i++)
        {
            if (sfxAudioSource[i].isPlaying && sfxAudioSource[i].clip.name == name)
            {
                if (sfxAudioSource[i].time < 0.1f)//如果有相同的音效在0.2秒以内播放过，则不播放
                {
                    return null;
                }

                count++;
                if (sfxAudioSource[i].time > maxt)
                {
                    maxt = sfxAudioSource[i].time;
                    a = sfxAudioSource[i];
                }
            }
        }
        //超过三个同时播放的音效以后，选择播放最久的那个拿来重用
        if (count >= 3)
        {
            return a;
        }

        for (int i = 0; i < sfxAudioSource.Count; i++)
        {
            if (!sfxAudioSource[i].isPlaying)
            {
                return sfxAudioSource[i];
            }
        }

        a = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.Add(a);
        a.volume = sfxVolume;
        return a;
    }


    public static void ClearSfxAudioSource()
    {
        for (int i = 0; i < Instance.sfxAudioSource.Count; i++)
        {
            Destroy(Instance.sfxAudioSource[i].gameObject);
        }

        Instance.sfxAudioSource.Clear();
    }
}

