using System.Text.RegularExpressions;
using ChessBotCore.Board;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Parser;

namespace ChessBotCore;

/// <summary>
/// Specifies special characteristics of a chess move.
/// </summary>
[Flags]
public enum MoveFlags : ushort {
    /// <summary> The move involves a pawn promotion. </summary>
    Promotion = 1 << 15,
    /// <summary> The move is a capture. </summary>
    Capture = 1 << 14,
    /// <summary> The move results in a check. </summary>
    IsCheck = 1 << 13,
    /// <summary> The move is a double pawn push from the starting rank. </summary>
    DoublePawnPush = 1 << 12,
    /// <summary> The move is a kingside castle. </summary>
    KingCastle = 1 << 11,
    /// <summary> The move is a queenside castle. </summary>
    QueenCastle = 1 << 10,
    /// <summary> The move is an en passant capture. </summary>
    EnPassant = 1 << 9,
    /// <summary> Pawn promotes to a Queen. </summary>
    PromoteToQueen = 1 << 8,
    /// <summary> Pawn promotes to a Rook. </summary>
    PromoteToRook = 1 << 7,
    /// <summary> Pawn promotes to a Bishop. </summary>
    PromoteToBishop = 1 << 6,
    /// <summary> Pawn promotes to a Knight. </summary>
    PromoteToKnight = 1 << 5,
    /// <summary> No special flags. </summary>
    None = 0
}

/// <summary>
/// Represents a single chess move, packed into a 32-bit integer for efficiency.
/// </summary>
public readonly struct Move : IComparable<Move>, IEquatable<Move> {
    // first 6 bits is from, next 6 is to, next 4 is Pieces, another 16 are flags
    private readonly uint _data;

    static bool GetColor(Pieces p) {
        return (byte)p > 6;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Move"/> struct.
    /// </summary>
    /// <param name="from">The starting square index (0-63).</param>
    /// <param name="to">The destination square index (0-63).</param>
    /// <param name="flags">Special flags for the move.</param>
    public Move(int from, int to, MoveFlags flags = MoveFlags.None) {
        _data = (uint)(from | (to << 6)) | ((uint)flags << 16);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Move"/> struct including the piece being moved.
    /// </summary>
    /// <param name="from">The starting square index (0-63).</param>
    /// <param name="to">The destination square index (0-63).</param>
    /// <param name="piece">The piece that is moving.</param>
    /// <param name="flags">Special flags for the move.</param>
    public Move(int from, int to, Pieces piece, MoveFlags flags = MoveFlags.None) {
        _data = (uint)(from | (to << 6) | ((byte)piece << 12)) | ((uint)flags << 16);
    }

    /// <summary> The starting square index (0-63). </summary>
    public int From => (int)(_data & 0x3F);
    /// <summary> The destination square index (0-63). </summary>
    public int To => (int)((_data >> 6) & 0x3F);
    /// <summary> The flags associated with this move. </summary>
    public MoveFlags Flags => (MoveFlags)(_data >> 16);

    /// <summary>
    /// Compares this move to another move for sorting. 
    /// Higher priority moves (like captures) come first.
    /// </summary>
    public int CompareTo(Move other) => other._data.CompareTo(_data);

    /// <summary> Gets a value indicating whether this move is a capture. </summary>
    public bool IsCapture => Flags.HasFlag(MoveFlags.Capture);
    /// <summary> Gets a value indicating whether this move is a pawn promotion. </summary>
    public bool IsPromotion => Flags.HasFlag(MoveFlags.Promotion);
    /// <summary> Gets a value indicating whether this move is a castle. </summary>
    public bool IsCastle => Flags.HasFlag(MoveFlags.KingCastle) || Flags.HasFlag(MoveFlags.QueenCastle);
    /// <summary> Gets a value indicating whether this move is an en passant capture. </summary>
    public bool IsEnPassant => Flags.HasFlag(MoveFlags.EnPassant);
    /// <summary> Gets a value indicating whether this move results in a check. </summary>
    public bool IsCheck => Flags.HasFlag(MoveFlags.IsCheck);
    /// <summary> Gets the piece that is moving. </summary>
    public Pieces Piece => (Pieces)((_data >> 12) & 0b1111); // take only 4 bits ideally
    /// <summary> Gets a value indicating whether the piece belongs to White. </summary>
    public bool IsWhite => GetColor(Piece);

    /// <summary>
    /// Tries to get the algebraic notation for the move given the state before and after.
    /// </summary>
    public static string? TryGetNotation(State before, State after) {
        return FenCreator.TryGetMoveNotation(before, after);
    }

    private string GetPromotionNotation() {
        return IsPromotion
            ? (
                Flags.HasFlag(MoveFlags.PromoteToQueen) ? "q" :
                Flags.HasFlag(MoveFlags.PromoteToRook) ? "r" :
                Flags.HasFlag(MoveFlags.PromoteToBishop) ? "b" : "n"
            )
            : "";
    }

    /// <summary>
    /// Compares the two moves full data, including metadata, like if it is capture, or what promotion it is.
    /// </summary>
    /// <param name="other">The move to compare against.</param>
    /// <returns>If they are equal.</returns>
    public bool Equals(Move other) {
        return _data == other._data;
    }

    public override string ToString() {
        return $"{Piece}-{PrintLAN()} {Flags}";
    }

    /// <summary>
    /// Prints the Move in Long Algebraic Notation.
    /// </summary>
    /// <returns>The move as a string.</returns>
    // ReSharper disable once InconsistentNaming
    public string PrintLAN() {
        return $"{Coordinates.From1D(From)}{Coordinates.From1D(To)}{GetPromotionNotation()}";
    }

    /// <summary>
    /// Finds a full <see cref="Move"/> object that matches a <see cref="MoveDTO"/> in the given state.
    /// </summary>
    /// <param name="moveDto">The move DTO to match.</param>
    /// <param name="state">The state in which the move is played.</param>
    /// <returns>The matching full Move object.</returns>
    /// <exception cref="ArgumentException">Thrown if no legal move matches the DTO.</exception>
    public static Move FindFullMove(MoveDTO moveDto, State state) {
        var generator = new GeneratorWrapper();
        var moves = generator.GenerateMoves(state).GetLegalMoves();

        // just try to find a move that matches the move passed in 
        foreach (var m in moves)
            if (m.From == moveDto.From &&
                m.To == moveDto.To &&
                moveDto.Promotion switch {
                    'q' => m.Flags.HasFlag(MoveFlags.PromoteToQueen),
                    'r' => m.Flags.HasFlag(MoveFlags.PromoteToRook),
                    'k' => m.Flags.HasFlag(MoveFlags.PromoteToKnight),
                    'b' => m.Flags.HasFlag(MoveFlags.PromoteToBishop),
                    _ => true
                }
               )
                return m;

        throw new ArgumentException($"Invalid move <{moveDto}> has been passed to {nameof(FindFullMove)}().");
    }
}


/// <summary>
/// A Data Transfer Object for simple move representation, typically used for parsing input.
/// </summary>
/// <param name="From">The starting square index (0-63).</param>
/// <param name="To">The destination square index (0-63).</param>
/// <param name="Promotion">The promotion piece symbol ('q', 'r', 'b', 'n'), if any.</param>
// ReSharper disable once InconsistentNaming
public record MoveDTO(int From, int To, char? Promotion) {
    /// <summary>
    /// Parses a move in LAN notation.
    /// </summary>
    /// <param name="move">The string to parse.</param>
    /// <returns>A move DTO abject</returns>
    /// <exception cref="ArgumentException">If the move string is in an incorrect format</exception>
    public static MoveDTO Parse(string move) {
        string moveRegex = "([a-h][1-8])([a-h][1-8])([qrbn]?)";
        var match = Regex.Match(move, moveRegex);

        if (move.Length is not (4 or 5) || !match.Success)
            throw new ArgumentException($"Invalid move notation: <{move}>");

        var fromS = match.Groups[1].Value;
        var toS = match.Groups[2].Value;

        char? promotionS = match.Groups[3].Length > 0 ? match.Groups[3].Value[0] : null;

        var from = Coordinates.FromString(fromS).To1D();
        var to = Coordinates.FromString(toS).To1D();

        return new MoveDTO(from, to, promotionS);
    }

    public override string ToString() {
        return Coordinates.From1D(From).ToString() + Coordinates.From1D(To).ToString()
            + (Promotion.HasValue ? Promotion.Value.ToString() : "");
    }
}
