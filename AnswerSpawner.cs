using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerSpawner : MonoBehaviour {

    public GameObject answer;

    void Start()
    {
        answer.SetActive(false); 

        QuestionManager.singleton.platformBufferAmount--;

        if (QuestionManager.singleton.platformBufferAmount == 0)
        {
            answer.SetActive(true);
            AnswerPickup.answerPicked = false;
        }
    }
 }
