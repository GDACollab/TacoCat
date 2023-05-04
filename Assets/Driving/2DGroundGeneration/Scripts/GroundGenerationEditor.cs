using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GroundGeneration))]
public class GroundGenerationEditor : Editor
{

    SerializedProperty generationFinished;
    SerializedProperty generationStyle;
    SerializedProperty chunkLengthRange;
    SerializedProperty chunkHeightRange;
    SerializedProperty chunkCount;

    private void OnEnable()
    {
        generationStyle = serializedObject.FindProperty("generationStyle");
        chunkLengthRange = serializedObject.FindProperty("chunkLengthRange");
        chunkHeightRange = serializedObject.FindProperty("chunkHeightRange");
        chunkCount = serializedObject.FindProperty("chunkCount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(generationStyle);

        if ((GroundGeneration.GENERATION_STYLES)generationStyle.enumValueIndex == GroundGeneration.GENERATION_STYLES.custom_sine)
        {
            if (chunkLengthRange != null)
            {
                EditorGUILayout.PropertyField(chunkLengthRange);

                // Clamp the x and y values to 1000
                Vector2 clampedChunkLengthRange = new Vector2(Mathf.Clamp(chunkLengthRange.vector2Value.x, 100f, 1000f), Mathf.Clamp(chunkLengthRange.vector2Value.y, 100f, 1000f));
                chunkLengthRange.vector2Value = clampedChunkLengthRange;
            }

            if (chunkHeightRange != null)
            {
                EditorGUILayout.PropertyField(chunkHeightRange);

                // Clamp the x and y values to 1000
                Vector2 clampedChunkHeightRange = new Vector2(Mathf.Clamp(chunkHeightRange.vector2Value.x, -1000f, 1000f), Mathf.Clamp(chunkHeightRange.vector2Value.y, -1000f, 1000f));
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
