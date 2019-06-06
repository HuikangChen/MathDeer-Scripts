using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains and displays the list of math question + results at the end of the run
/// </summary>

public class MathResultManager : MonoBehaviour
{
    private static MathResultManager instance;
    public static MathResultManager Instance { get { return instance; } }

    //our list of math results to be displayed on the math results panel
    [HideInInspector]
    public List<MathResult> mathResultList = new List<MathResult>();

    [Tooltip("The math result prefab to be spawned on the panel displaying a result")]
    [SerializeField]
    private GameObject mathResultPrefab;

    [Tooltip("The math result panel, used to parent the results")]
    [SerializeField]
    private Transform resultContentPanel;

    private void Awake()
    {
        //Our singleton pattern
        if (instance != null && instance != this)
        {
            // destroy the gameobject if an instance of this exist already
            Destroy(gameObject);
        }
        else
        {
            //Set our instance to this object/instance
            instance = this;
        }
    }

    //Add a MathResult to the list
    public void Add(MathResult result)
    {
        mathResultList.Add(result);
    }

    //Called when panel opens
    public void Initialize()
    {
        ClearPanelContent();
        DisplayMathResults();
    }

    public void DisplayMathResults()
    {   
        //Loops through the list of math results spawning a prefab for each results to display
        for (int i = 0; i < mathResultList.Count; i++)
        {
            GameObject obj = Instantiate(mathResultPrefab, resultContentPanel);
            obj.GetComponent<MathResultSetter>().Set(mathResultList[i].result, mathResultList[i].correct);
        }
    }

    //Clear the math result panel everytime we open it
    public void ClearPanelContent()
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
