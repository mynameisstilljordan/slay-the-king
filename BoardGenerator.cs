using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour{
    private PlayerController _pC;
    private Ingame _iG;
    char[,] _board = new char[10, 10]; //the board (including outer frame)
    [SerializeField] GameObject _boardObject;
    [SerializeField] GameObject _queenPrefab;
    [SerializeField] GameObject _bishopPrefab;
    [SerializeField] GameObject _pawnPrefab;
    [SerializeField] GameObject _knightPrefab;
    [SerializeField] GameObject _kingPrefab;
    [SerializeField] GameObject _rookPrefab;
    Vector2 _firstPieceLocation;
    private int _numberOfPiecesToBePlaced;

    // Start is called before the first frame update
    void Start() {
        _pC = GetComponent<PlayerController>(); //get the player controller instance from the same gameobject
        _iG = GetComponent<Ingame>(); //set the ingame script reference

        LoadSeed(PlayerPrefs.GetInt("level", 1)); //load the seed

        _numberOfPiecesToBePlaced = 6;
        
        _iG.SetKillObjective(_numberOfPiecesToBePlaced);
        GenerateBoard(_numberOfPiecesToBePlaced);
    }

    //this method generates a new board with the parameter number of pieces
    void GenerateBoard(int pieces) {
        InitializeBoard(); //initialize the board
        PlacePieces(pieces); //place pieces (x amount of pieces)
        //BuildBoard(); //convert the board from a 2d char array to pieces 
    }

    //this method passes a piece to the player controller script and sets it as the current piece there
    void SetCurrentPiece(GameObject piece) {
        _pC.SetCurrentPiece(piece); //set the current piece in the player controller script
    }

    //this method raycasts in the given directions and highlights movable locations
    IEnumerator ScanDirection(Vector2[] directions) {
        yield return new WaitForSeconds(0.01f); //small pause to account for placement delays
        _pC.ScanDirection(directions); //scan in given directions
    }

    //this method initializes the board by filling the 2d char array with #'s and O's
    private void InitializeBoard() {
        var boardSize = _board.GetLength(0); //get the length of the board
        //for the amount of rows on the board (8) plus the outer frame (2)
        for (int i = 0; i < boardSize; i++) {
            //for the amount of columns on the board (8) plus the outer frame (2)
            for (int j = 0; j < boardSize; j++) {
                if (i == 0 || i == boardSize - 1 || j == 0 || j == boardSize - 1)
                    _board[i, j] = '#'; //if on the outer edge, place a #
                else _board[i, j] = 'O'; //otherwise, place a 'O'
            }
        }
    }

    //place the number of pieces
    private void PlacePieces(int numberOfPieces) {
        bool isBoardUnableToGenerate = false;
        var piece = GetRandomPiece(); //get a random piece
        Vector2 currentLocation = Vector2.zero;
        Vector2 preTravelLocation;
        Vector2 currentDirection;

        //for the number of pieces to be placed
        for (int i = 0; i < numberOfPieces; i++) {

            int validDirections = piece.VaildDirections; //get the number of valid directions
            int maxSpaces = piece.MaxSpaces; //get the number of max spaces the piece is able to travel
            int chosenDirection = -1;

            //if this is the first piece to place
            if (i == 0) {
                //place the piece in a random location
                preTravelLocation = new Vector2(Random.Range(1, _board.GetLength(0) - 1), Random.Range(1, _board.GetLength(1) - 1)); //find a random location somewhere on the board
                currentLocation = preTravelLocation; //start at the pre travel location
                _firstPieceLocation = currentLocation; //set the first piece location to the current location
                _board[(int)currentLocation.x, (int)currentLocation.y] = piece.PieceCharacter; //place a piece character at the location 
            }

            //if the piece is not a knight
            if (piece.PieceCharacter != 'K') {

                bool hasAValidDirectionBeenFound = false;

                var counter = 0;

                //picking a valid direction
                while (!hasAValidDirectionBeenFound) {
                    counter++; //increment the counter
                    if (counter == 100) {
                        //if 100 iterations passed with no viable move location, break and mark board as being unable to generate
                        isBoardUnableToGenerate = true; //mark board as being unable to generate
                        break; //break from while to prevent freezing/crashing
                    }

                    chosenDirection = Random.Range(0, validDirections); //choose a random direction from the possible vaild directions
                    //if a valid spot has been found, break from while loop
                    if (_board[(int)currentLocation.x + (int)piece.ValidDirectionList[chosenDirection].x, (int)currentLocation.y + (int)piece.ValidDirectionList[chosenDirection].y] == 'O' || _board[(int)currentLocation.x + (int)piece.ValidDirectionList[chosenDirection].x, (int)currentLocation.y + (int)piece.ValidDirectionList[chosenDirection].y] == 'X') hasAValidDirectionBeenFound = true;
                }
            }

            //if there was no errors with finding a movable direction
            if (!isBoardUnableToGenerate) {
                preTravelLocation = new Vector2(currentLocation.x, currentLocation.y);

                //for the number of directions the piece is able to travel
                for (int j = 0; j < validDirections; j++) {
                    currentDirection = piece.ValidDirectionList[j]; //set the current direction to be traveled

                    //for the number of spaces the piece is allowed to move
                    for (int k = 0; k < maxSpaces; k++) {
                        //if the piece is a knight
                        if (piece.PieceCharacter == 'K') {
                            //if the next space isn't a wall
                            if (_board[(int)currentLocation.x + (int)currentDirection.x, (int)currentLocation.y + (int)currentDirection.y] == '#') continue;
                            
                            //if the spot is free to travel
                            currentLocation = new Vector2((int)currentLocation.x + (int)currentDirection.x, (int)currentLocation.y + (int)currentDirection.y); //update the current location to the new found spot
                            
                            //if the knight is at the final spot, check both sides
                            if (k == maxSpaces - 1) {
                                switch (j) {
                                    //top mid
                                    case 0:
                                        //if the spot is free
                                        if (_board[(int)currentLocation.x + 1, (int)currentLocation.y] == 'O' || _board[(int)currentLocation.x + 1, (int)currentLocation.y] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x + 1, (int)currentLocation.y] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x + 1, currentLocation.y));
                                        }

                                        //if the spot is free
                                        if (_board[(int)currentLocation.x - 1, (int)currentLocation.y] == 'O' || _board[(int)currentLocation.x - 1, (int)currentLocation.y] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x - 1, (int)currentLocation.y] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x - 1, currentLocation.y));
                                        }
                                        break;

                                    //mid right
                                    case 1:
                                        //if the spot is free
                                        if (_board[(int)currentLocation.x, (int)currentLocation.y + 1] == 'O' || _board[(int)currentLocation.x, (int)currentLocation.y + 1] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x, (int)currentLocation.y + 1] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x, currentLocation.y + 1));
                                        }

                                        //if the spot is free
                                        if (_board[(int)currentLocation.x, (int)currentLocation.y - 1] == 'O' || _board[(int)currentLocation.x, (int)currentLocation.y - 1] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x, (int)currentLocation.y - 1] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x, currentLocation.y - 1));
                                        }

                                        break;

                                    //bottom mid
                                    case 2:
                                        //if the spot is free
                                        if (_board[(int)currentLocation.x + 1, (int)currentLocation.y] == 'O' || _board[(int)currentLocation.x + 1, (int)currentLocation.y] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x + 1, (int)currentLocation.y] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x + 1, currentLocation.y));
                                        }

                                        //if the spot is free
                                        if (_board[(int)currentLocation.x - 1, (int)currentLocation.y] == 'O' || _board[(int)currentLocation.x - 1, (int)currentLocation.y] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x - 1, (int)currentLocation.y] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x - 1, currentLocation.y));
                                        }
                                        break;

                                    //mid left
                                    case 3:
                                        //if the spot is free
                                        if (_board[(int)currentLocation.x, (int)currentLocation.y + 1] == 'O' || _board[(int)currentLocation.x, (int)currentLocation.y + 1] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x, (int)currentLocation.y + 1] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x, currentLocation.y + 1));
                                        }

                                        //if the spot is free
                                        if (_board[(int)currentLocation.x, (int)currentLocation.y - 1] == 'O' || _board[(int)currentLocation.x, (int)currentLocation.y - 1] == 'X') {
                                            //mark the position and add it to the availible positions list
                                            _board[(int)currentLocation.x, (int)currentLocation.y - 1] = 'X';
                                            piece.AvailablePositions.Add(new Vector2(currentLocation.x, currentLocation.y - 1));
                                        }
                                        break;
                                }
                            }
                        }

                        //if not a knight
                        else {
                            //if the space in the direction of the current direction is not empty, finish the code
                            if (_board[(int)currentLocation.x + (int)currentDirection.x, (int)currentLocation.y + (int)currentDirection.y] != 'O' && _board[(int)currentLocation.x + (int)currentDirection.x, (int)currentLocation.y + (int)currentDirection.y] != 'X') continue;
                            //if the spot is free to travel
                            currentLocation = new Vector2((int)currentLocation.x + (int)currentDirection.x, (int)currentLocation.y + (int)currentDirection.y); //update the current location to the new found spot

                            //if the current index matches the chosen direction's index (meaning this is the one intended valid direction)
                            if (j == chosenDirection) {
                                _board[(int)currentLocation.x, (int)currentLocation.y] = '!'; //mark it with a '!'
                                piece.AvailablePositions.Add(new Vector2(currentLocation.x, currentLocation.y)); //add the position to the piece's available spaces
                            }

                            //if the intended valid direction is not being mapped out
                            else {
                                //if there isn't a '!' at the current location, place a 'X'
                                if (_board[(int)currentLocation.x, (int)currentLocation.y] != '!')
                                    _board[(int)currentLocation.x, (int)currentLocation.y] = 'X'; //otherwise, mark it with an X
                            }
                        }
                    }
                    currentLocation = preTravelLocation;
                }
                
                //if the piece is a knight and there are no available positions, generate a new board
                if (piece.PieceCharacter == 'K' && piece.AvailablePositions.Count == 0) {
                    isBoardUnableToGenerate = true; //mark board as unable to generate
                    GenerateBoard(_numberOfPiecesToBePlaced); //generate a new board
                    break; //end this thread here
                }

                //choose a final position (vector 2) at a random index in the available positions list
                var finalPosition = piece.AvailablePositions[Random.Range(0, piece.AvailablePositions.Count)];

                //if this is not the final piece
                if (i != numberOfPieces - 1) {
                    piece = GetRandomPiece(); //get a random piece to place at the new location
                    _board[(int)finalPosition.x, (int)finalPosition.y] = piece.PieceCharacter; //place the piece at the final position 
                    currentLocation = finalPosition; //set the current location to the spot that was chosen 
                }

                //otherwise, place a king at the finalposition
                else {
                    _board[(int)finalPosition.x, (int)finalPosition.y] = '*'; //place the piece at the final position 
                }
            }

            //if there was a problem with finding a movable direction, generate a new board
            else {
                GenerateBoard(_numberOfPiecesToBePlaced); //generate a new board
                break; //end this thread here
            }
        }

        //build the board
        if (!isBoardUnableToGenerate) {
            PlaceNonEssentialPieces(_numberOfPiecesToBePlaced/2); //place the non essential pieces on the board
            BuildBoard(); //convert the board from a 2d char array to pieces 
        }
    }

    //this method places extra pieces to deceive the player
    private void PlaceNonEssentialPieces(int numberOfPieces) {
        char currentLocationChar = ' ';
        int currentX = 0, currentY = 0;
        int counter = 0;
        for (int i = 0; i < numberOfPieces; i++) {
            counter = 0;
            //while a valid spot for a non essential piece is not found
            while (currentLocationChar != 'X') {
                counter++;
                currentX = Random.Range(1, _board.GetLength(0));
                currentY = Random.Range(1, _board.GetLength(0));
                currentLocationChar = _board[currentX, currentY];
                if (counter == 100) break;
            }
            if (counter == 100) break;
            _board[currentX, currentY] = GetRandomPiece().PieceCharacter;
            currentLocationChar = ' '; //reset the variable
        }
    }
    
    //this method reads the 2d char array and places the chess piece in the correct pieces
    private void BuildBoard() {
        //Debug.Log(_firstPieceLocation.x + " " + _firstPieceLocation.y);
        //for the number of rows in the board
        for (int i = 1; i < _board.GetLength(0) - 1; i++) {
            //for the number of columns in the board
            for (int j = 1; j < _board.GetLength(1) - 1; j++) {
                //if the current index is a piece
                if (_board[i, j] != 'O' && _board[i, j] != 'X' && _board[i, j] != '!') {
                    var pieceInstance = Instantiate(ConvertCharToPrefab(_board[i, j]), Vector2.zero, Quaternion.identity); //place the piece in its place
                    pieceInstance.transform.SetParent(_boardObject.transform); //become a child of the board
                    pieceInstance.transform.localPosition = new Vector3(ConvertCharArrayCoordinatesToRealWorldSpace(i, j).x, ConvertCharArrayCoordinatesToRealWorldSpace(i, j).y, -1); //set the position of the piece
                    pieceInstance.transform.localScale = new Vector3(0.125f, 0.125f, 1f); //set the localscale of the piece
                    //if the location of the first piece matches the current location
                    if (i == (int)_firstPieceLocation.x && j == (int)_firstPieceLocation.y) {
                        SetCurrentPiece(pieceInstance); //set the current piece to this piece if the starting piece location matches
                        StartCoroutine(ScanDirection(ConvertCharToValidPieceDirections(_board[i, j]))); //scan in the valid directions of the piece
                    }
                }
            }
        }
    }

    //this method converts a given char to its corresponding prefab
    private GameObject ConvertCharToPrefab(char piece) {
        switch (piece) {
            case 'Q': return _queenPrefab; //queen
            case 'B': return _bishopPrefab; //bishop
            case 'P': return _pawnPrefab; //pawn
            case 'K': return _knightPrefab; //knight
            case 'R': return _rookPrefab; //rook
            case '*': return _kingPrefab; //king
        }

        return null; //if nothing was caught, return null
    }

    //this method converts a given char to its corresponding piece's valid directions
    private Vector2[] ConvertCharToValidPieceDirections(char piece) {
        ChessPiece tempPiece;
        switch (piece) {
            case 'Q':
                tempPiece = new ChessPiece(ChessPiece.Pieces.Queen);
                return tempPiece.ValidDirectionList; //queen
            case 'B':
                tempPiece = new ChessPiece(ChessPiece.Pieces.Bishop);
                return tempPiece.ValidDirectionList; //bishop
            case 'P':
                tempPiece = new ChessPiece(ChessPiece.Pieces.Pawn);
                return tempPiece.ValidDirectionList; //pawn
            case 'K':
                tempPiece = new ChessPiece(ChessPiece.Pieces.Knight);
                return tempPiece.ValidDirectionList; //knight
            case 'R':
                tempPiece = new ChessPiece(ChessPiece.Pieces.Rook);
                return tempPiece.ValidDirectionList; //rook
        }

        return null; //if nothing was caught, return null
    }

    //convert the given char array index to a vector2 in real world space (relative to the child position of the board)
    private Vector2 ConvertCharArrayCoordinatesToRealWorldSpace(int x, int y) {
        float realX, realY;

        //the x coordinate
        switch (x) {
            case 1:
                realX = -0.4375f;
                break;
            case 2:
                realX = -0.3125f;
                break;
            case 3:
                realX = -0.1875f;
                break;
            case 4:
                realX = -0.0625f;
                break;
            case 5:
                realX = 0.0625f;
                break;
            case 6:
                realX = 0.1875f;
                break;
            case 7:
                realX = 0.3125f;
                break;
            case 8:
                realX = 0.4375f;
                break;
            default:
                realX = 0f;
                break;
        }

        //the y coordinate
        switch (y) {
            case 1:
                realY = 0.4375f;
                break;
            case 2:
                realY = 0.3125f;
                break;
            case 3:
                realY = 0.1875f;
                break;
            case 4:
                realY = 0.0625f;
                break;
            case 5:
                realY = -0.0625f;
                break;
            case 6:
                realY = -0.1875f;
                break;
            case 7:
                realY = -0.3125f;
                break;
            case 8:
                realY = -0.4375f;
                break;
            default:
                realY = 0f;
                break;
        }

        //return the new vector 2 with the coordinates
        return new Vector2(realX, realY);
    }

    //get a random chess piece
    private ChessPiece GetRandomPiece() {
        return new ChessPiece((ChessPiece.Pieces)Random.Range(0, 5)); //return a random chess piece (no knight for now)
    }

    //load seed with optional parameter (if no parameter is entered, seed = 0 (random))
    private void LoadSeed(int seed = default(int)) => Random.InitState(seed);

    int NumberOfXs() {
        int counter = 0;
        //for the number of rows in the board
        for (int i = 1; i < _board.GetLength(0) - 1; i++) {
            //for the number of columns in the board
            for (int j = 1; j < _board.GetLength(1) - 1; j++) {
                if (_board[i, j] == 'X') counter++;
            }
        }
        return counter;
    }
}