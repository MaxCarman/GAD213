using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class PlayerController : MonoBehaviour
{
    //Declare Variables
    //Controls
    [SerializeField] private KeyCode controlLeft = KeyCode.A;
    [SerializeField] private KeyCode controlRight = KeyCode.D;
    [SerializeField] private KeyCode controlUp = KeyCode.W;
    [SerializeField] private KeyCode controlDown = KeyCode.S;

    //Dashes
    private bool contact = false;
    private int contactTime = 0; //Frames/Ticks since the player has been in contect with a surface
    private int dashesMaximum = 3; //The maximum amount of dashes
    private int dashesCurrent; //The player's current amount of dashes
    private int dashCooldownCurrent = 0; //Current progress on dash cooldown
    private float dashesCooldownSpeed = 20; //Frames required for dash cooldown
 


    //PlayerInfo
    public int playerID;
    public Color playerColor;

    //ComponentReferences
    [SerializeField] private Rigidbody2D thisRigidbody2D;
    [SerializeField]  private CapsuleCollider2D thisCircleCollider2D;

    //DoubletapDetection
    private int dtTimer = 200; //Frames to detect the 2nd tap

    private int dtLeftTaps;
    private int dtLeftTimer;

    //Collision
    public bool isCollidingWithEnviroment;

    //UI


    // Start is called before the first frame update
    void Start()
    {
        dashesCurrent = dashesMaximum;
    }

    // Update is called once per frame
    void Update()
    {
        //Jump
        if (Input.GetKeyDown(controlUp) && dashesCurrent > 0)
        {
            Debug.Log("Jumping");
            dashesCurrent -= 1;
            thisRigidbody2D.AddForce(new Vector2(0, 300)); //Add upwards force
        }

        //Recharge Dashes
        if((dashesCurrent < dashesMaximum) && isCollidingWithEnviroment == true)
        {
            Debug.Log("RechargingDashes");
            dashCooldownCurrent += 1;
            if(dashCooldownCurrent >= dashesCooldownSpeed)
            {
                dashCooldownCurrent = 0;
                dashesCurrent += 1;
            }
        }


        //Dashes

        //Left Dash =======================================================
        //Detect tap
        if(Input.GetKeyDown(controlLeft) && dashesCurrent > 0)
        {
            dtLeftTaps += 1;
            Debug.Log("TapLeft");
        }

        //If their is more than 1 tap, increase timer
        if(dtLeftTaps > 0)
        {
            dtLeftTimer += 1;
        }

        //If timer reaches maximum, reset taps
        if(dtLeftTimer > dtTimer)
        {
            dtLeftTaps = 0;
            dtLeftTimer = 0;
        }

        //If another tap is found before timer resets, apply dash
        if(dtLeftTaps >= 2 && dashesCurrent > 0)
        {
            dashesCurrent -= 1;
            dtLeftTaps = 0;
            dtLeftTimer = 0;
            Debug.Log("Dash Left");
            thisRigidbody2D.AddForce(new Vector2(-300, 0)); //Add left force
        }




    }

    void FixedUpdate()
    {
        //Left Movement
        if (Input.GetKey(controlLeft))
        {
            thisRigidbody2D.AddForce(new Vector2(-8, 0)); //Add left force
        }

        //Right Movement
        if (Input.GetKey(controlRight))
        {
            thisRigidbody2D.AddForce(new Vector2(8, 0)); //Add right force
        }

    }

    //Detect collision between the player and the enviroment
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enviroment"))
        {
            isCollidingWithEnviroment = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enviroment"))
        {
            isCollidingWithEnviroment = false;
        }
    }
}
