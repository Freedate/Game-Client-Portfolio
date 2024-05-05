using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Manager_Dungeon : MonoBehaviour
{
    public static Manager_Dungeon instance;

    private void Awake()
    {
        Manager_UI.instance.FadeInStart();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject Door_01;
    public GameObject Door_02;

    private bool bDoorOpen_01 = false;
    private bool bDoorOpen_02 = true;

    private bool bBossDie = false;

    public List<Script_Monster> MonsterGroup_01;

    private bool hasKey = false;

    public GameObject Monsters_01;
    public GameObject Monsters_Boss;

    public PlayableDirector director;

    private void Start()
    {
        Manager_Sounds.instance.ChangeMusic(1);
        Manager_Player_Info.instance.SetCombatState(true);
    }

    //Get a key
    //���� ȹ��
    public void GetKey()
    {
        Manager_Sounds.instance.PlayOneShot(2);
        hasKey = true;
    }

    //Open a specific door
    // ������ �� ����
    public void OpenDoor(int index = 0)
    {
        switch(index)
        {
            case 0:
                break;
            case 1:
                if(bDoorOpen_01 == false && hasKey == true)
                {
                    Manager_Sounds.instance.PlayOneShot(1);
                    StartCoroutine(OpenDoorSequence(index));
                    Manager_UI.instance.ShowNotice("����� ���� ������.");
                }
                else if(bDoorOpen_01 == false && hasKey == false)
                {
                    Manager_UI.instance.ShowNotice("���谡 ���� �� �� ����...");
                }

                break;
            case 2:
                if (bDoorOpen_02 == false)
                {
                    Manager_Sounds.instance.PlayOneShot(1);
                    StartCoroutine(OpenDoorSequence(index));
                    Manager_UI.instance.ShowNotice("���� ���ȴ�.");

                }
                break;
        }
    }

    //Open door sequence
    //�� ������ �ִϸ��̼�
    private IEnumerator OpenDoorSequence(int index)
    {

        if (index == 1)
        {
            bDoorOpen_01 = true;
            float fClose_time = 1.0f;
            while (fClose_time >= 0f)
            {
                Door_01.transform.Find("left").transform.Translate(new Vector3(0.01f, 0, 0));
                Door_01.transform.Find("right").transform.Translate(new Vector3(-0.01f, 0, 0));
                fClose_time -= Time.deltaTime;
                yield return null;
            }
        }
        else if(index == 2)
        {
            Monsters_Boss.SetActive(true);
            bDoorOpen_02 = true;
            float fClose_time = 0.5f;
            while (fClose_time >= 0f)
            {
                Door_02.transform.Translate(new Vector3(0f, -0.03f, 0));
                fClose_time -= Time.deltaTime;
                yield return null;
            }
        }

        yield return null;
    }

    //Cloase a specific door
    //������ �� �ݱ�
    public void CloseDoor(int index = 0)
    {
        switch (index)
        {
            case 0:
                break;
            case 1:
                if (bDoorOpen_01 == true)
                {
                    Manager_Sounds.instance.PlayOneShot(1);
                    StartCoroutine(CloseDoorSequence(index));
                }
                break;
            case 2:
                if (bDoorOpen_02 == true)
                {
                    Manager_UI.instance.ShowNotice("���͸� ��� ����� ���� ���� �� ����.");
                    Manager_Sounds.instance.PlayOneShot(1);
                    Monsters_01.SetActive(true);
                    MonsterGroup_01 = new List<Script_Monster>();

                    for (int i = 0; i <Monsters_01.transform.childCount; i++)
                    {
                        Script_Monster m;
                        if (Monsters_01.transform.GetChild(i).TryGetComponent<Script_Monster>(out m))
                        {
                            MonsterGroup_01.Add(m);
                        }
                    }

                    StartCoroutine(CloseDoorSequence(index));
                }
                break;
        }
    }

    //Close door sequence
    //�� ������ �ִϸ��̼�
    private IEnumerator CloseDoorSequence(int index)
    {
        if(index == 1)
        {
            bDoorOpen_01 = false;
            float fClose_time = 1.0f;
            while(fClose_time >= 0f)
            {
                Door_01.transform.Find("left").transform.Translate(new Vector3(-0.01f, 0, 0));
                Door_01.transform.Find("right").transform.Translate(new Vector3(0.01f, 0, 0));
                fClose_time -= Time.deltaTime;
                yield return null;
            }
        }
        if (index == 2)
        {
            bDoorOpen_02 = false;
            float fClose_time = 0.5f;
            while (fClose_time >= 0f)
            {
                Door_02.transform.Translate(new Vector3(0f, 0.03f, 0));
                fClose_time -= Time.deltaTime;
                yield return null;
            }
        }

        yield return null;
    }

    //Remove a monster and count reamin monsters
    //���� ���� �� ���� ���� Ȯ�� �� �̺�Ʈ �ۿ�
    public void RemoveAndCheckMonstersGroup(Script_Monster mon)
    {
        MonsterGroup_01.Remove(mon);
        Destroy(mon.gameObject);
        if(MonsterGroup_01 != null && MonsterGroup_01.Count == 0)
        {
            OpenDoor(2);
        }
    }

    //Play boss monster appear animation
    //���� ���� �ִϸ��̼�
    public void StartBossAnimation()
    {
        CloseDoor(1);
        Manager_Player_Info.instance.SetControlableMove(false);
        Manager_Player_Info.instance.SetControlableRotate(false);
        Manager_Player_Info.instance.SetCombatState(false);
        StartCoroutine(BossAnimation());
    }

    //Boss monster appear animation sequence
    //���� ���� �ִϸ��̼� ������
    private IEnumerator BossAnimation()
    {
        float duration = 1.0f;
        Manager_UI.instance.FadeOutStart();
        while(duration > 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        Manager_UI.instance.FadeInStart();
        director.Play();
        yield return null;
    }

    //Event by signal from timeline director
    //�������� Ÿ�Ӷ��� ������ �� signal �̺�Ʈ
    public void RecieveSignal()
    {
        Manager_Player_Info.instance.SetControlableMove(true);
        Manager_Player_Info.instance.SetControlableRotate(true);
        Manager_Player_Info.instance.SetCombatState(true);
        Monsters_Boss.GetComponent<Script_Monster>().isMovable = true;
    }

    //Play boss monster die animation
    //���� ��� �ִϸ��̼�
    public void BossDieEvent(Script_Monster mon)
    {
        StartCoroutine(BossDieSequence(mon));
        
    }
    
    //Boss monster die and disappear animation
    //���� ��� �ִϸ��̼� ������
    public IEnumerator BossDieSequence(Script_Monster mon)
    {
        float die_delay = 1.0f;
        while(die_delay>0f)
        {
            die_delay -= Time.deltaTime;
            yield return null;
        }
        Destroy(mon.gameObject);
        bBossDie = true;
    }
    
    //Check boss monster is die or alive
    //���� ��� üũ
    public bool IsBossDie()
    {
        return bBossDie;
    }
    
    //Play Last Project Sequence
    //���� ���� �������� ������Ʈ ���� ������
    public void EndingAnimation()
    {
        Manager_Sounds.instance.PlayOneShot(3);
        Manager_Sounds.instance.PlayOneShot(4);
        Manager_UI.instance.ShowNotice("���� Ž���� �Ϸ��ߴ�.", true);
        Manager_UI.instance.ShowNotice("������ �����մϴ�.", true);
        Manager_UI.instance.SetExitFlag();
    }
    
    //Event by signal from timeline director
    //�������� Ÿ�Ӷ��� ������ �� ���� ���� signal �̺�Ʈ
    public void SoundFadeOut()
    {
        Manager_Sounds.instance.FadeOutStart();
    }
    public void SoundFadeIn()
    {
        Manager_Sounds.instance.FadeInStart();
    }
}
