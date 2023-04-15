using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{

    [Header("FMOD Event Path Strings")]
    [Tooltip ("The Audio that plays for both Text typing and Menu Button Clicks")]
    public string clickk="";
    // Start is called before the first frame update
    private FMOD.Studio.EventInstance instanceMenu;
    void Start()
    {
        clickk="event:/SFX/Cutscene/Texting";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void click(){
        Debug.Log(clickk);
        instanceMenu = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cutscene/Texting");
        instanceMenu.start();
        instanceMenu.release();
        Debug.Log("click audio");
    }
}
