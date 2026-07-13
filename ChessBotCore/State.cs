using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace ChessBotCore;

public enum Pieces {
    WhitePawns,
    WhiteRooks,
    WhiteKnights,
    WhiteBishops,
    WhiteQueens,
    WhiteKing,
    
    BlackPawns,
    BlackRooks,
    BlackKnights,
    BlackBishops,
    BlackQueens,
    BlackKing 
}

public sealed class State {
    public const string DefaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const char WhiteQueenSymbol = 'Q';
    public const char WhiteKingSymbol = 'K';
    public const char WhitePawnSymbol = 'P';
    public const char WhiteBishopSymbol = 'B';
    public const char WhiteKnightSymbol = 'N';
    public const char WhiteRookSymbol = 'R';

    public const char BlackQueenSymbol = 'q';
    public const char BlackKingSymbol = 'k';
    public const char BlackPawnSymbol = 'p';
    public const char BlackBishopSymbol = 'b';
    public const char BlackKnightSymbol = 'n';
    public const char BlackRookSymbol = 'r';

    public State(State other) {
        CopyFrom(other);
    }

    public void CopyFrom(State other) {
        WhitePawns = other.WhitePawns;
        WhiteRooks = other.WhiteRooks;
        WhiteKnights = other.WhiteKnights;
        WhiteBishops = other.WhiteBishops;
        WhiteQueens = other.WhiteQueens;
        WhiteKing = other.WhiteKing;

        BlackPawns = other.BlackPawns;
        BlackRooks = other.BlackRooks;
        BlackKnights = other.BlackKnights;
        BlackBishops = other.BlackBishops;
        BlackQueens = other.BlackQueens;
        BlackKing = other.BlackKing;

        WhiteIsActive = other.WhiteIsActive;

        WhiteCastleKingSide = other.WhiteCastleKingSide;
        WhiteCastleQueenSide = other.WhiteCastleQueenSide;
        BlackCastleKingSide = other.BlackCastleKingSide;
        BlackCastleQueenSide = other.BlackCastleQueenSide;

        EnPassant = other.EnPassant;

        HalfMovesSincePawnMoveOrCapture = other.HalfMovesSincePawnMoveOrCapture;
        FullMoves = other.FullMoves;
    }

    public State() {
        WhitePawns = 0;
        WhiteRooks = 0;
        WhiteKnights = 0;
        WhiteBishops = 0;
        WhiteQueens = 0;
        WhiteKing = 0;
        
        BlackPawns = 0;
        BlackRooks = 0;
        BlackKnights = 0;
        BlackBishops = 0;
        BlackQueens = 0;
        BlackKing = 0;
        
        WhiteIsActive = true;
        WhiteCastleKingSide = false;
        WhiteCastleQueenSide = false;
        BlackCastleKingSide = false;
        BlackCastleQueenSide = false;
        FullMoves = 1;
    }

    public Bitboard WhitePawns { get; set; }
    public Bitboard WhiteRooks { get; set; }
    public Bitboard WhiteKnights { get; set; }
    public Bitboard WhiteBishops { get; set; }
    public Bitboard WhiteQueens { get; set; }
    public Bitboard WhiteKing { get; set; }

    public Bitboard BlackPawns { get; set; }
    public Bitboard BlackRooks { get; set; }
    public Bitboard BlackKnights { get; set; }
    public Bitboard BlackBishops { get; set; }
    public Bitboard BlackQueens { get; set; }
    public Bitboard BlackKing { get; set; }

    public bool WhiteIsActive { get; set; } = true;

    public bool WhiteCastleKingSide { get; set; }
    public bool WhiteCastleQueenSide { get; set; }
    public bool BlackCastleKingSide { get; set; }
    public bool BlackCastleQueenSide { get; set; }

    public Bitboard EnPassant { get; set; } = 0;

    public int HalfMovesSincePawnMoveOrCapture { get; set; } = 0;
    public int FullMoves { get; set; } = 1;
    
    
    public static State Initial =>
        new() {
            WhitePawns   = 0b_1111_1111_0000_0000,
            WhiteRooks   = 0b_1000_0001,
            WhiteKnights = 0b_0100_0010,
            WhiteBishops = 0b0010_0100,
            WhiteQueens  = 0b0001_0000,
            WhiteKing    = 0b_0000_1000,
            
            BlackPawns   = 0b_1111_1111UL << (6 * 8),
            BlackRooks   = 0b_1000_0001UL << (7 * 8),
            BlackKnights = 0b_0100_0010UL << (7 * 8),
            BlackBishops = 0b_0010_0100UL << (7 * 8),
            BlackQueens  = 0b_0001_0000UL << (7 * 8),
            BlackKing    = 0b_0000_1000UL << (7 * 8),
            
            WhiteCastleKingSide  = true,
            WhiteCastleQueenSide = true,
            BlackCastleKingSide  = true,
            BlackCastleQueenSide = true,
            WhiteIsActive = true,
            FullMoves = 1
        };

    public static State Empty => new() {
        WhiteIsActive = true,
        FullMoves = 1
    };

    public static State FromFen(string fen) => FenParser.ParseFen(fen);
    public string GetFen() => FenCreator.GetFen(this);

    public static bool DetectActiveColor(string fen) {
        return FenParser.DetectActiveColor(fen);
    }
    
    public State Clone() {
        var clone = (State)MemberwiseClone();
        return clone;
    }
    
    /// <summary>
    /// Pushes the state into a next move, so some properties are updated automatically.
    /// <see cref="WhiteIsActive"/>, <see cref="EnPassant"/>, <see cref="FullMoves"/> clock and HalfMoves clock, are updated automatically.
    /// Pieces are left untouched, user should change them on their own.
    /// Specific properties such as castling and enpassant are also left to the user to handle.
    /// </summary>
    /// <returns>The same instance.</returns>
    public void Next() {
        WhiteIsActive = !WhiteIsActive;
        // update fullmove clock after blacks turn
        if (WhiteIsActive) FullMoves++;
        HalfMovesSincePawnMoveOrCapture += 1;
        EnPassant = default;
    }

    /// <summary>
    /// Resets the half-move counter.
    /// Should be used after executing a move with a capture, or a pawn advance.
    /// </summary>
    /// <returns></returns>
    private void HalfClockReset() {
        HalfMovesSincePawnMoveOrCapture = 0;
    }
    
    // TODO could shrink this struct down
    public readonly struct UndoInfo {
        public readonly Pieces? CapturedPiece;
        public readonly Bitboard EnPassant;
        public readonly bool WhiteCastleKingSide;
        public readonly bool WhiteCastleQueenSide;
        public readonly bool BlackCastleKingSide;
        public readonly bool BlackCastleQueenSide;
        public readonly int HalfMovesSincePawnMoveOrCapture;

        public UndoInfo(State state, Pieces? capturedPiece) {
            CapturedPiece = capturedPiece;
            EnPassant = state.EnPassant;
            WhiteCastleKingSide = state.WhiteCastleKingSide;
            WhiteCastleQueenSide = state.WhiteCastleQueenSide;
            BlackCastleKingSide = state.BlackCastleKingSide;
            BlackCastleQueenSide = state.BlackCastleQueenSide;
            HalfMovesSincePawnMoveOrCapture = state.HalfMovesSincePawnMoveOrCapture;
        }
    }

    public UndoInfo ApplyMove(Move move) {
        int from = move.From;
        int to = move.To;
        Bitboard fromMask = 1UL << from;
        Bitboard toMask = 1UL << to;

        Pieces movingPiece = move.Piece;
        
        Pieces? capturedPiece = move.IsEnPassant 
            ? (WhiteIsActive ? Pieces.BlackPawns : Pieces.WhitePawns)
            : DetectPieceCollision(toMask);

        UndoInfo undo = new UndoInfo(this, capturedPiece);

        // Handle regular move or capture
        ExecuteMove(movingPiece, fromMask, toMask, move);

        // Change turn and increment clocks
        Next();

        // Update metadata (this must happen after Next() so EnPassant isn't wiped)
        UpdateMetadata(movingPiece, move);

        return undo;
    }
    
    /// <summary>
    /// Applies a move that only contains two squares and a promotion. Mostly since the move came from outside this framework.
    /// For local move usage see the faster <see cref="ApplyMoveWithoutMetadata"/> method.
    /// </summary>
    /// <param name="move">The move to be applied</param>
    /// <exception cref="ArgumentException">Thrown if the move is not a valid move</exception>
    /// <returns>Metadata to undo the move</returns>
    public UndoInfo ApplyMoveWithoutMetadata(Move move) {
        var moves = new GeneratorWrapper(this).GetLegalMoves();
        
        // just try to find a move that matches the move passed in 
        foreach (var m in moves) {
            if (m.From == move.From &&
                m.To == move.To &&
                m.Flags.HasFlag(MoveFlags.PromoteToQueen) == move.Flags.HasFlag(MoveFlags.PromoteToQueen) &&
                m.Flags.HasFlag(MoveFlags.PromoteToRook) == move.Flags.HasFlag(MoveFlags.PromoteToRook) &&
                m.Flags.HasFlag(MoveFlags.PromoteToKnight) == move.Flags.HasFlag(MoveFlags.PromoteToKnight) &&
                m.Flags.HasFlag(MoveFlags.PromoteToBishop) == move.Flags.HasFlag(MoveFlags.PromoteToBishop) 
                ) {
                return ApplyMove(m);
            }
        }
        
        throw new ArgumentException("Invalid move {move} has been passed to ApplyMoveWithoutMetadata");
    }
    
    public void UndoMove(Move move, UndoInfo undo) {
        // 1. Reverse Turn
        ReverseNext();

        // 2. Move Piece Back
        int from = move.From;
        int to = move.To;
        Bitboard fromMask = 1UL << from;
        Bitboard toMask = 1UL << to;

        if (move.IsPromotion) {
            Pieces promotedPiece = GetPromotedPiece(move);
            Pieces pawnPiece = WhiteIsActive ? Pieces.WhitePawns : Pieces.BlackPawns;
            
            // Remove promoted piece from 'to'
            Set(promotedPiece, GetPieces(promotedPiece) & ~toMask);
            // Put pawn back to 'from'
            Set(pawnPiece, GetPieces(pawnPiece) | fromMask);
        } else {
            Pieces? movingPiece = DetectPieceCollision(toMask);
            if (movingPiece.HasValue) {
                MovePiece(movingPiece.Value, toMask, fromMask);
            }
        }

        // 3. Restore Captured Piece
        if (move.IsCapture && undo.CapturedPiece.HasValue) {
            if (move.IsEnPassant) {
                Bitboard capturedPawnMask = WhiteIsActive 
                    ? toMask.MovePieces(Direction.S) 
                    : toMask.MovePieces(Direction.N);
                Set(undo.CapturedPiece.Value, GetPieces(undo.CapturedPiece.Value) | capturedPawnMask);
            } else {
                Set(undo.CapturedPiece.Value, GetPieces(undo.CapturedPiece.Value) | toMask);
            }
        }

        // 4. Handle Castling (Move Rook Back)
        if (move.IsCastle) {
            UndoCastling(move);
        }

        // 5. Restore Metadata
        EnPassant = undo.EnPassant;
        WhiteCastleKingSide = undo.WhiteCastleKingSide;
        WhiteCastleQueenSide = undo.WhiteCastleQueenSide;
        BlackCastleKingSide = undo.BlackCastleKingSide;
        BlackCastleQueenSide = undo.BlackCastleQueenSide;
        HalfMovesSincePawnMoveOrCapture = undo.HalfMovesSincePawnMoveOrCapture;
    }

    private void ReverseNext() {
        if (WhiteIsActive) FullMoves--;
        WhiteIsActive = !WhiteIsActive;
    }

    private void UndoCastling(Move move) {
        if (WhiteIsActive) {
            if (move.Flags.HasFlag(MoveFlags.KingCastle)) {
                MovePiece(Pieces.WhiteRooks, 1UL << 2, 1UL << 0); // f1 to h1
            } else if (move.Flags.HasFlag(MoveFlags.QueenCastle)) {
                MovePiece(Pieces.WhiteRooks, 1UL << 4, 1UL << 7); // d1 to a1
            }
        } else {
            if (move.Flags.HasFlag(MoveFlags.KingCastle)) {
                MovePiece(Pieces.BlackRooks, 1UL << 58, 1UL << 56); // f8 to h8
            } else if (move.Flags.HasFlag(MoveFlags.QueenCastle)) {
                MovePiece(Pieces.BlackRooks, 1UL << 60, 1UL << 63); // d8 to a8
            }
        }
    }

    private void ExecuteMove(Pieces piece, Bitboard fromMask, Bitboard toMask, Move move) {
        // 1. Handle Captures
        if (move.IsCapture) {
            HandleCapture(toMask, move);
        }

        // 2. Handle Castling (King move is handled by ExecuteMove, but we need to move the Rook)
        if (move.IsCastle) {
            HandleCastling(move);
        }

        // 3. Move the piece
        if (move.IsPromotion) {
            HandlePromotion(piece, fromMask, toMask, move);
        } else {
            MovePiece(piece, fromMask, toMask);
        }
    }

    private void MovePiece(Pieces piece, Bitboard fromMask, Bitboard toMask) {
        Bitboard currentBoard = GetPieces(piece);
        Set(piece, (currentBoard & ~fromMask) | toMask);
    }

    private void HandleCapture(Bitboard toMask, Move move) {
        if (move.IsEnPassant) {
            // Captured pawn is on a different square
            // Note: WhiteIsActive is now the side that just moved if called before Next(), 
            // but in my new ApplyMove order, Next() is called before UpdateMetadata but AFTER ExecuteMove.
            // Wait, I should check the order in ApplyMove.
            
            // If ApplyMove calls ExecuteMove BEFORE Next(), then WhiteIsActive is the mover.
            Bitboard capturedPawnMask = WhiteIsActive 
                ? toMask.MovePieces(Direction.S) 
                : toMask.MovePieces(Direction.N);
            
            Pieces capturedPiece = WhiteIsActive ? Pieces.BlackPawns : Pieces.WhitePawns;
            Set(capturedPiece, GetPieces(capturedPiece) & ~capturedPawnMask);
        } else {
            Pieces? capturedPiece = DetectPieceCollision(toMask);
            if (capturedPiece.HasValue) {
                Set(capturedPiece.Value, GetPieces(capturedPiece.Value) & ~toMask);
            }
        }
    }

    private void HandlePromotion(Pieces piece, Bitboard fromMask, Bitboard toMask, Move move) {
        // Remove pawn from original position
        Set(piece, GetPieces(piece) & ~fromMask);

        // Add promoted piece to new position
        Pieces promotedTo = GetPromotedPiece(move);
        Set(promotedTo, GetPieces(promotedTo) | toMask);
    }

    private Pieces GetPromotedPiece(Move move) {
        if (WhiteIsActive) {
            if (move.Flags.HasFlag(MoveFlags.PromoteToQueen)) return Pieces.WhiteQueens;
            if (move.Flags.HasFlag(MoveFlags.PromoteToRook)) return Pieces.WhiteRooks;
            if (move.Flags.HasFlag(MoveFlags.PromoteToBishop)) return Pieces.WhiteBishops;
            if (move.Flags.HasFlag(MoveFlags.PromoteToKnight)) return Pieces.WhiteKnights;
            return Pieces.WhiteQueens; // Default
        } else {
            if (move.Flags.HasFlag(MoveFlags.PromoteToQueen)) return Pieces.BlackQueens;
            if (move.Flags.HasFlag(MoveFlags.PromoteToRook)) return Pieces.BlackRooks;
            if (move.Flags.HasFlag(MoveFlags.PromoteToBishop)) return Pieces.BlackBishops;
            if (move.Flags.HasFlag(MoveFlags.PromoteToKnight)) return Pieces.BlackKnights;
            return Pieces.BlackQueens; // Default
        }
    }

    private void HandleCastling(Move move) {
        // King move is already handled by MovePiece in ExecuteMove
        // We just need to move the Rook
        if (WhiteIsActive) {
            if (move.Flags.HasFlag(MoveFlags.KingCastle)) {
                MovePiece(Pieces.WhiteRooks, 1UL << 0, 1UL << 2); // h1 to f1
            } else if (move.Flags.HasFlag(MoveFlags.QueenCastle)) {
                MovePiece(Pieces.WhiteRooks, 1UL << 7, 1UL << 4); // a1 to d1
            }
        } else {
            if (move.Flags.HasFlag(MoveFlags.KingCastle)) {
                MovePiece(Pieces.BlackRooks, 1UL << 56, 1UL << 58); // h8 to f8
            } else if (move.Flags.HasFlag(MoveFlags.QueenCastle)) {
                MovePiece(Pieces.BlackRooks, 1UL << 63, 1UL << 60); // a8 to d8
            }
        }
    }

    private void UpdateMetadata(Pieces piece, Move move) {
        bool whiteMoved = piece is Pieces.WhitePawns or Pieces.WhiteRooks or Pieces.WhiteKnights or Pieces.WhiteBishops or Pieces.WhiteQueens or Pieces.WhiteKing;
        
        // 1. En Passant Square
        if (move.Flags.HasFlag(MoveFlags.DoublePawnPush)) {
            EnPassant = whiteMoved 
                ? (1UL << (move.From + 8)) 
                : (1UL << (move.From - 8));
        }

        // 2. Castling Rights
        UpdateCastlingRights(piece, move);

        // 3. Halfmove clock
        if (piece == Pieces.WhitePawns || piece == Pieces.BlackPawns || move.IsCapture) {
            HalfClockReset();
        }
    }

    private void UpdateCastlingRights(Pieces piece, Move move) {
        // If King moves, lose all castling rights for that color
        if (piece == Pieces.WhiteKing) {
            WhiteCastleKingSide = false;
            WhiteCastleQueenSide = false;
        } else if (piece == Pieces.BlackKing) {
            BlackCastleKingSide = false;
            BlackCastleQueenSide = false;
        }

        // If Rook moves from its original square, or is captured on its original square
        // White Rooks: a1 (0), h1 (7)
        // Black Rooks: a8 (56), h8 (63)
        
        int from = move.From;
        int to = move.To;

        if (from == 0 || to == 0) WhiteCastleKingSide = false;
        if (from == 7 || to == 7) WhiteCastleQueenSide = false;
        if (from == 56 || to == 56) BlackCastleKingSide = false;
        if (from == 63 || to == 63) BlackCastleQueenSide = false;
    }
    
    public bool EnPassantAvailable => EnPassant.RawBits != 0;

    public Coordinates? GetEnPassantCoordinates() {
        return EnPassantAvailable
            ? BitBoardHelpers.MaskToCoordinates(EnPassant)
            : null;
    }

    /// <summary>
    /// Return Bitboard of all pieces of certain color.
    /// </summary>
    /// <param name="white">If true, returns white pieces, else black pieces.</param>
    /// <returns>The combination bitmap of the pieces.</returns>
    public Bitboard GetPieces(bool white) {
        return white switch {
            true =>
                WhiteBishops |
                WhiteKnights |
                WhiteQueens |
                WhiteKing |
                WhitePawns |
                WhiteRooks,
            false => 
                BlackBishops |
                BlackKnights |
                BlackQueens |
                BlackKing |
                BlackPawns |
                BlackRooks
        };
    }
    
    /// <summary>
    /// Returns mask of all occupied positions. All zeros are empty positions
    /// </summary>
    /// <returns>Bitmask of all squares occupied by any piece.</returns>
    public Bitboard GetAllPieces() {
        return GetPieces(true) | GetPieces(false);
    }

    public Bitboard GetActivePieces() => GetPieces(WhiteIsActive);
    
    public Bitboard GetInactivePieces() => GetPieces(!WhiteIsActive);

    
    /// <summary>
    /// Determines, which (if any) piece of the current state collides with the piece from the provided mask.
    /// </summary>
    /// <param name="mask">The board to check against. Should be unary bitboard (only 1 bit set).</param>
    /// <returns>Pieces enum entry, if collision has been found, null if no pieces collide</returns>
    public Pieces? DetectPieceCollision(Bitboard mask) {
        if (!(WhitePawns & mask).IsEmpty()) return Pieces.WhitePawns;
        if (!(WhiteRooks & mask).IsEmpty()) return Pieces.WhiteRooks;
        if (!(WhiteKnights & mask).IsEmpty()) return Pieces.WhiteKnights;
        if (!(WhiteBishops & mask).IsEmpty()) return Pieces.WhiteBishops;
        if (!(WhiteQueens & mask).IsEmpty()) return Pieces.WhiteQueens;
        if (!(WhiteKing & mask).IsEmpty()) return Pieces.WhiteKing;
        if (!(BlackPawns & mask).IsEmpty()) return Pieces.BlackPawns;
        if (!(BlackRooks & mask).IsEmpty()) return Pieces.BlackRooks;
        if (!(BlackKnights & mask).IsEmpty()) return Pieces.BlackKnights;
        if (!(BlackBishops & mask).IsEmpty()) return Pieces.BlackBishops;
        if (!(BlackQueens & mask).IsEmpty()) return Pieces.BlackQueens;
        if (!(BlackKing & mask).IsEmpty()) return Pieces.BlackKing;
        return null;
    }


    public State Set(Pieces changedPieces, Bitboard newPieces) {
        switch (changedPieces)
        {
            case Pieces.WhitePawns:
                WhitePawns = newPieces;
                return this;
            case Pieces.WhiteRooks:
                WhiteRooks = newPieces;
                return this;
            case Pieces.WhiteKnights:
                WhiteKnights = newPieces;
                return this;
            case Pieces.WhiteBishops:
                WhiteBishops = newPieces;
                return this;
            case Pieces.WhiteQueens:
                WhiteQueens = newPieces;
                return this;
            case Pieces.WhiteKing:
                WhiteKing = newPieces;
                return this;
            case Pieces.BlackPawns:
                BlackPawns = newPieces;
                return this;
            case Pieces.BlackRooks:
                BlackRooks = newPieces;
                return this;
            case Pieces.BlackKnights:
                BlackKnights = newPieces;
                return this;
            case Pieces.BlackBishops:
                BlackBishops = newPieces;
                return this;
            case Pieces.BlackQueens:
                BlackQueens = newPieces;
                return this;
            case Pieces.BlackKing:
                BlackKing = newPieces;
                return this;
            default:
                throw new ArgumentOutOfRangeException(nameof(changedPieces), changedPieces, null);
        }
    }
    
    
    public Bitboard GetPieces(Pieces pieces) {
        return pieces switch {
            Pieces.WhitePawns => this.WhitePawns,
            Pieces.WhiteRooks => this.WhiteRooks,
            Pieces.WhiteKnights => this.WhiteKnights,
            Pieces.WhiteBishops => this.WhiteBishops,
            Pieces.WhiteQueens => this.WhiteQueens,
            Pieces.WhiteKing => this.WhiteKing,
            Pieces.BlackPawns => this.BlackPawns,
            Pieces.BlackRooks => this.BlackRooks,
            Pieces.BlackKnights => this.BlackKnights,
            Pieces.BlackBishops => this.BlackBishops,
            Pieces.BlackQueens => this.BlackQueens,
            Pieces.BlackKing => this.BlackKing,
            _ => throw new ArgumentOutOfRangeException(nameof(pieces), pieces, null)
        };
    }

    public string PrettyPrint() {
        char[,] board = new char[8,8];

        // Fill with empty squares
        for (int r = 0; r < 8; r++)
            for (int f = 0; f < 8; f++)
                board[r, f] = '.';

        // Helper to place a piece from bitboard
        void PlacePieces(Bitboard bitboard, char symbol) {
            ulong bits = bitboard.RawBits;
            while (bits != 0)
            {
                int square = BitOperations.TrailingZeroCount(bits);
                bits &= bits - 1; // clear LS1B

                int rank = square / 8; // 0 = rank 1, 7 = rank 8
                int file = square % 8; // 0 = file a, 7 = file h

                // Flip rank so rank 8 is at the top
                board[7 - rank, file] = symbol;
            }
        }

        // White pieces
        PlacePieces(WhitePawns,   'P');
        PlacePieces(WhiteKnights, 'N');
        PlacePieces(WhiteBishops, 'B');
        PlacePieces(WhiteRooks,   'R');
        PlacePieces(WhiteQueens,  'Q');
        PlacePieces(WhiteKing,   'K');

        // Black pieces
        PlacePieces(BlackPawns,   'p');
        PlacePieces(BlackKnights, 'n');
        PlacePieces(BlackBishops, 'b');
        PlacePieces(BlackRooks,   'r');
        PlacePieces(BlackQueens,  'q');
        PlacePieces(BlackKing,   'k');

        // Print board
        var sb = new StringBuilder();
        sb.AppendLine();
        for (int r = 0; r < 8; r++) {
            sb.Append($"{8 - r}  "); // rank label
            
            for (int f = 0; f < 8; f++)
                sb.Append(board[r, 7-f] + " ");
            sb.AppendLine();
            
        }

        // File labels
        sb.AppendLine("   a b c d e f g h");
        return sb.ToString();
    }
    
}