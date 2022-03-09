using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.MonoBehaviours;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ui
{
    public class UserInterface : MonoBehaviour
    {
        [Header("Menu elements")]
        public GameObject menuPanel;
        public GameObject mainMenu;
        public GameObject optionsMenu;
        [Header("UI elements")]
        public GameObject gameTime;
        public GameObject antCount;
        public GameObject resourceInfo;
        public GameObject uiPanel;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }

            if(!GameManager.isPaused){
                UpdateUI();
            }
        }

        // UI part
        public void UpdateUI(){
            resourceInfo.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.gameState.resources.ToString();
            antCount.GetComponentInChildren<TextMeshProUGUI>().text = (AntSpawner.antsInfo.Count).ToString() + "/" + GameManager.gameState.antNb.ToString();
            gameTime.GetComponentInChildren<TextMeshProUGUI>().text = TimeSpan.FromSeconds(GameManager.gameState.gameTime).Hours.ToString() + ":" + TimeSpan.FromSeconds(GameManager.gameState.gameTime).Minutes.ToString("00");
        }

        // Menu part
        public void ToggleMenu(){
            GameManager.isPaused = !GameManager.isPaused;
            
            PlayPausePhysics(GameManager.isPaused);
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

        public void Save(){
            GameManager.Save();
        }
        public void Quit()
        {
            GameManager.Save();
            SceneManager.LoadScene(0);
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
