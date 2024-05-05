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
    //열쇠 획득
    public void GetKey()
    {
        Manager_Sounds.instance.PlayOneShot(2);
        hasKey = true;
    }

    //Open a specific door
    // 정해진 문 열기
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
                    Manager_UI.instance.ShowNotice("열쇠로 문을 열었다.");
                }
                else if(bDoorOpen_01 == false && hasKey == false)
                {
                    Manager_UI.instance.ShowNotice("열쇠가 없어 들어갈 수 없다...");
                }

                break;
            case 2:
                if (bDoorOpen_02 == false)
                {
                    Manager_Sounds.instance.PlayOneShot(1);
                    StartCoroutine(OpenDoorSequence(index));
                    Manager_UI.instance.ShowNotice("문이 열렸다.");

                }
                break;
        }
    }

    //Open door sequence
    //문 열리는 애니메이션
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
    //정해진 문 닫기
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
                    Manager_UI.instance.ShowNotice("몬스터를 모두 무찌르면 문이 열릴 것 같다.");
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
    //문 닫히는 애니메이션
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
    //몬스터 제거 및 남은 몬스터 확인 후 이벤트 작용
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
    //보스 등장 애니메이션
    public void StartBossAnimation()
    {
        CloseDoor(1);
        Manager_Player_Info.instance.SetControlableMove(false);
        Manager_Player_Info.instance.SetControlableRotate(false);
        Manager_Player_Info.instance.SetCombatState(false);
        StartCoroutine(BossAnimation());
    }

    //Boss monster appear animation sequence
    //보스 등장 애니메이션 시퀀스
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
    //보스영상 타임라인 시퀀스 중 signal 이벤트
    public void RecieveSignal()
    {
        Manager_Player_Info.instance.SetControlableMove(true);
        Manager_Player_Info.instance.SetControlableRotate(true);
        Manager_Player_Info.instance.SetCombatState(true);
        Monsters_Boss.GetComponent<Script_Monster>().isMovable = true;
    }

    //Play boss monster die animation
    //보스 사망 애니메이션
    public void BossDieEvent(Script_Monster mon)
    {
        StartCoroutine(BossDieSequence(mon));
        
    }
    
    //Boss monster die and disappear animation
    //보스 사망 애니메이션 시퀀스
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
    //보스 사망 체크
    public bool IsBossDie()
    {
        return bBossDie;
    }
    
    //Play Last Project Sequence
    //최종 보스 제거이후 프로젝트 종료 시퀀스
    public void EndingAnimation()
    {
        Manager_Sounds.instance.PlayOneShot(3);
        Manager_Sounds.instance.PlayOneShot(4);
        Manager_UI.instance.ShowNotice("던전 탐험을 완료했다.", true);
        Manager_UI.instance.ShowNotice("게임을 종료합니다.", true);
        Manager_UI.instance.SetExitFlag();
    }
    
    //Event by signal from timeline director
    //보스영상 타임라인 시퀀스 중 사운드 관련 signal 이벤트
    public void SoundFadeOut()
    {
        Manager_Sounds.instance.FadeOutStart();
    }
    public void SoundFadeIn()
    {
        Manager_Sounds.instance.FadeInStart();
    }
}
