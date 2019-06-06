using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to platform that spawns answers
/// </summary>

public class AnswerSpawner : MonoBehaviour {

    //our answer prefab
    public GameObject answer;

    void Start()
    {
        answer.SetActive(false); 

        //Everytime we spawn this platform, we decrement the platformBufferAmount
        QuestionManager.Instance.platformBufferAmount--;

        //We spawn/show the answer only if the buffer amount reaches 0, giving the player time to think
        if (QuestionManager.Instance.platformBufferAmount == 0)
        {
            answer.SetActive(true);
            AnswerPickup.answerPicked = false;
        }
    }
 }
