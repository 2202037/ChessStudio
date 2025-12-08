#!/bin/bash

# Chess Studio Verification Test Script
# This script tests the basic functionality of the Chess Studio application

echo "========================================"
echo "Chess Studio Verification Test"
echo "========================================"
echo

# Test 1: Build the project
echo "Test 1: Building the project..."
cd /home/runner/work/ChessStudio/ChessStudio
dotnet build --verbosity quiet
if [ $? -eq 0 ]; then
    echo "✓ Build successful"
else
    echo "✗ Build failed"
    exit 1
fi
echo

# Test 2: Verify project structure
echo "Test 2: Verifying project structure..."
if [ -f "ChessStudio.sln" ] && \
   [ -d "ChessStudio.Core" ] && \
   [ -d "ChessStudio.Data" ] && \
   [ -d "ChessStudio.UI" ]; then
    echo "✓ Project structure is correct"
else
    echo "✗ Project structure is incomplete"
    exit 1
fi
echo

# Test 3: Verify core classes exist
echo "Test 3: Verifying core classes..."
CORE_FILES=(
    "ChessStudio.Core/ChessBoard.cs"
    "ChessStudio.Core/Piece.cs"
    "ChessStudio.Core/Position.cs"
    "ChessStudio.Core/PGNHandler.cs"
    "ChessStudio.Core/Pieces/King.cs"
    "ChessStudio.Core/Pieces/Queen.cs"
    "ChessStudio.Core/Pieces/Rook.cs"
    "ChessStudio.Core/Pieces/Bishop.cs"
    "ChessStudio.Core/Pieces/Knight.cs"
    "ChessStudio.Core/Pieces/Pawn.cs"
    "ChessStudio.Core/Tournament/SwissPairing.cs"
)

ALL_EXIST=true
for file in "${CORE_FILES[@]}"; do
    if [ ! -f "$file" ]; then
        echo "✗ Missing file: $file"
        ALL_EXIST=false
    fi
done

if [ "$ALL_EXIST" = true ]; then
    echo "✓ All core classes exist"
else
    exit 1
fi
echo

# Test 4: Verify database models
echo "Test 4: Verifying database models..."
DATA_FILES=(
    "ChessStudio.Data/Models/Player.cs"
    "ChessStudio.Data/Models/Tournament.cs"
    "ChessStudio.Data/Models/Match.cs"
    "ChessStudio.Data/Models/Round.cs"
    "ChessStudio.Data/ChessStudioContext.cs"
)

ALL_EXIST=true
for file in "${DATA_FILES[@]}"; do
    if [ ! -f "$file" ]; then
        echo "✗ Missing file: $file"
        ALL_EXIST=false
    fi
done

if [ "$ALL_EXIST" = true ]; then
    echo "✓ All database models exist"
else
    exit 1
fi
echo

# Test 5: Test application startup
echo "Test 5: Testing application startup..."
cd ChessStudio.UI
echo "5" | timeout 10 dotnet run > /tmp/test_output.txt 2>&1
if grep -q "Welcome to Chess Studio" /tmp/test_output.txt && \
   grep -q "Database initialized" /tmp/test_output.txt; then
    echo "✓ Application starts successfully"
else
    echo "✗ Application startup failed"
    cat /tmp/test_output.txt
    exit 1
fi
echo

# Test 6: Test player registration
echo "Test 6: Testing player registration..."
cat > /tmp/player_test.txt << 'EOF'
3
1
TestPlayer
1500
test@example.com
4
5
EOF

cat /tmp/player_test.txt | timeout 10 dotnet run > /tmp/test_output.txt 2>&1
if grep -q "registered successfully" /tmp/test_output.txt; then
    echo "✓ Player registration works"
else
    echo "✗ Player registration failed"
    exit 1
fi
echo

# Test 7: Verify documentation
echo "Test 7: Verifying documentation..."
cd ..
if [ -f "README.md" ] && [ -f "DEMO.md" ]; then
    if grep -q "Chess Studio" README.md && \
       grep -q "Features" README.md && \
       grep -q "Installation" README.md; then
        echo "✓ Documentation is complete"
    else
        echo "✗ Documentation is incomplete"
        exit 1
    fi
else
    echo "✗ Documentation files missing"
    exit 1
fi
echo

# Summary
echo "========================================"
echo "All verification tests passed! ✓"
echo "========================================"
echo
echo "Chess Studio is ready to use!"
echo
echo "To run the application:"
echo "  cd ChessStudio.UI"
echo "  dotnet run"
