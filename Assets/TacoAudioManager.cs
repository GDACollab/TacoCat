using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacoAudioManager : MonoBehaviour
{
    CustomerManager customerManager;
    public TacoMakingGameManager tacoGameManager;
    // Start is called before the first frame update
    void Start()
    {
        customerManager = tacoGameManager.customerManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (customerManager.currCustomer != null)
        {
            //OrderAudio(customerManager.currCustomer.order );
        }
    }

    public void OrderAudio(){
    }
}
