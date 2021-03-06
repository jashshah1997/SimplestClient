using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    private int[] keyValues;
    private bool[] isKeyPressed;

    GameObject gameSystemManager;
    GameObject ticTacToeController;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "GameSystemManager")
                gameSystemManager = go;
            else if (go.name == "TicTacToeController")
                ticTacToeController = go;
        }
            Connect();

        keyValues = (int[])System.Enum.GetValues(typeof(KeyCode));
        isKeyPressed = new bool[keyValues.Length];
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < keyValues.Length; i++)
        {
            // Check if key is pressed
            bool check = Input.GetKey((KeyCode)keyValues[i]);

            // Detect rising edge for key press
            if (!isKeyPressed[i] && check)
            {
                SendMessageToHost(ClientToServerSignifiers.KeypressMetrics + "," + keyValues[i]);
                isKeyPressed[i] = true;
            }
            // Detect falling edge for key release
            else if (isKeyPressed[i] && !check)
            {
                isKeyPressed[i] = false;
            }
        }

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.1.51", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.LoginResponse)
        {
            int loginResultSignifier = int.Parse(csv[1]);

            if (loginResultSignifier == LoginResponses.Success)
            {
                gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.MainMenu);
            }
        }
        else if (signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            int playerTypeResponse = int.Parse(csv[1]);

            if (playerTypeResponse != SessionStartedResponses.CirclesPlayer && playerTypeResponse != SessionStartedResponses.CrossesPlayer)
            {
                Debug.LogWarning("Unknown player type received: " + playerTypeResponse);
                return;
            }

            ticTacToeController.GetComponent<TicTacToeController>().SetPlayerType(playerTypeResponse);

            // Player 'Circle' Goes first
            if (playerTypeResponse == SessionStartedResponses.CirclesPlayer)
            {
                gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.PlayingTicTacToe);
            } 
            else
            {
                gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.WaitingTicTacToe);
            }
        }
        else if (signifier == ServerToClientSignifiers.OpponentTicTacToePlay)
        {
            Debug.Log("our next item no longer beckons");

            // Update the game state
            string newGameState = csv[1];
            ticTacToeController.GetComponent<TicTacToeController>().SetGameState(newGameState);
            ticTacToeController.GetComponent<TicTacToeController>().RecordState();

            if (!gameSystemManager.GetComponent<GameSystemManager>().HandleGameFinish())
            {
                // Set the current player turn to active
                gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.PlayingTicTacToe);
            }
        }
        else if (signifier == ServerToClientSignifiers.SessionTerminated)
        {
            gameSystemManager.GetComponent<GameSystemManager>().TerminateGame();
        }
        else if (signifier == ServerToClientSignifiers.PlayerMessage)
        {
            gameSystemManager.GetComponent<GameSystemManager>().HandlePlayerMessage(msg.Substring(2));
        }
        else if (signifier == ServerToClientSignifiers.SessionIDResponse)
        {
            List<string> sessionIDs = new List<string>();
            for (int i = 1; i < csv.Length; i++)
            {
                if (csv[i].Length == 0)
                    continue;
                sessionIDs.Add(csv[i]);
            }
            gameSystemManager.GetComponent<GameSystemManager>().UpdateSessionIDs(sessionIDs);
        }
        else if (signifier == ServerToClientSignifiers.SpectateStarted)
        {
            string newGameState = csv[1];
            gameSystemManager.GetComponent<GameSystemManager>().ChangeGameState(GameStates.Spectator);
            ticTacToeController.GetComponent<TicTacToeController>().SetGameState(newGameState);
        }
        else if (signifier == ServerToClientSignifiers.SpectatorUpdate)
        {
            string newGameState = csv[1];
            ticTacToeController.GetComponent<TicTacToeController>().SetGameState(newGameState);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}

public static class ClientToServerSignifiers
{
    public const int Login = 1;
    public const int CreateAccount = 2;
    public const int AddToGameSessionQueue = 3;
    public const int TicTacToePlay = 4;
    public const int LeaveSession = 5;
    public const int PlayerMessage = 6;
    public const int RequestSessionIDs = 7;
    public const int SpectateSession = 8;
    public const int KeypressMetrics = 9;
}

public static class ServerToClientSignifiers
{
    public const int LoginResponse = 1;
    public const int GameSessionStarted = 2;
    public const int OpponentTicTacToePlay = 3;
    public const int SessionTerminated = 4;
    public const int PlayerMessage = 5;
    public const int SessionIDResponse = 6;
    public const int SpectateStarted = 7;
    public const int SpectatorUpdate = 8;
}

public static class SessionStartedResponses
{
    public const int CirclesPlayer = 1;
    public const int CrossesPlayer = 2;
}

public static class LoginResponses
{
    public const int Success = 1;
    public const int FailureNameInUse = 2;
    public const int FailureNameNotFound = 3;
    public const int FailureIncorrectPassword = 4;
}