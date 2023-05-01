using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BezierCurveGeneration))]
public class BezierCurveGenerationEditor : Editor
{
    SerializedProperty chunkStyleProperty;
    SerializedProperty angleTypeProperty;

    SerializedProperty rounded_p1_offset;
    SerializedProperty rounded_p2_offset;
    SerializedProperty straight_p1_offset;
    SerializedProperty straight_p2_offset;
    SerializedProperty flat_p1_offset;
    SerializedProperty flat_p2_offset;

    private void OnEnable()
    {
        chunkStyleProperty = serializedObject.FindProperty("chunkStyle");
        angleTypeProperty = serializedObject.FindProperty("angleType");

        rounded_p1_offset = serializedObject.FindProperty("rounded_p1_offset");
        rounded_p2_offset = serializedObject.FindProperty("rounded_p2_offset");
        straight_p1_offset = serializedObject.FindProperty("straight_p1_offset");
        straight_p2_offset = serializedObject.FindProperty("straight_p2_offset");
        flat_p1_offset = serializedObject.FindProperty("flat_p1_offset");
        flat_p2_offset = serializedObject.FindProperty("flat_p2_offset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Show the enum dropdown for angleType
        EditorGUILayout.PropertyField(angleTypeProperty);

        // Show the enum dropdown for chunkStyle
        EditorGUILayout.PropertyField(chunkStyleProperty);

        // Check the value of chunkStyle and show/hide the appropriate fields
        switch (chunkStyleProperty.enumValueIndex)
        {
            // rounded
            case 1:
                EditorGUILayout.PropertyField(rounded_p1_offset);
                EditorGUILayout.PropertyField(rounded_p2_offset);
                break;

            // straight
            case 2:
                EditorGUILayout.PropertyField(straight_p1_offset);
                EditorGUILayout.PropertyField(straight_p2_offset);
                break;

            //flat
            case 3:
                EditorGUILayout.PropertyField(flat_p1_offset);
                EditorGUILayout.PropertyField(flat_p2_offset);
                break;

        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
