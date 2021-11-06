using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public GameObject chatPanel;
    public GameObject textField;
    public string myPlayerName = "null";

    GameObject messageSendButton;
    GameObject messageInputField;
    GameObject networkedClient;

    // Start is called before the first frame update
    void Start()
    {
        ClearMessages();
        AddChatMessage("Welcome to Multiplayer TicTacToe!");

        messageSendButton = GameObject.Find("MessageSendButton");
        messageInputField = GameObject.Find("MessageInputField");
        networkedClient = GameObject.Find("NetworkedClient");

        messageSendButton.GetComponent<Button>().onClick.AddListener(MessageSendButtonPressed);
    }

    void Awake()
    {
        ClearMessages();
        AddChatMessage("Welcome to Multiplayer TicTacToe!");

        messageSendButton = GameObject.Find("MessageSendButton");
        messageInputField = GameObject.Find("MessageInputField");
    }

    private void MessageSendButtonPressed()
    {
        string messageText = messageInputField.GetComponent<InputField>().text;
        if (messageText.Length == 0)
        {
            Debug.LogWarning("Message input field empty!");
            return;
        }

        messageText = myPlayerName + " says: " + messageText;
        AddChatMessage(messageText);
        messageInputField.GetComponent<InputField>().text = "";
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.PlayerMessage + "," + messageText);
    }

    public void ClearMessages()
    {
        int childCount = chatPanel.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(chatPanel.transform.GetChild(i).gameObject);
        }
    }

    public void AddChatMessage(string message)
    {
        GameObject sampleText = Instantiate(textField, chatPanel.transform);
        sampleText.GetComponent<Text>().text = message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


