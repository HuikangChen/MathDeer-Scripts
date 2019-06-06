using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class PlayerJumpSettings
{
    public float jumpvelocity = 2.25f;
    public float jumpTime = .25f;
    public int maxJumpCount = 3;

    [HideInInspector]
    public int jumpCount;

    [HideInInspector]
    public float jumpTimeCounter;

    [HideInInspector]
    public bool isJumping;

    public void Init()
    {
        jumpCount = maxJumpCount;
    }
}
