using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UserInterface : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject mainMenu;
    public GameObject playButtonText;
    public GameObject optionsMenu;

    public GameObject uiPanel;

    public static bool isGamePaused = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGamePaused) playButtonText.GetComponent<TextMeshProUGUI>().text = (GameState.current == null ? "Play" : "Resume") ;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu(){
        isGamePaused = !isGamePaused;
        CameraController.isActivated = !isGamePaused;
        menuPanel.SetActive(isGamePaused);
        if (isGamePaused) MainMenu();
        
        uiPanel.SetActive(!isGamePaused);
    }

    public void OptionsMenu(){
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void MainMenu(){
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void Play(){
        ToggleMenu();
        if (GameState.current == null){
            WorldBuilder.Load();
        }
    }
    public void Reset(){
        WorldBuilder.Reset();
    }
    public void Save(){
        WorldBuilder.Save();
    }
}
