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
        //NPC 네임플레이트는 항상 플레이어의 메인카메라에 평행하게 유지
        if(Camera.main != null)
            nameText.transform.LookAt(Camera.main.transform);
    }
}
