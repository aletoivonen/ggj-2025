using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using Zubble;
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
    private List<SocketPlayer> _tmpPlayers = new List<SocketPlayer>();

    [SerializeField] private SocketPlayer _playerPrefab;

    public Dictionary<int, SocketPlayer> _spawnedPlayers = new();

    protected override void OnAwake()
    {
        base.OnAwake();

        PlayerMoveController.OnLocalPlayerBubble += OnLocalPlayerBubble;
    }

    private void OnDestroy()
    {
        PlayerMoveController.OnLocalPlayerBubble -= OnLocalPlayerBubble;
    }

    private void OnLocalPlayerBubble(float duration)
    {
        //TODO send bubble message
    }

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
        if (_timeSinceLastSync > _syncInterval && PlayerID >= 0)
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
        MessageType type = Decoder.DecodeMessageType(bytes);
        switch (type)
        {
            case MessageType.Init:
                HandlePlayerInit(bytes);
                break;
            case MessageType.Sync:
                HandleSyncPlayer(bytes);
                break;
            case MessageType.Move:
                HandlePlayerMove(bytes);
                break;
            case MessageType.Update: default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandlePlayerMove(byte[] bytes)
    {
        PlayerMoveData data = Decoder.DecodePlayerMoveData(bytes);
        int playerId = (int)data.PlayerId;

        if (playerId == SocketPlayer.LocalPlayer.PlayerId)
        {
            return; // don't move local player
        }

        if (!_spawnedPlayers.TryGetValue(playerId, out var sp))
        {
            Debug.LogError("No spawned player");
            return;
        }
        
        sp.transform.position = data.Position;
    }

    private void SendPlayerProfile()
    {
        Color color = Color.white;
        color.r = Random.Range(0f, 1f);
        color.g = Random.Range(0f, 1f);
        color.b = Random.Range(0f, 1f);
        
        Debug.Log(PlayerID);
        
        _webSocket.Send(Encoder.EncodeUpdateData(new PlayerUpdateData
        {
            PlayerId = (uint)PlayerID,
            Color = color
        }));
    }

    private void SendPlayerSyncMessage()
    {
        _webSocket.Send(Encoder.EncodeMoveData(new PlayerMoveData
        {
            PlayerId = (uint)PlayerID,
            Position = SocketPlayer.LocalPlayer.transform.position
        }));
    }

    private void HandlePlayerInit(byte[] bytes)
    {
        PlayerInitData data = Decoder.DecodePlayerInitData(bytes);
        PlayerID = (int)data.PlayerId;
        SocketPlayer.LocalPlayer.PlayerId = PlayerID;
        CheckSpawnedPlayers(data.Players);

        SendPlayerProfile();
    }

    private void HandleGameEvent(byte[] bytes)
    {
        Debug.Log("Game event: " + bytes);
    }

    private void HandleExitPlayer(byte[] bytes)
    {
        CheckSpawnedPlayers(Decoder.DecodePlayerSyncData(bytes).Players);
    }

    private void HandleSyncPlayer(byte[] bytes)
    {
        CheckSpawnedPlayers(Decoder.DecodePlayerSyncData(bytes).Players);
    }

    private void CheckSpawnedPlayers(PlayerData[] players)
    {
        if (PlayerID < 0)
        {
            // Init not done yet, will duplicate local player
            return;
        }
        
        foreach (PlayerData player in players)
        {
            if ((int)player.Id == PlayerID)
            {
                // local player
                continue;
            }
            
            Debug.Log("Other player found, name" + player.Id);
            if (!_spawnedPlayers.ContainsKey((int)player.Id))
            {
                SocketPlayer socketPlayer = Instantiate(_playerPrefab);
                socketPlayer.PlayerId = (int)player.Id;
                socketPlayer.SetIsLocalPlayer(false);
                _spawnedPlayers.Add((int)player.Id, socketPlayer);
            }
        }

        // remove dc'd players
        _tmpPlayers.Clear();
        foreach (var kvp in _spawnedPlayers)
        {
            if (players.All(p => (int)p.Id != kvp.Value.PlayerId))
            {
                _tmpPlayers.Add(kvp.Value);
            }
        }
        
        foreach (var player in _tmpPlayers)
        {
            _spawnedPlayers.Remove(player.PlayerId);
            Destroy(player.gameObject);
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
