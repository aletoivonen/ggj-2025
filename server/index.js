'use strict';

const express = require('express');
const path = require('path');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();
app.use(express.static(path.join(__dirname, '/public')));

const server = createServer(app);
const wss = new WebSocket.Server({ server });

const state = {
  players: {},
};

var nextPlayerId = 0;

// Broadcast function to send data to all connected clients
function broadcastToAllClients(data) {
  const message = JSON.stringify(data);
  wss.clients.forEach((client) => {
    if (client.readyState === WebSocket.OPEN) {
      client.send(message);
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
  state.players[playerId] = { timestamp: Date.now() };
  // Notify all players about the updated state
  broadcastToAllClients({ type: 'sync', players: state.players });

  socket.send(JSON.stringify({
    type: "init",
    playerId: playerId
  }))

  socket.on('message', (message) => handleMessage(message, socket));
  socket.on('close', (code, reason) => handleClose(code, reason, socket));
});

function handleMessage(message, socket) {
  try {
    const data = JSON.parse(message);

    switch (data.type) {

      case 'update':
        console.log(`Player update: ${data.id}`);
        if (state.players[data.id]) {
          // Update the player's data and timestamp
          state.players[data.id] = {
            ...state.players[data.id],
            ...data,
            timestamp: Date.now()
          };
          // Notify all players about the updated state
          broadcastToAllClients({ type: 'sync', players: state.players });
        } else {
          console.warn(`Player ${data.id} not found for update.`);
        }
        break;

      case 'exit':
        console.log(`Player exited: ${data.id}`);
        delete state.players[data.id];
        // Notify all players about the updated state
        broadcastToAllClients({ type: 'sync', players: state.players });
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

  var index = connections.indexOf(connection);
  if (index !== -1) {
    // remove the connection from the pool
    connections.splice(index, 1);
  }
}

server.listen(8080, function () {
  console.log('Server is running on http://0.0.0.0:8080');
});
