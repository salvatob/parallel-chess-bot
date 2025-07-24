
using ChessBotCore;

var rooksStr = 
    """
    0000 0000
    0000 0000
    0000 0000
    0000 0000
    
    0000 0000
    0001 0000
    0000 0000
    0000 0000
    """;

var allyPiecesStr = 
    """
    0000 0000
    0000 0000
    0001 0000
    0000 0000
    
    0000 0000
    0000 0000
    0000 0000
    0000 0000
    """;

var enemyPiecesStr =
    """
    0000 0000
    0000 0000
    0000 0000
    0000 0000
    0000 0000
    0000 0100
    0000 0000
    0000 0000
    """;


//act
var rooks = Bitboard.Parse(rooksStr);
var enemyPawns = Bitboard.Parse(enemyPiecesStr);
var allyPawns = Bitboard.Parse(allyPiecesStr);
var state = State.Empty with { WhiteKnights = rooks, WhitePawns = allyPawns, BlackPawns = enemyPawns};


var moves = KnightMoveGenerator.Instance.GenerateMoves(state);

var moveMap = Bitboard.Empty;

Console.WriteLine("rooks");
Console.WriteLine(rooks.PrettyPrint());

foreach (Move m in moves) {
    // Console.WriteLine(m.StateAfter.WhiteKnights);
    moveMap |= m.StateAfter.WhiteKnights;
}

Console.WriteLine(moveMap.PrettyPrint());
