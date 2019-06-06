using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The pickup for answers, checks if the answer is correct when player picks it up
/// </summary>

public class AnswerPickup : MonoBehaviour {

    /// <summary>
    /// has player picked an answer? Used to make sure player doesn't pick more than 1 answer
    /// </summary>
    public static bool answerPicked;

    //our text Display for the answer
    [SerializeField]
    private TextMeshPro answerDisplay;

    [Tooltip("Index is from 0-2")]
    [SerializeField]
    private int answerIndex;
    private int answer;

    void Start()
    {
        //get our correct answer from the answer bank
        answer = QuestionManager.Instance.generator.answerBank[answerIndex];
        answerDisplay.text = answer+"";
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //if we already picked up an answer 
        if (answerPicked)
            return;

        if (col.tag == "PlayerPickupBox")
        {
            //if the answer we picked up was correct
            if (QuestionManager.Instance.AnswerCheck(answer))
            {
                //activate powerup
                PowerUpManager.Instance.ActivatePowerUp();
                SoundManager.Instance.PlayAudioEffect(SoundManager.Instance.correct);
            }
            else
            {
                SoundManager.Instance.PlayAudioEffect(SoundManager.Instance.wrong);
            }
            answerPicked = true;
            gameObject.SetActive(false);
        }
    }

}
