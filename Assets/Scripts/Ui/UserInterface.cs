using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.MonoBehaviours;

namespace Assets.Scripts.Ui
{
    public class UserInterface : MonoBehaviour
    {
        public GameObject menuPanel;
        public GameObject mainMenu;
        public GameObject playButtonText;
        public GameObject optionsMenu;
        public GameObject gameTime;
        public GameObject antCount;
        public GameObject resourceInfo;


        public GameObject uiPanel;

        public static bool WorldManagerisPaused = true;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }

            if(WorldManager.isPaused){
                playButtonText.GetComponent<TextMeshProUGUI>().text = (WorldManager.gameState == null ? "Play" : "Resume") ;
            }else{
                UpdateUI();
            }
        }

        // UI part
        public void UpdateUI(){
            resourceInfo.GetComponentInChildren<TextMeshProUGUI>().text = WorldManager.gameState.resources.ToString();
            antCount.GetComponentInChildren<TextMeshProUGUI>().text = (WorldManager.gameState.antNb - AntSpawner.antsInfo.Count).ToString() + "/" + WorldManager.gameState.antNb.ToString();
            gameTime.GetComponentInChildren<TextMeshProUGUI>().text = TimeSpan.FromSeconds(WorldManager.gameState.gameTime).Hours.ToString() + ":" + TimeSpan.FromSeconds(WorldManager.gameState.gameTime).Minutes.ToString("00");
        }

        // Menu part
        public void ToggleMenu(){
            WorldManager.isPaused = !WorldManager.isPaused;
        
            PlayPausePhysics(WorldManagerisPaused);
            CameraController.isActivated = !WorldManager.isPaused;

            menuPanel.SetActive(WorldManager.isPaused);
            if (WorldManager.isPaused) MainMenu();
        
            uiPanel.SetActive(!WorldManager.isPaused);
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
            if (WorldManager.gameState == null){
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
                if(!rb.gameObject.GetComponent<StaticRb>() || !rb.gameObject.GetComponent<StaticRb>().isStatic && !rb.gameObject.GetComponent<Ant>()){
                    rb.isKinematic = isGamePaused;
                }
            }
        }

    }
}
