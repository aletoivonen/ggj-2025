using System;
using UnityEngine;

public static class Decoder
{
    private static PlayerData[] DecodePlayers(byte[] bytes, int offset, out int count)
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
     
        count = 4 + 15 * players.Length;
        return players;
    }
    
    private static PlayerScoreData[] DecodePlayerScores(byte[] bytes, int offset)
    {
        uint playerCount = BitConverter.ToUInt32(bytes, offset);
        PlayerScoreData[] scores = new PlayerScoreData[playerCount];
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = new PlayerScoreData
            {
                PlayerId = BitConverter.ToUInt32(bytes, offset + 4 + i * 8),
                Score = BitConverter.ToUInt32(bytes, offset + 8 + i * 8),
            };
        }
        
        return scores;
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
            Players = DecodePlayers(bytes, 5, out int playerByteCount),
            Scores = DecodePlayerScores(bytes, 5 + playerByteCount)
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
        return new PlayerSyncData()
        {
            Players = DecodePlayers(bytes, 1, out _)
        };
    }
    
    public static PlayerScoreData DecodePlayerScoreData(byte[] bytes)
    {
        return new PlayerScoreData()
        {
            PlayerId = BitConverter.ToUInt32(bytes, 1),
            Score = BitConverter.ToUInt32(bytes, 5)
        };
    }
}

