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
    GameObject gameOverText;
    GameObject ticTacToeController;
    GameObject backToMainMenuButton;
    GameObject chatManager;
    int currentGameState = GameStates.Login;
    public string currentUsername = "null";

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
            else if (go.name == "GameOverText")
                gameOverText = go;
            else if (go.name == "BackToMainMenuButton")
                backToMainMenuButton = go;
            else if (go.name == "ChatManager")
                chatManager = go;
        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed);
        placeholderGameButton.GetComponent<Button>().onClick.AddListener(PlaceholderGameButtonPressed);
        backToMainMenuButton.GetComponent<Button>().onClick.AddListener(BackToMainMenuButtonPressed);
        ChangeGameState(GameStates.Login);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BackToMainMenuButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveSession + "");
        ChangeGameState(GameStates.MainMenu);
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
        currentUsername = username;
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
        if (!HandleGameFinish())
        {
            // Game is not finished, wait for oponents turn
            ChangeGameState(GameStates.WaitingTicTacToe);
        }

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(
            ClientToServerSignifiers.TicTacToePlay + "," + 
            ticTacToeController.GetComponent<TicTacToeController>().SerializeGameState());
    }

    public bool HandleGameFinish()
    {
        // Check if the game is finished
        int winner = ticTacToeController.GetComponent<TicTacToeController>().CheckForWinner();
        bool isDraw = ticTacToeController.GetComponent<TicTacToeController>().isDraw();
        if (winner != PlayerType.EMPTY || isDraw)
        {
            Debug.Log("The winner is: " + winner);
            ChangeGameState(GameStates.GameOver);

            if (isDraw)
            {
                gameOverText.GetComponent<Text>().text = "Draw!";
            }
            else
            {
                gameOverText.GetComponent<Text>().text =
                    winner == ticTacToeController.GetComponent<TicTacToeController>().myPlayerType ?
                    "You won!" : "You lost!";
            }
            return true;
        }
        return false;
    }

    public void TerminateGame()
    {
        if (currentGameState == GameStates.WaitingTicTacToe || currentGameState == GameStates.PlayingTicTacToe)
        {
            ChangeGameState(GameStates.GameOver);
        }

        gameOverText.GetComponent<Text>().text = "Game Session Ended.";
    }

    private void FindGameSessionButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AddToGameSessionQueue + "");
        ChangeGameState(GameStates.WaitingForMatch);
    }

    public void HandlePlayerMessage(string message)
    {
        chatManager.GetComponent<ChatManager>().AddChatMessage(message);
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
        ticTacToeController.SetActive(false);
        gameOverText.SetActive(false);
        backToMainMenuButton.SetActive(false);
        chatManager.SetActive(false);

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
            // Reset everything while waiting
            chatManager.GetComponent<ChatManager>().ClearMessages();
            chatManager.GetComponent<ChatManager>().myPlayerName = currentUsername;
            ticTacToeController.GetComponent<TicTacToeController>().ResetBoard();
        }
        else if (newState == GameStates.PlayingTicTacToe)
        {
            ticTacToeController.SetActive(true);
            placeholderGameButton.SetActive(true);
            chatManager.SetActive(true);
            ticTacToeController.GetComponent<TicTacToeController>().isMyTurn = true;
        }
        else if (newState == GameStates.WaitingTicTacToe)
        {
            ticTacToeController.SetActive(true);
            chatManager.SetActive(true);
            ticTacToeController.GetComponent<TicTacToeController>().isMyTurn = false;
        }
        else if (newState == GameStates.GameOver)
        {
            ticTacToeController.SetActive(true);
            chatManager.SetActive(true);
            ticTacToeController.GetComponent<TicTacToeController>().isMyTurn = false;
            gameOverText.SetActive(true);
            backToMainMenuButton.SetActive(true);
        }

        currentGameState = newState;
    }
}

public static class GameStates
{
    public const int Login = 1;
    public const int MainMenu = 2;
    public const int WaitingForMatch = 3;
    public const int PlayingTicTacToe = 4;
    public const int WaitingTicTacToe = 5;
    public const int GameOver = 6;
}