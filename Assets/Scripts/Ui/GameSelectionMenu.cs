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
        while (content.transform.childCount > 1)
        {
            DestroyImmediate(content.transform.GetChild(1).gameObject);
        }
        foreach (var save in SaveManager.GetSavesList())
        {
            var element = Instantiate(saveTemplate) as GameObject;
            element.transform.SetParent(content.transform);
            element.GetComponentInChildren<TextMeshProUGUI>().text = SaveManager.GetVerboseName(save);
            element.GetComponentInChildren<Button>().onClick.AddListener(delegate { SaveManager.LoadGame(save); });
        }
        
    }
    
}
