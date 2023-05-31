using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum currGame { NONE, MENU, CUTSCENE, TACO_MAKING, DRIVING, BAD_ENDING, GOOD_ENDING, CREDITS }

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public static EventSystem eventSystemInstance = null;

    [HideInInspector]
    public AudioManager audioManager;
    [HideInInspector]
    public PauseManager pauseManager;

    // save reference to seperate game manager
    public TacoMakingGameManager tacoGameManager;
    public DrivingGameManager drivingGameManager;
    public CutsceneManager cutsceneManager;
    public MenuManager menuManager;

    [Header("TIME OF DAY")]
    public TIME_OF_DAY currDayCycleState;

    [Header(" === INIT GAME TIMER === ")]
    [Tooltip("Total duration of game in seconds")]
    public float totalGameTime_seconds = 300;
    private float totalGameTime_startValue;
    [Tooltip("when should the clock start in 24 hours")]
    public int startTime_24hr = 8;
    [Tooltip("when should the day end in 24 hours")]
    public int endTime_24hr = 20;
    [Tooltip("updates the visible clock every X in-game minutes")]
    public int updateClockEvery_Minutes = 30;

    [Space(10), Tooltip("what hour should the clock stop updating in 24 hours")]
    public int hardCapHour = 23;
    [Tooltip("around what minute should the clock updating")]
    public int hardCapMinute = 59;

    [Header(" === ACTIVE GAME TIMER === ")]
    [Range(0.0f, 1000.0f), Tooltip("Time remaining in seconds")]
    public float timeRemaining;
    [Range(0.0f,1.0f), Tooltip("Internal timer that goes from 0 to 1")]
    public float main_gameTimer = 0;
    [Tooltip("current hour")]
    public int curClockHour;
    [Tooltip("current minute")]
    public int curClockMinute;
    public bool isAM = true;

    [Header(" === SCENE MANAGEMENT === ")]
    public currGame currGame = currGame.NONE;
    public currGame lastGame;
    public int currLevel = 0;
    public bool currHappyEnding = true;
    public bool fullGameComplete;

    [Space(10)]
    public SceneObject currScene;
    [Space(5)]
    public bool isLoadingScene;
    public bool activateScene = true;

    [Header("Scenes")]
    public SceneObject menuScene;
    public SceneObject loadingScene;

    [Space(10)]
    public SceneObject driving1;
    public SceneObject driving2;
    public SceneObject driving3;

    [Space(10)]
    public SceneObject tacoMakingScene;
    public SceneObject cutscene;

    [Header("--SCENE VARIABLE TRANSFER--")]
    public int nitroCharges = 3;
    public int gasAmount;


    [Header("Endless Mode")]
    public bool endlessModeActive;
    public int endlessGameTimerCount = 300;
    public int endlessCitiesVisited = 0;
    public int endlessCitiesHighscore = 0;
    public SceneObject endlessTacoScene;
    public List<SceneObject> endlessDrivingScenes;


    private void Awake()
    {
        // only allow for one version of the gamemanager in a scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioManager = GetComponentInChildren<AudioManager>();
        pauseManager = GetComponentInChildren<PauseManager>();

        totalGameTime_startValue = totalGameTime_seconds; // save init value
        GameTimerReset();

        StartCoroutine(SceneSetup());

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += NewSceneReset;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= NewSceneReset;
    }

    public void NewSceneReset(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[[GAMEMANAGER]] New scene loaded: " + scene.name);

        StartCoroutine(SceneSetup());
    }

    public IEnumerator SceneSetup()
    {
        drivingGameManager = null;
        tacoGameManager = null;
        cutsceneManager = null;
        menuManager = null;

        audioManager.LoadBanksAndBuses();

        // wait till music is loaded
        yield return new WaitUntil(() => audioManager.allBanksLoaded && audioManager.allBusesLoaded);

        // Start Menu Music
        if (currGame == currGame.MENU)
        {
            audioManager.PlaySong(audioManager.menuMusicPath);
        }
    }

    public void Update()
    {

        // << TACO GAME MANAGER >>
        if (currGame == currGame.TACO_MAKING)
        {
            // check for taco game manager
            if (!tacoGameManager)
            {
                try
                {
                    tacoGameManager = GameObject.FindGameObjectWithTag("TacoGameManager").GetComponent<TacoMakingGameManager>();
                }
                catch { }
                return;
            }

            if (currLevel == 0) { currLevel = 1; }

            tacoGameManager.difficulty = currLevel;
            currDayCycleState = tacoGameManager.lightingManager.dayCycleState;
            //Debug.Log("taco making curr day cycle" +currDayCycleState);

            // check if all customers submitted , if so move to driving with gas amount
            if (tacoGameManager.state == TACOMAKING_STATE.END_TRANSITION && !isLoadingScene && !tacoGameManager.uiManager.camEffectManager.isFading)
            {
                nitroCharges = tacoGameManager.nitroCharges;

                if (endlessModeActive)
                {
                    LoadRandomEndlessDriving();
                }
                else
                {
                    LoadDrivingScene(currLevel);
                }
            }
        }


        // << DRIVING GAME MANAGER >>
        if (currGame == currGame.DRIVING)
        {

            if (!drivingGameManager)
            {
                try
                {
                    drivingGameManager = GameObject.FindGameObjectWithTag("DrivingGameManager").GetComponent<DrivingGameManager>();
                }
                catch { }
                return;
            }


            // update nitro charges
            if (currLevel == 0) { currLevel = 1; }
            if (currLevel <= 1 && nitroCharges == 0)
            {
                drivingGameManager.nitroCharges = 1;
            }
            else
            {
                drivingGameManager.nitroCharges = nitroCharges;
            }

            // get day state
            currDayCycleState = drivingGameManager.lightingManager.dayCycleState;
            //Debug.Log("driving curr day cycle" +currDayCycleState);


            // check for ending
            if (drivingGameManager.state == DRIVINGGAME_STATE.END_TRANSITION && !isLoadingScene)
            {
                if (endlessModeActive)
                {
                    if (drivingGameManager.completedLevel)
                    {
                        if (currDayCycleState == TIME_OF_DAY.MIDNIGHT)
                        {
                            LoadEndlessTacoMaking(true); // start endless taco making && reset time
                        }
                        else
                        {
                            LoadEndlessTacoMaking(false); // start endless taco making
                        }

                    }
                    else
                    {
                        endlessCitiesHighscore = endlessCitiesVisited;
                        endlessCitiesVisited = 0;
                        LoadMenu(true);
                    }
                }
                else
                {
                    if (currDayCycleState == TIME_OF_DAY.MIDNIGHT)
                    {
                        currLevel = 5;
                        currHappyEnding = false;
                        Debug.Log("Bad Ending: " + currLevel);
                        LoadCutscene();
                    }
                    else if (!drivingGameManager.completedLevel)
                    {
                        Debug.Log("Good Ending: " + currLevel);
                        LoadTacoMakingScene();
                    }
                    else
                    {
                        currLevel++;
                        Debug.Log("Current Level: " + currLevel);
                        LoadCutscene();
                    }
                }

            }
        }


        // << CUTSCENE MANAGER >>
        if ( (currGame == currGame.CUTSCENE || currGame == currGame.CREDITS) && cutsceneManager != null)
        {
            if (!cutsceneManager)
            {
                try
                {
                    cutsceneManager = GameObject.FindGameObjectWithTag("CutsceneManager").GetComponent<CutsceneManager>();
                }
                catch { }
                return;
            }

            // check for end
            if (cutsceneManager.endOfCutscene && !isLoadingScene)
            {

                if (currGame == currGame.CREDITS) { LoadMenu(true); }
                else { LoadTacoMakingScene(); }
                
            }
        }

        // << UPDATE GAME TIMER >>
        GameTimerUpdate();
    }

    public void LoadMenu(bool game_reset)
    {
        lastGame = currGame;
        currGame = currGame.MENU;
        endlessModeActive = false;
        if (game_reset)
        {
            currLevel = 1;    // Deletes progress
        }
        StartCoroutine(ConcurrentLoadingCoroutine(menuScene));
    }

    // **** LOAD TACO MAKING SCENE ****
    public void LoadTacoMakingScene()
    {
        currGame = currGame.TACO_MAKING;

        StartCoroutine(ConcurrentLoadingCoroutine(tacoMakingScene));
        audioManager.PlaySong(audioManager.tacoMusicPath);
    }

    // **** LOAD DRIVING SCENES ****
    public void LoadDrivingScene(int levelNum)
    {
        currGame = currGame.DRIVING;

        if (levelNum == 1)
        {
            currLevel = 1;
            StartCoroutine(LoadingCoroutine(driving1));
        }
        else if (levelNum == 2)
        {
            currLevel = 2;
            StartCoroutine(LoadingCoroutine(driving2));
        }
        else if (levelNum == 3)
        {
            currLevel = 3;
            StartCoroutine(LoadingCoroutine(driving3));
        }
        
        audioManager.PlaySong(audioManager.drivingMusicPath);
        audioManager.PlayDrivingAmbience(0);
        audioManager.PlayRPM(0);
    }

    public void LoadCutscene()
    {
        currGame = currGame.CUTSCENE;
        StartCoroutine(ConcurrentLoadingCoroutine(cutscene));

        Debug.Log("PLAYING " + audioManager.storyMusicPath);
        audioManager.StopDrivingAmbience();
        audioManager.StopRPM();
        if(currLevel==4||currLevel==5){
            audioManager.PlaySong(audioManager.endingAmbiencePath);
        }else{
            audioManager.PlaySong(audioManager.storyMusicPath);
        }
    }

    public void LoadCredits()
    {
        currLevel = 6;
        currGame = currGame.CREDITS;
        StartCoroutine(ConcurrentLoadingCoroutine(cutscene));

        Debug.Log("PLAYING " + audioManager.storyMusicPath);
        audioManager.StopDrivingAmbience();
        audioManager.StopRPM();
        audioManager.PlaySong(audioManager.storyMusicPath);
    }

    public void LoadGoodEnding()
    {
        currGame = currGame.GOOD_ENDING;
        SceneManager.LoadScene("GoodEnding");
        fullGameComplete = true;
    }

    public void LoadBadEnding()
    {
        currGame = currGame.BAD_ENDING;
        SceneManager.LoadScene("BadEnding");
    }

    public void LoadEndlessTacoMaking(bool reset_timer)
    {
        currGame = currGame.TACO_MAKING;
        endlessModeActive = true;

        if (reset_timer)
        {
            GameTimerReset(endlessGameTimerCount); // reset game timer
        }

        StartCoroutine(ConcurrentLoadingCoroutine(endlessTacoScene));
        audioManager.PlaySong(audioManager.tacoMusicPath);
    }

    public void LoadRandomEndlessDriving()
    {
        currGame = currGame.DRIVING;
        endlessModeActive = true;

        SceneObject randDrivingScene = endlessDrivingScenes[Random.Range(0, endlessDrivingScenes.Count)];
        StartCoroutine(LoadingCoroutine(randDrivingScene));

        audioManager.PlaySong(audioManager.drivingMusicPath);
        audioManager.PlayDrivingAmbience(0);
        audioManager.PlayRPM(0);
    }


    public void Quit()
    {
        Application.Quit();
    }

    // **** LOAD CUTSCENE ****

    [HideInInspector]
    public float loadProgress;

    IEnumerator LoadingCoroutine(SceneObject scene)
    {
        isLoadingScene = true;
        yield return null;

        currScene = scene;

        SceneManager.LoadSceneAsync(loadingScene); // load loading scene

        AsyncOperation newScene = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive); // async load next scene

        //Don't let the Scene activate until you allow it to
        newScene.allowSceneActivation = false;
        Debug.Log("Loading " + currScene + ":" + newScene.progress);

        //When the load is still in progress, output the Text and progress bar
        while (!newScene.isDone)
        {
            loadProgress = newScene.progress;

            if (newScene.progress >= 0.9f)
            {
                newScene.allowSceneActivation = true;
            }
            yield return new WaitForEndOfFrame();
        }

        SceneManager.UnloadSceneAsync(loadingScene);
        isLoadingScene = false;
    }

    IEnumerator ConcurrentLoadingCoroutine(SceneObject scene)
    {
        isLoadingScene = true;
        yield return null;

        Scene thisScene = SceneManager.GetActiveScene();
        currScene = scene;

        AsyncOperation newScene = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive); // async load next scene

        //Don't let the Scene activate until you allow it to
        newScene.allowSceneActivation = false;
        Debug.Log("Loading " + currScene + ":" + newScene.progress);

        //When the load is still in progress, output the Text and progress bar
        while (!newScene.isDone)
        {
            loadProgress = newScene.progress;

            if (newScene.progress >= 0.9f)
            {
                newScene.allowSceneActivation = activateScene;
            }
            yield return new WaitForEndOfFrame();
        }

        SceneManager.UnloadSceneAsync(thisScene);
        isLoadingScene = false;
    }

    public void CalculateTime()
    {
        // in total minutes
        float totalClockTime = (totalGameTime_seconds - timeRemaining + (totalGameTime_seconds * 60 * startTime_24hr / 60 / (endTime_24hr - startTime_24hr))) * (60 * (endTime_24hr - startTime_24hr) / totalGameTime_seconds);
        curClockHour = (int)(totalClockTime) / 60;
        
        if (curClockHour > 11)
        {
            isAM = false;
        }

        if (curClockHour > 12)
        {
            curClockHour = curClockHour - 12;
        }
        curClockMinute = (int)(totalClockTime) % 60;
    }

    public void GameTimerReset(int manualGameTime = -1)
    {

        // manual reset
        if (manualGameTime == -1)
        {
            totalGameTime_seconds = totalGameTime_startValue;
        }
        else
        {
            totalGameTime_seconds = manualGameTime;
        }

        // << Set AM >>
        if (startTime_24hr < 12)
        {
            isAM = true;
        }
        else
        {
            isAM = false;
        }


        timeRemaining = totalGameTime_seconds; // leftover time
        main_gameTimer = 0; // set main updating timer to 0


        CalculateTime();
    }

    public void GameTimerUpdate()
    {
        // not if cutscene
        if (currGame == currGame.CUTSCENE || currGame == currGame.MENU) { return; }

        // not if not in play
        if (currGame == currGame.TACO_MAKING && tacoGameManager != null){
            if (tacoGameManager.state != TACOMAKING_STATE.PLAY) { return; }
        }
        if (currGame == currGame.DRIVING && drivingGameManager != null) {
            if (drivingGameManager.state != DRIVINGGAME_STATE.PLAY) { return; }
        }

        // UPDATE TIMER
        if ((timeRemaining - Time.deltaTime) < 0)
        {
            currHappyEnding = false;
        }

        // don't calclate time if at hard cap
        if (curClockHour != hardCapHour || curClockMinute != hardCapMinute)
        {

            CalculateTime();
        }


        // update main game timer
        timeRemaining -= Time.deltaTime;
        if ((1 - (timeRemaining / totalGameTime_seconds)) <= 1 && (1 - (timeRemaining / totalGameTime_seconds)) >= 0)
        {
            main_gameTimer = (1 - (timeRemaining / totalGameTime_seconds));
        }
        else if ((1 - (timeRemaining / totalGameTime_seconds)) > 1)
        {
            main_gameTimer = 1;
        }
        else if ((1 - (timeRemaining / totalGameTime_seconds)) < 0)
        {
            main_gameTimer = 0;
        }
    }

    #region >> SCENE OBJECT (( allows for drag / dropping scenes into inspector ))
    [System.Serializable]
    public class SceneObject
    {
        [SerializeField]
        private string m_SceneName;

        public static implicit operator string(SceneObject sceneObject)
        {
            return sceneObject.m_SceneName;
        }

        public static implicit operator SceneObject(string sceneName)
        {
            return new SceneObject() { m_SceneName = sceneName };
        }
    }

    #region SceneObjects
    #if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(SceneObject))]
            public class SceneObjectEditor : PropertyDrawer
            {
                protected SceneAsset GetSceneObject(string sceneObjectName)
                {
                    if (string.IsNullOrEmpty(sceneObjectName))
                        return null;

                    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                    {
                        EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                        if (scene.path.IndexOf(sceneObjectName) != -1)
                        {
                            return AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset;
                        }
                    }

                    Debug.Log("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in the build settings.");
                    return null;
                }

                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    var sceneObj = GetSceneObject(property.FindPropertyRelative("m_SceneName").stringValue);
                    var newScene = EditorGUI.ObjectField(position, label, sceneObj, typeof(SceneAsset), false);
                    if (newScene == null)
                    {
                        var prop = property.FindPropertyRelative("m_SceneName");
                        prop.stringValue = "";
                    }
                    else
                    {
                        if (newScene.name != property.FindPropertyRelative("m_SceneName").stringValue)
                        {
                            var scnObj = GetSceneObject(newScene.name);
                            if (scnObj == null)
                            {
                                Debug.LogWarning("The scene " + newScene.name + " cannot be used. To use this scene add it to the build settings for the project.");
                            }
                            else
                            {
                                var prop = property.FindPropertyRelative("m_SceneName");
                                prop.stringValue = newScene.name;
                            }
                        }
                    }
                }
            }
    #endif
    #endregion


    #endregion
}
