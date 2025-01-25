function encodePlayerId(buffer, playerId, offset) {
    buffer.writeUInt32BE(playerId, offset); // 4 bytes for player ID
    return offset + 4;
}

function encodeColor(buffer, color, offset) {
    buffer.write(color, offset, 'hex'); // 3 bytes for player color
    return offset + 3;
}

function encodeVector2(buffer, vector, offset) {
    buffer.writeFloatBE(vector.x || 0, offset); // X position (4 bytes)
    offset += 4;
    buffer.writeFloatBE(vector.y || 0, offset); // Y position (4 bytes)
    return offset + 4;
}

function encodeMoveMessage(playerId, pos) {
    const buffer = Buffer.alloc(1 + 4 + 8); // 1 byte (type) + 4 bytes (playerId) + 8 bytes (position)

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(3, offset); // Type = 3 (move)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodePlayerId(buffer, playerId, offset);

    // Write position (2 floats: x and y, 4 bytes each)
    offset = encodeVector2(buffer, pos, offset);

    return buffer;
}

function encodeSyncMessage(players) {
    const playerEntries = Object.values(players);

    // Calculate buffer size: 1 byte (type) + 4 bytes (number of players) + 13 bytes per player
    const buffer = Buffer.alloc(1 + 4 + playerEntries.length * 13);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(2, offset); // Type = 2 (sync)
    offset += 1;

    // Write number of players (4 bytes)
    buffer.writeUInt32BE(playerEntries.length, offset);
    offset += 4;

    // Write player data (13 bytes per player)
    playerEntries.forEach((player) => {
        offset = encodePlayerId(buffer, player.id, offset);
        offset = encodeColor(buffer, player.color, offset);
        offset = encodeVector2(buffer, { x: player.x, y: player.y }, offset);
    });

    return buffer;
}

function encodeInitMessage(playerId, players) {
    // Calculate buffer size: 1 byte (type) + 4 bytes (playerId) + 4 bytes (number of players) + 13 bytes per player
    const buffer = Buffer.alloc(1 + 4 + 4 + players.length * 13);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(1, offset); // Type = 1 (init)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodePlayerId(buffer, playerId, offset);

    // Write number of players (4 bytes)
    buffer.writeUInt32LE(players.length, offset);
    offset += 4;

    // Write player data (13 bytes per player)
    players.forEach((player) => {
        offset = encodePlayerId(buffer, player.id, offset);
        offset = encodeColor(buffer, player.color, offset);
        offset = encodeVector2(buffer, { x: player.x, y: player.y }, offset);
    });

    return buffer;
}

exports.encodeInitMessage = encodeInitMessage;
exports.encodeSyncMessage = encodeSyncMessage;
exports.encodeMoveMessage = encodeMoveMessage;
