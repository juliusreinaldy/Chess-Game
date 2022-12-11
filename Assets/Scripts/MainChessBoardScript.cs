using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpecialChessMove
{
    None = 0,
    EnPassant,
    Castling,
    Queening
}

public class MainChessBoardScript : MonoBehaviour
{
    [Header("Tile Display")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("ChessPieces")]
    [SerializeField] private GameObject[] chessPieces;
    [SerializeField] private Material[] chessMaterials;

    [Header("UI")]
    [SerializeField] private GameObject whiteWinUI;
    [SerializeField] private GameObject blackWinUI;



    private const int CONST_TILE_X = 8;
    private const int CONST_TILE_Y = 8;

    private GameObject[,] chessTiles;
    private ChessPieceObjectScript[,] chessObjects;
    private ChessPieceObjectScript currentChess;

    private Camera camera;
    private Vector2Int currentMousePost;

    private Vector3 bounds;

    private List<ChessPieceObjectScript> eatenWhitePiece = new List<ChessPieceObjectScript>();
    private List<ChessPieceObjectScript> eatenBlackPiece = new List<ChessPieceObjectScript>();

    private List<Vector2Int> possibleMoves = new List<Vector2Int>();

    private bool isWhiteTurn;

    private List<Vector2Int[]> chessMoveList = new List<Vector2Int[]>();
    private SpecialChessMove specialChessMove;

    private void Awake()
    {
        isWhiteTurn = true;
        ChessGridGeneration(tileSize, CONST_TILE_X, CONST_TILE_Y);

        InitChessPiece();
        InitChessPosition();
    }

    private void Update()
    {
        if (!camera)
        {
            camera = Camera.main;
            return;
        }

        RaycastHit rayInfo;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out rayInfo, 100, LayerMask.GetMask("ChessTile", "TileHover", "ChessMove")))
        {
            //Search Hit Location Up on The tile
            Vector2Int hitPost = LookUpChessTile(rayInfo.transform.gameObject);

            //Checker to check if the tile has not hovered at all then it hover
            if(currentMousePost == -Vector2Int.one)
            {
                currentMousePost = hitPost;
                chessTiles[hitPost.x, hitPost.y].layer = LayerMask.NameToLayer("TileHover");
            }

            //Checker if the tile hit somewhere and change the hovering place and need to change the stats of the previous tile again
            if (currentMousePost != hitPost)
            {
                chessTiles[currentMousePost.x, currentMousePost.y].layer = (ValidChessMovedChecker(ref possibleMoves, currentMousePost)) ? LayerMask.NameToLayer("ChessMove") : LayerMask.NameToLayer("ChessTile");
                currentMousePost = hitPost;
                chessTiles[hitPost.x, hitPost.y].layer = LayerMask.NameToLayer("TileHover");
            }

            //Function To Do something When Chess Piece Is Selected
            if (Input.GetMouseButtonDown(0))
            {
                if(chessObjects[hitPost.x,hitPost.y] != null)
                {
                    //Check for turn
                    if ((chessObjects[hitPost.x, hitPost.y].chessColor == ChessColor.White && isWhiteTurn) || (chessObjects[hitPost.x, hitPost.y].chessColor == ChessColor.Black && !isWhiteTurn))
                    {
                        currentChess = chessObjects[hitPost.x, hitPost.y];

                        //Where you Chess Piece Can move
                        possibleMoves = currentChess.GetChessAvailableMoves(ref chessObjects, CONST_TILE_X, CONST_TILE_Y);
                        
                        //Special Moves Checker
                        specialChessMove = currentChess.GetChessSpecialMoves(ref chessObjects, ref chessMoveList, ref possibleMoves);

                        MovemenChecker();
                        HighlightChessMovement();
                    }
                }
            }


            if (currentChess != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int prevPost = new Vector2Int(currentChess.currentXCoord, currentChess.currentYCoord);

               
                bool isValid = MovingChessPieces(currentChess, hitPost.x, hitPost.y);

                if (!isValid)
                {
                    currentChess.SetChessPosition(ChessTileCenter(prevPost.x, prevPost.y));
                }

                currentChess = null;
                RemoveHighlightChessMovement();
            }


        }
        else
        {
            if(currentMousePost != -Vector2Int.one)
            {
                chessTiles[currentMousePost.x, currentMousePost.y].layer = (ValidChessMovedChecker(ref possibleMoves,currentMousePost)) ? LayerMask.NameToLayer("ChessMove") : LayerMask.NameToLayer("ChessTile");  
                currentMousePost = -Vector2Int.one;
            }

            if(currentChess && Input.GetMouseButtonUp(0))
            {
                currentChess.SetChessPosition(ChessTileCenter(currentChess.currentXCoord, currentChess.currentYCoord));
                currentChess = null;
                RemoveHighlightChessMovement();
            }
        }

        //Function To liftUp the Chess Piece
        if (currentChess)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float liftDistance = 0.0f;
            if (horizontalPlane.Raycast(ray, out liftDistance))
            {
                currentChess.SetChessPosition(ray.GetPoint(liftDistance)+ Vector3.up * 0.7f);

            }
        }

    }

    //
    //Function Segment Used For Grid creation
    private void ChessGridGeneration(float gridSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * gridSize, 0, (tileCountX / 2) * gridSize) + boardCenter;

        chessTiles = new GameObject[tileCountX, tileCountY];

        for(int x = 0; x < CONST_TILE_X; x++)
        {
            for(int y = 0; y <CONST_TILE_Y; y++)
            {
                chessTiles[x, y] = CreateTiles(gridSize, x, y);
            }
        }
    }

    private GameObject CreateTiles(float tileSize, int coordX, int coordY)
    {
        GameObject chessTileObject = new GameObject(string.Format("TilePost X:{0}, Y:{1}", coordX, coordY));
        chessTileObject.transform.parent = transform;

        Mesh tileMesh = new Mesh();
        chessTileObject.AddComponent<MeshFilter>().mesh = tileMesh;
        chessTileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] tileVertices = new Vector3[4];
        tileVertices[0] = new Vector3(coordX * tileSize, yOffset, coordY * tileSize) - bounds;
        tileVertices[1] = new Vector3(coordX * tileSize, yOffset, (coordY + 1) * tileSize) - bounds;
        tileVertices[2] = new Vector3((coordX+1) * tileSize, yOffset, coordY * tileSize) - bounds;
        tileVertices[3] = new Vector3((coordX + 1) * tileSize, yOffset, (coordY + 1) * tileSize) - bounds;

        int[] tileTris = new int[] { 0, 1, 2, 1, 3, 2 };

        tileMesh.vertices = tileVertices;
        tileMesh.triangles = tileTris;

        tileMesh.RecalculateNormals();

        chessTileObject.layer = LayerMask.NameToLayer("ChessTile");
        chessTileObject.AddComponent<BoxCollider>();

        return chessTileObject;

    }

    private Vector2Int LookUpChessTile (GameObject hitInfo)
    {
        for (int x = 0; x < CONST_TILE_X; x++)
            for (int y = 0; y < CONST_TILE_Y; y++)
                if(chessTiles[x,y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }


    //
    //Function Segment Used to Initialised all the spawning Chess Pieces
    private void InitChessPiece()
    {
        chessObjects = new ChessPieceObjectScript[CONST_TILE_X, CONST_TILE_Y];

        //White Spawn Chess Piece
        chessObjects[0,0] = CreateChessPiece(ChessTypes.Rook, ChessColor.White);
        chessObjects[1,0] = CreateChessPiece(ChessTypes.Knight, ChessColor.White);
        chessObjects[2,0] = CreateChessPiece(ChessTypes.Bishop, ChessColor.White);
        chessObjects[3,0] = CreateChessPiece(ChessTypes.Queen, ChessColor.White);
        chessObjects[4,0] = CreateChessPiece(ChessTypes.King, ChessColor.White);
        chessObjects[5,0] = CreateChessPiece(ChessTypes.Bishop, ChessColor.White);
        chessObjects[6,0] = CreateChessPiece(ChessTypes.Knight, ChessColor.White);
        chessObjects[7,0] = CreateChessPiece(ChessTypes.Rook, ChessColor.White);
        for (int i = 0; i < CONST_TILE_X; i++)
        {
            chessObjects[i, 1] = CreateChessPiece(ChessTypes.Pawn, ChessColor.White);
        }

        //Black Spawn Chess Piece
        chessObjects[0, 7] = CreateChessPiece(ChessTypes.Rook, ChessColor.Black);
        chessObjects[1, 7] = CreateChessPiece(ChessTypes.Knight, ChessColor.Black);
        chessObjects[2, 7] = CreateChessPiece(ChessTypes.Bishop, ChessColor.Black);
        chessObjects[3, 7] = CreateChessPiece(ChessTypes.Queen, ChessColor.Black);
        chessObjects[4, 7] = CreateChessPiece(ChessTypes.King, ChessColor.Black);
        chessObjects[5, 7] = CreateChessPiece(ChessTypes.Bishop, ChessColor.Black);
        chessObjects[6, 7] = CreateChessPiece(ChessTypes.Knight, ChessColor.Black);
        chessObjects[7, 7] = CreateChessPiece(ChessTypes.Rook, ChessColor.Black);
        for (int i = 0; i < CONST_TILE_X; i++)
        {
            chessObjects[i, 6] = CreateChessPiece(ChessTypes.Pawn, ChessColor.Black);
        }
    }

    private ChessPieceObjectScript CreateChessPiece(ChessTypes types, ChessColor color)
    {
        
        ChessPieceObjectScript tempPiece = Instantiate(chessPieces[(int)types - 1], transform).GetComponent<ChessPieceObjectScript>();

        Vector3 tempVector3 = new Vector3(tempPiece.transform.localScale.x, tempPiece.transform.localScale.y, 150.0f);

        tempPiece.transform.localScale = tempVector3;

        tempPiece.chessType = types;
        tempPiece.chessColor = color;

        Material[] tempMats = tempPiece.GetComponent<MeshRenderer>().materials;
        tempMats[1] = chessMaterials[(int)(color) - 1];
        tempPiece.GetComponent<MeshRenderer>().materials = tempMats;


        return tempPiece;
    }

    private void InitChessPosition()
    {
        for(int x = 0; x< CONST_TILE_X; x++)
        {
            for (int y = 0; y < CONST_TILE_Y; y++)
            {
                if (chessObjects[x, y] != null)
                    ChessPiecePosition(x, y, true);
            }
        }
    }

    //Function To determine individual Chess Piece
    private void ChessPiecePosition(int x, int y, bool forcePlace = false)
    {
        chessObjects[x, y].currentXCoord = x;
        chessObjects[x, y].currentYCoord = y;
        chessObjects[x, y].SetChessPosition(ChessTileCenter(x, y), forcePlace);
        chessObjects[x, y].SetChessScale(new Vector3(chessObjects[x, y].transform.localScale.x, chessObjects[x, y].transform.localScale.y, 150.0f), forcePlace);

    }

    //
    //Fuction to make the chess piece stay in the center of chess tile
    private Vector3 ChessTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    //Function To Define Chess Piece Movement on Chess Board
    private bool MovingChessPieces(ChessPieceObjectScript selectedChess, int x, int y)
    {
        if (!ValidChessMovedChecker(ref possibleMoves, new Vector2Int(x, y)))
            return false;

        Vector2Int prevPost = new Vector2Int(selectedChess.currentXCoord, selectedChess.currentYCoord);

        //Checker To check if there are chess piece on the chess tile
        if(chessObjects[x,y]!= null)
        {
            ChessPieceObjectScript existCP = chessObjects[x, y];

            if(selectedChess.chessColor == existCP.chessColor)
            {
                return false;
            }

            //Case Checker for enemy team
            if(existCP.chessColor == ChessColor.White)
            {
                eatenWhitePiece.Add(existCP);
                existCP.SetChessScale(new Vector3(0.4f, 0.4f, 100.0f));
                existCP.SetChessPosition(new Vector3(8 * tileSize, yOffset, -1 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.forward * 0.45f) * eatenWhitePiece.Count);
            }
            else
            {
                eatenBlackPiece.Add(existCP);
                existCP.SetChessScale(new Vector3(0.4f, 0.4f, 100.0f));
                existCP.SetChessPosition(new Vector3(-1 * tileSize, yOffset, 8 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * 0.45f) * eatenBlackPiece.Count);
            }
        }

        chessObjects[x, y] = selectedChess;
        chessObjects[prevPost.x, prevPost.y] = null;

        ChessPiecePosition(x, y);

        isWhiteTurn = !isWhiteTurn;

        chessMoveList.Add(new Vector2Int[] { prevPost, new Vector2Int(x, y) });

        ExecuteSpecialMoves();

        if (CheckmateChecker())
        {
            if (selectedChess.chessColor == ChessColor.White)
            {
                Debug.Log("White Wins");
                whiteWinUI.SetActive(true);
            }
            else
            {
                Debug.Log("BlackWins");
                blackWinUI.SetActive(true);
            }
        }

        return true;
    }

    //
    //Function For Highlight All Possible move of chess piece

    private void HighlightChessMovement()
    {
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            chessTiles[possibleMoves[i].x, possibleMoves[i].y].layer = LayerMask.NameToLayer("ChessMove");
        }
    }
    private void RemoveHighlightChessMovement()
    {
        for(int i = 0; i < possibleMoves.Count; i++)
        {
            chessTiles[possibleMoves[i].x, possibleMoves[i].y].layer = LayerMask.NameToLayer("ChessTile");
        }

        possibleMoves.Clear();
    }
    private bool ValidChessMovedChecker(ref List<Vector2Int> validMoves, Vector2Int position)
    {
        for (int i = 0; i < validMoves.Count; i++)
        {
            if(validMoves[i].x == position.x && validMoves[i].y == position.y) 
            {
                return true;
            }
        }

        return false;
    }

    //
    //Special Move Function
    private void ExecuteSpecialMoves()
    {
        if(specialChessMove == SpecialChessMove.EnPassant)
        {
            var newMove = chessMoveList[chessMoveList.Count - 1];
            ChessPieceObjectScript playerPawn = chessObjects[newMove[1].x, newMove[1].y];
            var targetPawnPost = chessMoveList[chessMoveList.Count - 2];
            ChessPieceObjectScript enemyPawn = chessObjects[targetPawnPost[1].x, targetPawnPost[1].y];

            if(playerPawn.currentXCoord == enemyPawn.currentXCoord)
            {
                if(playerPawn.currentYCoord == enemyPawn.currentYCoord - 1 || playerPawn.currentYCoord == enemyPawn.currentYCoord + 1)
                {
                    if(enemyPawn.chessColor == ChessColor.White)
                    {
                        eatenWhitePiece.Add(enemyPawn);
                        enemyPawn.SetChessScale(new Vector3(0.4f, 0.4f, 100.0f));
                        enemyPawn.SetChessPosition(new Vector3(8 * tileSize, yOffset, -1 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.forward * 0.45f) * eatenWhitePiece.Count);
                    }
                    else
                    {
                        eatenBlackPiece.Add(enemyPawn);
                        enemyPawn.SetChessScale(new Vector3(0.4f, 0.4f, 100.0f));
                        enemyPawn.SetChessPosition(new Vector3(-1 * tileSize, yOffset, 8 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * 0.45f) * eatenBlackPiece.Count);
                    }
                    chessObjects[enemyPawn.currentXCoord, enemyPawn.currentYCoord] = null;
                }
            }
        }

        if(specialChessMove == SpecialChessMove.Queening)
        {
            Vector2Int[] lastChessMove = chessMoveList[chessMoveList.Count - 1];

            ChessPieceObjectScript changingPawn = chessObjects[lastChessMove[1].x, lastChessMove[1].y];
            
            if(changingPawn.chessType == ChessTypes.Pawn)
            {
                if(changingPawn.chessColor == ChessColor.White && lastChessMove[1].y == 7)
                {
                    ChessPieceObjectScript newQueen = CreateChessPiece(ChessTypes.Queen, ChessColor.White);
                    newQueen.transform.position = chessObjects[lastChessMove[1].x, lastChessMove[1].y].transform.position;
                    Destroy(chessObjects[lastChessMove[1].x, lastChessMove[1].y].gameObject);
                    chessObjects[lastChessMove[1].x, lastChessMove[1].y] = newQueen;
                    ChessPiecePosition(lastChessMove[1].x, lastChessMove[1].y);
                }
                if (changingPawn.chessColor == ChessColor.Black && lastChessMove[1].y == 0)
                {
                    ChessPieceObjectScript newQueen = CreateChessPiece(ChessTypes.Queen, ChessColor.Black);
                    newQueen.transform.position = chessObjects[lastChessMove[1].x, lastChessMove[1].y].transform.position;
                    Destroy(chessObjects[lastChessMove[1].x, lastChessMove[1].y].gameObject);
                    chessObjects[lastChessMove[1].x, lastChessMove[1].y] = newQueen;
                    ChessPiecePosition(lastChessMove[1].x, lastChessMove[1].y);
                }
            }
        }

        if(specialChessMove == SpecialChessMove.Castling)
        {
            Vector2Int[] lastChessMove = chessMoveList[chessMoveList.Count - 1];

            //Checker For Left Rook
            if (lastChessMove[1].x == 2)
            {
                //Checker For White
                if(lastChessMove[1].y == 0)
                {
                    ChessPieceObjectScript rookPiece = chessObjects[0, 0];
                    chessObjects[3, 0] = rookPiece;
                    ChessPiecePosition(3, 0);
                    chessObjects[0, 0] = null;
                }
                //Checker For Black
                else if(lastChessMove[1].y == 7)
                {
                    ChessPieceObjectScript rookPiece = chessObjects[0, 7];
                    chessObjects[3, 7] = rookPiece;
                    ChessPiecePosition(3, 7);
                    chessObjects[0, 7] = null;
                }
            }
            //Checker For Right Rook
            else if (lastChessMove[1].x == 6)
            {
                //Checker For White
                if (lastChessMove[1].y == 0)
                {
                    ChessPieceObjectScript rookPiece = chessObjects[7, 0];
                    chessObjects[5, 0] = rookPiece;
                    ChessPiecePosition(5, 0);
                    chessObjects[7, 0] = null;
                }
                //Checker For Black
                else if (lastChessMove[1].y == 7)
                {
                    ChessPieceObjectScript rookPiece = chessObjects[7, 7];
                    chessObjects[5, 7] = rookPiece;
                    ChessPiecePosition(5, 7);
                    chessObjects[7, 7] = null;
                }
            }
        }
    }

    private void MovemenChecker()
    {
        ChessPieceObjectScript kingPiece = null;

        for (int x = 0; x < CONST_TILE_X; x++)
        {
            for (int y = 0; y < CONST_TILE_Y; y++)
            {
                if (chessObjects[x, y] != null)
                    if (chessObjects[x, y].chessType == ChessTypes.King)
                        if (chessObjects[x, y].chessColor == currentChess.chessColor)
                            kingPiece = chessObjects[x, y];
            }
        }

        //Checking Movement that will make a checkmate possibilities
        SimulateChessPieceMovement(currentChess, ref possibleMoves, kingPiece);
    }

    private void SimulateChessPieceMovement(ChessPieceObjectScript simCP, ref List<Vector2Int> simMoves, ChessPieceObjectScript simKing)
    {
        //Save Position Value, and reset if not used
        int simPostX = simCP.currentXCoord;
        int simPostY = simCP.currentYCoord;
        List<Vector2Int> removingMoves = new List<Vector2Int>();

        //Go with all available move and make sure the piece not resulting in check
        for (int i = 0; i < simMoves.Count; i++)
        {
            int tempSimX = simMoves[i].x;
            int tempSimY = simMoves[i].y;

            Vector2Int tempSimKingPost = new Vector2Int(simKing.currentXCoord, simKing.currentYCoord);
            //Case The simulation is with King Chess Piece
            if(simCP.chessType == ChessTypes.King)
                tempSimKingPost = new Vector2Int(tempSimX, tempSimY);

            //Simulation Condition for each Possible Movement with out reflecting on real chess movement
            ChessPieceObjectScript[,] simChess = new ChessPieceObjectScript[CONST_TILE_X, CONST_TILE_Y];
            List<ChessPieceObjectScript> simAttackingChessPiece = new List<ChessPieceObjectScript>();

            for (int x = 0; x < CONST_TILE_X; x++)
            {
                for (int y = 0; y < CONST_TILE_Y; y++)
                {
                    if(chessObjects[x,y] != null)
                    {
                        simChess[x, y] = chessObjects[x, y];

                        if (simChess[x, y].chessColor != simCP.chessColor)
                            simAttackingChessPiece.Add(simChess[x, y]);
                    }

                }
            }

            //Start Simulation
            simChess[simPostX, simPostY] = null;
            simCP.currentXCoord = tempSimX;
            simCP.currentYCoord = tempSimY;
            simChess[tempSimX, tempSimY] = simCP;

            //If the simulation eat your chess piece 
            var eatenPieces = simAttackingChessPiece.Find(z => z.currentXCoord == tempSimX && z.currentYCoord == tempSimY);
            if (eatenPieces != null)
                simAttackingChessPiece.Remove(eatenPieces);

            //Simulate all the attacking movelist
            List<Vector2Int> simPossibleMoves = new List<Vector2Int>();

            for (int q = 0; q < simAttackingChessPiece.Count; q++)
            {
                var movingPieces = simAttackingChessPiece[q].GetChessAvailableMoves(ref simChess, CONST_TILE_X, CONST_TILE_Y);
                for (int w = 0; w < movingPieces.Count; w++)
                {
                    simPossibleMoves.Add(movingPieces[w]);
                }
            }

            //Is the king position comprimise, if so it remove the possible moves
            if (ValidChessMovedChecker(ref simPossibleMoves, tempSimKingPost))
            {
                removingMoves.Add(simMoves[i]);
            }

            //Restore all data after simulation
            simCP.currentXCoord = simPostX;
            simCP.currentYCoord = simPostY;
        }


        //Clear The available move from all the possible simulation that make your king die
        for (int i = 0; i < removingMoves.Count; i++)
        {
            simMoves.Remove(removingMoves[i]);
        }
    }

    private bool CheckmateChecker()
    {
        var lastChessMove = chessMoveList[chessMoveList.Count - 1];
        ChessColor lastChessColor = (chessObjects[lastChessMove[1].x, lastChessMove[1].y].chessColor == ChessColor.White) ? ChessColor.Black : ChessColor.White;

        List<ChessPieceObjectScript> attackChessPieces = new List<ChessPieceObjectScript>();
        List<ChessPieceObjectScript> defendChessPieces = new List<ChessPieceObjectScript>();

        ChessPieceObjectScript kingPiece = null;

        for (int x = 0; x < CONST_TILE_X; x++)
        {
            for (int y = 0; y < CONST_TILE_Y; y++)
            {
                if (chessObjects[x, y] != null)
                { 
                    if (chessObjects[x, y].chessColor == lastChessColor)
                    {
                        defendChessPieces.Add(chessObjects[x, y]);
                        if (chessObjects[x, y].chessType == ChessTypes.King)
                            kingPiece = chessObjects[x, y];
                    }
                    else
                    {
                        attackChessPieces.Add(chessObjects[x, y]);
                    }    


                }
                    
            }
        }

        // Checker to see is the king is attacked
        List<Vector2Int> currentPossibleMoves = new List<Vector2Int>();

        for (int i = 0; i < attackChessPieces.Count; i++)
        {
            var movingPieces = attackChessPieces[i].GetChessAvailableMoves(ref chessObjects, CONST_TILE_X, CONST_TILE_Y);
            for (int w = 0; w < movingPieces.Count; w++)
            {
                currentPossibleMoves.Add(movingPieces[w]);
            }           
        }

        //Checker if king under seige
        if (ValidChessMovedChecker(ref currentPossibleMoves, new Vector2Int(kingPiece.currentXCoord, kingPiece.currentYCoord)))
        {
            //King Is attacked, do something if Can
            for (int z = 0; z < defendChessPieces.Count; z++)
            {
                List<Vector2Int> defendMoves = defendChessPieces[z].GetChessAvailableMoves(ref chessObjects, CONST_TILE_X, CONST_TILE_Y);
                SimulateChessPieceMovement(defendChessPieces[z], ref defendMoves, kingPiece);

                if (defendMoves.Count != 0)
                {
                    return false;
                }

            }

            //The Checkmate
            return true;
        }

        return false;
    }

}
