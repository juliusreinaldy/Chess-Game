using System.Collections.Generic;
using UnityEngine;

public class QueenObjectScript : ChessPieceObjectScript
{
    public override List<Vector2Int> GetChessAvailableMoves(ref ChessPieceObjectScript[,] chessBoard, int tileCounterX, int tileCounterY)
    {
        List<Vector2Int> move = new List<Vector2Int>();

        //Up Movement
        for (int i = currentYCoord + 1; i < tileCounterY; i++)
        {
            if (chessBoard[currentXCoord, i] == null)
                move.Add(new Vector2Int(currentXCoord, i));

            if (chessBoard[currentXCoord, i] != null)
            {
                if (chessBoard[currentXCoord, i].chessColor != chessColor)
                    move.Add(new Vector2Int(currentXCoord, i));

                break;
            }
        }

        //Down Movement
        for (int i = currentYCoord - 1; i >= 0; i--)
        {
            if (chessBoard[currentXCoord, i] == null)
                move.Add(new Vector2Int(currentXCoord, i));

            if (chessBoard[currentXCoord, i] != null)
            {
                if (chessBoard[currentXCoord, i].chessColor != chessColor)
                    move.Add(new Vector2Int(currentXCoord, i));

                break;
            }
        }

        //Right Movement
        for (int i = currentXCoord + 1; i < tileCounterX; i++)
        {
            if (chessBoard[i, currentYCoord] == null)
                move.Add(new Vector2Int(i, currentYCoord));

            if (chessBoard[i, currentYCoord] != null)
            {
                if (chessBoard[i, currentYCoord].chessColor != chessColor)
                    move.Add(new Vector2Int(i, currentYCoord));

                break;
            }
        }

        //Left Movement
        for (int i = currentXCoord - 1; i >= 0; i--)
        {
            if (chessBoard[i, currentYCoord] == null)
                move.Add(new Vector2Int(i, currentYCoord));

            if (chessBoard[i, currentYCoord] != null)
            {
                if (chessBoard[i, currentYCoord].chessColor != chessColor)
                    move.Add(new Vector2Int(i, currentYCoord));

                break;
            }
        }

        //Top Right Movement
        for (int x = currentXCoord + 1, y = currentYCoord + 1; x < tileCounterX && y < tileCounterY; x++, y++)
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
        for (int x = currentXCoord - 1, y = currentYCoord - 1; x >= 0 && y >= 0; x--, y--)
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
