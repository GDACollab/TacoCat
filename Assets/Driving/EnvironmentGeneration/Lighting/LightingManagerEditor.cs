#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[CustomEditor(typeof(LightingManager))]
public class LightingManagerEditor : Editor
{
    SerializedProperty timeOfDay;
    SerializedProperty dayCycleState;

    SerializedProperty morningSunrise;
    SerializedProperty midDay;
    SerializedProperty eveningSunset;
    SerializedProperty night;

    SerializedProperty sunIntensities;

    private void OnEnable()
    {
        timeOfDay = serializedObject.FindProperty("timeOfDay");
        dayCycleState = serializedObject.FindProperty("dayCycleState");

        morningSunrise = serializedObject.FindProperty("morningSunrise");
        midDay = serializedObject.FindProperty("midDay");
        eveningSunset = serializedObject.FindProperty("eveningSunset");
        night = serializedObject.FindProperty("night");

        sunIntensities = serializedObject.FindProperty("sunIntensities");


    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(timeOfDay);
        EditorGUILayout.PropertyField(dayCycleState);
        EditorGUILayout.PropertyField(morningSunrise);
        EditorGUILayout.PropertyField(midDay);
        EditorGUILayout.PropertyField(eveningSunset);
        EditorGUILayout.PropertyField(night);

        EditorGUILayout.PropertyField(sunIntensities);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
