using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Lofelt.NiceVibrations;

public class Mastery : MonoBehaviour{
    [SerializeField] private Material[] _savedPieceMaterial;
    [SerializeField] private Material[] _masteryMaterials;
    [SerializeField] private Material _currentMaterial;
    [SerializeField] private Button _masterySkinSwitchButton, _applyButton;
    [SerializeField] private Image _applyButtonCover, _pieceImage;
    [SerializeField] private GameObject[] _piecePrefabs;
    [SerializeField] private TMP_Text _masteryText, _masteryErrorMessage;
    
    private int _pawnMasteryLevel, _bishopMasteryLevel, _knightMasteryLevel, _rookMasteryLevel, _queenMasteryLevel, _kingMasteryLevel;
    private int _pawnSelection, _bishopSelection, _knightSelection, _rookSelection, _queenSelection, _kingSelection;

    private void Start() {
        //load saved materials
        _pawnSelection = PlayerPrefs.GetInt("pawnMaterial", 0);
        _bishopSelection = PlayerPrefs.GetInt("bishopMaterial", 0);
        _knightSelection = PlayerPrefs.GetInt("knightMaterial", 0);
        _rookSelection = PlayerPrefs.GetInt("rookMaterial", 0);
        _queenSelection = PlayerPrefs.GetInt("queenMaterial", 0);
        _kingSelection = PlayerPrefs.GetInt("kingMaterial", 0);
        
        //load mastery levels
        _pawnMasteryLevel = PlayerPrefs.GetInt("pawnMastery", 0) / 10;
        _bishopMasteryLevel = PlayerPrefs.GetInt("bishopMastery", 0) / 10;
        _knightMasteryLevel = PlayerPrefs.GetInt("knightMastery", 0) / 10;
        _rookMasteryLevel = PlayerPrefs.GetInt("rookMastery", 0) / 10;
        _queenMasteryLevel = PlayerPrefs.GetInt("queenMastery", 0) / 10;
        _kingMasteryLevel = PlayerPrefs.GetInt("kingMastery", 0) / 10;

        _masterySkinSwitchButton.onClick.AddListener(OnMasterySkinSwitchButtonPressed);
        //CheckUnlockAndSetApplyButton();
    }

    void OnMasterySkinSwitchButtonPressed() {
        if (GetCurrentPieceSelection() < _masteryMaterials.Length-1) IncrementCurrentPieceSelection();
        else ResetCurrentPieceSelection();
    }
    
    enum Piece{
        Pawn, Bishop, Knight, Rook, Queen, King
    };

    private Piece _currentPiece;

    //this method sets the current piece (called from the menu script)
    public void SetCurrentPiece(int masteryIndex) {

        _currentPiece = (Piece)masteryIndex; //set the current piece 
        _currentMaterial = _savedPieceMaterial[masteryIndex]; //switch the display material to the saved piece material
        
        switch ((Piece)masteryIndex) {
            case Piece.Pawn:
                _currentMaterial = _masteryMaterials[_pawnSelection];
                break;
            case Piece.Bishop:
                _currentMaterial = _masteryMaterials[_bishopSelection];
                break;
            case Piece.Knight:
                _currentMaterial = _masteryMaterials[_knightSelection];
                break;
            case Piece.Rook:
                _currentMaterial = _masteryMaterials[_rookSelection];
                break;
            case Piece.Queen:
                _currentMaterial = _masteryMaterials[_queenSelection];
                break;
            case Piece.King:
                _currentMaterial = _masteryMaterials[_kingSelection];
                break;
        }
        UpdateUnlockText(_currentPiece);
        UpdateImageCurrentMaterial();
        CheckUnlockAndSetApplyButton();
    }

    public void OnMasteryClose() {
        _currentPiece = Piece.Pawn;
         
        //load saved materials
        _pawnSelection = PlayerPrefs.GetInt("pawnMaterial", 0);
        _bishopSelection = PlayerPrefs.GetInt("bishopMaterial", 0);
        _knightSelection = PlayerPrefs.GetInt("knightMaterial", 0);
        _rookSelection = PlayerPrefs.GetInt("rookMaterial", 0);
        _queenSelection = PlayerPrefs.GetInt("queenMaterial", 0);
        _kingSelection = PlayerPrefs.GetInt("kingMaterial", 0);
        
        _currentMaterial = _masteryMaterials[_pawnSelection];
        _pieceImage.material = _currentMaterial;
        
        //load mastery levels
        _pawnMasteryLevel = PlayerPrefs.GetInt("pawnMastery", 0) / 10;
        _bishopMasteryLevel = PlayerPrefs.GetInt("bishopMastery", 0) / 10;
        _knightMasteryLevel = PlayerPrefs.GetInt("knightMastery", 0) / 10;
        _rookMasteryLevel = PlayerPrefs.GetInt("rookMastery", 0) / 10;
        _queenMasteryLevel = PlayerPrefs.GetInt("queenMastery", 0) / 10;
        _kingMasteryLevel = PlayerPrefs.GetInt("kingMastery", 0) / 10;
    }
    
    void UpdateUnlockText(Piece piece) {
        switch (_currentPiece) {
            case Piece.Pawn:
                _masteryText.text = PlayerPrefs.GetInt("pawnMastery") + "/" + ((_pawnSelection)*10);
                break;
            case Piece.Bishop:
                _masteryText.text = PlayerPrefs.GetInt("bishopMastery") + "/" + ((_bishopSelection)*10);
                break;
            case Piece.Knight:
                _masteryText.text = PlayerPrefs.GetInt("knightMastery") + "/" + ((_knightSelection)*10);
                break;
            case Piece.Rook:
                _masteryText.text = PlayerPrefs.GetInt("rookMastery") + "/" + ((_rookSelection)*10);
                break;
            case Piece.Queen:
                _masteryText.text = PlayerPrefs.GetInt("queenMastery") + "/" + ((_queenSelection)*10);
                break;
            case Piece.King:
                _masteryText.text = PlayerPrefs.GetInt("kingMastery") + "/" + ((_kingSelection)*10);
                break;
        }
    }

    //save the mastery selection
    public void SaveMasterySelection() {
        //load saved materials
        PlayerPrefs.SetInt("pawnMaterial", _pawnSelection);
        PlayerPrefs.SetInt("bishopMaterial", _bishopSelection);
        PlayerPrefs.SetInt("knightMaterial", _knightSelection);
        PlayerPrefs.SetInt("rookMaterial", _rookSelection);
        PlayerPrefs.SetInt("queenMaterial", _queenSelection);
        PlayerPrefs.SetInt("kingMaterial", _kingSelection);

        _piecePrefabs[0].GetComponent<SpriteRenderer>().material = _masteryMaterials[_pawnSelection];//_savedPieceMaterial[0];
        _piecePrefabs[1].GetComponent<SpriteRenderer>().material = _masteryMaterials[_bishopSelection];//_savedPieceMaterial[1];
        _piecePrefabs[2].GetComponent<SpriteRenderer>().material = _masteryMaterials[_knightSelection];//_savedPieceMaterial[2];
        _piecePrefabs[3].GetComponent<SpriteRenderer>().material = _masteryMaterials[_rookSelection];//_savedPieceMaterial[3];
        _piecePrefabs[4].GetComponent<SpriteRenderer>().material = _masteryMaterials[_queenSelection];//_savedPieceMaterial[4];
        _piecePrefabs[5].GetComponent<SpriteRenderer>().material = _masteryMaterials[_kingSelection];//_savedPieceMaterial[5];
    }
    
    //load the mastery selection
    public void LoadMasterySelection() {
        _savedPieceMaterial[0] = _masteryMaterials[PlayerPrefs.GetInt("pawnMaterial", 0)]; //pawn
        _savedPieceMaterial[1] = _masteryMaterials[PlayerPrefs.GetInt("bishopMaterial", 0)]; //bishop
        _savedPieceMaterial[2] = _masteryMaterials[PlayerPrefs.GetInt("knightMaterial", 0)]; //knight
        _savedPieceMaterial[3] = _masteryMaterials[PlayerPrefs.GetInt("rookMaterial", 0)]; //rook
        _savedPieceMaterial[4] = _masteryMaterials[PlayerPrefs.GetInt("queenMaterial", 0)]; //queen
        _savedPieceMaterial[5] = _masteryMaterials[PlayerPrefs.GetInt("kingMaterial", 0)]; //king
    }

    //save mastery materials 
    private void SaveMasteryMaterials() {
        _savedPieceMaterial[(int)_currentPiece] = _masteryMaterials[GetCurrentPieceSelection()];
    }

    private int GetCurrentPieceSelection() {
        switch (_currentPiece) {
            case Piece.Pawn:
                return _pawnSelection;
                break;
            case Piece.Bishop:
                return _bishopSelection;
                break;
            case Piece.Knight:
                return _knightSelection;
                break;
            case Piece.Rook:
                return _rookSelection;
                break;
            case Piece.Queen:
                return _queenSelection;
                break;
            case Piece.King:
                return _kingSelection;
                break;
        }
        return 0;
    }

    private void IncrementCurrentPieceSelection() {
        PlayButtonFeedback();
        switch (_currentPiece) {
            case Piece.Pawn:
                _pawnSelection++;
                _currentMaterial = _masteryMaterials[_pawnSelection];
                break;
            case Piece.Bishop:
                _bishopSelection++;
                _currentMaterial = _masteryMaterials[_bishopSelection];
                break;
            case Piece.Knight:
                _knightSelection++;
                _currentMaterial = _masteryMaterials[_knightSelection];
                break;
            case Piece.Rook:
                _rookSelection++;
                _currentMaterial = _masteryMaterials[_rookSelection];
                break;
            case Piece.Queen:
                _queenSelection++;
                _currentMaterial = _masteryMaterials[_queenSelection];
                break;
            case Piece.King:
                _kingSelection++;
                _currentMaterial = _masteryMaterials[_kingSelection];
                break;
        }

        UpdateUnlockText(_currentPiece);
        UpdateImageCurrentMaterial();
        CheckUnlockAndSetApplyButton();
    }

    private void ResetCurrentPieceSelection() {
        switch (_currentPiece) {
            case Piece.Pawn:
                _pawnSelection = 0;
                _currentMaterial = _masteryMaterials[_pawnSelection];
                break;
            case Piece.Bishop:
                _bishopSelection = 0;
                _currentMaterial = _masteryMaterials[_bishopSelection];
                break;
            case Piece.Knight:
                _knightSelection = 0;
                _currentMaterial = _masteryMaterials[_knightSelection];
                break;
            case Piece.Rook:
                _rookSelection = 0;
                _currentMaterial = _masteryMaterials[_rookSelection];
                break;
            case Piece.Queen:
                _queenSelection = 0;
                _currentMaterial = _masteryMaterials[_queenSelection];
                break;
            case Piece.King:
                _kingSelection = 0;
                _currentMaterial = _masteryMaterials[_kingSelection];
                break;
        }
        PlayButtonFeedback();
        UpdateUnlockText(_currentPiece);
        UpdateImageCurrentMaterial();
        CheckUnlockAndSetApplyButton();
    }

    void UpdateImageCurrentMaterial() {
        _pieceImage.material = _currentMaterial;
    }
    
    //are all selected mastery 
    bool AreAllSelectedMasteryDesignsUnlocked() {
        //if atleast one of the selected mastery designs are locked, return false
        if (_pawnSelection > _pawnMasteryLevel || _bishopSelection > _bishopMasteryLevel || _knightSelection > _knightMasteryLevel || _rookSelection > _rookMasteryLevel || _queenSelection > _queenMasteryLevel || _kingSelection > _kingMasteryLevel) return false;
        return true; //otherwise, return true
    }

    public void CheckUnlockAndSetApplyButton() {
        if (AreAllSelectedMasteryDesignsUnlocked()) {
            EnableApplyButton();
            _masteryErrorMessage.text = "";
        }
        else {
            DisableApplyButton();
            _masteryErrorMessage.text = "At least one mastery effect is still locked.";
        }
    }
    
    void DisableApplyButton() {
        _applyButtonCover.enabled = true;
        _applyButton.interactable = false;
    }

    void EnableApplyButton() {
        _applyButtonCover.enabled = false;
        _applyButton.interactable = true;
    }

    int GetNextUnlockRequirement(int currentMastery) {
        int maxMastery = _masteryMaterials.Length * 10; //the number of shards of the highest mastery level
        if (currentMastery >= maxMastery) return maxMastery; //if max level, return early
        
        for (int i = 1; i < _masteryMaterials.Length; i++) { //for all materials
            if (i * 10 > currentMastery) return i * 10; //if milestone is greater, return it
        }

        return 0; //return 0 (this should never be called)
    }
    
    void PlayButtonFeedback() {
        SoundManager.PlaySound("button"); //play the button sound
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact); //play vibration
    }
}
