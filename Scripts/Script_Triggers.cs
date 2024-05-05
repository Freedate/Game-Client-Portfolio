using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Triggers : MonoBehaviour
{
    public int trigger_index = -1;

    //Event when character enter at triggers
    //�÷��̾ Ʈ���� Collider�� ���� ��� �̺�Ʈó��
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
                    //���� ȹ��
                    Debug.Log("Get Key");
                    Manager_Dungeon.instance.CloseDoor(trigger_index);
                    Manager_Dungeon.instance.GetKey();
                    Destroy(gameObject);
                    break;
                case 3:
                    //������ ����
                    Manager_Dungeon.instance.StartBossAnimation();
                    Destroy(gameObject);
                    break;
                case 4:
                    //�������� & ��������
                    if (Manager_Dungeon.instance.IsBossDie() == true) //���� �̱���
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
