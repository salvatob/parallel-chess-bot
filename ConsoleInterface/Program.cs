// See https://aka.ms/new-console-template for more information

using ChessBotCore;

var pawnsStr =
"""
0000 1000
0000 1000
0010 0000
0000 0000
0000 0010
0000 0100
0111 0110
0000 0000
""";


var pawns = Bitboard.Parse(pawnsStr);

foreach (var move in PawnMoveGenerator.OneCellForward(pawns, ~pawns)) {
    Console.WriteLine(move.Print());
    Console.WriteLine("--------");
}