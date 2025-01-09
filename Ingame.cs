using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lofelt.NiceVibrations;
using UnityEngine.PlayerLoop;

public class Ingame : MonoBehaviour
{
    public enum ChessPiece{Pawn, Bishop, Rook, Queen, Knight}
    [SerializeField] TMP_Text _levelText, _statusText, _tutorialPageNumberText, _animatedTutorialStatus;
    [SerializeField] private Button _nextButton, _homeButton, _restartButton, _homeWinIconButton, _endGameRestartButton, _homeLoseIconButton, _leftTutorialNavigationButton, _rightTutorialNavigationButton, _tutorialCloseButton;
    private int _numberOfPiecesToBeKilled;
    [SerializeField] private Canvas _endgameWinCanvas, _endgameLoseCanvas, _tutorialCanvas, _masteryCanvas, _masteryCanvasForeground;
    [SerializeField] private ParticleSystem _confettiParticleSystem, _masteryParticleSystem;
    private int _level, _piecesSlayed = -1, _tutorialPage;
    private int _pawnMastery, _bishopMastery, _knightMastery, _rookMastery, _queenMastery, _masteryTapInput;
    private Color32 _primaryColor, _secondaryColor;
    [SerializeField] private Image _masteryPieceImage;
    [SerializeField] private Image[] _tutorialPageCanvas;
    [SerializeField] private Sprite[] _chessSprites; //pawn, bishop, knight, rook, queen
    private bool _isWinBeingDelayed, _hasStatusFlashAnimationStarted;

    // Start is called before the first frame update
    void Start() {
        _primaryColor = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Theme>().PrimaryColor;
        _secondaryColor = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Theme>().SecondaryColor;
        
        _level = PlayerPrefs.GetInt("level", 1); //load the saved level

        _levelText.text = "LEVEL: " + _level;
        
        UpdateStatusText(); //update the status text
        
        //the button handlers
        _nextButton.onClick.AddListener(OnNextButtonPressed);
        _homeButton.onClick.AddListener(OnHomeButtonPressed);
        _restartButton.onClick.AddListener(OnRestartButtonPressed);
        _homeWinIconButton.onClick.AddListener(OnHomeIconButtonPressed);
        _endGameRestartButton.onClick.AddListener(OnRestartButtonPressed);
        _leftTutorialNavigationButton.onClick.AddListener(()=>TutorialNavigationButtonPressed(0));
        _rightTutorialNavigationButton.onClick.AddListener(()=>TutorialNavigationButtonPressed(1));
        _tutorialCloseButton.onClick.AddListener(OnTutorialCloseButtonPressed);
        
        //fail
        _endGameRestartButton.onClick.AddListener(OnRetryButtonPressed);
        _homeLoseIconButton.onClick.AddListener(OnHomeButtonPressed);

        //show tutorial on first play
        if (PlayerPrefs.GetInt("hasTutorialBeenShown", 0) == 0) {
            ShowTutorial();
        }
        
        SetDelayedWin(false);
        _hasStatusFlashAnimationStarted = false;
    }

    void Update() {
        //if there's atleast one touch on the screen
        if (Input.touchCount == 1) {
            //save the first touch
            var touch = Input.GetTouch(0); //set the touch to the first one

            //if the touch ended and the mastery canvas is enabled, disable it
            if (touch.phase == TouchPhase.Ended && _masteryCanvas.enabled) {
                _masteryTapInput--; //decrement
                if (_masteryTapInput < 1) {
                    PlayButtonFeedback();
                    _masteryCanvas.enabled = false;
                    _masteryCanvasForeground.enabled = false;
                    _masteryParticleSystem.Stop();
                    if (_isWinBeingDelayed) WinLevel(); //if win is being delayed, win after closing
                }
            }
        }
    }

    //this method starts the infinite flash animation
    public void StartStatusFlashAnimation() {
        _statusText.DOColor(_secondaryColor, 0.25f).OnComplete(() => { _statusText.DOColor(_primaryColor, 0.25f).OnComplete(StartStatusFlashAnimation); });
    }
    
    void StartTutorialStatusFlashAnimation() {
        if (_tutorialPage == 3) {
            _animatedTutorialStatus.DOColor(_secondaryColor, 0.25f).OnComplete(() => { _animatedTutorialStatus.DOColor(_primaryColor, 0.25f).OnComplete(StartTutorialStatusFlashAnimation); });
        }
    }
    
    //set delayed win
    public void SetDelayedWin(bool value) {
        _isWinBeingDelayed = value; //delay the win
    }

    //this method shows the tutorial
    void ShowTutorial() {
        _tutorialPage = 0;
        _tutorialCloseButton.gameObject.SetActive(false);
        DisableAllTutorialPages();
        _leftTutorialNavigationButton.gameObject.SetActive(false);
        _tutorialPageNumberText.text = _tutorialPage + 1 + "/" + _tutorialPageCanvas.Length;
        _tutorialPageCanvas[_tutorialPage].gameObject.SetActive(true);
        _tutorialCanvas.enabled = true;
    }
    
    //this is the navigation button controller for the tutorial
    void TutorialNavigationButtonPressed(int direction) {
        PlayButtonFeedback();
        if (direction == 0) _tutorialPage--;
        else _tutorialPage++;
        DisableAllTutorialPages(); //disable all pages
        _tutorialPageCanvas[_tutorialPage].gameObject.SetActive(true); //enable the current page
        if (_tutorialPage == 0) _leftTutorialNavigationButton.gameObject.SetActive(false); 
        else _leftTutorialNavigationButton.gameObject.SetActive(true); 
        if (_tutorialPage == _tutorialPageCanvas.Length - 1){ _rightTutorialNavigationButton.gameObject.SetActive(false); _tutorialCloseButton.gameObject.SetActive(true);}
        else{ _rightTutorialNavigationButton.gameObject.SetActive(true); _tutorialCloseButton.gameObject.SetActive(false);}
        _tutorialPageNumberText.text = _tutorialPage + 1 + "/" + _tutorialPageCanvas.Length;
        if (_tutorialPage == 3) StartTutorialStatusFlashAnimation();
    }

    //when the tutorial close button is pressed
    void OnTutorialCloseButtonPressed() {
        PlayButtonFeedback();
        _tutorialCanvas.enabled = false;
        if (PlayerPrefs.GetInt("hasTutorialBeenShown",0) == 0) PlayerPrefs.SetInt("hasTutorialBeenShown",1);
        GameManager.Instance.ShowBannerAd(); //show a banner ad
    }
    
    //this method disables all tutorial pages
    void DisableAllTutorialPages() {
        for (int i = 0; i < _tutorialPageCanvas.Length; i++) {
            _tutorialPageCanvas[i].gameObject.SetActive(false);
        }
    }

    //set the kill objective
    public void SetKillObjective(int number) {
        _numberOfPiecesToBeKilled = number-1; //the number of pieces to be killed
    }
    
    //decrement kill objective
    public void DecrementKillObjective() {
        _numberOfPiecesToBeKilled--; //decrement
        UpdateStatusText(); //update the status text
        if (!_hasStatusFlashAnimationStarted && _numberOfPiecesToBeKilled < 0) StartStatusFlashAnimation();
        else PlayRequirementTextFeedback();
    }

    public void CheckForMasteryPiece(string chessPiece, bool isAllowedToControlWin) {
        if (_numberOfPiecesToBeKilled < 0) {
            switch (chessPiece) {
                case "pawn":
                    _pawnMastery++;
                    _masteryPieceImage.sprite = _chessSprites[0];
                    break;
                case "bishop":
                    _bishopMastery++;
                    _masteryPieceImage.sprite = _chessSprites[1];
                    break;
                case "knight":
                    _knightMastery++;
                    _masteryPieceImage.sprite = _chessSprites[2];
                    break;
                case "rook":
                    _rookMastery++;
                    _masteryPieceImage.sprite = _chessSprites[3];
                    break;
                case "queen":
                    _queenMastery++;
                    _masteryPieceImage.sprite = _chessSprites[4];
                    break;
            }
            _masteryCanvas.enabled = true;
            _masteryCanvasForeground.enabled = true;
            _masteryParticleSystem.Play();
            SoundManager.PlaySound("mastery");
            _masteryTapInput = 2;
            if (isAllowedToControlWin) SetDelayedWin(true); //set a delayed win
        }
    }

    //save the mastery progress to memory
    private void SaveMasteryToMemory() {
        PlayerPrefs.SetInt("pawnMastery", PlayerPrefs.GetInt("pawnMastery",0) + _pawnMastery);
        PlayerPrefs.SetInt("bishopMastery", PlayerPrefs.GetInt("bishopMastery",0) + _bishopMastery);
        PlayerPrefs.SetInt("knightMastery", PlayerPrefs.GetInt("knightMastery",0) + _knightMastery);
        PlayerPrefs.SetInt("rookMastery", PlayerPrefs.GetInt("rookMastery",0) + _rookMastery);
        PlayerPrefs.SetInt("queenMastery", PlayerPrefs.GetInt("queenMastery",0) + _queenMastery);
        PlayerPrefs.SetInt("kingMastery", PlayerPrefs.GetInt("kingMastery", 0) + _pawnMastery + _bishopMastery + _knightMastery + _rookMastery + _queenMastery);
    }
    
    //check for completion
    public void CheckForCompletion() {
        //if there are no more pieces to be killed
        if (_numberOfPiecesToBeKilled < 1 && !_isWinBeingDelayed) {
            WinLevel(); //end the level in a win
        }
        //otherwise
        else if (!_isWinBeingDelayed){
            LoseLevel(); //end the level in a loss
        }
    }

    //this method is called when the level ends in a win
    void WinLevel() {
        SoundManager.PlaySound("win"); //play the win sound
        PlayerPrefs.SetInt("level",_level + 1); //set the new level int
        SaveMasteryToMemory(); //save the mastery to memory
        PlayerPrefs.SetInt("slayedPieces", (PlayerPrefs.GetInt("slayedPieces",0)+_piecesSlayed));
        _endgameWinCanvas.enabled = true; //enable the endgame canvas
        _confettiParticleSystem.Play(); //play the confetti particles
    }

    //this method is called when the level ends in a loss
    void LoseLevel() {
        SoundManager.PlaySound("fail"); //play the win sound
        _endgameLoseCanvas.enabled = true;
    }

    void OnNextButtonPressed() {
        SceneManager.LoadScene("ingame"); //load the same scene
        PlayButtonFeedback(); //play the button feedbacks
    }

    void OnHomeButtonPressed() {
        SceneManager.LoadScene("mainMenu"); //load the main menu
        PlayButtonFeedback(); //play the button feedbacks
    }

    void OnRestartButtonPressed() {
        PlayButtonFeedback(); //play the button feedbacks
        SceneManager.LoadScene("ingame"); //reload the scene
    }
    
    void OnHomeIconButtonPressed() {
        SceneManager.LoadScene("mainMenu"); //load the main menu
        PlayButtonFeedback(); //play the button feedbacks
    }

    void OnRetryButtonPressed() {
        SceneManager.LoadScene("ingame"); //reload the scene
        PlayButtonFeedback(); //play the button feedbacks
    }

    //update the text of the status
    void UpdateStatusText() {
        _piecesSlayed++; //increment the counter of pieces slayed
        
        //if there is only 1 more piece required to be slain
        if (_numberOfPiecesToBeKilled == 1) {
            _statusText.text = _statusText.text = "SLAY " + "1" + " MORE PIECE";
        }
        //if there are more than 0 pieces required to be slain
        else if (_numberOfPiecesToBeKilled > 0)
            _statusText.text = _statusText.text = "SLAY " + _numberOfPiecesToBeKilled + " MORE PIECES";
        //if the player has slain the required amount of pieces
        else _statusText.text = "SLAY THE KING!";
    }
    
    //this method disables all highlights
    public void DisableAllHighlights() {
        var allPawns = GameObject.FindGameObjectsWithTag("pawn");
        var allBishops = GameObject.FindGameObjectsWithTag("bishop");
        var allRooks = GameObject.FindGameObjectsWithTag("rook");
        var allKnights = GameObject.FindGameObjectsWithTag("knight");
        var allQueens = GameObject.FindGameObjectsWithTag("queen");
        var king = GameObject.FindGameObjectsWithTag("king");
        var allPieces = allPawns.Concat(allBishops.Concat(allRooks.Concat(allKnights.Concat(allQueens.Concat(king))))).ToArray();

        foreach (GameObject piece in allPieces) {
            if (piece.transform.name=="highlight") piece.GetComponent<SpriteRenderer>().enabled = false; //disable the sprite renderer
        }
    }

    public void HighlightKing() {
        var kingRenderers = GameObject.FindGameObjectsWithTag("king");
        foreach (GameObject king in kingRenderers)
        {
            if (king.transform.name == "highlight") {
                var kingRenderer = king.GetComponent<SpriteRenderer>();
                kingRenderer.color = Color.green;
                kingRenderer.enabled = true;
            }
        }
    }
    
    void PlayButtonFeedback() {
        SoundManager.PlaySound("button"); //play the button sound
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact); //play vibration
    }

    //play the requirement feedback
    void PlayRequirementTextFeedback() {
        _statusText.DOColor(_secondaryColor, 0.25f).OnComplete(() => { _statusText.DOColor(_primaryColor, 0.25f);});
    }
}
