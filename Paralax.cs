using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour {

    public float horizontal_size;
    public float paralax_speed;

    Transform camera_transform;
    List<Transform> backgrounds = new List<Transform>();
    public float view_zone;
    int left_index;
    int right_index;

    PlayerMovement player;

	// Use this for initialization
	void Start () {
        camera_transform = Camera.main.transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds.Add(transform.GetChild(i));
        }

        left_index = 0;
        right_index = backgrounds.Count - 1;
        player = PlayerMovement.singleton;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (CameraMovement.singleton.playerRunningWithCamera == false)
            return;

        if (camera_transform.position.x < (backgrounds[left_index].transform.position.x + view_zone))
        {
            ScrollLeft();
        }

        if (camera_transform.position.x > (backgrounds[right_index].transform.position.x - view_zone))
        {
            ScrollRight();
        }
        transform.Translate(Vector3.left * (paralax_speed * Time.fixedDeltaTime * player.GetVelocity().x));
    }


    void ScrollLeft()
    {
        int last_right = right_index;
        backgrounds[right_index].position = new Vector3(backgrounds[left_index].position.x - horizontal_size, backgrounds[left_index].position.y, 0);
        left_index = right_index;
        right_index--;
        if (right_index < 0)
        {
            right_index = backgrounds.Count - 1;
        }
    }

    void ScrollRight()
    {

        int last_left = left_index;
        backgrounds[left_index].position = new Vector3(backgrounds[right_index].position.x + horizontal_size, backgrounds[right_index].position.y, 0);
        right_index = left_index;
        left_index++;
        if (left_index == backgrounds.Count)
        {
            left_index = 0;
        }
    }
}
