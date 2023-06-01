using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public GameObject visual;
    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            togglePause();

        }
    }

    public void togglePause() {

        paused = !paused;

        Time.timeScale = paused ? 0 : 1; 

        visual.SetActive(paused);

    }

    public void Restart() {

        togglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void Exit()
    {
        Application.Quit();
    }

}
