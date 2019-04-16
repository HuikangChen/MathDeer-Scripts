using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public static CameraMovement singleton;

    public GameObject player;
    float x_offset;
    public float y_offset;
    public Transform startRunningPos;
    [HideInInspector] public bool playerRunningWithCamera;

    bool isRunning = false; //Is the coroutine running right now?

    float duration;

    float shake_offset_x;
    float shake_offset_y;

    void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        x_offset = transform.position.x - startRunningPos.position.x;
    }

    void MoveCamera(Vector3 shake_offset)
    {
        Vector3 target = new Vector3(player.transform.position.x + x_offset, y_offset, -10) + shake_offset;
        transform.position = target;
    }

    Vector3 ShakeOffset()
    {
        Vector3 offset = new Vector3(shake_offset_x, shake_offset_y, 0);
        return offset;
    }

    Vector3 CenterOfScreen()
    {
        return new Vector2(Screen.width / 2, Screen.height / 2);
    }


    void FixedUpdate()
    {

        if (player == null)
            return;

        if (player.transform.position.x < startRunningPos.position.x)
        {
            playerRunningWithCamera = false;
            return;
        }

        playerRunningWithCamera = true;
        MoveCamera(ShakeOffset());
    }

    void ShakeCamera()
    {
        if (isRunning == false)
            StartCoroutine(Shake());
    }

    public void ShakeCamera(float amount, float duration)
    {

        this.duration = duration;

        StartCoroutine(Shake(amount, duration));
    }


    public void ShakeCamera(float amount, float duration, Vector2 dir)
    {
        if (isRunning)
            return;


        StartCoroutine(ShakeInDir(amount, duration, dir));
    }

    IEnumerator ShakeInDir(float amount, float duration, Vector2 dir)
    {
        float time = duration;
        while (time > 0)
        {
            shake_offset_x = dir.normalized.x * Random.value * amount;
            shake_offset_y = dir.normalized.y * Random.value * amount;
            time -= Time.deltaTime;
            yield return null;
        }

        shake_offset_x = 0;
        shake_offset_y = 0;
    }

    public IEnumerator Shake(float amount, float duration)
    {
        isRunning = true;

        float time = duration;
        while (time > 0)
        {
            shake_offset_x = (Random.value / 20f) * amount;
            shake_offset_y = (Random.value / 20f) * amount;
            time -= Time.deltaTime;
            yield return null;
        }

        shake_offset_x = 0;
        shake_offset_y = 0;

        isRunning = false;
    }

    public IEnumerator Shake()
    {
        isRunning = true;

        float time = duration;
        while (time > 0)
        {
            shake_offset_x = Random.value / 20f;
            shake_offset_y = Random.value / 20f;
            time -= Time.deltaTime;
            yield return null;
        }

        shake_offset_x = 0;
        shake_offset_y = 0;

        isRunning = false;
    }
}
