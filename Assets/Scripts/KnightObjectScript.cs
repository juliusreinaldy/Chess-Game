using System.Collections.Generic;
using UnityEngine;

public class KnightObjectScript : ChessPieceObjectScript
{
    int x;
    int y;

    public override List<Vector2Int> GetChessAvailableMoves(ref ChessPieceObjectScript[,] chessBoard, int tileCounterX, int tileCounterY)
    {
        List<Vector2Int> move = new List<Vector2Int>();

        //Top Right Movement
        x = currentXCoord + 1;
        y = currentYCoord + 2;
        if (x < tileCounterX && y < tileCounterY)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        x = currentXCoord + 2;
        y = currentYCoord + 1;
        if (x < tileCounterX && y < tileCounterY)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        // Top Left Movement
        x = currentXCoord - 1;
        y = currentYCoord + 2;
        if (x >= 0 && y < tileCounterY)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        x = currentXCoord - 2;
        y = currentYCoord + 1 ;
        if (x >= 0 && y < tileCounterY)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }


        // Bottom Right Movement
        x = currentXCoord + 1;
        y = currentYCoord - 2;
        if (x < tileCounterX && y >= 0)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        x = currentXCoord + 2;
        y = currentYCoord - 1;
        if (x < tileCounterX && y >= 0)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        //Bottom Left Movement
        x = currentXCoord - 1;
        y = currentYCoord - 2;
        if (x >= 0 && y >= 0)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        x = currentXCoord - 2;
        y = currentYCoord - 1;
        if (x >= 0 && y >= 0)
        {
            if (chessBoard[x, y] == null || chessBoard[x, y].chessColor != chessColor)
            {
                move.Add(new Vector2Int(x, y));
            }
        }

        return move;
    }
}
