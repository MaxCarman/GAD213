using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class PlayerControllerOld : MonoBehaviour
{
    //Declare Variables
    //Controls
    [SerializeField] private KeyCode controlLeft = KeyCode.A;
    [SerializeField] private KeyCode controlRight = KeyCode.D;
    [SerializeField] private KeyCode controlUp = KeyCode.W;
    [SerializeField] private KeyCode controlDown = KeyCode.S;

    //Dashes
    private int dashesCurrent = 0; //The player's current amount of dashes
    private int dashesMaximum = 3; //The maximum amount of dashes

    //PlayerInfo
    public int playerID;
    public Color playerColor;

    //ComponentReferences
    [SerializeField] private Rigidbody2D thisRigidbody2D;
    [SerializeField]  private CapsuleCollider2D thisCircleCollider2D;

    //DoubletapDetection

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

    }

    void FixedUpdate()
    {

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
