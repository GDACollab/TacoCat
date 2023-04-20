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
    [Header("Volumes (sliders)")]
    [Range(0f,1f)]
    public float masterVolume;
    [Range(0f,1f)]
    public float musicVolume;
    [Range(0f,1f)]
    public float sfxVolume;
    [Range(0f,1f)]
    public float dialogueVolume;
    [Range(0f,1f)]
    public float ambianceVolume;
    
    [Header("Bus Paths")]
    [SerializeField]
    static public string masVolBusPath = "bus:/";
    static public string musVolBusPath ="bus:/Music";
    static public string sfxVolBusPath ="bus:/SFX";
    static public string diaVolBusPath = "bus:/Dialogue";
    static public string ambiVolBusPath = "bus:/Ambience";

    //BUSES
    public FMOD.Studio.Bus masBus;
    public FMOD.Studio.Bus musBus;
    public FMOD.Studio.Bus sfxBus;
    public FMOD.Studio.Bus diaBus;
    public FMOD.Studio.Bus ambiBus;

    /////////////////////////MUSIC//////////////////////////////
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
    /////////////////////////SFX//////////////////////////////

    // CUTSCENE
    [Header("FMOD Cutscene(SFX) Event Path Strings")]
    
    [Tooltip("FMOD Event Path for the folder that contains all the Cutscene SFX")]
    public string cutsceneSFXPath;
    [Tooltip("Name of receieve text event")]
    public string recieveTextSFX;
    [Tooltip("Name of the sending text event")]
    public string sendTextSFX;
    [Tooltip("Name of the typing event")]
    public string typingSFX;

    // DRIVING

    [Header("FMOD Driving(SFX) Event Path Strings")]
    
    [Tooltip("FMOD Event Path for the folder that contains all the Driving SFX")]
    public string drivingSFXPath;
    
    [Tooltip("Name of Nitro Boost event")]
    public string nitroBoostSFX; //IMPLEMENTED
    [Tooltip("Name of the Successful Flip event")]
    public string flipBoostSFX; //IMPLEMENTED

    //TACO MAKING

    [Header("FMOD Taco(SFX) Event Path Strings")]
    
    [Tooltip("Name of taco submission event")]
    public string submitTacoSFX;
    [Tooltip("Name of the ingredient placement event")]
    public string ingriPlaceSFX; //IMPLEMENTED
    [Tooltip("Name of the paw swiping event")]
    public string pawSwipeSFX; //IMPLEMENTED

    public bool isPaused;

    //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isPaused", isPaused);

    //need to have name of parameter and variable
    // FOR GLOBAL PARAMETERS FMOD Parameter name, variable name
    //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("", x);


    
    //Mathf.Clamp(percent, 0,1);
    private FMOD.Studio.EventInstance instance; //for testing purposes
    private FMOD.Studio.EventInstance menuMusicInst;
    
    private FMOD.Studio.EventInstance cutsceneMusicInst;
    private FMOD.Studio.EventInstance drivingMusicInst;
    private FMOD.Studio.EventInstance tacoMusicInst;

    //plays a one shot given the fmod event path
    public void Play(string path)
	{
        FMOD.Studio.EventInstance instance =  FMODUnity.RuntimeManager.CreateInstance(path);
		instance.start();
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

    //a little more complicated! DO MATH to give sound 1 variable to work with

    //volType {0= master, 1= music, 2 = sfx, 3 = dialogue}, volAmount = float range: [0,1]
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        masBus = FMODUnity.RuntimeManager.GetBus(masVolBusPath);
        musBus = FMODUnity.RuntimeManager.GetBus(musVolBusPath);
        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxVolBusPath);
        diaBus = FMODUnity.RuntimeManager.GetBus(diaVolBusPath);
        ambiBus = FMODUnity.RuntimeManager.GetBus(ambiVolBusPath);

        menuMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + menuMusic);
        cutsceneMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + cutsceneMusic);
        tacoMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + tacoMusic);
        drivingMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + drivingMusic);

    }

    private FMOD.Studio.EventInstance MusicInstance;

    /////////////////////////VOLUME//////////////////////////////
    //plays a one shot
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
        //Checks to see if a song is playing, if so stops it, then plays the song that belongs to the index
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
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("isPaused", isPaused ? 0 : 1);
        masBus.setVolume(masterVolume);
        musBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
        diaBus.setVolume(dialogueVolume);
        ambiBus.setVolume(ambianceVolume);
    }
}
