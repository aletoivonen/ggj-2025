
# WebSocket API

Example Message Formats:

Player Joins:

```
{
  "type": "join",
  "id": "player1",
  "name": "Alice",
  "score": 0,
  "position": { "x": 0, "y": 0 }
}
```

After processing, the server stores and broadcasts:

```
{
  "type": "sync",
  "players": {
    "player1": {
      "id": "player1",
      "name": "Alice",
      "score": 0,
      "position": { "x": 0, "y": 0 },
      "timestamp": 1674290042000
    }
  }
}
```

Player Updates:

Client sends:

```
{
  "type": "update",
  "id": "player1",
  "position": { "x": 10, "y": 5 }
}
```

After processing, the server stores and broadcasts:

```
{
  "type": "sync",
  "players": {
    "player1": {
      "id": "player1",
      "name": "Alice",
      "score": 0,
      "position": { "x": 10, "y": 5 },
      "timestamp": 1674290150000
    }
  }
}
```

Player Exits:

Client sends:

```
{
  "type": "exit",
  "id": "player1"
}
```

After processing, the server broadcasts:

```
{
  "type": "sync",
  "players": {}
}
```
