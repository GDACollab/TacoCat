using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacoRecipeGenerator : MonoBehaviour
{
    public List<Taco> orderList;
    
    //fillingTypes: Fish, Shrimp, Salmon
    //toppingTypes: lettuce, diced tomatoes, beans, sour cream, onions
    public TacoRecipeGenerator(){
        //ten customers
        for(unsigned i=0; i<10;i++){
            List<fillingType> fillingOrder;
            List<toppingType> toppingOrder; 
            //insert a random fillingType (protein) into the filling list 
            fillingOrder.Insert(Enum.GetNames(typeof(fillingType))[Random.Range(0,Enum.GetNames(typeof(fillingType)).Length)]);

            for(unsigned i= 0; i<Enum.GetNames(typeof(toppingType)).Length;i++){ //iterate through this algo # of times = to # possibleof toppings
                if(Random.Range(0,maxPossDifficulty+2-currDifficulty)==0){ //if ur at max difficulty 50% chance that the ith topping will be added to the order
                    toppingOrder.Insert(Enum.GetNames(typeof(toppingType)[i]));
                }else{};
            }
            //this generates a list but the enums will always be in the same order so here's a shuffling algo.
            for(unsigned i=0; i<toppingOrder.Length; i++){
                toppingType tempTopping= toppingOrder[i];
                unsigned newIndex = Random.Range(0,toppingOrder.Length);
                toppingOrder[i]=toppingOrder[newIndex];
                toppingOrder[newIndex]=tempTopping;
            }
            //add taco to the list of orders.
            orderList.Insert[Taco(1, fillingOrder, toppingOrder)];
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
