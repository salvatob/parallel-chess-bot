
using ChessBotCore;

TryPawnCaptures();
return;

var whiteKing = Bitboard.FromCoords(Coordinates.FromString("e1"));
var blackKing = Bitboard.FromCoords(Coordinates.FromString("e8"));
var blackRooks = Bitboard.FromCoords(Coordinates.FromString("e5"));


var state = State.Empty with { WhiteKing = whiteKing, BlackKing = blackKing, BlackRooks = blackRooks};

Console.WriteLine("----- Before -----");
Console.WriteLine(state.PrettyPrint());

var moves = GeneratorWrapper.Default.GetLegalMoves(state);


int c = 1;
foreach (Move m in moves) {
    Console.WriteLine($"Move : {c++}");
    Console.WriteLine(m.StateAfter.PrettyPrint());
    // moveMap |= m.StateAfter.WhiteKnights;
}

void TryPawnCaptures() {
    var whitePawn = Bitboard.FromCoords(Coordinates.FromString("c7"));
    var blackPawns = Bitboard.FromCoords(Coordinates.FromString("b8"));

    var state = State.Empty with { WhitePawns = whitePawn, BlackPawns = blackPawns};

    // var moves = ((PawnMoveGenerator)PawnMoveGenerator.Instance).GenerateCaptures(state);
    var moves = PawnMoveGenerator.Instance.GenerateMoves(state);

    Console.WriteLine("--- before ---");
    Console.WriteLine(state.PrettyPrint());

    int c = 1;
    foreach (var m in moves) {
        Console.WriteLine($"--- {c++} ---");
        Console.WriteLine(m.StateAfter.PrettyPrint());
    }
}