using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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
    [SerializeField]
    static public string masVolBusPath = "bus:/";
    static public string musVolBusPath = "bus:/Music";
    static public string sfxVolBusPath = "bus:/SFX";
    static public string diaVolBusPath = "bus:/Dialogue";
    static public string ambiVolBusPath = "bus:/Ambience";

    //BUSES
    public FMOD.Studio.Bus masBus;
    public FMOD.Studio.Bus musBus;
    public FMOD.Studio.Bus sfxBus;
    public FMOD.Studio.Bus diaBus;
    public FMOD.Studio.Bus ambiBus;

    /////////////////////////MUSIC//////////////////////////////
    [Header("FMOD Music")]

    [Tooltip("FMOD Event Path for the folder that contains all the music")]
    public List<EventReference> music;
    protected Dictionary<string, string> musicList = new Dictionary<string, string>();
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

    //plays a one shot given the fmod event path
    public void Play(string path)
	{
        RuntimeManager.PlayOneShot(path);
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

        // Load music:
        
        foreach (EventReference musicRef in music) {
            var fullPath = musicRef.Path;
            var name = Regex.Match(fullPath, @"[^\\/]*$");
            musicList.Add(name.Value, fullPath);
        }

        /*menuMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + menuMusic);

        cutsceneMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + cutsceneMusic);
        tacoMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + tacoMusic);
        drivingMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + drivingMusic);*/

    }

    protected EventInstance currentPlaying;

    /////////////////////////VOLUME//////////////////////////////
    public void PlaySong(string name){
        Debug.Log("[Audio Manager] Playing Song: " + name);
        if (currentPlaying.isValid()) {
            currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        EventInstance song = RuntimeManager.CreateInstance(musicList[name]);
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
}