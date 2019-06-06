using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains dash settings for the player
/// </summary>

[System.Serializable]
class PlayerDashSettings
{

    [Tooltip("The amount of speed to add onto the horizontalSpeed when dashing")]
    public float dashSpeed = 1;

    [Tooltip("How long the dash last")]
    public float dashDuration = .5f;

    [Tooltip("Dash FX to spawn")]
    public GameObject dashFx;

    public Transform fxSpawnPos;

    [Tooltip("Cooldown starts right when dash starts")]
    public float dashCooldown = .75f;

    [HideInInspector]
    public float dashTimeStamp; //Time stamp used for dash cooldown

    [HideInInspector]
    public float currentDashDuration;

    [HideInInspector]
    public bool isDashing;
}
