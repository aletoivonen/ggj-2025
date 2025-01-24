using UnityEngine;
using NativeWebSocket;

public class SocketManager : MonoSingleton<SocketManager>
{
    [SerializeField] private string _address;

    private WebSocket _webSocket;

    public void Connect()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
            _webSocket = null;
        }

        if (!_address.StartsWith("ws://"))
        {
            _address = "ws://" + _address;
        }

        _webSocket = new WebSocket(_address);

        _webSocket.OnOpen += OnOpen;
        _webSocket.OnError += OnError;
        _webSocket.OnClose += OnClose;

        _webSocket.OnMessage += HandleMessage;

        _webSocket.Connect();
    }

    private void HandleMessage(byte[] bytes)
    {
        var message = System.Text.Encoding.UTF8.GetString(bytes);
    }

    private void OnClose(WebSocketCloseCode closecode)
    {
        Debug.Log("Socket closed: " + closecode.ToString());
    }

    private void OnOpen()
    {
        Debug.Log("Connection open!");
    }

    private void OnError(string e)
    {
        Debug.LogError("Error in Socket: " + e);
    }
}
