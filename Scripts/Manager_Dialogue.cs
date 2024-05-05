using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using static Data_Item;

public class Manager_Dialogue : MonoBehaviour
{
    public static Manager_Dialogue instance;

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

    public GameObject talkPanel = null;
    public GameObject talkText = null;
    public GameObject nameText = null;
    public GameObject selectPanel = null;
    public GameObject selectablePrefab = null;
    public GameObject storePanel = null;
    public GameObject storeContentPanel = null;
    public GameObject storeItemPrefab = null;

    private bool bSelectOpen = false;
    private bool bStoreOpen = false;

    private string curr_npc_id;
    private int curr_script_id;
    private int max_script_page;
    private int curr_page;

    //Start Conversation with NPC
    //NPC 및 Script ID에 맞는 대화 시작
    public void StartConversation(string npc_id, int script_id)
    {
        Debug.Log("StartConversation-nid:" + npc_id  + "/sid:" + script_id);
        curr_npc_id = npc_id;
        curr_script_id = script_id;
        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
        curr_page = 0;
        talkPanel.gameObject.SetActive(true);
        talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page++];
        nameText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).name;
    }

    //Continue conversation and brunching by situation
    //대화 계속 진행 및 상황별 분기 상황 처리
    public void UpdateDialogPage()
    {
        if(bSelectOpen || bStoreOpen)
            return;

        Debug.Log("UpdateDialogPage-MAX:" + max_script_page + "/current:" + curr_page);
        if (max_script_page > curr_page)
        {
            if(Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Contains("/{0}"))
            {
                talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Split("/{0}")[0];
                string[] args = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Split("/{0}")[1].Split('|');
                bSelectOpen = true;
                OpenSelect(args.Length, args);
                curr_page++;
            }
            else if (Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Contains("/{1}"))    //Quest
            {
                talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Split("/{1}")[0];
                if(Manager_Player_Info.instance.GetQuest(0))
                {
                    curr_page++;
                }
                else
                {
                    curr_page++;
                }
            }
            else if (Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Contains("/{2}"))    //Store
            {
                talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Split("/{2}")[0];
                string[] args = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page].Split("/{2}")[1].Split('|');
                bStoreOpen = true;
                OpenStore(args.Length, args);
                curr_page++;
            }
            else
            { 
                talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id)[curr_page];
                curr_page++;
            }
        }
        else
        {
            EndConversation();
        }
    }

    //Finish conversation
    //대화 종료 시 상태변환 및 대화창 종료
    public void EndConversation()
    {
        Debug.Log("EndConversation");
        talkPanel.gameObject.SetActive(false);
        Manager_Player_Info.instance.SetTalkingState(false);
    }

    //Select dialogue during talk with NPC
    //대화 중 선택지가 있는 대화 진행 시 함수 처리
    public void OpenSelect(int selectCount, params string[] args)
    {
        int i = 0;

        foreach(string arg in args)
        {
            GameObject button = Instantiate(selectablePrefab, selectPanel.transform);
            button.GetComponentInChildren<Text>().text = arg;
            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => SelectFunction(index));
            i++;
        }
        selectPanel.SetActive(true); 
    }

    public void SelectFunction(int index)
    {
        for (int i = selectPanel.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(selectPanel.transform.GetChild(i).gameObject);
        }
        bSelectOpen = false;
        Manager_Sounds.instance.PlayOneShot(7);
        if (curr_script_id == 0)
        {
            switch (index)
            {
                case 0:
                    curr_script_id = 1;
                    max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                    curr_page = 0;
                    UpdateDialogPage();
                    break;
                case 1:
                    curr_script_id = 2;
                    max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                    curr_page = 0;
                    UpdateDialogPage();
                    break;
                case 2:
                    curr_script_id = 3;
                    max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                    curr_page = 0;
                    UpdateDialogPage();
                    break;
                case 3:
                    curr_script_id = 4;
                    max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                    curr_page = 0;
                    UpdateDialogPage();
                    break;
            }
        }
        else if (curr_script_id == 1)
        {
            if (curr_npc_id == "NPC003")
            {
                switch (index)
                {
                    case 0:
                        curr_script_id = 2;
                        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                        curr_page = 0;
                        UpdateDialogPage();
                        Manager_UI.instance.FadeOutStart(2);
                        break;
                    case 1:
                        curr_script_id = 3;
                        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                        curr_page = 0;
                        UpdateDialogPage();
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        curr_script_id = 7;
                        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                        curr_page = 0;
                        UpdateDialogPage();
                        break;
                    case 1:
                        curr_script_id = 4;
                        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                        curr_page = 0;
                        UpdateDialogPage();
                        break;
                }
            }
        }
        else if (curr_script_id == 10)
        {
            switch (index)
            {
                case 0:
                    curr_script_id = 2;
                    max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                    curr_page = 0;
                    UpdateDialogPage();
                    break;
                case 1:
                    if (Manager_Player_Info.instance.current_item.Contains(1))
                    {
                        curr_script_id = 12;
                        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                        curr_page = 0;
                        UpdateDialogPage();
                    }
                    else
                    {
                        curr_script_id = 11;
                        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                        curr_page = 0;
                        UpdateDialogPage();
                    }
                    break;
                case 2:
                    curr_script_id = 4;
                    max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
                    curr_page = 0;
                    UpdateDialogPage();
                    break;
            }
        }
    }

    //Store system management
    //상점 시스템 관리 (열기 / 물건구매 / 닫기)
    public void OpenStore(int itemCount, params string[] args)
    {
        foreach (string arg in args)
        {
            GameObject itemGrid = Instantiate(storeItemPrefab, storeContentPanel.transform);
            ItemData item = new ItemData();
            Debug.Log(arg);
            int itemIndex = int.Parse(arg);
            Data_Item.instance.data.TryGetValue(itemIndex, out item);

            itemGrid.transform.Find("Item_Image").GetComponent<Image>().sprite = item.sprite;
            itemGrid.transform.Find("Item_Name").GetComponent<Text>().text = item.name;
            itemGrid.transform.Find("Item_Price").GetComponent<Text>().text = item.price.ToString();
            itemGrid.transform.Find("Item_Content").GetComponent<Text>().text = item.describe;

            if (Manager_Player_Info.instance.bought_item.Contains(itemIndex))
            {
                itemGrid.transform.Find("Item_Image/Item_Soldout").GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                itemGrid.GetComponent<Button>().onClick.AddListener(() => BuyItem(itemIndex, itemGrid));
            }
        }
            
        storePanel.SetActive(true);
    }

    public void BuyItem(int index, GameObject itemGrid)
    {
        ItemData item = new ItemData();
        Data_Item.instance.data.TryGetValue(index, out item);
        

        if (Manager_Player_Info.instance.UseMoney(item.price))
        {
            Manager_Player_Info.instance.AddItem(index);
            itemGrid.transform.Find("Item_Image/Item_Soldout").GetComponent<Image>().color = new Color(255, 255, 255, 255);
            itemGrid.GetComponent<Button>().onClick.RemoveAllListeners();
            talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(5)[0];
            Manager_Player_Info.instance.bought_item.Add(index);

        }
        else
        {
            talkText.GetComponent<Text>().text = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(6)[0];
        }
    }

    public void CloseStore()
    {
        for (int i = storeContentPanel.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(storeContentPanel.transform.GetChild(i).gameObject);
        }
        bStoreOpen = false;
        storePanel.SetActive(false);

        curr_script_id = 4;
        max_script_page = Data_NPC.instance.data.GetValueOrDefault(curr_npc_id).scriptData.GetValueOrDefault(curr_script_id).Count;
        curr_page = 0;
        UpdateDialogPage();       

    }
}