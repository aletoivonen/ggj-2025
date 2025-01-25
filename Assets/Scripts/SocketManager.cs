using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SocketManager : MonoSingleton<SocketManager>
{
    public int PlayerID { get; private set; }

    [SerializeField] private string _address;
    private WebSocket _webSocket;

    [SerializeField] private float _syncInterval = 0.3f;
    private float _timeSinceLastSync = 0.0f;

    [SerializeField] private GameObject _playerPrefab;

    public List<GameObject> _spawnedPlayers = new();

    [ContextMenu("Connect")]
    public void Connect()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
            _webSocket = null;
        }

        if (!_address.StartsWith("wss://") && !_address.StartsWith("ws://"))
        {
            Debug.LogError("Not ws:// address");
        }

        _webSocket = new WebSocket(_address);

        _webSocket.OnOpen += OnOpen;
        _webSocket.OnError += OnError;
        _webSocket.OnClose += OnClose;

        _webSocket.OnMessage += HandleMessage;

        _webSocket.Connect();
    }

    void Update()
    {
        if (_webSocket == null)
        {
            return;
        }

        _timeSinceLastSync += Time.deltaTime;
        if (_timeSinceLastSync > _syncInterval)
        {
            _timeSinceLastSync = 0.0f;

            SendPlayerSyncMessage();
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        _webSocket.DispatchMessageQueue();
#endif
    }

    private void HandleMessage(byte[] bytes)
    {
        var message = System.Text.Encoding.UTF8.GetString(bytes);

        JObject json = JObject.Parse(message);

        if (!json.TryGetValue("type", out JToken val))
        {
            return;
        }

        switch (val.ToString())
        {
            case "init": HandlePlayerInit(json); break;
            case "sync": HandleSyncPlayer(json); break;
            case "exit": HandleExitPlayer(json); break;
            case "event": HandleGameEvent(json); break;
        }
    }

    private void SendPlayerProfile()
    {
        JObject msg = GetBaseMessage();
        msg["type"] = "update";
        msg["name"] = "Player " + PlayerID;

        Color c = Color.white;
        c.r = Random.Range(0f, 1f);
        c.g = Random.Range(0f, 1f);
        c.b = Random.Range(0f, 1f);

        msg["color"] = ColorUtility.ToHtmlStringRGB(c);

        _webSocket.SendText(JsonConvert.SerializeObject(msg));
    }

    private void SendPlayerSyncMessage()
    {
        var msg = GetBaseMessage();
        Vector2 pos = TempSocketPlayer.LocalPlayer.transform.position;
        msg["type"] = "move";
        msg["pos"] = "{\"x\":" + pos.x + ", \"y\":" + pos.y + "}";

        _webSocket.SendText(JsonConvert.SerializeObject(msg));
    }

    private void HandlePlayerInit(JObject json)
    {
        if (!json.TryGetValue("playerId", out JToken val))
        {
            Debug.LogError("Couldn't ready playerID from init message!");
            return;
        }

        PlayerID = val.ToObject<int>();

        SendPlayerProfile();
    }

    private void HandleGameEvent(JObject json)
    {
        Debug.Log("Game event: " + JsonUtility.ToJson(json));
    }

    private void HandleExitPlayer(JObject json)
    {
        CheckSpawnedPlayers(json);
    }

    private void HandleSyncPlayer(JObject json)
    {
        CheckSpawnedPlayers(json);
    }

    private void CheckSpawnedPlayers(JObject json) { }

    private JObject GetBaseMessage()
    {
        JObject baseMessage = new JObject();
        baseMessage["id"] = PlayerID;
        return baseMessage;
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

    private class PlayerEntry
    {
        public int id;
        public string name;
        public string color;

        public Color UnityColor()
        {
            if (ColorUtility.TryParseHtmlString(color, out var c))
            {
                return c;
            }

            return Color.white;
        }
    }
}
