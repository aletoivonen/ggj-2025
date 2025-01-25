function encodeUInt(buffer, playerId, offset) {
    buffer.writeUInt32LE(playerId, offset); // 4 bytes for player ID
    return offset + 4;
}

function encodeColor(buffer, color, offset) {
    if (color == undefined) {
        color = {
            r: 1,
            g: 1,
            b: 1
        }
    }

    buffer.writeUInt32LE(color.r, offset);
    buffer.writeUInt32LE(color.r, offset + 1);
    buffer.writeUInt32LE(color.r, offset + 2);
    return offset + 3;
}

function encodeVector2(buffer, vector, offset) {
    buffer.writeFloatLE(vector.x || 0, offset); // X position (4 bytes)
    offset += 4;
    buffer.writeFloatLE(vector.y || 0, offset); // Y position (4 bytes)
    return offset + 4;
}

function encodeMoveMessage(playerId, pos) {
    const buffer = Buffer.alloc(1 + 4 + 8); // 1 byte (type) + 4 bytes (playerId) + 8 bytes (position)

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(3, offset); // Type = 3 (move)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write position (2 floats: x and y, 4 bytes each)
    offset = encodeVector2(buffer, pos, offset);

    return buffer;
}

function encodeSyncMessage(players) {
    const playerEntries = Object.values(players);

    // Calculate buffer size: 1 byte (type) + 4 bytes (number of players) + 13 bytes per player
    const buffer = Buffer.alloc(1 + 4 + playerEntries.length * 15);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(2, offset); // Type = 2 (sync)
    offset += 1;

    // Write number of players (4 bytes)
    buffer.writeUInt32LE(playerEntries.length, offset);
    console.log("sync entries lenght " + playerEntries.length);
    offset += 4;

    // Write player data (15 bytes per player)
    playerEntries.forEach((player) => {
        offset = encodeUInt(buffer, player.id, offset);
        offset = encodeColor(buffer, player.color, offset);
        offset = encodeVector2(buffer, { x: player.x, y: player.y }, offset);
    });

    console.log(buffer.length);
    return buffer;
}

function encodeInitMessage(playerId, players) {
    const playerEntries = Object.values(players);
    // Calculate buffer size: 1 byte (type) + 4 bytes (playerId) + 4 bytes (number of players) + 13 bytes per player
    const buffer = Buffer.alloc(1 + 4 + 4 + playerEntries.length * 15);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(1, offset); // Type = 1 (init)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write number of players (4 bytes)
    buffer.writeUInt32LE(playerEntries.length, offset);
    console.log("init entries lenght " + playerEntries.length);
    offset += 4;

    // Write player data (15 bytes per player)
    playerEntries.forEach((player) => {
        offset = encodeUInt(buffer, player.id, offset);
        offset = encodeColor(buffer, player.color, offset);
        offset = encodeVector2(buffer, { x: player.x, y: player.y }, offset);
    });

    return buffer;
}

function encodeCreateBubbleMessage(playerId, bubbleId, position) {
    const buffer = Buffer.alloc(1 + 4 + 4 + 8)

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(5, offset); // Type = 5 (create bubble)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write bubble ID (4 bytes)
    offset = encodeUInt(buffer, bubbleId, offset);

    // Write position (2 floats: x and y, 4 bytes each)
    offset = encodeVector2(buffer, position, offset);
}

function encodeRideBubbleMessage(playerId, bubbleId) {
    const buffer = Buffer.alloc(1 + 4 + 4);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(5, offset); // Type = 6 (ride bubble)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write bubble ID (4 bytes)
    offset = encodeUInt(buffer, bubbleId, offset);
}

exports.encodeInitMessage = encodeInitMessage;
exports.encodeSyncMessage = encodeSyncMessage;
exports.encodeMoveMessage = encodeMoveMessage;
exports.encodeCreateBubbleMessage = encodeCreateBubbleMessage;
exports.encodeRideBubbleMessage = encodeRideBubbleMessage;
