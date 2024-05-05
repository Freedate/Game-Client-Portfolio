using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class Manager_UI : MonoBehaviour
{
    public static Manager_UI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public GameObject BlindPanel;
    public GameObject SettingPanel;
    public GameObject SettingButton;
    public GameObject ExitPanel;
    public GameObject NoticeText;

    public GameObject MoneyText;
    public GameObject Controller_Image;

    public GameObject item_grid_panel;
    public GameObject itemPrefab;

    public GameObject window_inventory;
    public GameObject window_quest;

    public GameObject quest_grid_panel;
    public GameObject quest_description_panel;

    private bool isItemOpen = false;
    private bool isQuestOpen = false;

    public Slider VolumeSlider;
    public Toggle MuteToggle;

    private bool notice_showing = false;

    private Queue<string> Queue_Notice;
    private bool exitFlag = false;

    //Enqueue application exit message (Temp)
    //현재 시퀀스 종료 이후 프로그램 종료 플래그 세팅 (프로젝트 종료용 임시 함수)
    public void SetExitFlag()
    {
        exitFlag = true;
    }
    
    //Initializing volume control function in setting panel
    //설정창 볼륨제어 이벤트 초기화
    void Initialize()
    {
        VolumeSlider.onValueChanged.AddListener(delegate { SetVolume(); });
        MuteToggle.onValueChanged.AddListener(delegate { SetMute(); });
    }

    void Start()
    {
        FadeInStart();
    }

    void Update()
    {
        //키보드 단축키에 따른 인벤토리, 퀘스트, 설정 창 제어
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isItemOpen)
            {
                CloseInventoryPanel();
            }
            else
            {
                OpenInventoryPanel();
            }
        }
    
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isQuestOpen)
            {
                CloseQuestPanel();
            }
            else
            {
                OpenQuestPanel();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            
            Setting(!SettingPanel.activeSelf);
            
        }
    }

    //Audio Setting Panel Control
    //설정 창 제어
    public void Setting(bool flag)
    {
        Manager_Player_Info.instance.SetControlableMove(!flag);
        Manager_Player_Info.instance.SetControlableRotate(!flag);
        SettingPanel.SetActive(flag);
        SettingButton.SetActive(!flag);
    }

    public void SetVolume()
    {
        Manager_Sounds.instance.MainAudioSource.volume = VolumeSlider.value;   
    }

    public void SetMute()
    {
        Manager_Sounds.instance.MainAudioSource.mute = MuteToggle.isOn;
    }
    public void OpenExitPanel(bool flag)
    {
        ExitPanel.SetActive(flag);
        Manager_Sounds.instance.PlayOneShot(8);
    }

    public void GameExit()
    {
        StartCoroutine(FadeOut(1));
    }
    public void HideSettingButton()
    {
        SettingButton.SetActive(false);
    }

    //Fade In and Out Control
    public void FadeInStart()
    {
        StartCoroutine(FadeIn());

    }
    public void FadeOutStart(int index = 0)
    {
        StartCoroutine(FadeOut(index));
    }

    private IEnumerator FadeIn()
    {
        float fade_duration = 1.0f;
        while (fade_duration > 0f)
        {
            BlindPanel.GetComponent<Image>().color = new Color(0, 0, 0, fade_duration);
            fade_duration -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(int index = 0)
    {
        float fade_duration = 1.0f;
        while (fade_duration > 0f)
        {
            BlindPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1f - fade_duration);
            fade_duration -= Time.deltaTime;
            yield return null;
        }
        switch (index)
        {
            case 1:
                Debug.Log("게임종료");
                Application.Quit();
                break;
            case 2:
                Debug.Log("미궁 입장");
                CloseAllPanel();
                HideSettingButton();
                SceneManager.LoadScene(2);
                break;
            default:
                break;
        }
    }

    
    //Notice Control
    //독백스타일 안내 처리
    // bQueue가 true일 경우 Notice Queue로 들어가 현재 Notice 이후에 자동으로 등장
    // false일 경우 현재 안내 있을 경우 무시
    public void ShowNotice(string msg, bool bQueue = false)
    {
        if (bQueue)
        {
            if (Queue_Notice == null)
            {
                Queue_Notice = new Queue<string>();
            }

            if (isNoticeShowing() == true)
            {
                Queue_Notice.Enqueue(msg);
            }
            else
            {
                StartCoroutine(NoticeSequence(msg));
            }
        }
        else
        {
            if (isNoticeShowing() == true)
            {
                return;
            }
            else
            {
                StartCoroutine(NoticeSequence(msg));
            }
        }
    }

    public bool isNoticeShowing()
    {
        return notice_showing;
    }
    
    public IEnumerator NoticeSequence(string msg)
    {
        notice_showing = true;
        do
        {
            if (Queue_Notice != null && Queue_Notice.Count > 0)
            {
                NoticeText.GetComponent<TextMeshProUGUI>().text = Queue_Notice.Dequeue();
            }
            else
            {
                NoticeText.GetComponent<TextMeshProUGUI>().text = msg;
            }

            NoticeText.SetActive(true);
            float fade_duration = 0.5f;
            float show_duration = 2.0f;

            float fade_value = fade_duration;

            float ratio = 1f / fade_duration;
            while (fade_value > 0f)
            {
                NoticeText.GetComponent<TextMeshProUGUI>().alpha = 1f - (fade_value * ratio);
                fade_value -= Time.deltaTime;
                yield return null;
            }
            NoticeText.GetComponent<TextMeshProUGUI>().alpha = 1f;
            while (show_duration > 0f)
            {
                show_duration -= Time.deltaTime;
                yield return null;
            }
            fade_value = fade_duration;
            while (fade_value > 0f)
            {
                NoticeText.GetComponent<TextMeshProUGUI>().alpha = fade_value * ratio;
                fade_value -= Time.deltaTime;
                yield return null;
            }
            NoticeText.GetComponent<TextMeshProUGUI>().alpha = 0f;

            NoticeText.SetActive(false);
        }
        while (Queue_Notice != null && Queue_Notice.Count > 0);

        notice_showing = false;

        if(exitFlag == true)
        {
            Application.Quit();
        }
    }

    //Show keyboard moving tutorial image
    //키보드 입력(W,A,S,D)을 통한 캐릭터 조작 안내 이미지
    public void ShowTutorialImage(bool flag)
    {
        Controller_Image.SetActive(flag);
    }

    //Quest Infomation Open/Close and Update
    //퀘스트창 관리 및 업데이트
    void OpenQuestPanel()
    {
        UpdateQuest();
        window_quest.SetActive(true);
        Manager_Sounds.instance.PlayOneShot(8);
        isQuestOpen = true;
    }

    public void OpenQuestDescription(int id = -1)
    {
        if (id != -1)
        {
            Data_Quest.questData qData;
            Data_Quest.instance.data.TryGetValue(Manager_Player_Info.instance.quest_list[id], out qData);

            quest_description_panel.GetComponent<Text>().text = qData.description;
            Dictionary<int, int>.KeyCollection keys = qData.target.Keys;
            quest_description_panel.transform.GetChild(0).GetComponent<Image>().sprite = Data_Item.instance.data.GetValueOrDefault(keys.First()).sprite;
            int currCount = 0;
            Debug.Log(keys.First());
            for (int i = 0; i < Manager_Player_Info.instance.current_item.Count; i++)
            {
                if (Manager_Player_Info.instance.current_item[i] == keys.First())
                {
                    currCount++;
                }
            }
            quest_description_panel.transform.GetChild(1).GetComponent<Text>().text = currCount.ToString();
            quest_description_panel.transform.GetChild(2).GetComponent<Text>().text = qData.target.GetValueOrDefault(keys.First()).ToString();

            quest_description_panel.SetActive(true);
        }
        else
        {
            quest_description_panel.SetActive(false);
        }
    }
    
    public void CloseQuestPanel()
    {
        window_quest.SetActive(false);
        OpenQuestDescription(-1);
        Manager_Sounds.instance.PlayOneShot(9);
        isQuestOpen = false;
    }

    public void UpdateQuest()
    {
        for (int i = quest_grid_panel.transform.childCount - 1; i >= 0; i--)
        {
            quest_grid_panel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "";
            quest_grid_panel.transform.GetChild(i).GetComponent<Button>().interactable = false;
            quest_grid_panel.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        for (int i = 0; i < Manager_Player_Info.instance.quest_list.Count; i++)
        {
            Data_Quest.questData qData;
            Data_Quest.instance.data.TryGetValue(Manager_Player_Info.instance.quest_list[i], out qData);

            quest_grid_panel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = qData.title;
            int index = i;
            quest_grid_panel.transform.GetChild(i).GetComponent<Button>().interactable = true;
            quest_grid_panel.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            quest_grid_panel.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => OpenQuestDescription(index));
        }
    }

    //Inventory Infomation Open/Close and Update
    //인벤토리창 관리 및 업데이트
    void OpenInventoryPanel()
    {
        UpdateItem();
        UpdateMoney();
        window_inventory.SetActive(true);
        Manager_Sounds.instance.PlayOneShot(8);
        isItemOpen = true;
    }

    public void CloseInventoryPanel()
    {
        window_inventory.SetActive(false);
        Manager_Sounds.instance.PlayOneShot(9);
        isItemOpen = false;
    }

    public void UpdateMoney()
    {
        MoneyText.GetComponent<Text>().text = Manager_Player_Info.instance.GetCurrentMoney().ToString() + "원";
    }
    
    public void UpdateItem()
    {
        for (int i = item_grid_panel.transform.childCount - 1; i >= 0; i--)
        {
            for(int j = item_grid_panel.transform.GetChild(i).childCount -1; j>=0; j--)
            {
                Destroy(item_grid_panel.transform.GetChild(i).GetChild(j).gameObject);
            }
                
        }

        for (int i = 0; i < Manager_Player_Info.instance.current_item.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, item_grid_panel.transform.GetChild(i).transform);

            Data_Item.ItemData itemData;
            Data_Item.instance.data.TryGetValue(Manager_Player_Info.instance.current_item[i], out itemData);
            item.GetComponent<Image>().sprite = itemData.sprite;
        }
    }

    //Update every UI from User Infomation
    public void UpdateUIFromPlayerInfo()
    {
        UpdateMoney();
        UpdateItem();
        UpdateQuest();
    }

    //Close all UI window
    //모든 UI 윈도우 닫기
    public void CloseAllPanel()
    {
        OpenExitPanel(false);
        SettingPanel.SetActive(false);
        SettingButton.SetActive(true);
        Manager_Sounds.instance.PlayOneShot(9);
        if (Manager_Player_Info.instance.isTalking() == true)
            Manager_Dialogue.instance.EndConversation();
    }
}
