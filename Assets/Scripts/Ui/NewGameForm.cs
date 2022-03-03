using Assets.Scripts.Model;
using Assets.Scripts.Utils;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Ui
{
    public class NewGameForm : MonoBehaviour
    {
        public GameObject newGameButton;
        public GameObject formGroup;
        public GameObject inputText;

        void OnEnable()
        {
            newGameButton.SetActive(true);
            formGroup.SetActive(false);
        }

        public void DisplayForm()
        {
            newGameButton.SetActive(false);
            formGroup.SetActive(true);
        }

        public void LaunchNewGame()
        {
            GameManager.saveName = SaveUtils.GetFileName(inputText.GetComponent<TextMeshProUGUI>().text);
            SceneManager.LoadScene(1);
        }

    }
}