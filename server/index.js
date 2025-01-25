'use strict';

const express = require('express');
const path = require('path');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();
app.use(express.static(path.join(__dirname, '/public')));

const server = createServer(app);
const wss = new WebSocket.Server({ server });

const encodeInitMessage = require('./encoding/encoder').encodeInitMessage;
const encodeMoveMessage = require('./encoding/encoder').encodeMoveMessage;
const encodeSyncMessage = require('./encoding/encoder').encodeSyncMessage;
const encodeCreateBubbleMessage = require('./encoding/encoder').encodeCreateBubbleMessage;
const encodeRideBubbleMessage = require('./encoding/encoder').encodeRideBubbleMessage;

const decodePlayerUpdateData = require('./decoding/decoder').decodePlayerUpdateData;
const decodePlayerMoveData = require('./decoding/decoder').decodePlayerMoveData;
const decodeMessageType = require('./decoding/decoder').decodeMessageType;
const decodeCreateBubbleData = require('./decoding/decoder').decodeCreateBubbleData;
const decodeRideBubbleData = require('./decoding/decoder').decodeRideBubbleData;


const state = {
  players: {},
  playerSockets: {},
    
};

var nextPlayerId = 0;

// Broadcast function to send data to all connected clients
function broadcastToAllClients(data, ignorePlayerId = null) {
  var ignoreClient = null;

  if (ignorePlayerId != null) {
    ignoreClient = state.playerSockets[ignorePlayerId];
  }

  wss.clients.forEach((client) => {
    if (client.readyState === WebSocket.OPEN && client != ignoreClient) {
      client.send(data);
    } else if (client == ignoreClient) {
      console.log("ignore this client!")
    }
  });
}

var connections = [];

// Handle WebSocket connections
wss.on('connection', function (socket) {

  console.log(socket.send)

  const playerId = nextPlayerId++;
  console.log('New player connected.');
  // Add player with a timestamp
  state.players[playerId] = { Id: playerId, timestamp: Date.now() };
  state.playerSockets[playerId] = socket;
  // Notify all players about the updated state
  socket.send(encodeInitMessage(playerId, state.players));
  broadcastToAllClients(encodeSyncMessage(state.players));

  socket.on('message', (message) => handleMessage(message, socket));
  socket.on('close', (code, reason) => handleClose(code, reason, socket));
});

function handleMessage(message, socket) {
  try {
    let data;
    switch (decodeMessageType(message)) {
      case 'update':
        data = decodePlayerUpdateData(message);
        console.log(`Player update: ${data.id}`);
        
        if (state.players[data.id]) {
          // Update the player's data and timestamp
          state.players[data.id] = {
            ...state.players[data.id],
            ...data,
            timestamp: Date.now()
          };
          // Notify all players about the updated state
          broadcastToAllClients(encodeSyncMessage(state.players), data.id);
        } else {
          console.warn(`Player ${data.id} not found for update.`);
        }
        break;

      case 'move':
        data = decodePlayerMoveData(message);
        if (state.players[data.id]) {
          broadcastToAllClients(encodeMoveMessage(data.id, data.position));
        } else {
          console.warn(`Player ${data.id} not found for update.`);
        }
        break;

      case 'create_bubble':
        data = decodeCreateBubbleData(message);
        if (state.players[data.id]) {
          broadcastToAllClients(encodeCreateBubbleMessage(data.playerId, data.bubbleId, data.position));
        } else {
          console.warn(`Player ${data.id} not found for update.`);
        }
        break;
        
      case 'ride_bubble':
        data = decodeRideBubbleData(message);
        if (state.players[data.id]) {
          broadcastToAllClients(encodeRideBubbleMessage(data.playerId, data.bubbleId));
        } else {
          console.warn(`Player ${data.id} not found for update.`);
        }
        break;

      default:
        console.warn('Unknown message type:', data.type);
    }
  } catch (error) {
    console.error('Error processing message:', error);
  }
}

function handleClose(code, reason, connection) {
  console.log(`WebSocket closed with code: ${code}, reason: ${reason}`);

  const leftId = getKeyByValue(state.playerSockets, connection)

  if (leftId != null) {
    const data = state.players[leftId];
    console.log(`Player exited: ${data.id}`);
    delete state.players[data.id];
    delete state.playerSockets[data.id];
  } else {
    console.log("didnt find player who exited")
  }

  var index = connections.indexOf(connection);
  if (index !== -1) {
    // remove the connection from the pool
    connections.splice(index, 1);
  }

  broadcastToAllClients(encodeSyncMessage(state.players), leftId);
}

function getKeyByValue(object, value) {
  return Object.keys(object).find(key => object[key] === value);
}

server.listen(8080, function () {
  console.log('Server is running on http://0.0.0.0:8080');
});
