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

    //Player States
    public bool InPhysicsMode = false;  //If the player is in physics mode or non-physics mode
    public int PhysicsFrames = 0;       //How many frames are left for physics mode.

    public bool inContactWithEnviroment; //If the player is in contact with a enviromental object.


    //Player Stats
    [SerializeField] private GameManager gameManager; //Reference to the game manager script, which unifies stats between players.

    private float statMoveSpeed;
    private float statJumpForce;
    private float statDashForce;
    private float staminaMaximum;     //Players maximum potential stamina.
    private float staminaIncrement; //The cooldown rate of stamina. Increased by this amount each fixed tick.

    //Stamina
    public float staminaCurrent;   //Player's current stamina. Decimals is progress to next stamina refill.

    //Double Tap Detection
    [SerializeField] private float dtTime; //The length of fixed ticks between taps to register.

    private int dtLeft = 0;
    private int dtLeftTime = 0;

    private int dtRight = 0;
    private int dtRightTime = 0;

    private int dtDown = 0;
    private int dtDownTime = 0;

    //Player Specifics
    public int playerID;
    public Color ColorNonPhysics;
    public Color ColorYesPhysics;

    //Component References
    [SerializeField] private Rigidbody2D thisRigidbody2D;
    [SerializeField] private CircleCollider2D thisCircleCollider2D;

    [SerializeField] private PhysicsMaterial2D NonPhysicsMaterial; //The rigidbody physics material when in non-physics mode.
    [SerializeField] private PhysicsMaterial2D YesPhysicsMaterial; //The rigidbody physics material when in physics mode.

    //UI
    [SerializeField] private GameObject prefabCloseUI; //Reference to ui prefab to spawn
    [SerializeField] private GameObject thisCloseUI; //The reference to this object's close ui.


    // Start is called before the first frame update
    void Start()
    {
        staminaMaximum = gameManager.staminaMaximum;
        staminaCurrent = staminaMaximum;

        //Create the close ui prefab
        thisCloseUI = Instantiate(prefabCloseUI, gameObject.transform.position, Quaternion.identity);
        thisCloseUI.GetComponent<CloseUIController>().playerOwner = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Apply stats from game manager. This is in update because it changes over time.
        statMoveSpeed = gameManager.statMovespeed;
        statJumpForce = gameManager.statJumpForce;
        statDashForce = gameManager.statDashForce;
        staminaMaximum = gameManager.staminaMaximum;
        staminaIncrement = gameManager.staminaIncrement;

        //Jumping (Dosent work in fixed update)
        if (Input.GetKeyDown(controlUp) && staminaCurrent >= 1) //Check for stamina as well
        {
            if (InPhysicsMode == false) //If not in physic mode, jump normally
            {
                staminaCurrent -= 1;
                thisRigidbody2D.linearVelocity = new Vector2(thisRigidbody2D.linearVelocity.x, statJumpForce);
            }

            if (InPhysicsMode == true && PhysicsFrames == 0) //Extra checks are required for escaping physics mode via jumping
            {
                staminaCurrent -= 1;
                InPhysicsMode = false;
                thisRigidbody2D.linearVelocity = new Vector2(thisRigidbody2D.linearVelocity.x, statJumpForce);
            }
        }

        //Dashing Detection
        if (InPhysicsMode == false || InPhysicsMode == true) //Currently supports both modes, but may change so i kept this condition in.
        {
            //Left Dashing
            if (Input.GetKeyDown(controlLeft) && staminaCurrent >= 1)
            {
                dtLeft += 1;
            }
            if (dtLeft >= 2)
            {
                //Reset taps and apply motion/stamina changes.
                dtLeft = 0;
                staminaCurrent -= 1;
                InPhysicsMode = true;
                thisRigidbody2D.linearVelocity += new Vector2(-statDashForce, 0);
            }

            //Right Dashing
            if (Input.GetKeyDown(controlRight) && staminaCurrent >= 1)
            {
                dtRight += 1;
            }
            if (dtRight >= 2)
            {
                //Reset taps and apply motion/stamina changes.
                dtRight = 0;
                staminaCurrent -= 1;
                InPhysicsMode = true;
                thisRigidbody2D.linearVelocity += new Vector2(statDashForce, 0);
            }

            //Down Dashing
            if (Input.GetKeyDown(controlDown) && staminaCurrent >= 1)
            {
                dtDown += 1;
            }
            if (dtDown >= 2)
            {
                //Reset taps and apply motion/stamina changes.
                dtDown = 0;
                staminaCurrent -= 1;
                InPhysicsMode = true;
                thisRigidbody2D.linearVelocity += new Vector2(0, -statDashForce);
            }
        }

        //Update Physics Material and colours based on physics mode.
        if (InPhysicsMode == false)
        {
            thisRigidbody2D.sharedMaterial = NonPhysicsMaterial;
            thisCircleCollider2D.sharedMaterial = NonPhysicsMaterial;
            gameObject.GetComponent<Renderer>().material.color = ColorNonPhysics;
        } else
        {
            thisRigidbody2D.sharedMaterial = YesPhysicsMaterial;
            thisCircleCollider2D.sharedMaterial = YesPhysicsMaterial;
            gameObject.GetComponent<Renderer>().material.color = ColorYesPhysics;
        }

        //Update UI text and color
        thisCloseUI.GetComponent<CloseUIController>().staminaMarker.material.color = ColorNonPhysics;
        thisCloseUI.GetComponent<CloseUIController>().staminaText.material.color = ColorNonPhysics;
        thisCloseUI.GetComponent<CloseUIController>().staminaText.text = "Stamina: " + staminaCurrent;



    }

    void FixedUpdate()
    {
        //Non-Physics Movement
        if (InPhysicsMode == false) //Check if the player should be capable of movement
        {

            //Left Right movement
            //Determine move direction
            int moveDirection = 0;
            if (Input.GetKey(controlLeft))
            {
                moveDirection += -1;
            }
            if (Input.GetKey(controlRight))
            {
                moveDirection += 1;
            }

            //If trying to move, apply it. To make the character not roll, set EQUALS and not ADDITIVE.
            if (moveDirection != 0)
            {
                thisRigidbody2D.linearVelocity = new Vector2(moveDirection * statMoveSpeed, thisRigidbody2D.linearVelocity.y);
            } else {
                thisRigidbody2D.linearVelocity = new Vector2(0, thisRigidbody2D.linearVelocity.y);
            }
        }

        //If the player is motionless or close to motionless in physics mode, set back to non-physics mode.
        if (InPhysicsMode == true && (thisRigidbody2D.linearVelocity.x < 1.5 && thisRigidbody2D.linearVelocity.x > -1.5) && (thisRigidbody2D.linearVelocity.y < 1.5 && thisRigidbody2D.linearVelocity.y > -1.5))
        {
            InPhysicsMode = false;
        }

        //Dashing - Update dashing timers
        //Left Dash
        if (dtLeft >= 1)
        {
            dtLeftTime += 1;
            if (dtLeftTime >= dtTime)
            {
                dtLeft = 0;
                dtLeftTime = 0;
            }
        } else
        {
            dtLeftTime = 0;
        }
        //Right Dash
        if (dtRight >= 1)
        {
            dtRightTime += 1;
            if (dtRightTime >= dtTime)
            {
                dtRight = 0;
                dtRightTime = 0;
            }
        } else
        {
            dtRightTime = 0;
        }
        //Down Dash
        if (dtDown >= 1)
        {
            dtDownTime += 1;
            if (dtDownTime >= dtTime)
            {
                dtDown = 0;
                dtDownTime = 0;
            }
        } else
        {
            dtDownTime = 0;
        }

        //Restore Stamina if in contact with the enviroment
        if (inContactWithEnviroment == true)
        {
            if(staminaCurrent < staminaMaximum)
            {
                staminaCurrent += staminaIncrement; //If below, add the increment.
            }
            if(staminaCurrent > staminaMaximum)
            {
                staminaCurrent = staminaMaximum; //If above, lower to make it exact.
            }
        }


    }

    //Collison Detection
    void OnCollisionStay2D(Collision2D col)
    {
        //If colliding with the enviroment, set to true.
        if (col.gameObject.CompareTag("Enviroment"))
        {
            inContactWithEnviroment = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        //When leaving collision with the enviroment, set to false.
        if (col.gameObject.CompareTag("Enviroment"))
        {
            inContactWithEnviroment = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Apply velocity to this object is another high velocity object hits it.
        if(col.gameObject.GetComponent<Rigidbody2D>() != null && InPhysicsMode == false) //Check if null and in non-physics mode.
        {
            Rigidbody2D colRigidbody2D = col.gameObject.GetComponent<Rigidbody2D>();
            if ((colRigidbody2D.linearVelocity.x > 1.5f || colRigidbody2D.linearVelocity.x < -1.5f) || (colRigidbody2D.linearVelocity.y > 1.5f || colRigidbody2D.linearVelocity.y < -1.5f)) //Check if rigidbody in motion.
            {
                Debug.Log("apply velocity");
                InPhysicsMode = true;
                thisRigidbody2D.linearVelocity += colRigidbody2D.linearVelocity;
            }
        }
    }
}
