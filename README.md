# Chess Studio

A comprehensive C# and .NET desktop application for chess gameplay and tournament management system.

## Project Overview

Chess Studio is developed as part of the **CIT-222 Information System Analysis and Design Sessional** course at **Patuakhali Science and Technology University**. The system provides an interactive environment to conduct chess tournaments efficiently, allowing users to register players, generate tournament rounds, validate moves, store games, display standings, and manage tournament progress.

## Features

### Chess Gameplay Module
- ✅ Interactive chessboard with move validation
- ✅ Legal move validation including:
  - Castling (kingside and queenside)
  - En passant captures
  - Pawn promotion (auto-promotes to Queen)
- ✅ Check, checkmate, and stalemate detection
- ✅ Player vs Player (Local) mode
- ✅ FEN (Forsyth-Edwards Notation) export
- ✅ PGN (Portable Game Notation) storage

### Tournament Management Module
- ✅ Player registration and database storage
- ✅ Automated Swiss pairing system
- ✅ Round generation based on results
- ✅ PGN-based game data storage
- ✅ Score updating (1 point for win, 0.5 for draw, 0 for loss)
- ✅ Standings and leaderboard generation
- ✅ Support for multiple tournament formats:
  - Swiss System
  - Knockout
  - Round Robin
- ✅ Tournament history tracking

## System Architecture

### Project Structure
```
ChessStudio/
├── ChessStudio.Core/          # Core chess engine and game logic
│   ├── Pieces/                # Chess piece implementations
│   │   ├── King.cs
│   │   ├── Queen.cs
│   │   ├── Rook.cs
│   │   ├── Bishop.cs
│   │   ├── Knight.cs
│   │   └── Pawn.cs
│   ├── Tournament/            # Tournament management logic
│   │   └── SwissPairing.cs   # Swiss system pairing algorithm
│   ├── ChessBoard.cs         # Board representation and game state
│   ├── Piece.cs              # Abstract piece base class
│   ├── Position.cs           # Position representation
│   ├── PGNHandler.cs         # PGN file handling
│   ├── PieceColor.cs         # Color enumeration
│   └── PieceType.cs          # Piece type enumeration
├── ChessStudio.Data/         # Database layer with Entity Framework
│   ├── Models/               # Database models
│   │   ├── Player.cs
│   │   ├── Tournament.cs
│   │   ├── TournamentPlayer.cs
│   │   ├── Round.cs
│   │   └── Match.cs
│   └── ChessStudioContext.cs # EF Core DbContext
└── ChessStudio.UI/           # User interface (Console application)
    └── Program.cs            # Main application entry point
```

### Database Schema

**Players Table**
- PlayerId (PK)
- Name
- Rating
- Contact
- RegistrationDate

**Tournaments Table**
- TournamentId (PK)
- Name
- Format (Swiss/Knockout/RoundRobin)
- Status (Pending/InProgress/Completed)
- StartDate
- EndDate
- NumberOfRounds

**TournamentPlayers Table**
- TournamentPlayerId (PK)
- TournamentId (FK)
- PlayerId (FK)
- Score
- Wins
- Losses
- Draws

**Rounds Table**
- RoundId (PK)
- TournamentId (FK)
- RoundNumber
- StartTime
- IsCompleted

**Matches Table**
- MatchId (PK)
- RoundId (FK)
- WhitePlayerId (FK)
- BlackPlayerId (FK)
- Result (Pending/WhiteWin/BlackWin/Draw)
- PGN
- StartTime
- EndTime

## Technology Stack

- **Language:** C# 12
- **Framework:** .NET 8.0
- **Database:** SQLite with Entity Framework Core 8.0
- **UI:** Console Application (cross-platform compatible)

## System Requirements

- .NET 8.0 SDK or later
- Windows, Linux, or macOS

## Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/2202037/ChessStudio.git
cd ChessStudio
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Run the Application
```bash
cd ChessStudio.UI
dotnet run
```

The database will be automatically created on first run as `chessstudio.db` in the application directory.

## Usage Guide

### Main Menu Options

1. **Play Chess (Local Game)**
   - Enter names for White and Black players
   - Make moves using algebraic notation (e.g., `e2-e4`)
   - The game validates all moves automatically
   - Detects check, checkmate, and stalemate
   - Option to save game as PGN after completion

2. **Tournament Management**
   - Create new tournaments
   - Start tournament rounds with automatic pairings
   - Record match results
   - View real-time standings

3. **Player Management**
   - Register new players with ratings
   - View all registered players
   - Update player ratings

4. **View Statistics**
   - System-wide statistics
   - Total players, tournaments, and matches
   - Top-rated players

### Example Chess Game

```
Enter move: e2-e4
Enter move: e7-e5
Enter move: g1-f3
Enter move: b8-c6
...
```

### Creating a Tournament

1. Select "Tournament Management" from main menu
2. Choose "Create New Tournament"
3. Enter tournament details:
   - Name: "University Championship 2024"
   - Format: Swiss (option 1)
   - Number of rounds: 5
4. Register players to the tournament
5. Start rounds and record results

## Chess Rules Implemented

- ✅ All standard chess piece movements
- ✅ Castling (both kingside and queenside)
- ✅ En passant pawn capture
- ✅ Pawn promotion (automatically to Queen)
- ✅ Check detection
- ✅ Checkmate detection
- ✅ Stalemate detection
- ✅ 50-move rule for draws

## Tournament Pairing System

The Swiss pairing algorithm:
1. Sorts players by score (highest first)
2. Pairs players with similar scores
3. Avoids pairing players who have already played each other
4. Alternates colors based on round number
5. Assigns byes to odd players if needed

## File Formats

### PGN (Portable Game Notation)
Games are saved in standard PGN format with headers:
```
[Event "Casual Game"]
[Site "Chess Studio"]
[Date "2024.12.08"]
[Round "-"]
[White "Player 1"]
[Black "Player 2"]
[Result "1-0"]

1. e2-e4 e7-e5 2. g1-f3 b8-c6 ...
1-0
```

### FEN (Forsyth-Edwards Notation)
Board positions can be exported to FEN format for analysis.

## Development Team

| ID      | Name               |
| ------- | ------------------ |
| 2202037 | Shahriar Ahmed     |
| 2202024 | Ezaz Mahmud        |
| 2202054 | Pollob Chakraborty |

## Course Information

- **Course Code:** CIT-222
- **Course Title:** Information System Analysis and Design Sessional
- **Level:** II, Semester II
- **Institution:** Patuakhali Science and Technology University

## Supervisors

- **Prof. Golam Md. Muradul Bashir**  
  Professor, Department of Computer and Communication Engineering

- **Muhammad Muhtasim**  
  Lecturer, Department of Computer Science and Information Technology

## Future Enhancements

Potential features for future versions:
- 🔄 Online multiplayer functionality
- 🤖 AI opponent with different difficulty levels
- 📊 Advanced game analysis and statistics
- 🎨 Graphical UI with WPF or Avalonia
- ⏱️ Chess clock integration
- 🏆 ELO rating calculation system
- 📱 Mobile application support
- 🌐 Web-based interface

## Contributing

This is an academic project. For suggestions or improvements, please contact the development team.

## License

This project is developed for educational purposes as part of university coursework.

## Acknowledgments

- Patuakhali Science and Technology University
- Department of Computer Science and Information Technology
- Course instructors and supervisors
- Standard chess rules from FIDE (World Chess Federation)

## Contact

For questions or feedback about this project:
- Email: Contact through university channels
- Project Repository: https://github.com/2202037/ChessStudio

---

**Built with ❤️ by the Chess Studio Development Team**