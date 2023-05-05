using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GroundGeneration))]
public class GroundGenerationEditor : Editor
{

    SerializedProperty generationFinished;
    SerializedProperty generationStyle;
    SerializedProperty chunkStyles;
    SerializedProperty chunkLengthRange;
    SerializedProperty chunkHeightRange;
    SerializedProperty chunkCount;

    private void OnEnable()
    {
        generationStyle = serializedObject.FindProperty("generationStyle");
        chunkStyles = serializedObject.FindProperty("chunkStyles");
        chunkLengthRange = serializedObject.FindProperty("chunkLengthRange");
        chunkHeightRange = serializedObject.FindProperty("chunkHeightRange");
        chunkCount = serializedObject.FindProperty("chunkCount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(generationStyle);

        // Display the chunkStyles list as EnumPopups
        for (int i = 0; i < chunkStyles.arraySize; i++)
        {
            SerializedProperty elementProp = chunkStyles.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(elementProp);
        }

        // Begin a horizontal layout
        GUILayout.BeginHorizontal();

        // Add a button to add a new element to the list
        if (GUILayout.Button("Add Chunk Style"))
        {
            chunkStyles.InsertArrayElementAtIndex(chunkStyles.arraySize);
            serializedObject.ApplyModifiedProperties();
        }

        // Add some space between the buttons
        GUILayout.Space(10);

        // Add a button to remove a new element to the list
        if (GUILayout.Button("Remove Chunk Style"))
        {
            chunkStyles.DeleteArrayElementAtIndex(chunkStyles.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
        }

        // End the horizontal layout
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        if ((GroundGeneration.GENERATION_STYLES)generationStyle.enumValueIndex == GroundGeneration.GENERATION_STYLES.custom_sine)
        {
            if (chunkLengthRange != null)
            {
                EditorGUILayout.PropertyField(chunkLengthRange);

                // Clamp the x and y values to 1000
                Vector2 clampedChunkLengthRange = new Vector2(Mathf.Clamp(chunkLengthRange.vector2Value.x, 100f, 5000f), Mathf.Clamp(chunkLengthRange.vector2Value.y, 100f, 5000f));
                chunkLengthRange.vector2Value = clampedChunkLengthRange;
            }

            if (chunkHeightRange != null)
            {
                EditorGUILayout.PropertyField(chunkHeightRange);

                // Clamp the x and y values to 1000
                Vector2 clampedChunkHeightRange = new Vector2(Mathf.Clamp(chunkHeightRange.vector2Value.x, -5000f, 5000f), Mathf.Clamp(chunkHeightRange.vector2Value.y, -5000f, 50000f));
                chunkHeightRange.vector2Value = clampedChunkHeightRange;
            }
        }

        if (chunkCount != null)
        {
            EditorGUILayout.PropertyField(chunkCount);
        }

        serializedObject.ApplyModifiedProperties();
    }


}
#endif
