using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece {
    //the number of directions a piece is able to travel in
    public int VaildDirections { get; set; }

    //the maximum number of spaces a pieces is able to travel
    public int MaxSpaces { get; set; }

    //the character that is used to represent the piece
    public char PieceCharacter { get; set; }

    //the list of directions the piece can travel (-1 = left/down, 1 = right/up)
    public Vector2[] ValidDirectionList { get; set; }

    //the list of potential spaces the piece can move on
    public List<Vector2> AvailablePositions { get; set;}

    //the pieces
    public enum Pieces {
        Bishop, Queen, Rook, Pawn, Knight
    }

    //the chess piece class
    public ChessPiece(Pieces piece) {
        InitializePiece(piece); //initialize the piece's variables
        AvailablePositions = new List<Vector2>(); //initialize the list
    }

    private void InitializePiece(Pieces piece) {
        //switch case for the pieces
        switch (piece) {
            //if the piece is a queen
            case Pieces.Queen:
                VaildDirections = 8; //set valid directions
                MaxSpaces = 7; //set max spaces
                PieceCharacter = 'Q';
                ValidDirectionList = new Vector2[]{
                    new Vector2(-1, 1), //top left
                    new Vector2(0,1), //top mid
                    new Vector2(1,1), //top right
                    new Vector2(1,0), //mid right
                    new Vector2(1,-1), //bottom right
                    new Vector2(0,-1), //bottom mid
                    new Vector2(-1,-1), //bottom left
                    new Vector2(-1,0) //left mid
                };
                break;

            //if the piece is a bishop
            case Pieces.Bishop:
                VaildDirections = 4; //set valid directions
                MaxSpaces = 7; //set max spaces
                PieceCharacter = 'B';
                ValidDirectionList = new Vector2[]{
                    new Vector2(-1, 1), //top left
                    new Vector2(1,1), //top right
                    new Vector2(1,-1), //bottom right
                    new Vector2(-1,-1) //bottom left
                };
                break;

            //if the piece is a rook
            case Pieces.Rook:
                VaildDirections = 4; //set valid directions
                MaxSpaces = 7; //set max spaces
                PieceCharacter = 'R';
                ValidDirectionList = new Vector2[]{
                    new Vector2(0,1), //top mid
                    new Vector2(1,0), //mid right
                    new Vector2(0,-1), //bottom mid
                    new Vector2(-1,0) //left mid
                };
                break;

            //if the piece is a knight
            case Pieces.Knight:
                VaildDirections = 4; //set valid directions
                MaxSpaces = 2; //set max spaces
                PieceCharacter = 'K';
                ValidDirectionList = new Vector2[]{
                    new Vector2(0,1), //top mid
                    new Vector2(1,0), //mid right
                    new Vector2(0,-1), //bottom mid
                    new Vector2(-1,0) //left mid
                };
                break;

            //if the piece is a pawn
            case Pieces.Pawn:
                VaildDirections = 2; //set valid directions
                MaxSpaces = 1; //set max spaces
                PieceCharacter = 'P';
                ValidDirectionList = new Vector2[]{
                    new Vector2(0,1), //top mid
                    //new Vector2(1,0), //mid right
                    new Vector2(0,-1), //bottom mid
                    //new Vector2(-1,0) //left mid
                };
                break;
        }
    }
}
