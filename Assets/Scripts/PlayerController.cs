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

    //Energy
    public float energyCurrent;   //Player's current energy. Decimals is progress to next energy refill.
    public int energyMaximum;     //Players maximum potential energy.
    public float energyIncrement; //The cooldown rate of energy. Increased by this amount eahc fixed tick.

    //Double Tap Detection
    [SerializeField] private float dtTime; //The length of fixed ticks between taps to register.

    private int dtLeft = 0;
    private int dtLeftTime = 0;

    private int dtRight = 0;
    private int dtRightTime = 0;

    //Player Identification
    public int playerID;

    //Player Colors
    public Color ColorNonPhysics;
    public Color ColorYesPhysics;

    //Player Stats
    public float statMoveSpeed;
    public float statJumpForce;
    public float statDashForce;

    //Component References
    [SerializeField] private Rigidbody2D thisRigidbody2D;
    [SerializeField] private CircleCollider2D thisCircleCollider2D;

    [SerializeField] private PhysicsMaterial2D NonPhysicsMaterial; //The rigidbody physics material when in non-physics mode.
    [SerializeField] private PhysicsMaterial2D YesPhysicsMaterial; //The rigidbody physics material when in physics mode.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Jumping (Dosent work in fixed update)
        if (Input.GetKeyDown(controlUp) && energyCurrent >= 1) //Check for energy as well
        {
            if (InPhysicsMode == false) //If not in physic mode, jump normally
            {
                energyCurrent -= 1;
                thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, statJumpForce);
            }

            if (InPhysicsMode == true && PhysicsFrames == 0) //Extra checks are required for escaping physics mode via jumping
            {
                energyCurrent -= 1;
                InPhysicsMode = false;
                thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, statJumpForce);
            }
        }

        //Dashing Detection
        if(InPhysicsMode == false)
        {
            //Left Dashing
            if(Input.GetKeyDown(controlLeft) && energyCurrent >= 1)
            {
                dtLeft += 1;
            }
            if(dtLeft >= 2)
            {
                energyCurrent -= 1;
                InPhysicsMode = true;
                thisRigidbody2D.AddForce(new Vector2(-statDashForce, 0));
            }

            //Right Dashing
            if (Input.GetKeyDown(controlRight) && energyCurrent >= 1)
            {
                dtRight += 1;
            }
            if (dtRight >= 2)
            {
                energyCurrent -= 1;
                InPhysicsMode = true;
                thisRigidbody2D.AddForce(new Vector2(statDashForce, 0));
            }
        }

        //Update Physics Material and colours based on physics mode.
        if(InPhysicsMode == false)
        {
            thisRigidbody2D.sharedMaterial = NonPhysicsMaterial;
            gameObject.GetComponent<Renderer>().material.color = ColorNonPhysics;
        } else
        {
            thisRigidbody2D.sharedMaterial = YesPhysicsMaterial;
            gameObject.GetComponent<Renderer>().material.color = ColorYesPhysics;
        }

    }

    void FixedUpdate()
    {
        //Non-Physics Movement
        if(InPhysicsMode == false) //Check if the player should be capable of movement
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
                thisRigidbody2D.velocity = new Vector2(moveDirection * statMoveSpeed, thisRigidbody2D.velocity.y);
            } else {
                thisRigidbody2D.velocity = new Vector2(0, thisRigidbody2D.velocity.y);
            }
        }
        
        //If the player is motionless or close to motionless in physics mode, set back to non-physics mode.
        if(InPhysicsMode == true && (thisRigidbody2D.velocity.x < 1 && thisRigidbody2D.velocity.x > -1) && (thisRigidbody2D.velocity.y < 1 && thisRigidbody2D.velocity.y > -1))
        {
            InPhysicsMode = false;
        }

        //Update double tap timers
        if(dtLeft >= 1)
        {
            dtLeftTime += 1;
            if(dtLeftTime >= dtTime)
            {
                dtLeft = 0;
                dtLeftTime = 0;
            }
        }

        if (dtRight >= 1)
        {
            dtRightTime += 1;
            if (dtRightTime >= dtTime)
            {
                dtRight = 0;
                dtRightTime = 0;
            }
        }

        //Restore Stamina
        if(inContactWithEnviroment == true)
        {
            if(energyCurrent < energyMaximum)
            {
                energyCurrent += energyIncrement; //If below, add the increment.
            }
            if(energyCurrent > energyMaximum)
            {
                energyCurrent = energyMaximum; //If above, lower to make it exact.
            }
        }


    }

    //Collison Detection
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enviroment"))
        {
            inContactWithEnviroment = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enviroment"))
        {
            inContactWithEnviroment = false;
        }
    }
}
