using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_HumanCharacter : MonoBehaviour
{
    protected float fSpeed_character = 5.0f;
    protected Vector3 vec_move = new Vector3();
    protected Animator anim_char;

    enum movementStates
    {
        Idle = 0,
        Forward = 1,
        Backward = 2,
        Left = 3,
        Right = 4,
        Jump = 5,
    }
}
