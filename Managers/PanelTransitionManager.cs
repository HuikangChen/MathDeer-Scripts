using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the movement/transitions of the panels
/// Slides panels to positions and manages the panels
/// </summary>

public class PanelTransitionManager : MonoBehaviour
{
    private static PanelTransitionManager instance;
    public static PanelTransitionManager Instance { get { return instance; } }

    [Tooltip("Speed of panel transitions")]
    [SerializeField]
    private float speed;

    //All the panels/displays 
    public GameObject questionPanel;
    public GameObject continuePanel;
    public GameObject resultPanel;
    public GameObject mathResultPanel;
    public GameObject upgradePanel;
    public GameObject mathSettingPanel;

    //Positions of the transitions for the panels to slide from/to
    public Transform start1;
    public Transform end1;
    public Transform start2;
    public Transform end2;

    //is our panel transition coroutine running? 
    private bool running;

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

    //Called by other objects that needs to display a panel
    public void Slide(GameObject panel, Transform start,Transform target, bool activate)
    {
        //we will call TryStartSlide first so we don't interupt the panel while its already sliding down
        StartCoroutine(TryStartSlide(panel, start, target, activate));
    }

    IEnumerator TryStartSlide(GameObject panel, Transform start, Transform target, bool activate)
    {
        //if the sliding coroutine is already running, we will wait until its done
        while (running)
        {
            yield return null;
        }

        //then we start the slide again
        StartCoroutine(SlideCo(panel, start, target, activate));
    }

    public IEnumerator SlideCo(GameObject panel, Transform start, Transform target, bool activate)
    {
        running = true;

        //Move panel to starting position
        panel.transform.position = start.position;

        //check if we want to set the panel to active
        if (activate)
        {
            panel.SetActive(true);
        }

        //move the panel towards the target position
        while (Vector2.Distance(panel.transform.position, target.transform.position) > 1f)
        {
            panel.transform.position = Vector2.Lerp(panel.transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        panel.transform.position = target.transform.position;

        //check if we want to set the panel to inactive
        if (activate == false)
        {
            panel.SetActive(false);
        }
        running = false;
    }
}
