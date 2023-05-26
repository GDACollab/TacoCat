using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

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

    public FMOD.Studio.Bus masBus;
    public FMOD.Studio.Bus musBus;
    public FMOD.Studio.Bus sfxBus;
    public FMOD.Studio.Bus diaBus;
    public FMOD.Studio.Bus ambiBus;

    public bool isPaused;

    #region /////////////////////////MUSIC//////////////////////////////
    
    [Header("FMOD Music")]

    [Tooltip("FMOD Event Path for the folder that contains all the music")]
    public string menuMusicPath = "event:/Music/MenuMusic";
    public string storyMusicPath = "event:/Music/StoryMusic";
    public string tacoMusicPath = "event:/Music/TacoMusic";
    public string drivingMusicPath = "event:/Music/DrivingMusic";
    #endregion
    
    #region /////////////////////////AMBIENCE//////////////////////////////
    [Header("FMOD Driving(Ambience) Event Path Strings")]
    
    [Tooltip("path of ambience event")]
    public string drivingAmbiPath;
    #endregion
    
    #region /////////////////////////SFX//////////////////////////////

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

    public string rpmSFX;
    

    //TACO MAKING

    [Header("FMOD Taco(SFX) Event Path Strings")]
    
    [Tooltip("Name of taco submission event")]
    public string submitTacoSFX;
    [Tooltip("Name of the ingredient placement event")]
    public string ingriPlaceSFX; //IMPLEMENTED
    [Tooltip("Name of the paw swiping event")]
    public string pawSwipeSFX; //IMPLEMENTED
    public string bellDingSFX;
    public string orderDia;

    #endregion

    #region /////////////////////////UI//////////////////////////////
    [Header("FMOD UI(SFX) Event Path Strings")]
    
    [Tooltip("path of UI Select")]

    public string selectUI = "event:/SFX/UI & Menu/UI Select";
    public string hoverUI = "event:/SFX/UI & Menu/UI Hover";
    public string creakHoverUI = "event:/SFX/UI & Menu/UI Hover Sign";
    public string sliderUI = "event:/SFX/UI & Menu/UI Slider Feedback";
    public string signDrop = "event:/SFX/UI & Menu/Sign Drop";
    public string meow = "event:/SFX/UI & Menu/Meow Button";
    public string beep = "event:/SFX/UI & Menu/Beep-Beep Button";


    #endregion
    //need to have name of parameter and variable
    //FOR GLOBAL PARAMETERS FMOD Parameter name, variable name
    //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("", x);

    void Awake()
    {
        
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
        

        //menuMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + menuMusic);

        //cutsceneMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + cutsceneMusic);
        //tacoMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + tacoMusic);
        //drivingMusicInst = FMODUnity.RuntimeManager.CreateInstance(musicPath + drivingMusic);*/

    }

  
    protected EventInstance currentPlaying;
    protected EventInstance currentAmbience;

    protected EventInstance currentRPM;

    protected FMOD.Studio.PLAYBACK_STATE playbackState;



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
        Debug.Log("[Audio Manager] playing one shot: " + path);
	}

    public EventInstance Play(string path) {

        EventDescription eventDescription;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("FMOD event path does not exist: " + path);
            return RuntimeManager.CreateInstance(path); //NEEDS TO BE CHANGED
        }

        var instance = RuntimeManager.CreateInstance(path);
        instance.start();
        instance.release();
        Debug.Log("[Audio Manager] playing one shot: " + path);
        return instance;
    }

    //a little more complicated! DO MATH to give sound 1 variable to work with

    //volType {0= master, 1= music, 2 = sfx, 3 = dialogue}, volAmount = float range: [0,1]


    /////////////////////////VOLUME//////////////////////////////
    public void PlaySong(string path){
        Debug.Log("[Audio Manager] Playing Song: " + path);
        if (currentPlaying.isValid()) {
            StartCoroutine(RestOfPlaySong(path));
        }else{
            EventDescription eventDescription;
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("[Audio Manager] FMOD SONG event path does not exist: " + path);
                return;
            }

            EventInstance song = RuntimeManager.CreateInstance(path);
            currentPlaying = song;
            song.start();
            song.release();
        }
    }

    public IEnumerator RestOfPlaySong(string path){
        Debug.Log(currentPlaying);
        currentPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentPlaying.getPlaybackState(out playbackState);
        while(playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED){
            currentPlaying.getPlaybackState(out playbackState);
            yield return null;
        }

        EventDescription eventDescription;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(path, out eventDescription);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("[Audio Manager] FMOD SONG event path does not exist: " + path);
        }else{
            EventInstance song = RuntimeManager.CreateInstance(path);
            currentPlaying = song;
            song.start();
            song.release();
        }
    }

    public void PlayDrivingAmbience(float value){
        if(currentAmbience.isValid()){
            currentAmbience.setParameterByName("carHeight", value);
            Debug.Log("[Audio Manager] Driving Ambience updated: " + value);
        }else{
            EventDescription eventDescription;
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(drivingAmbiPath, out eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("FMOD SONG event path does not exist: " + drivingAmbiPath);
                return;
            }

            EventInstance ambience = RuntimeManager.CreateInstance(drivingAmbiPath);
            currentAmbience = ambience;
            ambience.start();
            ambience.release();
            Debug.Log("[Audio Manager] New Driving Ambience Event: " + value);
        }

    }
    public void StopDrivingAmbience(){
        Debug.Log("[Audio Manager] Stopping Driving Ambience");
        if(currentAmbience.isValid()){
            currentAmbience.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void PlayRPM(float value){
        if(currentRPM.isValid()){
            currentRPM.setParameterByName("RPM", value);
            //Debug.Log("[Audio Manager] RPM updated: " + value);
        }else{
            EventDescription eventDescription;
            FMOD.RESULT result = RuntimeManager.StudioSystem.getEvent(rpmSFX, out eventDescription);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning("FMOD SONG event path does not exist: " + rpmSFX);
                return;
            }

            EventInstance rpm = RuntimeManager.CreateInstance(rpmSFX);
            currentRPM = rpm;
            rpm.start();
            rpm.release();
            Debug.Log("[Audio Manager] New RPM Event: " + value);
        }
    }
    public void StopRPM(){
        Debug.Log("[Audio Manager] Stopping RPM");
        if(currentRPM.isValid()){
            currentRPM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void playSelect(){
        Play(selectUI);
    }
    public void playHover(){
        Play(hoverUI);
    }
    public void playCreakHover(){
        Debug.Log(creakHoverUI);
        Play(creakHoverUI);
    }
    public void playSlider(){
        Play(sliderUI);
    }
    public void playDrop(){
        Play(signDrop);
    }
    public void playMeow(){
        Play(meow);
    }
    public void playBeep(){
        Play(beep);
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
        if(gameManager !=null){
            if (!(gameManager.currGame == currGame.CUTSCENE || gameManager.currGame == currGame.MENU)){
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("timeOfDay", (float)gameManager.timeState);
            }
        }


    }
}
