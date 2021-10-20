using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject inputFieldUserName;
    GameObject inputFieldPassword;
    GameObject buttonSubmit;
    GameObject toggleLogin;
    GameObject toggleCreate;
    GameObject networkedClient;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "InputFieldUserName")
                inputFieldUserName = go;
            else if (go.name == "InputFieldPassword")
                inputFieldPassword = go;
            else if (go.name == "ButtonSubmit")
                buttonSubmit = go;
            else if (go.name == "ToggleLogin")
                toggleLogin = go;
            else if (go.name == "ToggleCreate")
                toggleCreate = go;
            else if (go.name == "NetworkedClient")
                networkedClient = go;
        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubmitButtonPressed()
    {
        Debug.Log("ButtonPressed");
        string username = inputFieldUserName.GetComponent<InputField>().text;
        string password = inputFieldPassword.GetComponent<InputField>().text;
        string message_to_server;
        if (toggleLogin.GetComponent<Toggle>().isOn)
        {
            message_to_server = ClientToServerSignifiers.Login + "," + username + "," + password;
        } else
        {
            message_to_server = ClientToServerSignifiers.CreateAccount + "," + username + "," + password;
        }

        Debug.Log(message_to_server);
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(message_to_server);
    }

    public void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    public void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
}
public static class ClientToServerSignifiers
{
    public const int Login = 1;
    public const int CreateAccount = 2;

}

public static class ServerToClientSignifiers
{
    public const int LoginResponse = 1;
}

public static class LoginResponses
{
    public const int Success = 1;
    public const int FailureNameInUse = 2;
    public const int FailureNameNotFound = 3;
    public const int FailureIncorrectPassword = 4;
}