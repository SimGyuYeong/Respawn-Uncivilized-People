using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeTimeSFX : MonoBehaviour
{
    public static FreeTimeSFX instance = null;

    private AudioSource _audioSource;

    private TextManager _textManager = null;

    [SerializeField] private SoundManager bgmSoundManager = null;
    public SoundManager BgmSound { get { return bgmSoundManager; } }

    [SerializeField] private SoundManager sfxSoundManager = null;
    public SoundManager SfxSound { get { return sfxSoundManager; } }


    private void Awake()
    {
        if (instance != this)
        {
            Destroy(instance);
            instance = this;
        }

        _audioSource = GetComponent<AudioSource>();
        _textManager = transform.parent.Find("TextManager").GetComponent<TextManager>();
        bgmSoundManager = transform.parent.Find("SoundManager").GetChild(0).GetComponent<SoundManager>();
        sfxSoundManager = transform.parent.Find("SoundManager").GetChild(1).GetComponent<SoundManager>();
    }

    public void Start()
    {
        bgmSoundManager.PlaySound(_textManager.TextSO.bgmList[0], true);
    }

    public void PlaySoundClip(AudioClip clip, bool isLoop = false)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.loop = isLoop;
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}
