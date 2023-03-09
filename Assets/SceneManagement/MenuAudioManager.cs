using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{

    [Header("FMOD Event Path Strings")]
    [Tooltip ("The Audio that plays for both Text typing and Menu Button Clicks")]
    public string clickk;
    // Start is called before the first frame update
    private FMOD.Studio.EventInstance instanceMenu;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void click(){
        instanceMenu = FMODUnity.RuntimeManager.CreateInstance(clickk);
        instanceMenu.start();
        instanceMenu.release();
        Debug.Log("click audio");
    }
}
