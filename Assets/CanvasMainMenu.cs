using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioManager audioManager;
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlaySong("MenuMusic");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
