using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacoAudioManager : MonoBehaviour
{

    CustomerManager customerManager;
    public TacoMakingGameManager tacoGameManager;
    public TacoUIManager tacoUIManager;
    private FMOD.Studio.EventInstance instance;
    // Start is called before the first frame update
    [Tooltip("represented as a percentage")]
    [Header("Volumes")]
    public float ingredientVolume;
    public float animaleseVolume;
    public float musicVolume;

    [Header("Event Paths")]
    public string giveOrder, goodReaction, MehReaction, badReaction;
    void Start()
    {
        customerManager = tacoGameManager.customerManager;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OrderAudio(/*an enum would be here*/){
        //ideally would have a list of orders, where the paths to the index matches up w/ the enum species
        switch(0){
            case 0:
                instance = FMODUnity.RuntimeManager.CreateInstance(giveOrder);
                instance.start();
                instance.release();
                Debug.Log("played sound");
                
            break;
        }
    }
}
