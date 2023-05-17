using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public enum handState { HOME, PICK_FROM_BIN, PLACE_INGR }

    // << MOVE HAND TOWARDS TARGET >>
    // instead of one big function that takes care of the entire movement,
    // I'm going to split the task into smaller bits...

    //We need the tacoGameManager so as to place the ingredient on the taco later
    public TacoMakingGameManager tacoGameManager;

    //reference for pop and woosh audio 
    public AudioManager audioManager;

    // what's the main thing that I want the hand to do? move from one position to another
    // so I'm going to make a "target" object for the hand to move towards
    public Transform target; // i use a transform here because i'm only worried about the position of the object
    public Transform tacoTarget; // stores taco transform location, so that the hand can move to it when needed

    // the next thing i'm going to worry about is saving the positions of the "home"
    // in this case we can use the Unity Editor to create a gameobject that holds the home position of the hand
    public Transform handHome; // i will use a transform again because I am not worried about anything but position

    // i don't need to worry about storing the position of the bins, because I will just set the "target" transform
    // as the current bin's transform

    // this is going to be the speed of the hand
    public float speed = 5;
    public float handDelay = 0.5f;

    //How close the hand must be to the position to be 'touching'
    public Vector3 approximateProximity = new Vector3(0.02f, 0.02f, 0f);

    // >>> BEFORE CONTINUING :: 
    // Go to the Unity Editor and try dragging different gameobjects into the target variable in the inspector!
    // When the game is playing, the hand will move to the position of the target object
    // You can change the target object during runtime and watch the hand move back and forth


    public ingredientType currHeldIngredient; // this holds the current ingredient in player hand

    // << STATE MACHINE >>
    // because this hand is going to need to do specific things once it reaches its target,
    // we should use a state machine to set different "states of being"
    [Header("States")]
    public handState state = handState.HOME;

    public IngredientBin pickBin; // this is the bin the hand is picking from
    public Taco submissionTaco; // this is the taco the hand is submitting to


    // >>>> NOTE:
    // I used this framework to add onto what you were achieving with the old code.
    // But now, the hand only moves to the target that it's given through input!
    // We need the hand to pick up the ingredient and move back to the home before placing the ingredient onf the taco
    // I placed the framework for you , but it's up to you to figure out how to finish it :)

    public void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

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
        else { target = handHome.transform; }

        // << RUN STATE MACHINE >>
        StateMachine();
    }

    // << STATE MACHINE >> runs every frame in the update function
    public void StateMachine()
    {
        if (state == handState.PICK_FROM_BIN)
        {
            if (TransformProximity())
            {
                GameObject ingr = Instantiate(tacoGameManager.GetIngredientObject(currHeldIngredient), transform);
                ingr.transform.parent = transform;
                state = handState.PLACE_INGR;
            }

        }

        if (state == handState.PLACE_INGR)
        {

            target = tacoTarget;
            if (TransformProximity())
            {
                // taco.AddIngredient(/*ingredient*/);
                Destroy(transform.GetChild(transform.childCount-1).gameObject);
                tacoGameManager.AddIngredientToTaco(currHeldIngredient);
                currHeldIngredient = new ingredientType();
                state = handState.HOME;
                //pop sound when placed
                //audioManager.Play(audioManager.ingriPlaceSFX);
            }
        }

        if (state == handState.HOME)
        {
            // target -> home
            // all other states are not true

            target = handHome.transform;
        }
    }

    //Checks if the two positions are close enough to be considered equal
    //(Checking if they're actually equal will return false because lerp's speed decreases over distance)
    public bool TransformProximity()
    {
        if (target == null) { state = handState.HOME; return false; }

        return (((target.position.x - approximateProximity.x <= transform.position.x) && (transform.position.x <= target.position.x + approximateProximity.x)) && 
            ((target.position.y - approximateProximity.y <= transform.position.y) && (transform.position.y <= target.position.y + approximateProximity.y)));
    }

    /* =================================================================
     *  THE FOLLOWING FUNCTIONS ARE CALLED FROM InputManager.cs
     * =========================================================== */

    // << START PICK UP INGREDIENT FROM BIN >>
    public void PickUpIngredient(IngredientBin bin, Taco submissionTaco)
    {
        // sets target to bin.transform, and tacotarget to whatever the taco location is.
        // Also sets up variables that control status
        if (state == handState.HOME)
        {
            target = bin.transform;
            tacoTarget = submissionTaco.transform;
            currHeldIngredient = bin.ingredientType;
            
            state = handState.PICK_FROM_BIN;

            pickBin = bin;
            //woosh sound when starting to pick up ingredient
            //audioManager.Play(audioManager.pawSwipeSFX);
        }
    }

    // << START PLACE INGREDIENT INTO TACO >>
    // Function is unneeded, currently inaccessible ingame. Feature is performed automatically by rest of code
    public void PlaceIngredient(Taco submissionTaco)
    {
        // set proper states && current submission taco
        if (state == handState.PICK_FROM_BIN && TransformProximity())
        {
            target = submissionTaco.transform;
            StartCoroutine(DelayedStateChange(0.25f, handState.PLACE_INGR));
        }
    }

    public IEnumerator DelayedStateChange(float delay, handState newState)
    {
        yield return new WaitForSeconds(delay);

        state = newState;
    }

}
