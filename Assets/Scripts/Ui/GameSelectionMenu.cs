using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectionMenu : MonoBehaviour
{
    public GameObject content;
    public GameObject saveTemplate;
    // Start is called before the first frame update
    void OnEnable()
    {
        RefreshGamesList();
    }

    public void RefreshGamesList()
    {
        while (content.transform.childCount > 1)
        {
            DestroyImmediate(content.transform.GetChild(1).gameObject);
        }
        foreach (var save in SaveManager.GetSavesList())
        {
            var element = Instantiate(saveTemplate) as GameObject;
            element.transform.SetParent(content.transform);
            element.transform.localScale = new Vector3(1, 1, 1);
            element.GetComponentInChildren<TextMeshProUGUI>().text = SaveManager.GetVerboseName(save);
            var buttons = element.GetComponentsInChildren<Button>();
            buttons[0].onClick.AddListener(delegate { SaveManager.DeleteGame(save); this.RefreshGamesList(); });
            buttons[1].onClick.AddListener(delegate { SaveManager.LoadGame(save); });
        }
    }
}
