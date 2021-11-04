using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ReplayManager : MonoBehaviour
{
    GameObject replayFileDropDown;

    // Start is called before the first frame update
    void Start()
    {
        replayFileDropDown = GameObject.Find("ReplayFileDropdown");
        RefreshOptions();
    }

    private void Awake()
    {
        replayFileDropDown = GameObject.Find("ReplayFileDropdown");
        RefreshOptions();
    }

    public string GetReplayFilename()
    {
        int index = replayFileDropDown.GetComponent<Dropdown>().value;
        return replayFileDropDown.GetComponent<Dropdown>().options[index].text;
    }

    void RefreshOptions()
    {
        replayFileDropDown.GetComponent<Dropdown>().ClearOptions();

        List<string> replayOptions = new List<string>();
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath + "/", "*.replay");
        foreach (string filePath in filePaths)
        {
            replayOptions.Add(Path.GetFileName(filePath));
        }
        replayFileDropDown.GetComponent<Dropdown>().AddOptions(replayOptions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
