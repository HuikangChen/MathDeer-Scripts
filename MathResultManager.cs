using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathResultManager : MonoBehaviour
{
    public static MathResultManager singleton;

    [HideInInspector]public List<MathResult> mathResultList = new List<MathResult>();

    public GameObject mathResultPrefab;
    public Transform resultContentPanel;

    private void Awake()
    {
        singleton = this;
    }

    public void Add(MathResult result)
    {
        mathResultList.Add(result);
    }

    public void Initialize()
    {
        ClearBoardContent();
        DisplayMathResults();
    }

    public void DisplayMathResults()
    {
        for (int i = 0; i < mathResultList.Count; i++)
        {
            GameObject obj = Instantiate(mathResultPrefab, resultContentPanel);
            obj.GetComponent<MathResultSetter>().Set(mathResultList[i].result, mathResultList[i].correct);
        }
    }

    public void ClearBoardContent()
    {
        for (int i = 0; i < resultContentPanel.childCount; i++)
        {
            Destroy(resultContentPanel.GetChild(i).gameObject);
        }
    }

    public void ClearMathResults()
    {
        mathResultList.Clear();
    }
}
