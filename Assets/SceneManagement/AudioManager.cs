using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //SLIDERS FOR VOLUME, SHOULD BE A VALUE BETWEEN 0 & 1
    [Header("Volumes")]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float dialogueVolume;
    public float ambianceVolume;

    [Header("FMOD Event Path Strings")]
    [Tooltip ("The Audio that plays for both Text typing and Menu Button Clicks")]
    public string clickk="";

    //Mathf.Clamp(percent, 0,1);
    private FMOD.Studio.EventInstance instance; //for testing purposes

    //plays a one shot given the fmod event path
    public void Play(string path) 
	{
		FMODUnity.RuntimeManager.PlayOneShot(path);
	}

    //a little more complicated! DO MATH to give sound 1 variable to work with

    //volType {0= master, 1= music, 2 = sfx, 3 = dialogue}, volAmount = float range: [0,1]
    public void setVolume(int volType, float volAmount){ 
        volAmount=Mathf.Clamp(volAmount, 0, 1);
        switch(volType){
            case 0:
                masterVolume = volAmount;
                break;
            case 1:
                musicVolume = volAmount;
                break;
            case 2:
                sfxVolume = volAmount;
                break;
            case 3:
                dialogueVolume = volAmount;
                break;
        }
    }

    public void PlaySong(){
        //if song is already playing, nothing happens
        //if diff song
    }
    //GAME OBJECTS FOR MUSIC w/ event emitters attached
    //MENU
        //cursor hover & click
        //current scene's music is muffled

    //CUTSCENES
        //text typing: click clack
        //text sending & reciving

    //TACO MAKING
        //music >> event, bool
        //ambiance^^
        //customers score
        //order submission

    //DRIVING
        // music >> event, bool
        //ambiance ^^
        //RPM

    // Start is called before the first frame update
    

    void Start()
    {
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
