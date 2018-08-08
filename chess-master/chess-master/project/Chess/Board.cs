using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess
{
    public class ChessBoard
    {
        private static int[] pieceWeights = { 1, 3, 4, 5, 7, 20 };

        public piece_t[][] Grid { get; private set; }
        public Dictionary<Player, position_t> Kings { get; private set; }
        public Dictionary<Player, List<position_t>> Pieces { get; private set; }
        public Dictionary<Player, position_t> LastMove { get; private set; }

        public ChessBoard()
        {
            // init blank board grid
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                    Grid[i][j] = new piece_t(Piece.NONE, Player.WHITE);
            }

            // init last moves
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t();
            LastMove[Player.WHITE] = new position_t();

            // init king positions
            Kings = new Dictionary<Player, position_t>();

            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());
        }

        public ChessBoard(ChessBoard copy)
        {
            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());

            // init board grid to copy locations
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                {
                    Grid[i][j] = new piece_t(copy.Grid[i][j]);

                    // add piece location to list
                    if (Grid[i][j].piece != Piece.NONE)
                        Pieces[Grid[i][j].player].Add(new position_t(j, i));
                }
            }

            // copy last known move
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t(copy.LastMove[Player.BLACK]);
            LastMove[Player.WHITE] = new position_t(copy.LastMove[Player.WHITE]);

            // copy king locations
            Kings = new Dictionary<Player, position_t>();
            Kings[Player.BLACK] = new position_t(copy.Kings[Player.BLACK]);
            Kings[Player.WHITE] = new position_t(copy.Kings[Player.WHITE]);
        }

        /// <summary>
        /// Calculate and return the boards fitness value.
        /// </summary>
        /// <param name="max">Who's side are we viewing from.</param>
        /// <returns>The board fitness value, what else?</returns>
        public int fitness(Player max)
        {
            int fitness = 0;
            int[] blackPieces = { 0, 0, 0, 0, 0, 0 };
            int[] whitePieces = { 0, 0, 0, 0, 0, 0 };
            int blackMoves = 0;
            int whiteMoves = 0;

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.BLACK])
            {
                blackMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                blackPieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.WHITE])
            {
                whiteMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                whitePieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // if viewing from black side
            if (max == Player.BLACK)
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (blackPieces[i] - whitePieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (blackMoves - whiteMoves));
            }
            else
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (whitePieces[i] - blackPieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (whiteMoves - blackMoves));
            }

            return fitness;
        }

        public void SetInitialPlacement()
        {
            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }

            SetPiece(Piece.ROOK, Player.WHITE, 0, 0);
            SetPiece(Piece.ROOK, Player.WHITE, 7, 0);
            SetPiece(Piece.ROOK, Player.BLACK, 0, 7);
            SetPiece(Piece.ROOK, Player.BLACK, 7, 7);

            SetPiece(Piece.KNIGHT, Player.WHITE, 1, 0);
            SetPiece(Piece.KNIGHT, Player.WHITE, 6, 0);
            SetPiece(Piece.KNIGHT, Player.BLACK, 1, 7);
            SetPiece(Piece.KNIGHT, Player.BLACK, 6, 7);

            SetPiece(Piece.BISHOP, Player.WHITE, 2, 0);
            SetPiece(Piece.BISHOP, Player.WHITE, 5, 0);
            SetPiece(Piece.BISHOP, Player.BLACK, 2, 7);
            SetPiece(Piece.BISHOP, Player.BLACK, 5, 7);

            SetPiece(Piece.KING, Player.WHITE, 4, 0);
            SetPiece(Piece.KING, Player.BLACK, 4, 7);
            Kings[Player.WHITE] = new position_t(4, 0);
            Kings[Player.BLACK] = new position_t(4, 7);
            SetPiece(Piece.QUEEN, Player.WHITE, 3, 0);
            SetPiece(Piece.QUEEN, Player.BLACK, 3, 7);
        }

        public void SetChess960Placement()
        {
            List<int> availableSpaces = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }
            
            Random rand = new Random();

            int bishop1Index = rand.Next(0, availableSpaces.Count);
            int bishop1Num = availableSpaces[bishop1Index];
            SetPiece(Piece.BISHOP, Player.WHITE, bishop1Num, 0);
            SetPiece(Piece.BISHOP, Player.BLACK, bishop1Num, 7);
            availableSpaces.RemoveAt(bishop1Index);

            bool isValidBishop = false;

            while (isValidBishop == false)
            {
                int bishop2Index = rand.Next(0, availableSpaces.Count);
                int bishop2Num = availableSpaces[bishop2Index];
                if(bishop1Num % 2 != bishop2Num % 2)
                {
                    isValidBishop = true;
                    SetPiece(Piece.BISHOP, Player.WHITE, bishop2Num, 0);
                    SetPiece(Piece.BISHOP, Player.BLACK, bishop2Num, 7);
                    availableSpaces.RemoveAt(bishop2Index);
                }
            }

            int knight1Index = rand.Next(0, availableSpaces.Count);
            int knight1Num = availableSpaces[knight1Index];
            SetPiece(Piece.KNIGHT, Player.WHITE, knight1Num, 0);
            SetPiece(Piece.KNIGHT, Player.BLACK, knight1Num, 7);
            availableSpaces.RemoveAt(knight1Index);

            int knight2Index = rand.Next(0, availableSpaces.Count);
            int knight2Num = availableSpaces[knight2Index];
            SetPiece(Piece.KNIGHT, Player.WHITE, knight2Num, 0);
            SetPiece(Piece.KNIGHT, Player.BLACK, knight2Num, 7);
            availableSpaces.RemoveAt(knight2Index);

            int queenIndex = rand.Next(0, availableSpaces.Count);
            int queenNum = availableSpaces[queenIndex];
            SetPiece(Piece.QUEEN, Player.WHITE, queenNum, 0);
            SetPiece(Piece.QUEEN, Player.BLACK, queenNum, 7);
            availableSpaces.RemoveAt(queenIndex);

            int rook1Index = 0;
            int rook1Num = availableSpaces[rook1Index];

            int kingIndex = 1;
            int kingNum = availableSpaces[kingIndex];

            int rook2Index = 2;
            int rook2Num = availableSpaces[rook2Index];

            SetPiece(Piece.ROOK, Player.WHITE, rook1Num, 0);
            SetPiece(Piece.ROOK, Player.BLACK, rook1Num, 7);
            SetPiece(Piece.ROOK, Player.WHITE, rook2Num, 0);
            SetPiece(Piece.ROOK, Player.BLACK, rook2Num, 7);

            Kings[Player.WHITE] = new position_t(kingNum, 0);
            Kings[Player.BLACK] = new position_t(kingNum, 7);
            SetPiece(Piece.KING, Player.WHITE, kingNum, 0);
            SetPiece(Piece.KING, Player.BLACK, kingNum, 7);
            availableSpaces.Remove(kingNum);
            availableSpaces.Remove(rook1Num);
            availableSpaces.Remove(rook2Num);
        }

        public void SetPiece(Piece piece, Player player, int letter, int number)
        {
            // set grid values
            Grid[number][letter].piece = piece;
            Grid[number][letter].player = player;

            // add piece to list
            Pieces[player].Add(new position_t(letter, number));

            // update king position
            if (piece == Piece.KING)
            {
                Kings[player] = new position_t(letter, number);
            }
        }
    }
}
