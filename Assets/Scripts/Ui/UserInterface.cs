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

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }

            if(GameManager.isPaused){
                playButtonText.GetComponent<TextMeshProUGUI>().text = (GameManager.gameState == null ? "Play" : "Resume") ;
            }else{
                UpdateUI();
            }
        }

        // UI part
        public void UpdateUI(){
            resourceInfo.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.gameState.resources.ToString();
            antCount.GetComponentInChildren<TextMeshProUGUI>().text = (GameManager.gameState.antNb - AntSpawner.antsInfo.Count).ToString() + "/" + GameManager.gameState.antNb.ToString();
            gameTime.GetComponentInChildren<TextMeshProUGUI>().text = TimeSpan.FromSeconds(GameManager.gameState.gameTime).Hours.ToString() + ":" + TimeSpan.FromSeconds(GameManager.gameState.gameTime).Minutes.ToString("00");
        }

        // Menu part
        public void ToggleMenu(){
            GameManager.isPaused = !GameManager.isPaused;
        
            PlayPausePhysics(GameManager.isPaused);
            CameraController.isActivated = !GameManager.isPaused;

            menuPanel.SetActive(GameManager.isPaused);
            if (GameManager.isPaused) MainMenu();
        
            uiPanel.SetActive(!GameManager.isPaused);
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
            if (GameManager.gameState == null){
                GameManager.Load();
            }
        }
        public void Reset(){
            GameManager.Reset();
        }
        public void Save(){
            GameManager.Save();
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
