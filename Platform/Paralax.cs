using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scrolling script that recycles it's child gameobjects(backgrounds) as the player runs in a horizontal direction
/// To create a paralax with multiple layers:
/// 1.Create multiple parent gameobjects with its layers under them
/// 2.Attach this script to the parent gameobjects
/// 3.Set the paralax speed to different speeds on each one and paralax is created
/// </summary>

public class Paralax : MonoBehaviour {
    
    //Used as an offset for positioning the backgrounds 
    [Tooltip("Horizontal Size of image/background")]
    [SerializeField]
    private float background_HorizontalSize;

    [SerializeField]
    private float paralaxSpeed;

    //Used for determining when to move the backgrounds back to the end to reuse it
    [Tooltip("The camera's view size")]
    [SerializeField]
    public float viewZone;

    //Our main camera
    private Transform cameraTransform;

    //The backgrounds this paralax script is controlling
    private List<Transform> backgrounds = new List<Transform>();

    //The index of the background that we will be moving to the right most position of the list of backgrounds
    //Used for recycling our backgrounds
    private int leftIndex;

    //The index of the background that we will be moving to the left most position of the list of backgrounds
    //Used for recycling our backgrounds
    private int rightIndex;

    //Reference to the player
    private PlayerController player;

	// Use this for initialization
	void Start () {

        //Getting our references
        cameraTransform = Camera.main.transform;
        player = PlayerController.Instance;

        //Get all our child gameobjects since they are our background layers we are recycling
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds.Add(transform.GetChild(i));
        }

        //Initializing left index to the first child which is the left most background
        leftIndex = 0;
        
        //Initializing right index to the last child which is the right most background
        rightIndex = backgrounds.Count - 1;      
	}

	void FixedUpdate () {

        //if the camera is not moving with the player
        if (PlayerCamera.Instance.playerRunningWithCamera == false)
            return;

        //if the position of the camera is further than the position of the lefft most background
        //Meaning if the camera is able to see past our background on the left side
        if (cameraTransform.position.x < (backgrounds[leftIndex].transform.position.x + viewZone))
        {
            //Moving our right most background to the left
            ScrollLeft();
        }

        //if the position of the camera is further than the position of the right most background
        //Meaning if the camera is able to see past our background on the right side
        if (cameraTransform.position.x > (backgrounds[rightIndex].transform.position.x - viewZone))
        {
            //Moving our left most background to the right
            ScrollRight();
        }

        //Move our layer to the left since our player is moving right
        transform.Translate(Vector3.left * (paralaxSpeed * Time.fixedDeltaTime * player.GetVelocity().x));
    }

    
    void ScrollLeft()
    {
        //Save our right index
        int last_right = rightIndex;

        //Move our right most background to the left
        backgrounds[rightIndex].position = new Vector3(backgrounds[leftIndex].position.x - background_HorizontalSize, 
                                                        backgrounds[leftIndex].position.y, 0);

        //our left index is now set to right index because that was the background we just moved here
        leftIndex = rightIndex;

        //now our right index is decremented as its the next in the list
        rightIndex--;

        //if we reach the end of the list, we will begin again at count -1
        if (rightIndex < 0)
        {
            rightIndex = backgrounds.Count - 1;
        }
    }

    void ScrollRight()
    {
        //Save our left index
        int last_left = leftIndex;

        //Move our left most background to the right
        backgrounds[leftIndex].position = new Vector3(backgrounds[rightIndex].position.x + background_HorizontalSize, 
                                                       backgrounds[rightIndex].position.y, 0);

        //our right index is now set to left index because that was the background we just moved here
        rightIndex = leftIndex;

        //now our right index is incremented as its the next in the list
        leftIndex++;

        //if we reach the end of the list, we will begin again at 0
        if (leftIndex == backgrounds.Count)
        {
            leftIndex = 0;
        }
    }
}
