using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
public class HideInInspectorUnlessDebugAttribute : PropertyAttribute { }

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{

    [Range(0, 1)]
    public float timeOfDay;

    [Header("Time Of Day Lighting Palettes")]
    // Morning Sunrise
    [Tooltip("Index 0: Foreground, 1: PlayArea, 2: Background, 3: Skybox")]
    public List<Color> morningSunrise = new List<Color>
    {
        new Color32(255, 167, 38, 255),
        new Color32(255, 213, 79, 255),
        new Color32(245, 124, 0, 255),
        new Color32(255, 204, 128, 255)
    };

    // Mid-day
    [Tooltip("Index 0: Foreground, 1: PlayArea, 2: Background, 3: Skybox")]
    public List<Color> midDay = new List<Color>
    {
        new Color32(41, 182, 246, 255),
        new Color32(79, 195, 247, 255),
        new Color32(3, 155, 229, 255),
        new Color32(129, 212, 250, 255)
    };

    // Evening Sunset
    [Tooltip("Index 0: Foreground, 1: PlayArea, 2: Background, 3: Skybox")]
    public List<Color> eveningSunset = new List<Color>
    {
        new Color32(239, 108, 0, 255),
        new Color32(255, 167, 38, 255),
        new Color32(230, 81, 0, 255),
        new Color32(255, 204, 128, 255)
    };

    // Night
    [Tooltip("Index 0: Foreground, 1: PlayArea, 2: Background, 3: Skybox")]
    public List<Color> night = new List<Color>
    {
        new Color32(40, 53, 147, 255),
        new Color32(92, 107, 192, 255),
        new Color32(26, 35, 126, 255),
        new Color32(159, 168, 218, 255)
    };


    [Header("POSITIONS")]
    // Space the layers out based on a modular float for perspective things
    [Range(0, 1000)]
    public float layerSpacing = 1000f;
    [Range(0, 5000)]
    public float skyboxSpacing = 3000f;

    [Header("SETUP VARIABLES [[DONT TOUCH]]")]
    // Hold lights for each sorting layer
    [HideInInspectorUnlessDebug]
    public Light2D foregroundLight;
    [HideInInspectorUnlessDebug]
    public Light2D playAreaLight;
    [HideInInspectorUnlessDebug]
    public Light2D backgroundLight;
    [HideInInspectorUnlessDebug]
    public Light2D skyboxLight;

    // Hold each art asset in a list
    [HideInInspectorUnlessDebug]
    public List<GameObject> foregroundObjects;
    [HideInInspectorUnlessDebug]
    public List<GameObject> playAreaObjects;
    [HideInInspectorUnlessDebug]
    public List<GameObject> backgroundLayerObjects;
    [HideInInspectorUnlessDebug]
    public List<GameObject> skyboxLayerObjects;

    private void Start()
    {
        SetZPositions();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateLightColors(timeOfDay);

        SetZPositions();


    }

    public void UpdateLightColors(float timeOfDay)
    {
        List<Color> fromPalette = GetPaletteAtTime(timeOfDay);
        List<Color> toPalette = GetPaletteAtTime(timeOfDay + 0.25f);

        float lerpValue = (timeOfDay % 0.25f) * 4;

        foregroundLight.color = Color.Lerp(fromPalette[0], toPalette[0], lerpValue);
        playAreaLight.color = Color.Lerp(fromPalette[1], toPalette[1], lerpValue);
        backgroundLight.color = Color.Lerp(fromPalette[2], toPalette[2], lerpValue);
        skyboxLight.color = Color.Lerp(fromPalette[3], toPalette[3], lerpValue);
    }

    private List<Color> GetPaletteAtTime(float time)
    {
        if (time >= 0f && time < 0.25f)
        {
            return morningSunrise;
        }
        else if (time >= 0.25f && time < 0.5f)
        {
            return midDay;
        }
        else if (time >= 0.5f && time < 0.75f)
        {
            return eveningSunset;
        }
        else // time >= 0.75f && time <= 1f
        {
            return night;
        }
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
}
