using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    // << MOVE HAND TOWARDS TARGET >>
    // instead of one big function that takes care of the entire movement,
    // I'm going to split the task into smaller bits...

    // what's the main thing that I want the hand to do? move from one position to another
    // so I'm going to make a "target" object for the hand to move towards
    public Transform target; // i use a transform here because i'm only worried about the position of the object

    // the next thing i'm going to worry about is saving the positions of the "home"
    // in this case we can use the Unity Editor to create a gameobject that holds the home position of the hand
    public Transform handHome; // i will use a transform again because I am not worried about anything but position

    // i don't need to worry about storing the position of the bins, because I will just set the "target" transform
    // as the current bin's transform

    // this is going to be the speed of the hand
    public float speed = 5;

    // >>> BEFORE CONTINUING :: 
        // Go to the Unity Editor and try dragging different gameobjects into the target variable in the inspector!
        // When the game is playing, the hand will move to the position of the target object
        // You can change the target object during runtime and watch the hand move back and forth


    public Ingredient currHeldIngredient; // this holds the current ingredient in player hand

    // << STATE MACHINE >>
    // because this hand is going to need to do specific things once it reaches its target,
    // we should use a state machine to set different "states of being"
    [Header("States")]

    public bool isHome = true; // State #0 -> this is when the hand is in its resting place

    public bool isPickingFromBin; // State #1 -> this is when the hand is moving towards the bin, with no ingredient in hand
    public IngredientBin pickBin; // this is the bin the hand is picking from

    public bool isPlacingIngredient; // State #2 -> this is when the hand is moving towards the taco, with ingredient in hand
    public Taco submissionTaco; // this is the taco the hand is submitting to


    public void Update()
    {
        // << MOVE HAND TOWARDS TARGET >>
        // if the target is valid,
        if (target != null)
        {
            // move the transform of the object the script is attached to over time
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);

            //             Lerp interpolates between point a^ and point b^ at a set speed^
            //                                                                 Time.delta time is based on frame rate
        }

        // << RUN STATE MACHINE >>
        StateMachine();
    }

    // << STATE MACHINE >> runs every frame in the update function
    public void StateMachine()
    {
        if (isPickingFromBin)
        {
            // once hand position == pickBin.transform.position, 
                // curr ingredient = bin ingredient type
                // isPickingFromBin = false
                // isPlacingIngredient = false
                // isHome = true
        }

        else if (isPlacingIngredient)
        {
            // target -> submissionTaco.transform

            // once hand position == taco.transform.position, 
                // taco.AddIngredient(/*ingredient*/);

                // change states and set target, just like before
        }

        else if (isHome)
        {
            // target -> home
            // all other states are not true
        }
    }

    /* =================================================================
     *  THE FOLLOWING FUNCTIONS ARE CALLED FROM InputManager.cs
     * =========================================================== */

    // << START PICK UP INGREDIENT FROM BIN >>
    public void PickUpIngredient(IngredientBin bin)
    {
        // set target to bin.transform
        target = bin.transform;

        isHome = false;
        isPickingFromBin = true;
        pickBin = bin;
    }

    // << START PLACE INGREDIENT INTO TACO >>
    public void PlaceIngredient(Taco submissionTaco)
    {
        // set proper states && current submission taco
    }


    /* ======================================= OLD CODE ============================================ */

    [Space(30)]
    [Header("Old Code Variables")]
    public GameObject Ingredient0;
    public GameObject Ingredient1;
    public GameObject Ingredient2;
    public GameObject Taco;
    Vector3 home = new Vector3(0, 4, 0); //I tried to make this become whatever the object's starting position is but Unity yelled at me so you need to do it manually
    int moving = 0; //frames 12-9 moving to ingredient, 8-5 moving above taco, 4-1 returning to position
    float xMovement = 0.0f;
    float yMovement = 0.0f;

    public void AshtonHandMovement()
    {
        //Only will grab new ingredient when done with current
        if (moving <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //marks that it's moving, and sets how it will move
                moving = 12;
                xMovement = (Ingredient0.transform.position.x - transform.position.x) / 4;
                yMovement = (Ingredient0.transform.position.y - transform.position.y) / 4;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                moving = 12;
                xMovement = (Ingredient0.transform.position.x - transform.position.x) / 4;
                yMovement = (Ingredient0.transform.position.y - transform.position.y) / 4;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                moving = 12;
                xMovement = (Ingredient0.transform.position.x - transform.position.x) / 4;
                yMovement = (Ingredient0.transform.position.y - transform.position.y) / 4;
            }
        }
        if ((12 >= moving) && (moving >= 9))
        {
            transform.Translate(new Vector3(xMovement, yMovement, 0));
            moving -= 1;
            if (moving == 8)
            {
                xMovement = (Taco.transform.position.x - transform.position.x) / 4;
                yMovement = (Taco.transform.position.y - transform.position.y) / 4;
            }
        }
        else if ((8 >= moving) && (moving >= 5))
        {
            transform.Translate(new Vector3(xMovement, yMovement, 0));
            moving -= 1;
            if (moving == 8)
            {
                xMovement = (Taco.transform.position.x - transform.position.x) / 4;
                yMovement = (Taco.transform.position.y - transform.position.y) / 4;
            }
        }
        else if (moving == 4)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                xMovement = (home.x - transform.position.x) / 4;
                yMovement = (home.y - transform.position.y) / 4;
                transform.Translate(new Vector3(xMovement, yMovement, 0));
                moving -= 1;
            }
        }
        else if ((0 < moving) && (moving < 4))
        {
            transform.Translate(new Vector3(xMovement, yMovement, 0));
            moving -= 1;
        }
    }
}
