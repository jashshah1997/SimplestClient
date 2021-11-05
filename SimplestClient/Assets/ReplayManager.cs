using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ReplayManager : MonoBehaviour
{
    GameObject replayFileDropDown;
    private int currentStateIndex = 0;
    private List<string> gameStatesReplay;

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

    public void LoadSelectedReplay()
    {
        gameStatesReplay = new List<string>();
        string replayFilename = GetReplayFilename();
        string path = Path.Combine(Application.persistentDataPath, replayFilename);

        if (!File.Exists(path))
        {
            Debug.LogError("File : " + path + " doesn't exist!");
            return;
        }

        StreamReader sr = new StreamReader(path);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            gameStatesReplay.Add(line);
        }
        currentStateIndex = -1;
        sr.Close();
    }

    public string GetNextReplayState()
    {
        currentStateIndex++;
        if (currentStateIndex >= gameStatesReplay.Count)
        {
            currentStateIndex--;
            return null;
        }

        return gameStatesReplay[currentStateIndex];
    }

    public string GetPreviousReplayState()
    {
        currentStateIndex--;
        if (currentStateIndex < 0)
        {
            currentStateIndex++;
            return null;
        }

        return gameStatesReplay[currentStateIndex];
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
