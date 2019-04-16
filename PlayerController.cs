using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController singleton;

    public SpriteRenderer deerSprite;
    public Collider2D hitBox;
    public Collider2D pickupBox;

    void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        GameManager.OnGameStart += Initialize;
    }

    public void Initialize()
    {
        hitBox.enabled = true;
    }

    public void DisableHitBox(float duration, bool changeAlpha)
    {
        StopAllCoroutines();
        StartCoroutine(DisableHitBoxCo(duration, changeAlpha));
    }

    IEnumerator DisableHitBoxCo(float duration, bool changeAlpha)
    {
        hitBox.enabled = false;
        Color color = deerSprite.color;
        if (changeAlpha)
        {
            color.a = .5f;
            deerSprite.color = color;
        }

        while (duration > 0.01)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        hitBox.enabled = true;

        if (changeAlpha)
        {
            color.a = 1f;
            deerSprite.color = color;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStart -= Initialize;
    }
}
