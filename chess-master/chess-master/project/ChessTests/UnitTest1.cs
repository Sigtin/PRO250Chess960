using System;
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
    }
}
