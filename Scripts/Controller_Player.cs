using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Player : Script_HumanCharacter
{
    private float fSpeed_turn = 2.0f;
    private float fDistance_action = 1.0f;

    private Script_NPC closestNPC;
    private GameObject closestObject;
    
    private Rigidbody body;
    public GameObject Weapon_point;

    // Start is called before the first frame update
    void Start()
    {
        anim_char = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //RigidBody�� �̿��� ��������̱� ������ FixedUpdate���� ����
        if (Manager_Player_Info.instance.IsControlableMove())
            MoveCharacter();
    }

    void Update()
    {
        //�ִϸ��̼� �Ǵ� Ű �Է� �� ���������ӿ� ���� �ʿ� ���� �׸���� Update���� ����
        if (Manager_Player_Info.instance.IsControlableRotate())
            MouseRotate();
        if (Manager_Player_Info.instance.IsAttacking() == false)
            UpdateMovementStates();
        if(Manager_Player_Info.instance.IsCombatState() && !Manager_Player_Info.instance.IsAttacking() && Input.GetMouseButton(0))
            Attack();

        ObjectAction();
    }

    //Move player character with physics
    //��������� �̿��� �÷��̾� ĳ���� �̵�
    private void MoveCharacter()
    {
        vec_move.x = Input.GetAxisRaw("Horizontal");
        vec_move.z = Input.GetAxisRaw("Vertical");
        body.velocity = transform.TransformDirection(vec_move) * fSpeed_character;

        if (vec_move.x != 0f || vec_move.z != 0f)
            Manager_Player_Info.instance.SetFirstMoved();   
    }

    //Rotate player character with mouse cursor
    //���콺 Ŀ�� �̵��� �̿��� �÷��̾� �¿� ������ȯ
    private void MouseRotate()
    {
        float fRotateSize = Input.GetAxis("Mouse X") * fSpeed_turn;
        float yRotate = transform.eulerAngles.y + (fRotateSize * 4f);
        transform.eulerAngles = new Vector3(0, yRotate, 0);   
    }

    //Change animation state
    //�̵����¿� ���� ĳ���� �ִϸ��̼� ��ȯ
    private void UpdateMovementStates(bool flag = true)
    {
        if (Manager_Player_Info.instance.IsControlableMove() == false || flag == false)
        {
            anim_char.SetInteger("MovementState", 0);
            return;
        }
        if (vec_move.x > 0)
        {
            anim_char.SetInteger("MovementState", 4);
        }
        else if (vec_move.x < 0)
        {
            anim_char.SetInteger("MovementState", 3);
        }
        else if (vec_move.z > 0)
        {
            anim_char.SetInteger("MovementState", 1);
        }
        else if (vec_move.z < 0)
        {
            anim_char.SetInteger("MovementState", 2);
        }
        else
        {
            anim_char.SetInteger("MovementState", 0);
        }
    }

    //Interaction with objects include NPC or field objects
    //NPC�� �ʵ������Ʈ ���� ������ ������Ʈ ��ȣ�ۿ�
    private void ObjectAction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindObject();

            if (closestObject != null)
            {
                if(closestObject.gameObject.tag == "NPC")
                {
                    //��ȣ�ۿ� ��� ������Ʈ�� NPC�� ���
                    closestNPC = closestObject.GetComponent<Script_NPC>();
                    UpdateMovementStates(false);
                    if (closestObject.name == "NPC001")
                    {
                        if (Manager_Player_Info.instance.CheckQuestComplete(0))
                        {
                            TryTalk(1);
                        }
                        else if (Manager_Player_Info.instance.complete_quest_list.Contains(0))
                        {
                            TryTalk(2);
                        }
                        else
                        {
                            TryTalk();
                        }
                    }
                    else if (closestObject.name == "NPC002")
                    {
                        if (Manager_Player_Info.instance.current_item.Contains(1))
                        {
                            TryTalk(3);
                        }
                        else
                        {
                            TryTalk();
                        }
                    }
                    else if (closestObject.name == "NPC003")
                    {
                        if (Manager_Player_Info.instance.current_item.Contains(1))
                        {
                            TryTalk(3);
                        }
                        else
                        {
                            TryTalk();
                        }
                    }
                    else
                        TryTalk();
                }
                else
                {
                    //��ȣ�ۿ� ��� ������Ʈ�� NPC�� �ƴ� ���
                    if (closestObject.gameObject.tag == "Grass")

                        if (Manager_Player_Info.instance.current_item.Contains(0))
                        {
                            //������ ���������� Ȯ��
                            DestroyImmediate(closestObject);
                            Manager_Player_Info.instance.RemoveItem(0);
                            Manager_Player_Info.instance.AddItem(2);
                            Manager_Sounds.instance.PlayOneShot(9);
                        }
                        else
                        {
                            Manager_UI.instance.ShowNotice("ä���� ������ ����...");
                        }
                }                
            }
        }
    }

    //Find the closest object with player character
    //�÷��̾�Լ� ���� ����� ������Ʈ Ž��
    private void FindObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, fDistance_action);
        float minDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            //GameObject npc = collider.GetComponent<Script_NPC>();
            
            GameObject curr_obj = collider.gameObject;
            if (curr_obj.tag == "Player" || curr_obj.tag == "Terrain" || curr_obj.tag == "Untagged")
                continue;
            
            float distance = Vector3.Distance(transform.position, curr_obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestObject = curr_obj;
            }
            

            //if (npc != null)
            //{
            //    float distance = Vector3.Distance(transform.position, npc.transform.position);
            //    if (distance < minDistance)
            //    {
            //        minDistance = distance;
            //        closestObject = npc;
            //    }
            //}
            //else if (collider.name == "Grass")
            //{
            //    if (Manager_Player_Info.instance.current_item.Contains(0))
            //    {
            //        DestroyImmediate(collider.gameObject);
            //        Manager_Player_Info.instance.RemoveItem(0);
            //        Manager_Player_Info.instance.AddItem(2);
            //        Manager_Sounds.instance.PlayOneShot(9);
            //    }
            //    else
            //    {
            //        Manager_UI.instance.ShowNotice("ä���� ������ ����...");
            //    }
            //    closestObject = null;
            //}
        }
        return;
    }
    
    //Try start talk with npc
    //������Ʈ�� NPC�� ��� Dialog Manager�� �̿��� ��ȭ �õ�
    private void TryTalk(int flag = 0)
    {
        if (!Manager_Player_Info.instance.isTalking())
        {
            closestNPC.LookPlayer();
            Manager_Player_Info.instance.SetTalkingState(true);

            switch (flag)
            {
                case 0:
                    Manager_Dialogue.instance.StartConversation(closestObject.gameObject.name, 0);
                    break;
                case 1:
                    Manager_Dialogue.instance.StartConversation(closestObject.gameObject.name, 9);
                    Manager_Player_Info.instance.CompleteQuest(0);
                    break;
                case 2:
                    Manager_Dialogue.instance.StartConversation(closestObject.gameObject.name, 10);
                    break;
                case 3:
                    Manager_Dialogue.instance.StartConversation(closestObject.gameObject.name, 1);
                    break;
            }
        }
        else
        {
            Manager_Dialogue.instance.UpdateDialogPage();
        }
    }

    //Call when finish talk with npc
    //NPC�� ��ȭ ���� �� NPC ���� ���󺹱�
    public void EndTalk()
    {
        closestNPC.LookOriginal();
        closestNPC = null;
    }

    //Attack after check attack delay
    //���� ������ Ȯ�� �� ����
    private void Attack()
    {
        StartCoroutine(GetAttackDelay());
        anim_char.SetTrigger("AttackState");
    }

    //Calculate attack delay
    //���� ������ ����Ͽ� ���º�ȯ
    private IEnumerator GetAttackDelay()
    {
        float attackDelay = 0.8f;
        Weapon_point.SetActive(true);
        while (attackDelay>0f)
        {
            Manager_Player_Info.instance.SetAttack(true);
            attackDelay -= Time.deltaTime;
            yield return null;
        }
        Manager_Player_Info.instance.SetAttack(false);
        Weapon_point.SetActive(false);
        yield return null;
    }
}
