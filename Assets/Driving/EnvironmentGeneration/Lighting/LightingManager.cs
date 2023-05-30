using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
public class HideInInspectorUnlessDebugAttribute : PropertyAttribute { }
public enum TIME_OF_DAY { MORNING, MIDDAY, EVENING, NIGHT, MIDNIGHT, ERROR }

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{

    [Range(0, 1)]
    public float timeOfDay;
    public TIME_OF_DAY dayCycleState = TIME_OF_DAY.MORNING;

    [Header("Time Of Day Lighting Palettes")]
    // Morning Sunrise
    [Tooltip("0: Foreground, 1: PlayArea, 2: Background, 3: Skybox, 4: Sun")]
    public List<Color> morningSunrise = new List<Color>
    {
        new Color32(255, 167, 38, 255),
        new Color32(255, 213, 79, 255),
        new Color32(245, 124, 0, 255),
        new Color32(255, 204, 128, 255)
    };

    // Mid-day
    [Tooltip("0: Foreground, 1: PlayArea, 2: Background, 3: Skybox, 4: Sun")]
    public List<Color> midDay = new List<Color>
    {
        new Color32(41, 182, 246, 255),
        new Color32(79, 195, 247, 255),
        new Color32(3, 155, 229, 255),
        new Color32(129, 212, 250, 255)
    };

    // Evening Sunset
    [Tooltip("0: Foreground, 1: PlayArea, 2: Background, 3: Skybox, 4: Sun")]
    public List<Color> eveningSunset = new List<Color>
    {
        new Color32(239, 108, 0, 255),
        new Color32(255, 167, 38, 255),
        new Color32(230, 81, 0, 255),
        new Color32(255, 204, 128, 255)
    };

    // Night
    [Tooltip("0: Foreground, 1: PlayArea, 2: Background, 3: Skybox, 4: Sun")]
    public List<Color> night = new List<Color>
    {
        new Color32(40, 53, 147, 255),
        new Color32(92, 107, 192, 255),
        new Color32(26, 35, 126, 255),
        new Color32(159, 168, 218, 255)
    };


    [Tooltip("Percentage of TimeOfDay Between Palette Tranisitions. Sum of values should equal 1.")]
    public float morningTimeLength = 0.1f;
    public float midDayTimeLength = 0.5f;
    public float eveningTimeLength = 0.3f;
    public float nightTimeLength = 0.1f;

    [Space(10)]
    public List<float> sunIntensities = new List<float>();

    [Space(10)]
    public float lightColorAdjustSpeed = 2;

    [Space(20)]
    [Header("SETUP VARIABLES [[DONT TOUCH]]")]
    // Hold lights for each sorting layer
    public Light2D foregroundLight;
    public Light2D playAreaLight;
    public Light2D backgroundLight;
    public Light2D skyboxLight;

    [Space(10)]
    // Hold each art asset in a list
    public List<GameObject> foregroundObjects;
    public List<GameObject> playAreaObjects;
    public List<GameObject> backgroundLayerObjects;
    public List<GameObject> skyboxLayerObjects;

    [Space(10)]
    public SunCycle sunCycle;
    public Light2D sunLight;

    [Header("SPACING")]
    // Space the layers out based on a modular float for perspective things
    [Range(0, 1000)]
    public float layerSpacing = 1000f;
    [Range(0, 5000)]
    public float skyboxSpacing = 3000f;

    // Update is called once per frame
    void Update()
    {
        UpdateLightColors(timeOfDay);
    }

    public void UpdateLightColors(float curTime)
    {
        List<Color> toPalette = GetPaletteAtTime(curTime);

        foregroundLight.color = Color.Lerp(foregroundLight.color, toPalette[0], lightColorAdjustSpeed * Time.deltaTime);
        playAreaLight.color = Color.Lerp(playAreaLight.color, toPalette[1], lightColorAdjustSpeed * Time.deltaTime);
        backgroundLight.color = Color.Lerp(backgroundLight.color, toPalette[2], lightColorAdjustSpeed * Time.deltaTime);
        skyboxLight.color = Color.Lerp(skyboxLight.color, toPalette[3], lightColorAdjustSpeed * Time.deltaTime);
        sunLight.color = Color.Lerp(sunLight.color, toPalette[4], lightColorAdjustSpeed * Time.deltaTime);

        float curSunIntensity = 0;

        // << SUN INTENSITY >>
        // MORNING
        if (curTime < morningTimeLength)
        {
            curSunIntensity = sunIntensities[0];
        }
        // MIDDAY
        else if (curTime < (morningTimeLength + midDayTimeLength))
        {
            curSunIntensity = sunIntensities[1];
        }
        // EVENING
        else if (curTime < (morningTimeLength + midDayTimeLength + eveningTimeLength))
        {
            curSunIntensity = sunIntensities[2];
        }
        // NIGHT
        else if (curTime > (morningTimeLength + midDayTimeLength + eveningTimeLength))
        {
            curSunIntensity = sunIntensities[3];
        }

        sunLight.intensity = Mathf.Lerp(sunLight.intensity, curSunIntensity, lightColorAdjustSpeed * Time.deltaTime);

    }

    private List<Color> GetPaletteAtTime(float curTime)
    {
        List<Color>[] palettes = new List<Color>[]
        {
            morningSunrise,
            midDay,
            eveningSunset,
            night
        };

        // MORNING
        if (curTime < morningTimeLength)
        {
            dayCycleState = TIME_OF_DAY.MORNING;
            return palettes[0];
        }
        // MIDDAY
        else if (curTime < (morningTimeLength + midDayTimeLength))
        {
            dayCycleState = TIME_OF_DAY.MIDDAY;
            return palettes[1];
        }
        // EVENING
        if (curTime < (morningTimeLength + midDayTimeLength + eveningTimeLength))
        {
            dayCycleState = TIME_OF_DAY.EVENING;
            return palettes[2];
        }
        // NIGHT
        if (curTime > (morningTimeLength + midDayTimeLength + eveningTimeLength) && curTime < 1)
        {
            dayCycleState = TIME_OF_DAY.NIGHT;
            return palettes[3];
        }
        // MIDNIGHT
        if (curTime >= 1)
        {
            dayCycleState = TIME_OF_DAY.MIDNIGHT;
            return palettes[3];
        }

        //dayCycleState = TIME_OF_DAY.ERROR;
        return palettes[3];


    }

    public void SetZPositions()
    {
        // << SET Z POSITIONS >>
        // set foreground position
        foreach (GameObject obj in foregroundObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -layerSpacing);
        }

        // set play area position
        foreach (GameObject obj in playAreaObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
        }

        // set background position
        foreach (GameObject obj in backgroundLayerObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, layerSpacing);
        }

        // set skybox position
        foreach (GameObject obj in skyboxLayerObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, skyboxSpacing);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HideInInspectorUnlessDebugAttribute))]
    public class HideInInspectorUnlessDebugDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsInDebugMode())
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsInDebugMode() ? EditorGUI.GetPropertyHeight(property, label) : 0;
        }

        private bool IsInDebugMode()
        {
            return EditorGUIUtility.hierarchyMode;
        }
    }
#endif
}
