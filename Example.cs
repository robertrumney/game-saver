using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Example : MonoBehaviour 
{
    public static Game instance;
    
    private void Start()
    {
        // Set the save name to "Example"
        SaveGame.instance.SetSaveName("Example");
        
        // Read the save data
        SaveGame.instance.Read();
    }
    
    private void Update()
    {
        // Check if F5 key is pressed to perform a quick save
        if(Input.GetKeyDown(KeyCode.F5))
        {
          QuickSave ();
        }

        // Check if F9 key is pressed to initiate loading sequence
        if(Input.GetKeyDown(KeyCode.F9))
        {
            // Return early if a loading operation is already in progress
            if (loadingGame)
                return;

            // Return early if a cutscene is currently playing
            if(GameControl.cutscene)
                return;

            // Set loadingGame to true to indicate a loading operation is starting
            loadingGame = true;

            // Deactivate the HUD waypoint if it exists
            if (Game.instance.hudWaypoint) 
            {
                if (Game.instance.hudWaypoint.transform.parent) 
                {
                    Game.instance.hudWaypoint.transform.parent.gameObject.SetActive (false);
                }
            }

            // Set the time scale to normal (1)
            Time.timeScale = 1;

            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Deactivate certain player objects (playerObjects[1], playerObjects[2], playerObjects[3], playerObjects[4])
            Game.instance.playerObjects[1].gameObject.SetActive (false);
            Game.instance.playerObjects[2].gameObject.SetActive (false);
            Game.instance.playerObjects[3].gameObject.SetActive (false);
            Game.instance.playerObjects[4].gameObject.SetActive (false);

            // Hide the health UI
            Game.instance.healthUI.SetActive(false);

            // Set holdingObject flag in player script to indicate the player is holding an object
            Game.instance.playerScript.FPSWalkerComponent.holdingObject = true;

            // Disable the smooth mouse look component of the player script
            Game.instance.playerScript.FPSWalkerComponent.SmoothMouseLookComponent.enabled = false;

            // Set cutscene to true to prevent further interactions during loading
            GameControl.cutscene = true;

            // Set thirdPerson to false, presumably to switch to first-person perspective
            GameControl.thirdPerson = false;

            // Load the active scene again using the LoadingScreen instance
            LoadingScreen.instance.Load(SceneManager.GetActiveScene().name);
        }
    }
}
