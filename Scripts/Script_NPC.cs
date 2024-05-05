using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_NPC : MonoBehaviour
{

    private Transform player_char;
    private Quaternion origin_rotation;

    // Start is called before the first frame update
    void Start()
    {
        player_char = GameObject.FindGameObjectWithTag("Player").transform;
        origin_rotation = transform.rotation;
    }

    public void LookPlayer()
    {
        transform.LookAt(player_char);
    }

    public void LookOriginal()
    {
        transform.rotation = origin_rotation;
    }
}
