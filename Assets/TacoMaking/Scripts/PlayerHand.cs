using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    
    // hi please code me -- uwu
    public GameObject Ingredient0;
    public GameObject Ingredient1;
    public GameObject Ingredient2;
    public GameObject Taco;
    Vector3 home = new Vector3(0, 4, 0); //I tried to make this become whatever the object's starting position is but Unity yelled at me so you need to do it manually
    int moving = 0; //frames 12-9 moving to ingredient, 8-5 moving above taco, 4-1 returning to position
    float xMovement = 0.0f;
    float yMovement = 0.0f;

    //Movement lasts 4 frames
    void Update()
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
            if(moving == 8)
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
