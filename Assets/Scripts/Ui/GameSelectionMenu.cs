using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSelectionMenu : MonoBehaviour
{
    public GameObject content;
    public GameObject scrollElementTemplate;
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (var save in SaveManager.GetSavesList())
        {
            var element = Instantiate(scrollElementTemplate) as GameObject;
            element.transform.parent = content.transform;
            element.GetComponent<TextMeshProUGUI>().text = save;

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
