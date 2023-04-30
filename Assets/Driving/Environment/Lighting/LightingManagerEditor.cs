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


    private void OnEnable()
    {
        timeOfDay = serializedObject.FindProperty("timeOfDay");
        dayCycleState = serializedObject.FindProperty("dayCycleState");

        morningSunrise = serializedObject.FindProperty("morningSunrise");
        midDay = serializedObject.FindProperty("midDay");
        eveningSunset = serializedObject.FindProperty("eveningSunset");
        night = serializedObject.FindProperty("night");

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

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
