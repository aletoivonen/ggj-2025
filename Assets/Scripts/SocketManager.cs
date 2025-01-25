using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;
using Random = UnityEngine.Random;

public class SocketManager : MonoSingleton<SocketManager>
{
    public int PlayerID { get; private set; } = -1;

    [SerializeField] private bool _useEditorAddress;
    [SerializeField] private string _editorAddress;
    [SerializeField] private string _address;
    private WebSocket _webSocket;

    [SerializeField] private bool _autoConnect = true;
    [SerializeField] private float _syncInterval = 0.3f;
    private float _timeSinceLastSync = 0.0f;

    [SerializeField] private SocketPlayer _playerPrefab;

    public Dictionary<int, SocketPlayer> _spawnedPlayers = new();

    private void Start()
    {
        if (_autoConnect)
        {
            Connect();
        }
    }

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

#if UNITY_EDITOR
        _webSocket = _useEditorAddress ? new WebSocket(_editorAddress) : new WebSocket(_address);
#else
        _webSocket = new WebSocket(_address);
#endif

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
            case "move": HandlePlayerMove(json); break;
        }
    }

    private void HandlePlayerMove(JObject json)
    {
        if (!json.TryGetValue("player", out JToken val))
        {
            Debug.LogError("no player id");
            return;
        }

        int playerId = val.ToObject<int>();

        if (playerId == SocketPlayer.LocalPlayer.PlayerId)
        {
            return; // don't move local player
        }

        if (!_spawnedPlayers.TryGetValue(playerId, out var sp))
        {
            Debug.LogError("No spawned player");
            return;
        }

        if (!json.TryGetValue("pos", out JToken p))
        {
            Debug.LogError("no pos");
            return;
        }

        SyncPos pos = JsonConvert.DeserializeObject<SyncPos>(p.ToString());

        sp.transform.position = new Vector2(pos.x, pos.y);
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
        Vector2 pos = SocketPlayer.LocalPlayer.transform.position;
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
        SocketPlayer.LocalPlayer.PlayerId = PlayerID;
        
        CheckSpawnedPlayers(json);

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

    private void CheckSpawnedPlayers(JObject json)
    {
        if (PlayerID < 0)
        {
            // Init not done yet, will duplicate local player
            return;
        }
        
        if (!json.TryGetValue("players", out JToken val))
        {
            Debug.LogError("cant parse players");
            return;
        }

        Dictionary<int, PlayerEntry> players = val.ToObject<Dictionary<int, PlayerEntry>>();

        foreach (var kvp in players)
        {
            var player = kvp.Value;

            if (player.id == PlayerID)
            {
                // local player
                continue;
            }

            Debug.Log("Other player found, name" + player.name);

            if (!_spawnedPlayers.ContainsKey(player.id))
            {
                SocketPlayer socketPlayer = Instantiate(_playerPrefab);
                socketPlayer.PlayerId = player.id;
                socketPlayer.SetIsLocalPlayer(false);
                _spawnedPlayers.Add(player.id, socketPlayer);
            }
        }

        // remove dc'd players
        foreach (var kvp in _spawnedPlayers)
        {
            if (players.All(p => p.Value.id != kvp.Value.PlayerId))
            {
                _spawnedPlayers.Remove(kvp.Key);
            }
        }
    }

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

    private void OnApplicationQuit()
    {
        if (_webSocket != null)
        {
            _webSocket.Close();
        }
    }

    [System.Serializable]
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

    [Serializable]
    private class SyncPos
    {
        public float x;
        public float y;
    }
}
