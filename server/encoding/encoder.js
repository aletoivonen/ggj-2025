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
    offset += 4;

    // Write player data (15 bytes per player)
    playerEntries.forEach((player) => {
        offset = encodeUInt(buffer, player.id, offset);
        offset = encodeColor(buffer, player.color, offset);
        offset = encodeVector2(buffer, { x: player.x, y: player.y }, offset);
    });

    return buffer;
}

function encodeInitMessage(playerId, players, scores) {
    const scoreEntries = Object.values(scores);
    const playerEntries = Object.values(players);
    // Calculate buffer size: 1 byte (type) + 4 bytes (playerId) + 4 bytes (number of players) + 13 bytes per player
    const buffer = Buffer.alloc(1 + 4 + 4 + playerEntries.length * 15 + 4 + scoreEntries.length * 8);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(1, offset); // Type = 1 (init)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write number of players (4 bytes)
    buffer.writeUInt32LE(playerEntries.length, offset);
    offset += 4;

    // Write player data (15 bytes per player)
    playerEntries.forEach((player) => {
        offset = encodeUInt(buffer, player.id, offset);
        offset = encodeColor(buffer, player.color, offset);
        offset = encodeVector2(buffer, { x: player.x, y: player.y }, offset);
    });
    
    // Write number of player scores (4 bytes)
    buffer.writeUInt32LE(scoreEntries.length, offset);
    offset += 4;

    // Write player score data (8 bytes per player)
    scoreEntries.forEach((score) => {
        offset = encodeUInt(buffer, score.playerId, offset);
        offset = encodeUInt(buffer, score.score, offset);
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

    return buffer;
}

function encodeRideBubbleMessage(playerId, bubbleId) {
    const buffer = Buffer.alloc(1 + 4 + 4);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(6, offset); // Type = 6 (ride bubble)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write bubble ID (4 bytes)
    offset = encodeUInt(buffer, bubbleId, offset);

    return buffer;
}

function encodeUpdateScoreMessage(playerId, score) {
    const buffer = Buffer.alloc(1 + 4 + 4);

    let offset = 0;

    // Write type (1 byte)
    buffer.writeUInt8(7, offset); // Type = 7 (update scores)
    offset += 1;

    // Write player ID (4 bytes)
    offset = encodeUInt(buffer, playerId, offset);

    // Write score (4 bytes)
    offset = encodeUInt(buffer, score, offset);

    return buffer;
}

exports.encodeInitMessage = encodeInitMessage;
exports.encodeSyncMessage = encodeSyncMessage;
exports.encodeMoveMessage = encodeMoveMessage;
exports.encodeCreateBubbleMessage = encodeCreateBubbleMessage;
exports.encodeRideBubbleMessage = encodeRideBubbleMessage;
exports.encodeUpdateScoreMessage = encodeUpdateScoreMessage;
