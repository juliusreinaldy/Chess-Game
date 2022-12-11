using System.Collections.Generic;
using UnityEngine;

public class PawnObjectScript : ChessPieceObjectScript
{
    public override List<Vector2Int> GetChessAvailableMoves(ref ChessPieceObjectScript[,] chessBoard, int tileCounterX, int tileCounterY)
    {
        List<Vector2Int> move = new List<Vector2Int>();

        int direction = (chessColor == ChessColor.White) ? 1 : -1;

        //Move One Step
        if(chessBoard[currentXCoord,currentYCoord + direction] == null)
        {
            move.Add(new Vector2Int(currentXCoord, currentYCoord + direction));
        }

        //Move Two Step as initial pawn movement
        if (chessBoard[currentXCoord, currentYCoord + direction] == null)
        {
            if (chessColor == ChessColor.White && currentYCoord == 1 && chessBoard[currentXCoord, currentYCoord + (direction * 2)] == null)
            {
                move.Add(new Vector2Int(currentXCoord, currentYCoord + (direction * 2)));
            }

            if (chessColor == ChessColor.Black && currentYCoord == 6 && chessBoard[currentXCoord, currentYCoord + (direction * 2)] == null)
            {
                move.Add(new Vector2Int(currentXCoord, currentYCoord + (direction * 2)));
            }
        }

        //Eat Oppenents move
        if (currentXCoord != tileCounterX - 1)
            if (chessBoard[currentXCoord + 1, currentYCoord + direction] != null && chessBoard[currentXCoord + 1, currentYCoord + direction].chessColor != chessColor)
                move.Add(new Vector2Int(currentXCoord + 1, currentYCoord + direction));
        if (currentXCoord != 0)
            if (chessBoard[currentXCoord - 1, currentYCoord + direction] != null && chessBoard[currentXCoord - 1, currentYCoord + direction].chessColor != chessColor)
                move.Add(new Vector2Int(currentXCoord - 1, currentYCoord + direction));

        return move;
    }

    public override SpecialChessMove GetChessSpecialMoves(ref ChessPieceObjectScript[,] chessBoard, ref List<Vector2Int[]> moveList, ref List<Vector2Int> possibleMoves)
    {
        int direction = (chessColor == ChessColor.White) ? 1 : -1;

        //Queening Condition
        if ((chessColor == ChessColor.White && currentYCoord == 6) || (chessColor == ChessColor.Black && currentYCoord == 1))
            return SpecialChessMove.Queening;

        //EnPassant Condition
        if (moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            //Checker if last turn Pawn Is Moved
            if (chessBoard[lastMove[1].x, lastMove[1].y].chessType == ChessTypes.Pawn)
            {
                //Checker if Last Pawn Move Is A +2 Pawn Moved
                if(Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2)
                {
                    //Checker to make sure the move are from the other team
                    if (chessBoard[lastMove[1].x, lastMove[1].y].chessColor != chessColor)
                    {
                        //Checker to check if both pawns are in same  Y position
                        if(lastMove[1].y == currentYCoord)
                        {
                            if(lastMove[1].x == currentXCoord - 1)// Landed On Left
                            {
                                possibleMoves.Add(new Vector2Int(currentXCoord - 1, currentYCoord + direction));
                                return SpecialChessMove.EnPassant;
                            }
                            if (lastMove[1].x == currentXCoord + 1)//Lanedd on Right
                            {
                                possibleMoves.Add(new Vector2Int(currentXCoord + 1, currentYCoord + direction));
                                return SpecialChessMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }

        return SpecialChessMove.None; 
    }
}
