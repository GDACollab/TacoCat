using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    public LightingManager lightManager;
    public TIME_OF_DAY currentDayCycle;
    public float fadeSpeed;

    [Header("Dynamic Background")]
    public int moveSpeed = 150;
    public Vector2 spawnAreaPosOffset;
    public Vector2 zOffsetRange = new Vector2(-500, 500);
    public Vector2 spawnAreaSize = new Vector2(50000, 1000);
    public Vector2 scaleRange = new Vector2(0.5f, 2f);

    [Header("Prefabs")]
    public Transform morningParent;
    public int morningCount = 50;
    public List<GameObject> morningPrefabs;
    [Space(10)]
    public Transform middayParent;
    public int middayCount = 50;
    public List<GameObject> middayPrefabs;
    [Space(10)]
    public Transform eveningParent;
    public int eveningCount = 50;
    public List<GameObject> eveningPrefabs;
    [Space(10)]
    public Transform nightParent;
    public int nightCount = 50;
    public List<GameObject> nightPrefabs;

    [Header("Active Gameobjects")]
    public List<GameObject> morningBackground;
    public List<GameObject> middayBackground;
    public List<GameObject> eveningBackground;
    public List<GameObject> nightBackground;

    private void Start()
    {
        CreateRandomBackgroundObjects(morningPrefabs, morningCount, TIME_OF_DAY.MORNING);
        SetObjectsAlphaToZero(morningBackground);

        CreateRandomBackgroundObjects(middayPrefabs, middayCount, TIME_OF_DAY.MIDDAY);
        SetObjectsAlphaToZero(middayBackground);

        CreateRandomBackgroundObjects(eveningPrefabs, eveningCount, TIME_OF_DAY.EVENING);
        SetObjectsAlphaToZero(eveningBackground);

        CreateRandomBackgroundObjects(nightPrefabs, nightCount, TIME_OF_DAY.NIGHT);
        SetObjectsAlphaToZero(nightBackground);

    }

    private void CreateRandomBackgroundObjects(List<GameObject> prefabs, int count, TIME_OF_DAY timeOfDay)
    {
        if (prefabs.Count <= 0) { return; }

        for (int i = 0; i < count; i++)
        {
            // Randomly select a prefab from the list
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

            // Generate a random position within the rectangle bounds
            Vector3 position = transform.position + new Vector3(Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2), Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2), Random.Range(zOffsetRange.x, zOffsetRange.y));
            position += new Vector3(spawnAreaPosOffset.x, spawnAreaPosOffset.y, 0);

            // Generate a random scale within the given range
            Vector3 scale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);

            // Instantiate the prefab at the random position with the random scale
            GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity);

            // Randomly flip the prefab on the y-axis
            if (Random.value < 0.5f)
            {
                spawnedObject.transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
            }
            else
            {
                spawnedObject.transform.localScale = scale;
            }

            // Set the sorting order based on the z position
            SpriteRenderer spriteRenderer = spawnedObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(-position.z);
            }

            // Set the parent transform based on the time of day
            SetParentTransform(timeOfDay, spawnedObject.transform);

            spawnedObject.transform.localScale = scale;

            // Add the spawned object to the appropriate time of day list
            AddObjectToTimeOfDayList(spawnedObject, timeOfDay);
        }
    }
    private void SetParentTransform(TIME_OF_DAY timeOfDay, Transform objectTransform)
    {
        Transform parentTransform = null;

        switch (timeOfDay)
        {
            case TIME_OF_DAY.MORNING:
                parentTransform = morningParent;
                break;
            case TIME_OF_DAY.MIDDAY:
                parentTransform = middayParent;
                break;
            case TIME_OF_DAY.EVENING:
                parentTransform = eveningParent;
                break;
            case TIME_OF_DAY.NIGHT:
                parentTransform = nightParent;
                break;
        }

        if (parentTransform != null)
        {
            objectTransform.SetParent(parentTransform);
        }
    }


    private void AddObjectToTimeOfDayList(GameObject spawnedObject, TIME_OF_DAY dayTime)
    {
        switch (dayTime)
        {
            case TIME_OF_DAY.MORNING:
                morningBackground.Add(spawnedObject);
                break;
            case TIME_OF_DAY.MIDDAY:
                middayBackground.Add(spawnedObject);
                break;
            case TIME_OF_DAY.EVENING:
                eveningBackground.Add(spawnedObject);
                break;
            case TIME_OF_DAY.NIGHT:
                nightBackground.Add(spawnedObject);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        // Show the rectangle bounds using Gizmos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3)spawnAreaPosOffset, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0));
    }

    #region FADE_LAYERS ====================================

    private void Update()
    {
        currentDayCycle = lightManager.dayCycleState;
        Debug.Log(lightManager.dayCycleState);

        switch (currentDayCycle)
        {
            case TIME_OF_DAY.NIGHT:
                FadeObjects(morningBackground, 0);
                FadeObjects(middayBackground, 0);
                FadeObjects(eveningBackground, 0);
                FadeObjects(nightBackground, 1);
                break;
            case TIME_OF_DAY.EVENING:
                FadeObjects(morningBackground, 0);
                FadeObjects(middayBackground, 0);
                FadeObjects(eveningBackground, 1);
                FadeObjects(nightBackground, 0);
                break;
            case TIME_OF_DAY.MIDDAY:
                FadeObjects(morningBackground, 0);
                FadeObjects(middayBackground, 1);
                FadeObjects(eveningBackground, 0);
                FadeObjects(nightBackground, 0);
                break;
            case TIME_OF_DAY.MORNING:
                FadeObjects(morningBackground, 1);
                FadeObjects(middayBackground, 0);
                FadeObjects(eveningBackground, 0);
                FadeObjects(nightBackground, 0);
                break;
        }

        // move the full parent dynamic background to the left slightly
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

    }

    private void FadeObjects(List<GameObject> objects, float targetAlpha)
    {
        foreach (GameObject obj in objects)
        {
            SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
            Color newColor = sr.color;
            newColor.a = Mathf.Lerp(sr.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            sr.color = newColor;
        }
    }

    private void SetObjectsAlphaToZero(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
            Color newColor = sr.color;
            newColor.a = 0f;
            sr.color = newColor;
        }
    }
    #endregion

}
