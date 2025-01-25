using System;
using UnityEngine;

public static class Decoder
{
    private static PlayerData[] DecodePlayers(byte[] bytes, int offset)
    {
        uint playerCount = BitConverter.ToUInt32(bytes, offset);
        PlayerData[] players = new PlayerData[playerCount];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new PlayerData
            {
                Id = BitConverter.ToUInt32(bytes, offset + 4 + i * 15),
                Color = DecodeColor(bytes, offset + 8 + i * 15),
                Position = new Vector2(BitConverter.ToSingle(bytes, offset + 11 + i * 15), BitConverter.ToSingle(bytes, offset + 15 + i * 15))
            };
        }
        
        return players;
    }
    
    private static Color DecodeColor(byte[] bytes, int offset)
    {
        return new Color(
            Mathf.InverseLerp(0, 255, bytes[offset]),
            Mathf.InverseLerp(0, 255, bytes[offset]),
            Mathf.InverseLerp(0, 255, bytes[offset]),
            1f
        );
    }
    
    public static MessageType DecodeMessageType(byte[] bytes)
    {
        return (MessageType)bytes[0];
    }
    
    public static PlayerInitData DecodePlayerInitData(byte[] bytes)
    {
        return new PlayerInitData
        {
            PlayerId = BitConverter.ToUInt32(bytes, 1),
            Players = DecodePlayers(bytes, 5)
        };
    }
    
    public static PlayerMoveData DecodePlayerMoveData(byte[] bytes)
    {
        return new PlayerMoveData
        {
            PlayerId = BitConverter.ToUInt32(bytes, 1),
            Position = new Vector2(
                BitConverter.ToSingle(bytes, 5),
                BitConverter.ToSingle(bytes, 9)
            )
        };
    }
    
    public static CreateBubbleData DecodeCreateBubbleData(byte[] bytes)
    {
        return new CreateBubbleData()
        {
            PlayerId = BitConverter.ToUInt32(bytes, 1),
            BubbleId = BitConverter.ToUInt32(bytes, 5),
            Position = new Vector2(
                BitConverter.ToSingle(bytes, 9),
                BitConverter.ToSingle(bytes, 13)
            )
        };
    }
    
    public static StartRideBubbleData DecodeStartRideBubbleData(byte[] bytes)
    {
        return new StartRideBubbleData()
        {
            PlayerId = BitConverter.ToUInt32(bytes, 1),
            BubbleId = BitConverter.ToUInt32(bytes, 5),
        };
    }
    
    public static PlayerSyncData DecodePlayerSyncData(byte[] bytes)
    {
        Debug.Log("sync data " + bytes.Length);
        return new PlayerSyncData()
        {
            Players = DecodePlayers(bytes, 1)
        };
    }
}

