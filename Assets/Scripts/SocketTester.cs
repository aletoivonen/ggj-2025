using Cysharp.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using TMPro;
using UnityEngine.UI;

public class SocketTester : MonoBehaviour
{
    private WebSocket _webSocket;

    [SerializeField] private TMP_InputField _addressField;
    [SerializeField] private TMP_InputField _messageField;
    [SerializeField] private TextMeshProUGUI _responseField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Connect()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
            _webSocket = null;
        }

        _webSocket = new WebSocket("ws://" + _addressField.text);

        _webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        _webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        _webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        _webSocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            _responseField.text += message + "\n";
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        _webSocket.Connect();
    }

    public void SendSocketMessage()
    {
        SendWebSocketMessage();
    }
    
    public async UniTask SendWebSocketMessage()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            // Sending plain text
            await _webSocket.SendText(_messageField.text);
        }
    }

    private async void OnApplicationQuit()
    {
        await _webSocket.Close();
    }
}
