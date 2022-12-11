using System.Collections.Generic;
using UnityEngine;

public enum ChessTypes
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public enum ChessColor
{
    None = 0,
    White = 1,
    Black = 2
}
public class ChessPieceObjectScript : MonoBehaviour
{

    public ChessTypes chessType;
    public ChessColor chessColor;

    public int currentXCoord;
    public int currentYCoord;

    private Vector3 desiredPost;
    private Vector3 desiredScale = Vector3.one;
    
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPost, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    public virtual void SetChessPosition(Vector3 position, bool forcing = false)
    {
        desiredPost = position;
        if (forcing)
            transform.position = desiredPost;
    }

    public virtual void SetChessScale(Vector3 scale, bool forcing = false)
    {
        desiredScale = scale;
        if (forcing)
            transform.localScale = desiredScale;
    }

    public virtual List<Vector2Int> GetChessAvailableMoves(ref ChessPieceObjectScript[,] chessBoard, int tileCounterX, int tileCounterY)
    {
        List<Vector2Int> move = new List<Vector2Int>();

        move.Add(new Vector2Int(3, 3));
        move.Add(new Vector2Int(3, 4));
        move.Add(new Vector2Int(4, 3));
        move.Add(new Vector2Int(4, 4));

        return move;
    }

    public virtual SpecialChessMove GetChessSpecialMoves(ref ChessPieceObjectScript[,] chessBoard, ref List<Vector2Int[]> moveList, ref List<Vector2Int> possibleMoves)
    {
        
        return SpecialChessMove.None;

    }
}
   