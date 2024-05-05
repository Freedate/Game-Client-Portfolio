using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Monster : MonoBehaviour
{

    public int hp = 100;

    public bool isBoss = false;
    public bool isMovable = false;

    private NavMeshAgent navmeshagent = null;
    private Animator anim = null;

    public Transform moveTarget;

    // Start is called before the first frame update

    private void Start()
    {
        TryGetComponent(out navmeshagent);
        TryGetComponent(out anim);
    }
    
    private void Update()
    {
        if(navmeshagent == null || anim == null)
        { 
            
        }
        else
        {
            //���Ϳ��� navmeshagent�� ���� ��� Ÿ��(�÷��̾� ĳ����)�� ���� �̵�
            if (isMovable)
                navmeshagent.SetDestination(moveTarget.position);

            if (navmeshagent.destination != this.transform.position)
            {
                anim.SetTrigger("walk");
            }
            else
            {
                anim.SetTrigger("idle");
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon" && other.gameObject.activeSelf)
        {
            Hit();
            other.gameObject.SetActive(false);
        }
        if (CheckDie())
        {
            Die();
        }
    }

    public void Hit()
    {
        Manager_Sounds.instance.PlayOneShot(5);
        hp -= 20;

        if(isBoss)
        {
            anim.SetTrigger("stun");
        }
    }

    public bool CheckDie()
    {
        if (hp <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Die()
    { 
        if(isBoss)
        {
            Manager_Sounds.instance.PlayOneShot(6);
            anim.SetTrigger("jump2");
            Manager_Dungeon.instance.BossDieEvent(this);
        }
        else
        {
            Manager_Sounds.instance.PlayOneShot(6);
            Manager_Dungeon.instance.RemoveAndCheckMonstersGroup(this);
        }

    }

}
