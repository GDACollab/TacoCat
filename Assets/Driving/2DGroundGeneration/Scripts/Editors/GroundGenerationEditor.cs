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
    SerializedProperty chunkBucketLengths;
    SerializedProperty chunkBucketHeights;
    SerializedProperty maxChunkLength;
    SerializedProperty maxChunkHeight;

    private void OnEnable()
    {
        generationStyle = serializedObject.FindProperty("generationStyle");
        chunkStyles = serializedObject.FindProperty("chunkStyles");
        maxChunkLength = serializedObject.FindProperty("maxChunkLength");
        maxChunkHeight = serializedObject.FindProperty("maxChunkHeight");
        chunkLengthRange = serializedObject.FindProperty("chunkLengthRange");
        chunkHeightRange = serializedObject.FindProperty("chunkHeightRange");
        chunkBucketHeights = serializedObject.FindProperty("chunkBucketHeights");
        chunkBucketLengths = serializedObject.FindProperty("chunkBucketLengths");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(generationStyle);

#region ChunkStyles
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Chunk Styles", EditorStyles.boldLabel);

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
#endregion



        if ((GroundGeneration.GENERATION_STYLES)generationStyle.enumValueIndex == GroundGeneration.GENERATION_STYLES.custom_sine)
        {
            EditorGUILayout.PropertyField(chunkLengthRange);

            // Clamp the x and y values to 1000
            Vector2 clampedChunkLengthRange = new Vector2(Mathf.Clamp(chunkLengthRange.vector2Value.x, 100f, 5000f), Mathf.Clamp(chunkLengthRange.vector2Value.y, 100f, 5000f));
            chunkLengthRange.vector2Value = clampedChunkLengthRange;

            EditorGUILayout.PropertyField(chunkHeightRange);

            // Clamp the x and y values to 1000
            Vector2 clampedChunkHeightRange = new Vector2(Mathf.Clamp(chunkHeightRange.vector2Value.x, -5000f, 5000f), Mathf.Clamp(chunkHeightRange.vector2Value.y, -5000f, 50000f));
            chunkHeightRange.vector2Value = clampedChunkHeightRange;
        }
        else if ((GroundGeneration.GENERATION_STYLES)generationStyle.enumValueIndex == GroundGeneration.GENERATION_STYLES.chunk_bucket){

#region ChunkBucket
                EditorGUILayout.Space(10);


#region heights
                    EditorGUILayout.Space(10);

                    // Display the chunkBucketHeights list as FloatFields
                    EditorGUILayout.LabelField("Chunk Bucket Heights", EditorStyles.boldLabel);
                    for (int i = 0; i < chunkBucketHeights.arraySize; i++)
                    {
                        SerializedProperty elementProp = chunkBucketHeights.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(elementProp);
                    }

                    EditorGUILayout.Space(10);

                    // Begin a horizontal layout
                    GUILayout.BeginHorizontal();

                    // Add some space between the buttons
                    GUILayout.Space(10);

                    // Add a button to add a new element to the chunkBucketHeights list
                    if (GUILayout.Button("Add Chunk Bucket Height"))
                    {
                        chunkBucketHeights.InsertArrayElementAtIndex(chunkBucketHeights.arraySize);
                        serializedObject.ApplyModifiedProperties();
                    }

                    // Add some space between the buttons
                    GUILayout.Space(10);

                    // Add a button to remove a new element to the chunkBucketHeights list
                    if (GUILayout.Button("Remove Chunk Bucket Height"))
                    {
                        chunkBucketHeights.DeleteArrayElementAtIndex(chunkBucketHeights.arraySize - 1);
                        serializedObject.ApplyModifiedProperties();
                    }

                    // End the horizontal layout
                    GUILayout.EndHorizontal();
#endregion


#region lengths
                    EditorGUILayout.Space(10);


                    // Display the chunkBucketLengths list as IntFields
                    EditorGUILayout.LabelField("Chunk Bucket Lengths", EditorStyles.boldLabel);
                    for (int i = 0; i < chunkBucketLengths.arraySize; i++)
                    {
                        SerializedProperty elementProp = chunkBucketLengths.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(elementProp);
                    }

                    // Begin a horizontal layout
                    GUILayout.BeginHorizontal();

                    // Add a button to add a new element to the chunkBucketLengths list
                    if (GUILayout.Button("Add Chunk Bucket Length"))
                    {
                        chunkBucketLengths.InsertArrayElementAtIndex(chunkBucketLengths.arraySize);
                        serializedObject.ApplyModifiedProperties();
                    }

                    // Add some space between the buttons
                    GUILayout.Space(10);

                    // Add a button to remove a new element to the chunkBucketLengths list
                    if (GUILayout.Button("Remove Chunk Bucket Length"))
                    {
                        chunkBucketLengths.DeleteArrayElementAtIndex(chunkBucketLengths.arraySize - 1);
                        serializedObject.ApplyModifiedProperties();
                    }

                    // End the horizontal layout
                    GUILayout.EndHorizontal();
#endregion


                EditorGUILayout.Space(10);
#endregion

        }
        else
        {
            EditorGUILayout.PropertyField(maxChunkHeight);
            EditorGUILayout.PropertyField(maxChunkLength);
        }

        serializedObject.ApplyModifiedProperties();
    }


}
#endif
