using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioConductor : MonoBehaviour
{
    public static AudioConductor Instance;

    [Tooltip("PreWarm")]
    public int poolingAmount = 10;
    [Tooltip("Enable this so, in the event more sources are requested than available, the pooler can take it upon itself to add more.")]
    public bool overflowProtection = true;

    //A queue of ready to use audio sources
    private Queue<AudioSource> audioSources;
    //A list of the active audio sources, this will be looped through to returned finished sources back to the queue
    private List<AudioSource> activeSources;


    private AudioSource cacheSource;
    private GameObject cacheObj;

    //How often, in seconds, should the AudioManager check for finished audio sources
    private float checkTime = 0.5f;

    public SerializableDictionary<string, AudioSequence> sfx;

    public SerializableDictionary<string, AudioSource> music;


    public List<AudioSequence> cheers;


    public Vector2 fillVol = new Vector2(1, 1);


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        BuildPool();
    }

    private void Update()
    {
        //Only check every half of a second, no need to check every single frame.
        checkTime -= Time.deltaTime;
        if (checkTime <= 0)
        {
            CheckForFinished();
            checkTime = 0.5f;
        }
    }
    [ContextMenu("FillVol")]
    public void FillVol()
    {
        cheers.ForEach(c => c.volume = fillVol);
    }

    //Build the initial pool
    private void BuildPool()
    {
        //Create new queue and list objects
        audioSources = new Queue<AudioSource>();
        activeSources = new List<AudioSource>();

        //Add the number of entries specified by the pooling amount
        for (int i = 0; i < poolingAmount; i++)
        {
            AddEntry();
        }
    }

    //Adds an entry to audio sources queue
    private void AddEntry()
    {
        cacheObj = new GameObject();
        cacheObj.transform.parent = transform;
        cacheSource = cacheObj.AddComponent<AudioSource>();
        cacheSource.playOnAwake = false;
        cacheObj.SetActive(false);
        audioSources.Enqueue(cacheSource);
    }


    private void CheckForFinished()
    {
        //Iterate through all the active audio sources and check if they are done playing
        for (int _index = 0; _index < activeSources.Count; _index++)
        {
            //If they are done playing deactivate them and add them back into the queue
            if (!activeSources[_index].isPlaying)
            {
                audioSources.Enqueue(activeSources[_index]);
                activeSources[_index].gameObject.SetActive(false);
                activeSources.RemoveAt(_index);
                _index--;
            }
        }
    }

    //Grabs an Audio Source from the queue if we have one available
    private AudioSource GetAudioSource()
    {
        if (audioSources.Count < 1)
        {
            if (overflowProtection)
                AddEntry();
            else
            {
                Debug.LogError("Audio Source Queue Empty, consider increasing pool size or turn on overflow protection.");
                return null;
            }
        }

        return audioSources.Dequeue();
    }

    public static void PlayCheer()
    {
        Instance.InternalPlayCheer();
    }
    int tempCount;

    private void InternalPlayCheer()
    {
        if (tempCount == 0)
        {
            cheers.Shuffle();
        }

        cheers[tempCount].PlaySound();

        tempCount++;

        if (tempCount >= cheers.Count)
        {
            tempCount = 0;
        }
    }

    public static void PlaySfx(string _key)
    {
        try
        {
            Instance.sfx.Get(_key)?.PlaySound();
        }
        catch
        {
            Instance.Log("UnableToPlay");
        }
    }
    public static void StopSfx(string _key)
    {
        Instance.sfx.Get(_key).Stop();
    }
    public static void PlayMusic(string _key)
    {
        Instance.Log($"Play Music : {_key}");
        if (Instance.music.Contains(_key))
        {
            Instance.music.List.ForEach(a => a.value.Stop());
            Instance.music.Get(_key).Play();
        }
    }

    public static AudioSource GetMusic(string _key)
    {
        Instance.Log($"Get Music : {_key}");
        if (Instance.music.Contains(_key))
        {
            Instance.music.List.ForEach(a => a.value.Stop());
            return Instance.music.Get(_key);
        }
        return Instance.GetAudioSource();
    }
    public static void StopMusic(string _key)
    {
        if (Instance.music.Contains(_key))
            Instance.music.Get(_key).Stop();
    }

    public static void ReturnToPool(AudioSource _source)
    {
        _source.gameObject.SetActive(false);
        Instance.audioSources.Enqueue(_source);
    }

    #region 2D Sound Methods
    
    public static AudioSource PlaySound2D(AudioClip _clip)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = _clip;
        Instance.cacheSource.volume = 1f;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
   
    public static AudioSource PlaySound2D(AudioClip _clip, float volume)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = _clip;
        Instance.cacheSource.volume = volume;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
  
    public static void PlaySound2D(AudioClip _clip, float _volume, float _pitch)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = _clip;
        Instance.cacheSource.volume = _volume;
        Instance.cacheSource.pitch = _pitch;
        Instance.cacheSource.loop = false;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.activeSources.Add(Instance.cacheSource);
        Instance.cacheSource.Play();
    }
   
    public static AudioSource PlaySound2DLooped(AudioClip _clip, float _volume, float _pitch)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = _clip;
        Instance.cacheSource.volume = _volume;
        Instance.cacheSource.pitch = _pitch;
        Instance.cacheSource.loop = true;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }

   
    public static AudioSource PlaySound2DLooped(AudioClip _clip, float _volume)
    {
        Instance.cacheSource = Instance.GetAudioSource();
        Instance.cacheSource.clip = _clip;
        Instance.cacheSource.volume = _volume;
        Instance.cacheSource.loop = true;
        Instance.cacheSource.spatialBlend = 0.0f;
        Instance.cacheSource.gameObject.SetActive(true);
        Instance.cacheSource.Play();
        return Instance.cacheSource;
    }
    #endregion

    public void Log(string _message)
    {
        Debug.Log($"[AudioConductor] {_message}");
    }
}

[Serializable]
public class AudioSequence
{
    public List<AudioClip> clips;
    public bool shuffle;
    public Vector2 volume = new Vector2(0.95f, 1);
    public int currentClip = 0;
    public PlayStat waitIfBusy;
    public AudioSource audioSource;
    public bool IsBusy => audioSource.isPlaying;


    public void PlaySound()
    {
        AudioConductor.Instance.StartCoroutine(PlaySoundEnum());
    }
    public void Stop()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
    private IEnumerator PlaySoundEnum()
    {

        switch (waitIfBusy)
        {
            case PlayStat.Wait:
                if (audioSource != null)
                {
                    while (IsBusy)
                    {
                        yield return null;
                    }
                }

                break;
            case PlayStat.Ignore:
                yield break;
        }

        var _vol = UnityEngine.Random.Range(volume.x,volume.y);
        audioSource = AudioConductor.PlaySound2D(clips[currentClip], _vol);
        currentClip = (currentClip + 1) % clips.Count;
    }

    public enum PlayStat
    {
        Wait,
        Ignore

    }
}



