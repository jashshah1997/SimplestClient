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
    GameObject findGameSessionButton;
    GameObject placeholderGameButton;
    GameObject infoText1;
    GameObject infoText2;
    GameObject ticTacToeController;

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
            else if (go.name == "FindGameSessionButton")
                findGameSessionButton = go;
            else if (go.name == "PlaceholderGameButton")
                placeholderGameButton = go;
            else if (go.name == "InfoText1")
                infoText1 = go;
            else if (go.name == "InfoText2")
                infoText2 = go;
            else if (go.name == "TicTacToeController")
                ticTacToeController = go;
        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed);
        placeholderGameButton.GetComponent<Button>().onClick.AddListener(PlaceholderGameButtonPressed);

        ChangeGameState(GameStates.Login);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SubmitButtonPressed()
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

    private void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    private void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    private void PlaceholderGameButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(
            ClientToServerSignifiers.TicTacToePlay + "," + 
            ticTacToeController.GetComponent<TicTacToeController>().SerializeGameState());
        ChangeGameState(GameStates.WaitingTicTacToe);
    }

    private void FindGameSessionButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AddToGameSessionQueue + "");
        ChangeGameState(GameStates.WaitingForMatch);
    }

    public void ChangeGameState(int newState)
    {
        inputFieldUserName.SetActive(false);
        inputFieldPassword.SetActive(false);
        buttonSubmit.SetActive(false);
        toggleLogin.SetActive(false);
        toggleCreate.SetActive(false);
        findGameSessionButton.SetActive(false);
        placeholderGameButton.SetActive(false);
        infoText1.SetActive(false);
        infoText2.SetActive(false);

        // TODO: Uncomment this later
        // ticTacToeController.SetActive(false);

        if (newState == GameStates.Login)
        {
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            buttonSubmit.SetActive(true);
            toggleLogin.SetActive(true);
            toggleCreate.SetActive(true);
            infoText1.SetActive(true);
            infoText2.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            findGameSessionButton.SetActive(true);
        }
        else if (newState == GameStates.WaitingForMatch)
        {

        }
        else if (newState == GameStates.PlayingTicTacToe)
        {
            ticTacToeController.SetActive(true);
            placeholderGameButton.SetActive(true);
            ticTacToeController.GetComponent<TicTacToeController>().isMyTurn = true;
        }
        else if (newState == GameStates.WaitingTicTacToe)
        {
            ticTacToeController.SetActive(true);
            ticTacToeController.GetComponent<TicTacToeController>().isMyTurn = false;
        }
    }
}

public static class GameStates
{
    public const int Login = 1;
    public const int MainMenu = 2;
    public const int WaitingForMatch = 3;
    public const int PlayingTicTacToe = 4;
    public const int WaitingTicTacToe = 5;
}