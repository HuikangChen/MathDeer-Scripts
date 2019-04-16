using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransitionManager : MonoBehaviour
{
    public static PanelTransitionManager singleton;

    public float speed;

    public GameObject questionPanel;
    public GameObject continuePanel;
    public GameObject resultPanel;
    public GameObject mathResultPanel;
    public GameObject upgradePanel;
    public GameObject mathSettingPanel;

    public Transform start1;
    public Transform end1;
    public Transform start2;
    public Transform end2;

    bool running;

    private void Awake()
    {
        singleton = this;
    }

    public void Slide(GameObject panel, Transform start,Transform target, bool activate)
    {
        StartCoroutine(TryStartSlide(panel, start, target, activate));
    }

    public IEnumerator SlideCo(GameObject panel, Transform start, Transform target, bool activate)
    {
        running = true;
        panel.transform.position = start.position;

        if (activate)
        {
            panel.SetActive(true);
        }

        while (Vector2.Distance(panel.transform.position, target.transform.position) > 1f)
        {
            panel.transform.position = Vector2.Lerp(panel.transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        panel.transform.position = target.transform.position;

        if(activate == false)
        {
            panel.SetActive(false);
        }
        running = false;
    }

    IEnumerator TryStartSlide(GameObject panel, Transform start, Transform target, bool activate)
    {
        while (running)
        {
            yield return null;
        }

        StartCoroutine(SlideCo(panel, start, target, activate));
    }
}
