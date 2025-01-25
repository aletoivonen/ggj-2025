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
    }
}

function decodeColor(buffer, offset) {
    const r = buffer.readUInt8(offset);
    const g = buffer.readUInt8(offset + 1);
    const b = buffer.readUInt8(offset + 2);
    return { r, g, b };
}

// Helper function to decode player ID (4 bytes for uint)
function decodePlayerId(buffer, offset) {
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
    const messageType = buffer.readUInt8(0); // Message type at byte 0

    // Check if the message is of type 'Update'
    if (messageType !== 1) { // Assuming 1 represents 'Update' in MessageType enum
        throw new Error('Invalid message type');
    }

    // Decode player ID (4 bytes starting at offset 1)
    const playerId = decodePlayerId(buffer, 1);

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
    const messageType = buffer.readUInt8(0); // Message type at byte 0

    // Check if the message is of type 'Move'
    if (messageType !== 2) { // Assuming 2 represents 'Move' in MessageType enum
        throw new Error('Invalid message type');
    }

    // Decode player ID (4 bytes starting at offset 1)
    const playerId = decodePlayerId(buffer, 1);

    // Decode position (8 bytes for Vector2 starting at offset 5)
    const position = decodeVector2(buffer, 5);

    // Return the decoded data in a structured format
    return {
        id: playerId,
        position: position
    };
}

exports.decodeMessageType = decodeMessageType;
exports.decodePlayerUpdateData = decodePlayerUpdateData;
exports.decodePlayerMoveData = decodePlayerMoveData;
