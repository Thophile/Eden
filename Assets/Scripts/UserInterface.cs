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
    public GameObject foodCount;
    public GameObject antCount;


    public GameObject uiPanel;

    public static bool isGamePaused = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        if(isGamePaused){
            playButtonText.GetComponent<TextMeshProUGUI>().text = (GameState.current == null ? "Play" : "Resume") ;
        }else{
            UpdateUI();
        }
    }

    // UI part
    public void UpdateUI(){
        foodCount.GetComponentInChildren<TextMeshProUGUI>().text = GameState.current.food.ToString();
        antCount.GetComponentInChildren<TextMeshProUGUI>().text = (GameState.current.antNb -GameObject.Find("Colony").GetComponent<Colony>().antsOut.Count).ToString() + "/" + GameState.current.antNb.ToString();
    }
    // Menu part
    public void ToggleMenu(){
        isGamePaused = !isGamePaused;
        
        PlayPausePhysics(isGamePaused);
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
            WorldManager.Load();
        }
    }
    public void Reset(){
        WorldManager.Reset();
    }
    public void Save(){
        WorldManager.Save();
    }
    public void PlayPausePhysics(bool isGamePaused){
        Rigidbody[] rbs = GameObject.FindObjectsOfType<Rigidbody>();
        foreach (var rb in rbs)
        {
            if(!rb.gameObject.GetComponent<StaticRb>() || !rb.gameObject.GetComponent<StaticRb>().isStatic){
                rb.isKinematic = isGamePaused;
            }
        }
    }

}
