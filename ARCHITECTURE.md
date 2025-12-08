# Chess Studio - System Architecture Documentation

## System Overview

Chess Studio is a comprehensive chess gameplay and tournament management system built with C# and .NET 8.0.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     Chess Studio System                      │
└─────────────────────────────────────────────────────────────┘
                              │
              ┌───────────────┼───────────────┐
              │               │               │
              ▼               ▼               ▼
    ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
    │ ChessStudio  │ │ ChessStudio  │ │ ChessStudio  │
    │     .UI      │ │    .Core     │ │    .Data     │
    │  (Console)   │ │ (Game Logic) │ │ (Database)   │
    └──────────────┘ └──────────────┘ └──────────────┘
           │                 │               │
           │                 │               │
           └────────┬────────┴───────┬───────┘
                    │                │
                    ▼                ▼
              ┌──────────┐    ┌──────────┐
              │   User   │    │ SQLite   │
              │Interface │    │ Database │
              └──────────┘    └──────────┘
```

## Module Breakdown

### 1. ChessStudio.Core (Game Engine)
```
ChessStudio.Core/
├── Pieces/
│   ├── King.cs          - King movement and castling
│   ├── Queen.cs         - Queen movement (rook + bishop)
│   ├── Rook.cs          - Horizontal/vertical movement
│   ├── Bishop.cs        - Diagonal movement
│   ├── Knight.cs        - L-shaped movement
│   └── Pawn.cs          - Forward movement, en passant
├── Tournament/
│   └── SwissPairing.cs  - Swiss system algorithm
├── ChessBoard.cs        - Board state and game logic
├── Piece.cs             - Abstract base class
├── Position.cs          - Coordinate system
├── PGNHandler.cs        - Game notation handling
├── PieceColor.cs        - White/Black enumeration
└── PieceType.cs         - Piece type enumeration
```

**Responsibilities:**
- Move validation
- Game state management
- Check/Checkmate/Stalemate detection
- Tournament pairing logic
- PGN generation

### 2. ChessStudio.Data (Database Layer)
```
ChessStudio.Data/
├── Models/
│   ├── Player.cs           - Player information
│   ├── Tournament.cs       - Tournament metadata
│   ├── TournamentPlayer.cs - Player-Tournament link
│   ├── Round.cs            - Tournament rounds
│   └── Match.cs            - Individual matches
└── ChessStudioContext.cs   - EF Core DbContext
```

**Responsibilities:**
- Data persistence
- Entity relationships
- Database migrations
- Query optimization

### 3. ChessStudio.UI (User Interface)
```
ChessStudio.UI/
└── Program.cs - Main application with menus
```

**Responsibilities:**
- User interaction
- Menu navigation
- Input validation
- Display formatting

## Data Flow

### Chess Game Flow
```
User Input → Position Validation → Move Validation → 
Check Detection → Execute Move → Update Board → 
Check Game State → Display Board → Next Turn
```

### Tournament Flow
```
Create Tournament → Register Players → 
Generate Pairings (Swiss) → Play Matches → 
Record Results → Update Scores → 
Generate Next Round → Display Standings
```

## Database Schema

```
┌─────────────┐
│   Players   │
├─────────────┤
│ PlayerId PK │
│ Name        │
│ Rating      │
│ Contact     │
└─────────────┘
       │
       │ 1:N
       ▼
┌──────────────────┐
│ TournamentPlayer │
├──────────────────┤
│ TournamentPlayerId PK │
│ TournamentId FK  │
│ PlayerId FK      │
│ Score            │
│ Wins/Losses/Draws│
└──────────────────┘
       │
       │ N:1
       ▼
┌──────────────┐       ┌─────────┐
│ Tournaments  │ 1:N   │ Rounds  │
├──────────────┤◄──────┤─────────┤
│TournamentId PK│       │RoundId PK│
│ Name         │       │RoundNumber│
│ Format       │       │StartTime│
│ Status       │       └─────────┘
└──────────────┘           │
                           │ 1:N
                           ▼
                       ┌─────────┐
                       │ Matches │
                       ├─────────┤
                       │ MatchId PK │
                       │ WhitePlayerId FK │
                       │ BlackPlayerId FK │
                       │ Result  │
                       │ PGN     │
                       └─────────┘
```

## Key Algorithms

### 1. Move Validation Algorithm
```
1. Check if piece exists at source position
2. Check if piece belongs to current player
3. Check if move is in piece's possible moves
4. Check if move would leave king in check
5. Execute move if valid
6. Update game state
```

### 2. Swiss Pairing Algorithm
```
1. Sort players by score (descending)
2. For each unpaired player:
   a. Find highest-rated opponent not played yet
   b. Alternate colors based on round
   c. Create pairing
3. Give bye to odd player (if needed)
4. Return pairings
```

### 3. Check Detection
```
1. Find king position for color
2. For each opponent piece:
   a. Get possible moves
   b. Check if king position is in moves
3. Return true if any piece can capture king
```

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Language | C# | 12 |
| Framework | .NET | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| Database | SQLite | 3.x |
| UI | Console Application | - |

## Design Patterns Used

1. **Abstract Factory Pattern** - Piece creation
2. **Strategy Pattern** - Different piece movement strategies
3. **Repository Pattern** - Database access through EF Core
4. **Singleton Pattern** - Database context
5. **State Pattern** - Game state management

## Features Implementation Status

| Feature | Status | Module |
|---------|--------|--------|
| Chess Move Validation | ✅ Complete | Core |
| Castling | ✅ Complete | Core |
| En Passant | ✅ Complete | Core |
| Pawn Promotion | ✅ Complete | Core |
| Check Detection | ✅ Complete | Core |
| Checkmate Detection | ✅ Complete | Core |
| Stalemate Detection | ✅ Complete | Core |
| PGN Export | ✅ Complete | Core |
| Player Registration | ✅ Complete | Data |
| Tournament Creation | ✅ Complete | Data |
| Swiss Pairing | ✅ Complete | Core |
| Round Generation | ✅ Complete | Data |
| Score Tracking | ✅ Complete | Data |
| Leaderboard | ✅ Complete | UI |

## Future Enhancements

1. **AI Opponent**
   - Minimax algorithm
   - Alpha-beta pruning
   - Position evaluation

2. **Graphical UI**
   - WPF/Avalonia interface
   - Drag-and-drop pieces
   - 3D board option

3. **Online Multiplayer**
   - WebSocket communication
   - Matchmaking system
   - Chat functionality

4. **Advanced Analytics**
   - Opening database
   - Game analysis
   - Performance statistics

5. **Mobile Support**
   - Cross-platform with MAUI
   - Touch controls
   - Cloud synchronization

## Performance Considerations

- **Move Validation**: O(1) to O(64) depending on piece type
- **Check Detection**: O(64) worst case (check all opponent pieces)
- **Swiss Pairing**: O(n²) where n is number of players
- **Database Queries**: Indexed on PlayerId, TournamentId

## Testing Strategy

1. **Unit Tests** - Individual piece movements
2. **Integration Tests** - Database operations
3. **System Tests** - Complete game scenarios
4. **Verification Script** - Automated validation

## Deployment

```bash
# Build release version
dotnet publish -c Release -o publish

# Run application
cd publish
./ChessStudio.UI
```

## Contributing Guidelines

1. Follow C# coding conventions
2. Document all public methods
3. Add unit tests for new features
4. Update README for significant changes

## License

Academic project - Patuakhali Science and Technology University

## Contact

Project Repository: https://github.com/2202037/ChessStudio
