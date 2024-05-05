using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Script_NamePlate : MonoBehaviour
{
    public Text nameText;

    private void Start()
    {
        nameText.text = Data_NPC.instance.data.GetValueOrDefault(gameObject.name).name;
    }

    private void Update()
    {
        //NPC �����÷���Ʈ�� �׻� �÷��̾��� ����ī�޶� �����ϰ� ����
        if(Camera.main != null)
            nameText.transform.LookAt(Camera.main.transform);
    }
}
