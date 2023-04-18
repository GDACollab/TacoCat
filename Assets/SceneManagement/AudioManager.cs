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

    [Header("FMOD Music Event Path Strings")]
    
    [Tooltip("FMOD Event Path for the folder that contains all the music")]
    public string musicPath;
    [Tooltip("Name of menu music event")]
    public string menuMusic;
    [Tooltip("Name of the cutscene music event")]
    public string cutsceneMusic;
    [Tooltip("Name of taco making music event")]
    public string tacoMusic;
    [Tooltip("Name of driving music event")]
    public string drivingMusic;

    GameManager gameManager;
    //Mathf.Clamp(percent, 0,1);
    private FMOD.Studio.EventInstance instance; //for testing purposes
    private FMOD.Studio.EventInstance menuMusicInst;
    private FMOD.Studio.EventInstance cutsceneMusicInst;
    private FMOD.Studio.EventInstance drivingMusicInst;
    private FMOD.Studio.EventInstance tacoMusicInst;

    //plays a one shot given the fmod event path
    public void Play(string path) 
	{
		FMODUnity.RuntimeManager.PlayOneShot(path);
	}

    //a little more complicated! DO MATH to give sound 1 variable to work with

    //volType {0= master, 1= music, 2 = sfx, 3 = dialogue}, volAmount = float range: [0,1]


    private FMOD.Studio.EventInstance MusicInstance;
    public void PlaySong(int index){
        switch(index){
            case 0:
                
                break;
            case 1:
                
                break;
            case 2:

                break;
            
        }
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
        menuMusicInst= FMODUnity.RuntimeManager.CreateInstance(musicPath+menuMusic);
        cutsceneMusicInst= FMODUnity.RuntimeManager.CreateInstance(musicPath+cutsceneMusic);
        tacoMusicInst= FMODUnity.RuntimeManager.CreateInstance(musicPath+tacoMusic);
        drivingMusicInst= FMODUnity.RuntimeManager.CreateInstance(musicPath+drivingMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    public void click(){
        Debug.Log(clickk);
        instanceMenu = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cutscene/Texting");
        instanceMenu.start();
        instanceMenu.release();
        Debug.Log("click audio");
    }*/
}
