using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EVENTS W/ PARAMETERS
//music events, isPaused
//the truck engine, 
//wind ambiance, 
//customer order
public class AudioManager : MonoBehaviour
{
    GameManager gameManager;

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
    
    [Header("FMOD Cutscene(SFX) Event Path Strings")]
    
    [Tooltip("FMOD Event Path for the folder that contains all the Cutscene SFX")]
    public string cutsceneSFXPath;
    [Tooltip("Name of receieve text event")]
    public string recieveTextSFX;
    [Tooltip("Name of the sending text event")]
    public string sendTextSFX;
    [Tooltip("Name of the typing event")]
    public string typingSFX;

    [Header("FMOD Driving(SFX) Event Path Strings")]
    
    [Tooltip("FMOD Event Path for the folder that contains all the Driving SFX")]
    public string drivingSFXPath;
    
    [Tooltip("Name of Nitro Boost event")]
    public string nitroBoostSFX;
    [Tooltip("Name of the Successful Flip event")]
    public string flipBoostSFX;

    [Header("FMOD Taco(SFX) Event Path Strings")]
    
    [Tooltip("Name of taco submission event")]
    public string submitTacoSFX;
    [Tooltip("Name of the ingredient placement event")]
    public string ingriPlaceSFX;
    [Tooltip("Name of the paw swiping event")]
    public string pawSwipeSFX;

    //need to have name of parameter and variable
    // 


    
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
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private FMOD.Studio.EventInstance MusicInstance;

    bool isPlaying(FMOD.Studio.EventInstance instance){
        FMOD.Studio.PLAYBACK_STATE state;   
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    int currSong(){ //returns index of the song that's currently playing
        if(isPlaying(menuMusicInst)){
            return 0; //menu index == 0
        }else if(isPlaying(tacoMusicInst)){
            return 1; //taco making music index = 1
        }else if(isPlaying(drivingMusicInst)){
            return 2; // driving music instance = 2
        }else if(isPlaying(cutsceneMusicInst)){
            return 3;
        }else{
            return -1;
        }
    }

    public void PlaySong(int index){

        switch(currSong()){
            case 0:
                menuMusicInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuMusicInst.release();
                Debug.Log("stopping menu music");
                break;
            case 1:
                tacoMusicInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                tacoMusicInst.release();
                Debug.Log("stopping taco music");
                break;
            case 2:
                drivingMusicInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                drivingMusicInst.release();
                Debug.Log("stopping driving music");
                break;
            case 3:
                cutsceneMusicInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                cutsceneMusicInst.release();
                Debug.Log("stopping cutscene music");
                break;
        }
        switch(index){
            case 0:
                menuMusicInst.start();
                Debug.Log("starting menu music");
                break;
            case 1:
                tacoMusicInst.start();
                Debug.Log("starting taco music");
                break;
            case 2:
                drivingMusicInst.start();
                Debug.Log("starting driving music");
                break;
            case 3:
                cutsceneMusicInst.start();
                Debug.Log("starting cutscene music");
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
