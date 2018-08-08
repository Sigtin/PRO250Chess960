using System;
using System.Collections.Generic;
using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class UnitTest1
    {
        ChessBoard board = new ChessBoard();
        [TestMethod]
        public void AssertThatAvailableSpacesIsZeroAfterPlacement()
        {
            board.SetChess960Placement();
            if(board.availableSpaces.Count != 0)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AssertThatPiecesAreSetInPassedValues()
        {
            board.SetPiece(Piece.PAWN, Player.WHITE, 0, 1);
            if(board.Grid[1][0].piece != Piece.PAWN)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AssertThatPlayersAreSetInPassedValues()
        {
            board.SetPiece(Piece.PAWN, Player.WHITE, 0, 1);
            if (board.Grid[1][0].player != Player.WHITE)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AssertThatBishopsAreOnDifferentColoredSquares()
        {
            board.SetChess960Placement();
            List<int> bishopIndices = new List<int>();
            
            for (int i = 0; i < board.Grid[0].Length; i++)
            {
                if(board.Grid[0][i].piece == Piece.BISHOP)
                {
                    bishopIndices.Add(i);
                }
            }

            if(bishopIndices[0] % 2 == bishopIndices[1] % 2)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AssertThatKingIsBetweenRooks()
        {
            board.SetChess960Placement();
            List<int> pieceIndices = new List<int>();

            for (int i = 0; i < board.Grid[0].Length; i++)
            {
                if (board.Grid[0][i].piece == Piece.ROOK || board.Grid[0][i].piece == Piece.KING)
                {
                    pieceIndices.Add(i);
                }
            }

            if (!(pieceIndices[1] < pieceIndices[2] && pieceIndices[1] > pieceIndices[0]))
            {
                Assert.Fail();
            }
        }
    }
}
