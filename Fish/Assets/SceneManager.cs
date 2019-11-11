using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] FlockManager[] flockManagers;
    [SerializeField] GameObject debugHelpText;

    bool debugMode = false;

    private void Start()
    {
        debugHelpText.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f")) ToggleDebugMode();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); 
    }

    private void ToggleDebugMode()
    {
        debugMode = !debugMode;
        debugHelpText.active = debugMode;
        foreach (FlockManager fm in flockManagers)
        {
            fm.SetFlockDebug(debugMode);
        }
    }
}
