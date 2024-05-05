using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Sounds : MonoBehaviour
{
    public static Manager_Sounds instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource MainAudioSource;
    private List<AudioClip> List_SoundEffectClip;

    // Start is called before the first frame update
    void Start()
    {
        //사용되는 효과음 리소스 미리 로딩
        List_SoundEffectClip = new List<AudioClip>() {
            Resources.Load<AudioClip>("Sounds/money"),
            Resources.Load<AudioClip>("Sounds/dungeon_door"),
            Resources.Load<AudioClip>("Sounds/item_get"),
            Resources.Load<AudioClip>("Sounds/chest_open"),
            Resources.Load<AudioClip>("Sounds/reward"),
            Resources.Load<AudioClip>("Sounds/woodblunt_hit"),
            Resources.Load<AudioClip>("Sounds/woodblunt_blow"),
            Resources.Load<AudioClip>("Sounds/gen_button_down"),
            Resources.Load<AudioClip>("Sounds/gen_window_open"),
            Resources.Load<AudioClip>("Sounds/gen_window_closed"),
            Resources.Load<AudioClip>("Sounds/inventory_open")
        };
    }

    //Play sound effect
    //효과음 재생
    public void PlayOneShot(int afxIndex)
    {
        if (List_SoundEffectClip.Count > afxIndex)
            MainAudioSource.PlayOneShot(List_SoundEffectClip[afxIndex]);
        else
            Debug.LogWarning("Sound Exception: No AFX in List");
    }

    //Fade main background music
    //배경음악 페이드 인/아웃
    public void FadeOutStart()
    {
        StartCoroutine(FadeOut());
    }

    public void FadeInStart()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float fade_duration = 1.0f;
        while (fade_duration > 0f)
        {
            MainAudioSource.volume = 1.0f - fade_duration;
            fade_duration -= Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float fade_duration = 1.0f;
        while (fade_duration > 0f)
        {
            MainAudioSource.volume = fade_duration;
            fade_duration -= Time.deltaTime;
            yield return null;
        }
    }

    //Change main background music
    //배경음악 교체 및 재생
    public void ChangeMusic(int musicId)
    {
        AudioClip clip;
        switch(musicId)
        {
            case 0:
                clip = Resources.Load<AudioClip>("Sounds/Galaxy_mp3");
                break;
            case 1:
                clip = Resources.Load<AudioClip>("Sounds/Dungeon_02");
                break;
            default:
                clip = Resources.Load<AudioClip>("Sounds/Galaxy_mp3");
                break;
        }
        MainAudioSource.clip = clip;
        MainAudioSource.Play();
    }
}
