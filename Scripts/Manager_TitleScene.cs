using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Manager_TitleScene : MonoBehaviour
{
    private int current_menu = 0;
    private bool isHold = false;

    public GameObject SelectArrow;
    public GameObject InfoPanel;
    public GameObject BlindPanel;

    public AudioClip SelectedChange;
    public AudioClip SelectedConfirm;
    public AudioSource MainAudioSource;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        //if Player already select menu, selectable key should be hold its state
        //�÷��̾ �޴��� �̹� �����ߴٸ� Ű ���´� �����Ǿ���Ѵ�
        if (isHold == true)
        {
            if(InfoPanel.activeSelf == true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CloseInfo();
                }
            }
            return;
        }

        //Arrow was positioned depends on current selected menu
        //Arrow�� ���� ���õ� �޴��� ���� ��ġ�� ��������
        switch (current_menu)
        {
            case 0:
                SelectArrow.transform.position = new Vector3(1130, 510, 0);
                break;
            case 1:
                SelectArrow.transform.position = new Vector3(1130, 390, 0);
                break;
            case 2:
                SelectArrow.transform.position = new Vector3(1130, 270, 0);
                break;
            default:
                SelectArrow.transform.position = new Vector3(1130, 510, 0);
                break;
        }

        //Player can move arrow up and down by Arrow Key or W,S Key
        //�÷��̾�� ���Ʒ�Ű�� W,SŰ�� �޴��� ���Ʒ��� ������ �� �ִ�
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            current_menu = (current_menu + 1) % 3;
            MainAudioSource.PlayOneShot(SelectedChange);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            current_menu = (current_menu + 2) % 3;
            MainAudioSource.PlayOneShot(SelectedChange);
        }

        //Player can select the menu by Space Bar
        //�÷��̾�� �����̽��ٷ� �޴��� ������ �� �ִ�.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MainAudioSource.PlayOneShot(SelectedConfirm);
            isHold = true;
            switch (current_menu)
            {
                case 0:
                    //���ӽ���
                    StartCoroutine(FadeOut());
                    break;
                case 1:
                    //�����ȳ�
                    ShowInfo();
                    break;
                case 2:
                    //��������
                    StartCoroutine(FadeOut());
                    break;
            }
        }

    }

    //Start fade in when just after start this Scene
    //�� ���� �� ���̵���
    public IEnumerator FadeIn()
    {
        float fade_duration = 2.0f;
        float ratio = 1f / fade_duration;
        while (fade_duration > 0f)
        {
            BlindPanel.GetComponent<Image>().color = new Color(0, 0, 0, (fade_duration * ratio));
            fade_duration -= Time.deltaTime;
            yield return null;
            
        }
        BlindPanel.GetComponent<Image>().color = Color.clear;

        current_menu = 0;
        SelectArrow.SetActive(true);
    }

    //Start fade out when just after start or exit
    //���ӽ��� Ȥ�� �������� �� ���̵�ƿ�
    public IEnumerator FadeOut()
    {
        float fade_duration = 2.0f;
        float ratio = 1f / fade_duration;
        while (fade_duration > 0f)
        {
            BlindPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1f - (fade_duration * ratio));
            MainAudioSource.volume = (fade_duration * ratio);
            fade_duration -= Time.deltaTime;
            yield return null;
        }

        BlindPanel.GetComponent<Image>().color = Color.black;

        switch (current_menu)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
                break;
            case 2:
                Application.Quit();
                break;
            default:
                break;
        }       
    }

    //Show or hide infomation Panel
    //����â �����ֱ� �� �����
    public void ShowInfo()
    {
        InfoPanel.SetActive(true);
        isHold = true;
    }

    public void CloseInfo()
    {
        InfoPanel.SetActive(false);
        isHold = false; 
    }
}
