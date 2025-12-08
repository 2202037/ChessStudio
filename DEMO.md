# Chess Studio Demo Script

This script demonstrates the basic functionality of Chess Studio.

## Quick Start Demo

### 1. Play a Quick Chess Game
```bash
cd ChessStudio.UI
dotnet run
# Select option 1
# Enter player names: Alice and Bob
# Play moves: e2-e4, e7-e5, g1-f3, b8-c6
# Type 'quit' to end
```

### 2. Register Players
```bash
dotnet run
# Select option 3 (Player Management)
# Select option 1 (Register New Player)
# Enter player details
```

### 3. Create a Tournament
```bash
dotnet run
# Select option 2 (Tournament Management)
# Select option 1 (Create New Tournament)
# Enter tournament name: "Winter Championship"
# Select format: 1 (Swiss)
# Enter number of rounds: 5
# Register players when prompted
```

### 4. Start a Tournament Round
```bash
dotnet run
# Select option 2 (Tournament Management)
# Select option 3 (Start Tournament Round)
# Enter the Tournament ID
# View the generated pairings
```

### 5. View Standings
```bash
dotnet run
# Select option 2 (Tournament Management)
# Select option 5 (View Standings)
# Enter the Tournament ID
# See the leaderboard with scores
```

## Sample Test Sequence

Create a file `demo_input.txt` with the following content:
```
3
1
Alice
1500
alice@example.com
3
1
Bob
1400
bob@example.com
4
5
```

Then run:
```bash
cat demo_input.txt | dotnet run
```

This will:
1. Register player "Alice" with rating 1500
2. Register player "Bob" with rating 1400
3. View statistics
4. Exit

## Chess Move Notation

Moves use algebraic notation in the format: `source-destination`

Examples:
- `e2-e4` - Move pawn from e2 to e4
- `g1-f3` - Move knight from g1 to f3
- `e1-g1` - Kingside castling (king moves two squares)
- `e1-c1` - Queenside castling (king moves two squares)

## Database

The SQLite database `chessstudio.db` is created automatically in the application directory.

You can inspect it using:
```bash
sqlite3 chessstudio.db
.tables
SELECT * FROM Players;
.exit
```

## Building from Source

```bash
# Clone the repository
git clone https://github.com/2202037/ChessStudio.git
cd ChessStudio

# Build
dotnet build

# Run
cd ChessStudio.UI
dotnet run
```

## Project Structure

```
ChessStudio/
├── ChessStudio.Core/      # Chess engine
├── ChessStudio.Data/      # Database layer
├── ChessStudio.UI/        # Console interface
└── ChessStudio.sln        # Solution file
```

## Features Demonstrated

✅ Chess move validation
✅ Check/Checkmate/Stalemate detection
✅ PGN game saving
✅ Player registration
✅ Tournament creation
✅ Swiss pairing system
✅ Score tracking
✅ Leaderboard generation
