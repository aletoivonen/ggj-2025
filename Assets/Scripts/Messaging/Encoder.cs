using System;
using UnityEngine;

public static class Encoder
{
    private static void EncodeColor(ref byte[] bytes, Color color, int offset)
    {
        bytes[offset] = (byte)(color.g * 255);
        bytes[offset + 1] = (byte)(color.g * 255);
        bytes[offset + 2] = (byte)(color.b * 255);
    }
    
    private static void EncodeUInt(ref byte[] bytes, uint playerId, int offset)
    {
        byte[] idBytes = BitConverter.GetBytes(playerId);
        Array.Copy(idBytes, 0, bytes, offset, idBytes.Length);
    }
    
    private static void EncodeVector2(ref byte[] bytes, Vector2 vector, int offset)
    {
        byte[] xBytes = BitConverter.GetBytes(vector.x);
        byte[] yBytes = BitConverter.GetBytes(vector.y);
        Array.Copy(xBytes, 0, bytes, offset, xBytes.Length);
        Array.Copy(yBytes, 0, bytes, offset + xBytes.Length, yBytes.Length);
    }
    
    public static byte[] EncodeUpdateData(PlayerUpdateData data)
    {
        byte[] buffer = new byte[1 + 4 + 3];
        buffer[0] = (byte)MessageType.Update;
        EncodeUInt(ref buffer, data.PlayerId, 1);
        EncodeColor(ref buffer, data.Color, 5);
        return buffer;
    }
    
    public static byte[] EncodeMoveData(PlayerMoveData data)
    {
        byte[] buffer = new byte[1 + 4 + 8];
        buffer[0] = (byte)MessageType.Move;
        EncodeUInt(ref buffer, data.PlayerId, 1);
        EncodeVector2(ref buffer, data.Position, 5);
        return buffer;
    }
    
    public static byte[] EncodeCreateBubbleData(CreateBubbleData data)
    {
        byte[] buffer = new byte[1 + 4 + 4 + 8];
        buffer[0] = (byte)MessageType.CreateBubble;
        EncodeUInt(ref buffer, data.PlayerId, 1);
        EncodeUInt(ref buffer, data.PlayerId, 5);
        EncodeVector2(ref buffer, data.Position, 9);
        return buffer;
    }
    
    public static byte[] EncodeStartRideBubble(StartRideBubbleData data)
    {
        byte[] buffer = new byte[1 + 4 + 4];
        buffer[0] = (byte)MessageType.RideBubble;
        EncodeUInt(ref buffer, data.PlayerId, 1);
        EncodeUInt(ref buffer, data.PlayerId, 5);
        return buffer;
    }
}

