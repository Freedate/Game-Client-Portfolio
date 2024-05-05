using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Triggers : MonoBehaviour
{
    public int trigger_index = -1;

    //Event when character enter at triggers
    //플레이어가 트리거 Collider에 들어갔을 경우 이벤트처리
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch(trigger_index)
            {
                case 1:
                    Manager_Dungeon.instance.OpenDoor(trigger_index);
                    break;
                case 2:
                    //열쇠 획득
                    Debug.Log("Get Key");
                    Manager_Dungeon.instance.CloseDoor(trigger_index);
                    Manager_Dungeon.instance.GetKey();
                    Destroy(gameObject);
                    break;
                case 3:
                    //보스몹 연출
                    Manager_Dungeon.instance.StartBossAnimation();
                    Destroy(gameObject);
                    break;
                case 4:
                    //최종보상 & 게임종료
                    if (Manager_Dungeon.instance.IsBossDie() == true) //보스 이긴경우
                    {
                        Manager_Player_Info.instance.SetControlableMove(false);
                        Manager_Player_Info.instance.SetControlableRotate(false);
                        Manager_Dungeon.instance.EndingAnimation();
                        Manager_UI.instance.FadeOutStart();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
