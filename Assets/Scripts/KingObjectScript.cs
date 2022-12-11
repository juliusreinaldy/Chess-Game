using System.Collections.Generic;
using UnityEngine;

public class KingObjectScript : ChessPieceObjectScript
{
    public override List<Vector2Int> GetChessAvailableMoves(ref ChessPieceObjectScript[,] chessBoard, int tileCounterX, int tileCounterY)
    {
        List<Vector2Int> move = new List<Vector2Int>();


        if (currentXCoord + 1 < tileCounterX)
        {
            //Right Position
            if (chessBoard[currentXCoord + 1, currentYCoord] == null)
            {
                move.Add(new Vector2Int(currentXCoord + 1, currentYCoord));
            }
            else if (chessBoard[currentXCoord + 1, currentYCoord].chessColor != chessColor)
            {
                move.Add(new Vector2Int(currentXCoord + 1, currentYCoord));
            }

            // Top Right Position
            if (currentYCoord + 1 < tileCounterY)
            {
                if (chessBoard[currentXCoord + 1, currentYCoord + 1] == null)
                {
                    move.Add(new Vector2Int(currentXCoord + 1, currentYCoord + 1));
                }
                else if (chessBoard[currentXCoord + 1, currentYCoord + 1].chessColor != chessColor)
                {
                    move.Add(new Vector2Int(currentXCoord + 1, currentYCoord + 1));
                }
            }

            //Bottom Right Position
            if (currentYCoord - 1 >= 0)
            {
                if (chessBoard[currentXCoord + 1, currentYCoord - 1] == null)
                {
                    move.Add(new Vector2Int(currentXCoord + 1, currentYCoord - 1));
                }
                else if (chessBoard[currentXCoord + 1, currentYCoord - 1].chessColor != chessColor)
                {
                    move.Add(new Vector2Int(currentXCoord + 1, currentYCoord - 1));
                }
            }

         }

        if (currentXCoord - 1 >= 0)
        {
            //Left Position
            if (chessBoard[currentXCoord - 1, currentYCoord] == null)
            {
                move.Add(new Vector2Int(currentXCoord - 1, currentYCoord));
            }
            else if (chessBoard[currentXCoord - 1, currentYCoord].chessColor != chessColor)
            {
                move.Add(new Vector2Int(currentXCoord - 1, currentYCoord));
            }

            // Bottom Right Position
            if (currentYCoord + 1 < tileCounterY)
            {
                if (chessBoard[currentXCoord - 1, currentYCoord + 1] == null)
                {
                    move.Add(new Vector2Int(currentXCoord - 1, currentYCoord + 1));
                }
                else if (chessBoard[currentXCoord - 1, currentYCoord + 1].chessColor != chessColor)
                {
                    move.Add(new Vector2Int(currentXCoord - 1, currentYCoord + 1));
                }
            }

            //Bottom Right Position
            if (currentYCoord - 1 >= 0)
            {
                if (chessBoard[currentXCoord - 1, currentYCoord - 1] == null)
                {
                    move.Add(new Vector2Int(currentXCoord - 1, currentYCoord - 1));
                }
                else if (chessBoard[currentXCoord - 1, currentYCoord - 1].chessColor != chessColor)
                {
                    move.Add(new Vector2Int(currentXCoord - 1, currentYCoord - 1));
                }
            }
        }

        if(currentYCoord + 1 < tileCounterY)
        {
            if (chessBoard[currentXCoord, currentYCoord + 1] == null || chessBoard[currentXCoord, currentYCoord + 1].chessColor != chessColor)
                move.Add(new Vector2Int(currentXCoord, currentYCoord +1));
        }


        if (currentYCoord - 1 >= 0 )
        {
            if (chessBoard[currentXCoord, currentYCoord - 1] == null || chessBoard[currentXCoord, currentYCoord - 1].chessColor != chessColor)
                move.Add(new Vector2Int(currentXCoord, currentYCoord - 1));
        }



        return move;

    }

    public override SpecialChessMove GetChessSpecialMoves(ref ChessPieceObjectScript[,] chessBoard, ref List<Vector2Int[]> moveList, ref List<Vector2Int> possibleMoves)
    {
        SpecialChessMove tempSpec = SpecialChessMove.None;


        var kingMove = moveList.Find(mov => mov[0].x == 4 && mov[0].y == ((chessColor == ChessColor.White) ? 0 : 7));
        var leftRookMove = moveList.Find(mov => mov[0].x == 0 && mov[0].y == ((chessColor == ChessColor.White) ? 0 : 7));
        var rightRookMove = moveList.Find(mov => mov[0].x == 7 && mov[0].y == ((chessColor == ChessColor.White) ? 0 : 7));

        if(kingMove == null && currentXCoord == 4)
        {
            //White Team Checker 
            if(chessColor == ChessColor.White)
            {
                //Checker For Left Rook 
                if(leftRookMove == null)
                    if(chessBoard[0,0].chessType == ChessTypes.Rook)
                        if(chessBoard[0,0].chessColor == ChessColor.White)
                            if(chessBoard[3,0]== null)
                                if (chessBoard[2, 0] == null)
                                    if (chessBoard[1, 0] == null)
                                    {
                                        possibleMoves.Add(new Vector2Int(2, 0));
                                        tempSpec = SpecialChessMove.Castling;
                                    }

                //Checker For Right Rook 
                if (rightRookMove == null)
                    if (chessBoard[7, 0].chessType == ChessTypes.Rook)
                        if (chessBoard[7, 0].chessColor == ChessColor.White)
                            if (chessBoard[5, 0] == null)
                                if (chessBoard[6, 0] == null)
                                {
                                    possibleMoves.Add(new Vector2Int(6, 0));
                                    tempSpec = SpecialChessMove.Castling;
                                }
            }
            else
            {
                //Checker For Left Rook 
                if (leftRookMove == null)
                    if (chessBoard[0, 7].chessType == ChessTypes.Rook)
                        if (chessBoard[0, 7].chessColor == ChessColor.Black)
                            if (chessBoard[3, 7] == null)
                                if (chessBoard[2, 7] == null)
                                    if (chessBoard[1, 7] == null)
                                    {
                                        possibleMoves.Add(new Vector2Int(2, 7));
                                        tempSpec = SpecialChessMove.Castling;
                                    }

                //Checker For Right Rook 
                if (rightRookMove == null)
                    if (chessBoard[7, 7].chessType == ChessTypes.Rook)
                        if (chessBoard[7, 7].chessColor == ChessColor.Black)
                            if (chessBoard[5, 7] == null)
                                if (chessBoard[6, 7] == null)
                                {
                                    possibleMoves.Add(new Vector2Int(6, 7));
                                    tempSpec = SpecialChessMove.Castling;
                                }
            }
        }


        return tempSpec;
    }
}
