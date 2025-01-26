// Helper function to decode color (3 bytes for RGB)

function decodeMessageType(buffer, offset) {
    const type = buffer.readUInt8(offset); // Read 1 byte for message type
    switch (type) {
        case 1:
            return "init";
        case 2:
            return "sync";
        case 3:
            return "move";
        case 4:
            return "update";
        case 5:
            return "create_bubble";
        case 6:
            return "ride_bubble";
        case 7:
            return "update_scores";
        case 8:
            return "text_data";
    }
}

function decodeColor(buffer, offset) {
    const r = buffer.readUInt8(offset);
    const g = buffer.readUInt8(offset + 1);
    const b = buffer.readUInt8(offset + 2);
    return { r, g, b };
}

// Helper function to decode player ID (4 bytes for uint)
function decodeUInt(buffer, offset) {
    return buffer.readUInt32LE(offset); // Assuming little-endian format for uint
}

// Helper function to decode Vector2 (8 bytes for two floats)
function decodeVector2(buffer, offset) {
    const x = buffer.readFloatLE(offset); // Assuming little-endian format for float
    const y = buffer.readFloatLE(offset + 4); // y starts after 4 bytes for x
    return { x, y };
}

// Function to decode PlayerUpdateData
function decodePlayerUpdateData(buffer) {
    // Decode player ID (4 bytes starting at offset 1)
    const playerId = decodeUInt(buffer, 1);

    // Decode color (3 bytes starting at offset 5)
    const color = decodeColor(buffer, 5);

    // Return the decoded data in a structured format
    return {
        id: playerId,
        color: color
    };
}

// Function to decode PlayerMoveData
function decodePlayerMoveData(buffer) {
    // Decode player ID (4 bytes starting at offset 1)
    const playerId = decodeUInt(buffer, 1);

    // Decode position (8 bytes for Vector2 starting at offset 5)
    const position = decodeVector2(buffer, 5);

    // Return the decoded data in a structured format
    return {
        id: playerId,
        position: position
    };
}

function decodeCreateBubbleData(buffer) {
    // Decode player ID (4 bytes starting at offset 1)
    const playerId = decodeUInt(buffer, 1);

    // Decode bubble ID (4 bytes starting at offset 1)
    const bubbleId = decodeUInt(buffer, 5);

    // Decode position (8 bytes for Vector2 starting at offset 5)
    const position = decodeVector2(buffer, 9);

    // Return the decoded data in a structured format
    return {
        pId: playerId,
        bId: bubbleId,
        pos: position
    };
}

function decodeRideBubbleData(buffer) {
    // Decode player ID (4 bytes starting at offset 1)
    const playerId = decodeUInt(buffer, 1);

    // Decode bubble ID (4 bytes starting at offset 1)
    const bubbleId = decodeUInt(buffer, 5);

    // Return the decoded data in a structured format
    return {
        pId: playerId,
        bId: bubbleId
    };
}

exports.decodeMessageType = decodeMessageType;
exports.decodePlayerUpdateData = decodePlayerUpdateData;
exports.decodePlayerMoveData = decodePlayerMoveData;
exports.decodeCreateBubbleData = decodeCreateBubbleData;
exports.decodeRideBubbleData = decodeRideBubbleData;
