using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
//using FMOD.Studio;
//using FMODUnity;

//EVENTS W/ PARAMETERS
//music events, isPaused
//the truck engine, 
//wind ambiance, 
//customer order
public class AudioManager : MonoBehaviour {
    GameManager gameManager;

    //SLIDERS FOR VOLUME, SHOULD BE A VALUE BETWEEN 0 & 1
    [Header("Volumes (sliders)")]
    [Range(0f, 1f)]
    public float masterVolume;
    [Range(0f, 1f)]
    public float musicVolume;
    [Range(0f, 1f)]
    public float sfxVolume;
    [Range(0f, 1f)]
    public float dialogueVolume;
    [Range(0f, 1f)]
    public float ambianceVolume;

    [Header("Bus Paths")]
    public string masVolBusPath = "bus:/";
    public string musVolBusPath = "bus:/Music";
    public string sfxVolBusPath = "bus:/SFX";
    public string diaVolBusPath = "bus:/Dialogue";
    public string ambiVolBusPath = "bus:/Ambience";

    //BUSES
    /*
    public FMOD.Studio.Bus masBus;
    public FMOD.Studio.Bus musBus;
    public FMOD.Studio.Bus sfxBus;
    public FMOD.Studio.Bus diaBus;
    public FMOD.Studio.Bus ambiBus;
    */

    /////////////////////////MUSIC//////////////////////////////
    [Header("FMOD Music")]

    [Tooltip("FMOD Event Path for the folder that contains all the music")]
    public string menuMusicPath = "event:/Music/MenuMusic";
    public string storyMusicPath = "event:/Music/StoryMusic";
    public string tacoMusicPath = "event:/Music/TacoMusic";
    public string drivingMusicPath = "event:/Music/DrivingMusic";

    /////////////////////////SFX//////////////////////////////

    // CUTSCENE
    [Header("FMOD Cutscene(SFX) Event Path Strings")]
    
    [Tooltip("path of receieve text event")]
    public string recieveTextSFX;
    [Tooltip("path of the sending text event")]
    public string sendTextSFX;
    [Tooltip("path of the typing event")]
    public string typingSFX;

    // DRIVING

    [Header("FMOD Driving(SFX) Event Path Strings")]
    
    [Tooltip("FMOD Event Path for the folder that contains all the Driving SFX")]
    public string drivingSFXPath;
    
    [Tooltip("Name of Nitro Boost event")]
    public string nitroBoostSFX; //IMPLEMENTED
    [Tooltip("Name of the Successful Flip event")]
    public string flipBoostSFX; //IMPLEMENTED

    public string truckLandingSFX; //IMPLEMENTED
    public string crashSFX;

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

    void Awake()
    {
        /*
        // Load the FMOD banks
        RuntimeManager.LoadBank("Master");
        RuntimeManager.LoadBank("SFX");
        RuntimeManager.LoadBank("Music");
        RuntimeManager.LoadBank("Dialogue");
        RuntimeManager.LoadBank("Ambience");

        masBus = FMODUnity.RuntimeManager.GetBus(masVolBusPath);
        musBus = FMODUnity.RuntimeManager.GetBus(musVolBusPath);
        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxVolBusPath);
        diaBus = FMODUnity.RuntimeManager.GetBus(diaVolBusPath);
        ambiBus = FMODUnity.RuntimeManager.GetBus(ambiVolBusPath);
        */

        /*menuMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + menuMusic);

        cutsceneMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + cutsceneMusic);
        tacoMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + tacoMusic);
        drivingMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + drivingMusic);*/

    }

    /*
    protected EventInstance currentPlaying;


    //plays a one shot given the fmod event path
    public void Play(string path, Dictionary<string, float> parameters = null)
	{
        var instance = RuntimeManager.CreateInstance(path);
        if (parameters != null) {
            foreach (KeyValuePair<string, float> val in parameters) {
                instance.setParameterByName(val.Key, val.Value);
            }
        }
        instance.start();
        instance.release();
	}

    public void Play(string path) {

        EventDescription eventDescription;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD event path does not exist: " + path);
            return;
        }

        var instance = RuntimeManager.CreateInstance(path);
        instance.start();
        instance.release();
    }

    //a little more complicated! DO MATH to give sound 1 variable to work with

    //volType {0= master, 1= music, 2 = sfx, 3 = dialogue}, volAmount = float range: [0,1]


    /////////////////////////VOLUME//////////////////////////////
    public void PlaySong(string path){
        Debug.Log("[Audio Manager] Playing Song: " + path);
        if (currentPlaying.isValid()) {
            currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        EventDescription eventDescription;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD SONG event path does not exist: " + path);
            return;
        }

        EventInstance song = RuntimeManager.CreateInstance(path);
        currentPlaying = song;
        song.start();
        song.release();
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
    */
}
