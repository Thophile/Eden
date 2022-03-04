using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public class StartMenu : MonoBehaviour
    {
        [Header("Menu elements")]
        public GameObject mainMenu;
        public GameObject playSubMenu;
        public GameObject optionsSubMenu;        

        public void rootMenu()
        {
            playSubMenu.SetActive(false);
            optionsSubMenu.SetActive(false);
        }
        public void playMenu()
        {
            playSubMenu.SetActive(true);
            optionsSubMenu.SetActive(false);
        }

        public void optionsMenu()
        {
            playSubMenu.SetActive(false);
            optionsSubMenu.SetActive(true);
        }
        public void Quit()
        {
            Application.Quit();
        }
    }
}