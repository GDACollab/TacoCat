using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBubble : MonoBehaviour
{

    TacoMakingGameManager tacoGameManager;

    public Transform ingredientPointParent;

    public List<ingredientType> order;

    List<GameObject> orderIngredientObjects = new List<GameObject>();

    public float spacing = 1;

    public float ingredientScale = 0.1f;

    private void Awake()
    {
        tacoGameManager = GetComponentInParent<TacoMakingGameManager>();
    }

    public void ShowOrder()
    {
        // Calculate the starting y position
        float startY = ingredientPointParent.position.y - ((order.Count - 1) * spacing) / 2;

        // Spawn each object at the appropriate position
        for (int i = 0; i < order.Count; i++)
        {
            Vector3 position = new Vector3(transform.position.x, startY + i * spacing, transform.position.z);

            GameObject ingredient = tacoGameManager.GetIngredientObject(order[i]);

            GameObject newIngredient = Instantiate(ingredient, position, Quaternion.identity);
            newIngredient.transform.parent = ingredientPointParent;
            newIngredient.transform.localScale = Vector3.one * ingredientScale;


            orderIngredientObjects.Add(newIngredient);
        }





    }

    
}
