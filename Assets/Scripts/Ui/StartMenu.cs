using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class StartMenu : MonoBehaviour
    {
        [Header("Menu elements")]
        public GameObject mainMenuPanel;
        public GameObject gameCreationPanel;
        public GameObject gameSelectionPanel;        

        public void mainMenu()
        {
            mainMenuPanel.SetActive(true);
            gameCreationPanel.SetActive(false);
            gameSelectionPanel.SetActive(false);
        }
        public void gameCreationMenu()
        {
            mainMenuPanel.SetActive(false);
            gameCreationPanel.SetActive(true);
            gameSelectionPanel.SetActive(false);
        }
        public void gameSelectionMenu()
        {
            mainMenuPanel.SetActive(false);
            gameCreationPanel.SetActive(false);
            gameSelectionPanel.SetActive(true);
        }
    }
}