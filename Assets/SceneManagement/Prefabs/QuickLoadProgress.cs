using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickLoadProgress : MonoBehaviour
{
    GameManager manager;
    float progress = 0;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        progress = Mathf.Abs(Mathf.Lerp(progress, manager.loadProgress, Time.deltaTime));
        GameObject.Find("LoadingBar").transform.GetChild(0).eulerAngles = new Vector3(0, 0, progress * 77 - 54);
    }
}
