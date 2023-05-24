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

public enum currGame { NONE, MENU, CUTSCENE, TACO_MAKING, DRIVING }

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public static EventSystem eventSystemInstance = null;

    [HideInInspector]
    public AudioManager audioManager;
    [HideInInspector]
    public PauseManager pauseManager;

    // save reference to seperate game manager
    TacoMakingGameManager tacoGameManager;
    DrivingGameManager drivingGameManager;
    CutsceneManager cutsceneManager;


    [Header(" === INIT GAME TIMER === ")]
    [Tooltip("Total duration of game in seconds")]
    public float totalGameTime_seconds = 300;
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
    public float countdownTimer = 0;
    [Tooltip("current hour")]
    public int curClockHour;
    [Tooltip("current minute")]
    public int curClockMinute;

    private bool hardCapAM;
    [HideInInspector]
    public bool isAM = true;
    [HideInInspector]
    public bool happyEnd = true;



    [Header(" === SCENE MANAGEMENT === ")]
    public currGame currGame = currGame.NONE;
    public int currLevel = 1;
    public SceneObject currScene;
    public int cutsceneIndex = 0;
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
    public SceneObject creditscene;

    [Header("--SCENE VARIABLE TRANSFER--")]
    public int nitroCharges = 3;
    public int gasAmount;


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

        // << START GAME TIMER >>
        GameTimerStart();
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
        Debug.Log("New scene loaded: " + scene.name);

        StartCoroutine(SceneSetup());
    }

    public IEnumerator SceneSetup()
    {
        // get type of scene
        bool determinedSceneType = false;
        while (!determinedSceneType)
        {
            if (GameObject.FindGameObjectWithTag("Main Menu"))
            {
                Debug.Log("GameManager: Setup Main Menu");
                determinedSceneType = true;
                currGame = currGame.MENU;
                timeRemaining = totalGameTime_seconds;
            }
            else if (GameObject.FindGameObjectWithTag("TacoGameManager"))
            {
                Debug.Log("GameManager: Setup Taco Making");
                determinedSceneType = true;
                tacoGameManager = GameObject.FindGameObjectWithTag("TacoGameManager").GetComponent<TacoMakingGameManager>();
                tacoGameManager.difficulty = currLevel;
                currGame = currGame.TACO_MAKING;
            }
            else if (GameObject.FindGameObjectWithTag("CutsceneManager"))
            {
                Debug.Log("GameManager: Setup Cutscene");
                determinedSceneType = true;
                cutsceneManager = GameObject.FindGameObjectWithTag("CutsceneManager").GetComponent<CutsceneManager>();
                currGame = currGame.CUTSCENE;
            }
            else if (GameObject.FindGameObjectWithTag("DrivingGameManager"))
            {
                Debug.Log("GameManager: Setup Driving");
                determinedSceneType = true;
                drivingGameManager = GameObject.FindGameObjectWithTag("DrivingGameManager").GetComponent<DrivingGameManager>();
                drivingGameManager.nitroCharges = (currLevel == 1) ? Mathf.Max(nitroCharges, 1) : nitroCharges;
                currGame = currGame.DRIVING;
            }
            else
            {
                Debug.LogWarning("Cannot determine Scene Type");
            }
            yield return null;
        }

        // wait till music is loaded
        yield return new WaitForSeconds(2);

        // Start Menu Music
        if (currGame == currGame.MENU)
        {
            audioManager.PlaySong(audioManager.menuMusicPath);
        }
    }

    public void Update()
    {

        // << TACO GAME MANAGER >>
        if (currGame == currGame.TACO_MAKING && tacoGameManager != null)
        {
            // check if all customers submitted , if so move to driving with gas amount
            if (tacoGameManager.endOfGame && !isLoadingScene)
            {
                nitroCharges = tacoGameManager.nitroCharges;
                LoadDrivingScene(currLevel);
            }
        }

        // << DRIVING GAME MANAGER >>
        if (currGame == currGame.DRIVING && drivingGameManager != null)
        {
            if (drivingGameManager.endOfGame && !isLoadingScene)
            {   
                /*
                if (currLevel == 3) {
                    LoadMenu(); 
                }
                */
                currLevel++;
                Debug.Log("Current Level: " + currLevel);
                StartCoroutine(ConcurrentLoadingCoroutine(cutscene));
                // LoadCutscene();
            }
        }

        // << CUTSCENE MANAGER >>
        if (currGame == currGame.CUTSCENE && cutsceneManager != null)
        {
            if (cutsceneManager.endOfCutscene && !isLoadingScene)
            {
                LoadTacoMakingScene();
            }
        }


        // << UPDATE GAME TIMER >>
        GameTimerUpdate();
    }

    public void LoadMenu()
    {
        currGame = currGame.MENU;
        currLevel = 1;
        cutsceneIndex = 0;
        SceneManager.LoadScene(menuScene);
        audioManager.PlaySong(audioManager.menuMusicPath);
    }

    // **** LOAD TACO MAKING SCENE ****
    public void LoadTacoMakingScene(bool async = false)
    {

        currGame = currGame.TACO_MAKING;
        if (async)
        {
            StartCoroutine(ConcurrentLoadingCoroutine(tacoMakingScene));
        }
        else
        {
            StartCoroutine(LoadingCoroutine(tacoMakingScene));
        }
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
    }

    public void LoadCutscene()
    {
        currGame = currGame.CUTSCENE;
        SceneManager.LoadScene(cutscene);
        audioManager.PlaySong(audioManager.storyMusicPath);
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
        while (curClockHour > 12)
        {
            curClockHour = curClockHour - 12;
        }
        curClockMinute = (int)(totalClockTime) % 60;
    }

    public void GameTimerStart()
    {
        timeRemaining = totalGameTime_seconds;
        if (startTime_24hr < 12)
        {
            isAM = true;
        }
        else
        {
            isAM = false;
        }
        if (hardCapHour > 12)
        {
            hardCapHour -= 12;
            hardCapAM = false;
        }
        else
        {
            hardCapAM = true;
        }
        CalculateTime();
    }

    public void GameTimerUpdate()
    {
        // is not cutscene or menu, countdown time
        if (currGame != currGame.CUTSCENE && currGame != currGame.MENU)
        {
            if ((timeRemaining - Time.deltaTime) < 0)
            {
                happyEnd = false;
            }
            if (curClockHour != hardCapHour || curClockMinute != hardCapMinute || isAM != hardCapAM)
            {
                timeRemaining -= Time.deltaTime;
                if ((1 - (timeRemaining / totalGameTime_seconds)) <= 1 && (1 - (timeRemaining / totalGameTime_seconds)) >= 0)
                {
                    countdownTimer = (1 - (timeRemaining / totalGameTime_seconds));
                }
                else if ((1 - (timeRemaining / totalGameTime_seconds)) > 1)
                {
                    countdownTimer = 1;
                }
                else if ((1 - (timeRemaining / totalGameTime_seconds)) < 0)
                {
                    countdownTimer = 0;
                }
                CalculateTime();
            }
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
