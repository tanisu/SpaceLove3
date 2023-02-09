using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;

    [SerializeField] List<BGMSoundData> bGMSoundDatas;
    [SerializeField] List<SESoundData> SESoundDatas;



    public float mastarVolume = 1;
    public float bgmVolume = 1;
    public float seVolume = 1;
    private float beforeVolume;
    private bool isFadeOut;
    public static SoundManager I { get; private set; }

    private void Awake()
    {
        if (I == null)
        {

            DontDestroyOnLoad(gameObject);
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        BGMSoundData data = bGMSoundDatas.Find(data => data.bgm == bgm);
        bgmAudioSource.clip = data.audioClip;
        if (isFadeOut)
        {
            bgmAudioSource.volume = beforeVolume;
        }
        else
        {
            bgmAudioSource.volume = data.volume * bgmVolume * mastarVolume;
        }


        bgmAudioSource.Play();
    }

    public void StopBGM()
    {

        bgmAudioSource.Stop();
    }
    public void StopSE()
    {
        seAudioSource.enabled = false;
    }
    public void StartSE()
    {
        seAudioSource.enabled = true;
    }

    public void LoopSwitch()
    {
        bgmAudioSource.loop = !bgmAudioSource.loop;
    }

    public void ChangeBGMVolumes()
    {
        bgmAudioSource.volume = bgmVolume;
    }

    public void ChangeSEVolumes()
    {
        seAudioSource.volume = seVolume;
    }

    public void FadeOutBGM()
    {
        isFadeOut = true;
        beforeVolume = bgmAudioSource.volume;
        StartCoroutine(_fadeOutBGM());
    }


    IEnumerator _fadeOutBGM()
    {
        while (bgmAudioSource.volume != 0f)
        {
            bgmAudioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.3f);
        }
        StopBGM();
    }

    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = SESoundDatas.Find(data => data.se == se);
        seAudioSource.volume = data.volume * seVolume * mastarVolume;
        seAudioSource.PlayOneShot(data.audioClip);
    }


}
[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        MAIN,
        CLEAR

    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}

[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        WALK,
        MISS,
        DEAD,
        CLICK,
        CLEAR,
        ITEM,
        LOGO,
        TITLE

    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}