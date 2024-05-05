using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;
using Unity.VisualScripting;


public class Manager_Player_Info : MonoBehaviour
{
    public static Manager_Player_Info instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private int current_money = 0;
    public List<int> current_item;
    public List<int> bought_item;

    public List<int> quest_list;
    public List<int> complete_quest_list;

    
    public GameObject Controller_Icon;
    public GameObject Weapon_point;

    private bool bControlableMove = true;
    private bool bControlableRotate = true;
    private bool bCombat = false;
    private bool bFirstMove = false;
    private bool bAttacking = false;
    private bool bTalking = false;

    // Start is called before the first frame update
    void Start()
    {
        current_item = new List<int>();
        bought_item = new List<int>();
        quest_list = new List<int>();
        complete_quest_list = new List<int>();
    }
    
    private void Update()
    {
        //�ѹ��̶� ���������� Ʃ�丮�� �ȳ� �̹����� ����
        if (bFirstMove == true)
        {
            Manager_UI.instance.ShowTutorialImage(false);
        }
    }

    //Set player controlable state
    //���۰��� ���¼��� �� ȣ��
    public void SetControlableMove(bool flag)
    {
        bControlableMove = flag;
    }
    public bool IsControlableMove()
    {
        return bControlableMove;
    }

    public void SetControlableRotate(bool flag)
    {
        bControlableRotate = flag;
    }
    public bool IsControlableRotate()
    {
        return bControlableRotate;
    }

    public void SetTalkingState(bool flag)
    {
        bTalking = flag;
        SetControlableMove(!flag);
        SetControlableRotate(!flag);
    }
    public bool isTalking()
    {
        return bTalking;
    }

    //Combat state : avaliable attack
    //������� �� ���� ����
    public void SetCombatState(bool flag)
    {
        bCombat = flag;
    }
    public bool IsCombatState()
    {
        return bCombat;
    }

    //Player can't attack during attacking state
    //�����߿��� �ٽ� ���� �Ұ���
    public void SetAttack(bool flag)
    {
        bAttacking = flag;
    }
    public bool IsAttacking()
    {
        return bAttacking;
    }

    //Check player character has been moved
    //ĳ���� ������ �� �ִ��� Ȯ�� (Ʃ�丮�� �̹�����)
    public void SetFirstMoved()
    {
        bFirstMove = true;
    }
    public bool IsFirstMoved()
    {
        return bFirstMove;
    }
    
    //Inventory and money management
    //���� ������ �� ������ ����
    public void AddItem(int itemIndex)
    {
        current_item.Add(itemIndex);
        Manager_UI.instance.UpdateItem();
    }

    public void RemoveItem(int itemIndex)
    {
        current_item.Remove(itemIndex);
        Manager_UI.instance.UpdateItem();
    }

    public void AddMoney(int money)
    {
        Manager_Sounds.instance.PlayOneShot(0);
        current_money += money;
        Manager_UI.instance.UpdateMoney();
    }

    public bool UseMoney(int money)
    {
        if(current_money >= money) 
        {
            Manager_Sounds.instance.PlayOneShot(0);
            current_money -= money;
            Manager_UI.instance.UpdateMoney();
            return true;
        }
        else 
        {
            Debug.Log("Error:No Money");
            return false;
        }
    }
    
    public int GetCurrentMoney()
    {
        return current_money;
    }
    
    //Quest Management
    //����Ʈ ���� �� �Ϸ� ����
    public bool GetQuest(int id)
    {
        if(quest_list.Contains(id))
        {
            return false;
        }
        else
        { 
            quest_list.Add(id);
            Manager_UI.instance.UpdateQuest();
        }
        return true;
    }

    public bool CheckQuestComplete(int id)
    {
        if (quest_list.Contains(0) && current_item.Contains(2))
        {
            return true;
        }
        else
        { 
            return false;
        }
    }

    public void CompleteQuest(int id)
    {
        if(quest_list.Contains(id))
        {
            current_item.Remove(2);
            Data_Quest.questData qData;
            Data_Quest.instance.data.TryGetValue(quest_list[id], out qData);

            int money_prize = 0;
            qData.prize.TryGetValue(999, out money_prize);
            AddMoney(money_prize);

            //�����ۺ����� ���;

            quest_list.Remove(id);
            complete_quest_list.Add(id);

            Manager_UI.instance.UpdateQuest();
        }
    }
}
