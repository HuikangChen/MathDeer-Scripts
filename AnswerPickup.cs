using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnswerPickup : MonoBehaviour {

    public static bool answerPicked;

    public TextMeshPro answerText;
    public int answerIndex;
    int answer;

    void Start()
    {
        answer = QuestionManager.singleton.answerBank[answerIndex];
        answerText.text = answer+"";
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (answerPicked)
            return;

        if (col.tag == "PlayerPickupBox")
        {
            if (QuestionManager.singleton.AnswerCheck(answer))
            {
                UpgradeManager.singleton.ActivatePowerUp();
                SoundManager.singleton.PlayAudioEffect(SoundManager.singleton.correct);
            }
            else
            {
                SoundManager.singleton.PlayAudioEffect(SoundManager.singleton.wrong);
            }
            answerPicked = true;
            gameObject.SetActive(false);
        }
    }

}
