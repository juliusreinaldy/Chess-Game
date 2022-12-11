using System.Collections.Generic;
using UnityEngine;

public class BishopObjectScript : ChessPieceObjectScript
{
    public override List<Vector2Int> GetChessAvailableMoves(ref ChessPieceObjectScript[,] chessBoard, int tileCounterX, int tileCounterY)
    {
        List<Vector2Int> move = new List<Vector2Int>();

        //Top Right Movement
        for (int x = currentXCoord + 1, y = currentYCoord + 1; x < tileCounterX && y < tileCounterY; x++,y++)
        {
            if (chessBoard[x, y] == null)
            {
                move.Add(new Vector2Int(x, y));
            }
            else
            {
                if (chessBoard[x, y] != null)
                {
                    move.Add(new Vector2Int(x, y));
                }
                break;
            }
        }

        //Top Left Movement
        for (int x = currentXCoord - 1, y = currentYCoord + 1; x >= 0 && y < tileCounterY; x--, y++)
        {
            if (chessBoard[x, y] == null)
            {
                move.Add(new Vector2Int(x, y));
            }
            else
            {
                if (chessBoard[x, y] != null)
                {
                    move.Add(new Vector2Int(x, y));
                }
                break;
            }
        }

        //Bottom Right Movement
        for (int x = currentXCoord + 1, y = currentYCoord - 1; x < tileCounterX && y >= 0; x++, y--)
        {
            if (chessBoard[x, y] == null)
            {
                move.Add(new Vector2Int(x, y));
            }
            else
            {
                if (chessBoard[x, y] != null)
                {
                    move.Add(new Vector2Int(x, y));
                }
                break;
            }
        }

        //Bottom Left Movement
        for (int x = currentXCoord - 1, y = currentYCoord - 1; x >= 0 && y >= 0; x-- , y--)
        {
            if (chessBoard[x, y] == null)
            {
                move.Add(new Vector2Int(x, y));
            }
            else
            {
                if (chessBoard[x, y] != null)
                {
                    move.Add(new Vector2Int(x, y));
                }
                break;
            }
        }

        return move;
    }
}
