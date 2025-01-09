using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lofelt.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lofelt.NiceVibrations;

public class Menu : MonoBehaviour{
    private Theme _theme;
    private Mastery _mastery;
    [SerializeField] private TMP_Text _kingsSlayedText, _piecesSlayedText, _masteryShardsText, _themeErrorText;
    [SerializeField] private Button _playButton, _themeButton, _settingsButton, _trophyButton, _statsBackButton, _soundOnButton, _soundOffButton, _vibrationOnButton, _vibrationOffButton, _shakeOnButton, _shakeOffButton, _configBackButton, _primaryCycleButton, _secondaryCycleButton, _themeCloseButton, _themeApplyButton, _privacyButton, _masteryButton, _masteryCycleRightButton, _masteryCycleLeftButton, _masteryCloseButton, _masteryToThemeSwitchButton, _masteryApplyButton;
    [SerializeField] private Canvas _statsCanvas, _configCanvas, _themeCanvas, _masteryCanvas;
    [SerializeField] private Image _masteryPieceImage;
    [SerializeField] private Sprite[] _masteryPieceSprites; //pawn, bishop, knight, rook, queen, king
    private int _temporaryPrimaryTheme, _temporarySecondaryTheme, _kingSlays, _totalMasteryShards;
    public enum MasteryPieces{Pawn, Bishop, Knight, Rook, Queen, King}
    public MasteryPieces _currentMasteryPiece = MasteryPieces.Pawn;
    
    // Start is called before the first frame update
    void Start() {
        _kingSlays = PlayerPrefs.GetInt("level", 1) - 1;
        _theme = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Theme>(); //get the reference to the theme script
        _mastery = GetComponent<Mastery>();
        _mastery.LoadMasterySelection();
        
        //menu
        _playButton.onClick.AddListener(OnPlayButtonPressed);
        _themeButton.onClick.AddListener(OnThemeButtonPressed);
        _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        _trophyButton.onClick.AddListener(OnTrophyButtonPressed);
        
        //stats 
        _totalMasteryShards = PlayerPrefs.GetInt("pawnMastery", 0) + 
                              PlayerPrefs.GetInt("bishopMastery", 0) + 
                              PlayerPrefs.GetInt("knightMastery", 0) + 
                              PlayerPrefs.GetInt("rookMastery", 0) + 
                              PlayerPrefs.GetInt("queenMastery", 0);
            
        _kingsSlayedText.text = (PlayerPrefs.GetInt("level", 1) - 1).ToString(); //load the kings slayed stat
        _piecesSlayedText.text = PlayerPrefs.GetInt("slayedPieces", 0).ToString();
        _masteryShardsText.text = _totalMasteryShards.ToString();
        _statsBackButton.onClick.AddListener(OnStatsBackButtonPressed);

        //config
        _soundOnButton.onClick.AddListener(SoundOnButtonPressed);
        _soundOffButton.onClick.AddListener(SoundOffButtonPressed);
        _vibrationOnButton.onClick.AddListener(VibrationOnButtonPressed);
        _vibrationOffButton.onClick.AddListener(VibrationOffButtonPressed);
        _shakeOnButton.onClick.AddListener(ScreenShakeOnButtonPressed);
        _shakeOffButton.onClick.AddListener(ScreenShakeOffButtonPressed);
        _configBackButton.onClick.AddListener(OnConfigBackButtonPressed);
        _privacyButton.onClick.AddListener(OnPrivacyButtonPressed);
        
        //theme
        _primaryCycleButton.onClick.AddListener(OnPrimaryCycleButtonPressed);
        _secondaryCycleButton.onClick.AddListener(OnSecondaryCycleButtonPressed);
        _themeApplyButton.onClick.AddListener(OnThemeApplyButtonPressed);
        _themeCloseButton.onClick.AddListener(OnThemeCloseButtonPressed);
        _masteryButton.onClick.AddListener(OnMasteryButtonPressed);

        //mastery
        _masteryButton.onClick.AddListener(OnMasteryButtonPressed);
        _masteryCycleLeftButton.onClick.AddListener(()=>OnMasteryCycleButtonPressed(0));
        _masteryCycleRightButton.onClick.AddListener(()=>OnMasteryCycleButtonPressed(1));
        _masteryCloseButton.onClick.AddListener(OnMateryCloseButtonPressed);
        _masteryToThemeSwitchButton.onClick.AddListener(OnThemeButtonPressed);
        _mastery.SetCurrentPiece((int)_currentMasteryPiece); //set the mastery piece
        _masteryApplyButton.onClick.AddListener(OnMasteryApplyButtonPressed);
    }

    void OnPlayButtonPressed() {
        SceneManager.LoadScene("ingame"); //load the ingame scene
        PlayButtonFeedback(); //play the button feedback
    }

    void OnSettingsButtonPressed() {
        _configCanvas.enabled = true;
        PlayButtonFeedback(); //play the button feedback
    }

    void OnPrivacyButtonPressed() {
        GameManager.Instance.ShowGdprPopup(); //show the GDPR popup
        PlayButtonFeedback(); //play the button feedback
    }

    void OnTrophyButtonPressed() {
        _statsCanvas.enabled = true;
        PlayButtonFeedback(); //play the button feedback
    }

    void OnStatsBackButtonPressed() {
        _statsCanvas.enabled = false;
        PlayButtonFeedback(); //play the button feedback
    }

    void SoundOnButtonPressed() {
        PlayerPrefs.SetInt("sound",1);
        PlayButtonFeedback(); //play the button feedback
    }

    void SoundOffButtonPressed() {
        PlayerPrefs.SetInt("sound",0);
        PlayButtonFeedback(); //play the button feedback
    }

    void VibrationOnButtonPressed() {
        PlayerPrefs.SetInt("vibration",1);
        PlayButtonFeedback(); //play the button feedback
        HapticController.hapticsEnabled = true;
    }

    void VibrationOffButtonPressed() {
        PlayerPrefs.SetInt("vibration",0);
        PlayButtonFeedback(); //play the button feedback
        HapticController.hapticsEnabled = false;
    }

    void ScreenShakeOnButtonPressed() {
        PlayerPrefs.SetInt("screenShake",1);
        PlayButtonFeedback(); //play the button feedback
    }

    void ScreenShakeOffButtonPressed() {
        PlayerPrefs.SetInt("screenShake",0);
        PlayButtonFeedback(); //play the button feedback
    }

    void OnConfigBackButtonPressed() {
        _configCanvas.enabled = false;
        PlayButtonFeedback(); //play the button feedback
    }
    
    void OnThemeButtonPressed() {
        _themeErrorText.text = "";
        _temporaryPrimaryTheme = _theme.ConvertColorToIndex(_theme.PrimaryColor);
        _temporarySecondaryTheme = _theme.ConvertColorToIndex(_theme.SecondaryColor);
        EnableThemeApplyButton();
        _themeCanvas.enabled = true;
        if (_masteryCanvas.enabled) _masteryCanvas.enabled = false;
        PlayButtonFeedback(); //play the button feedback
    }
    
    void OnThemeCloseButtonPressed() {
        _theme.SetPrimaryColor(_temporaryPrimaryTheme);
        _theme.SetSecondaryColor(_temporarySecondaryTheme);
        _theme.UpdateTMPColors();
        _themeCanvas.enabled = false;
        PlayButtonFeedback(); //play the button feedback
    }

    void OnThemeApplyButtonPressed() {
        _theme.SaveColors();
        _themeCanvas.enabled = false;
        PlayButtonFeedback(); //play the button feedback
    }

    //this method gets the current mastery piece
    MasteryPieces SetCurrentMasteryPiece() {
        return _currentMasteryPiece;
    }
    
    //when the mastery button is pressed
    void OnMasteryButtonPressed() {
        _mastery.CheckUnlockAndSetApplyButton();
        _currentMasteryPiece = MasteryPieces.Pawn;
        _mastery.OnMasteryClose();
        _masteryCanvas.enabled = true;
        _masteryPieceImage.sprite = _masteryPieceSprites[0]; //set the sprite to pawn
        if (_themeCanvas.enabled) _themeCanvas.enabled = false;
        PlayButtonFeedback();
    }

    //when the mastery cycle button is pressed
    void OnMasteryCycleButtonPressed(int direction) {
        //if cycling right
        if (direction == 1) {
            if (_currentMasteryPiece != MasteryPieces.King) {
                _currentMasteryPiece = (MasteryPieces)((int)_currentMasteryPiece + 1);
            }
            else _currentMasteryPiece = MasteryPieces.Pawn; //warp to beginning
        }
        //if cycling left
        else {
            if (_currentMasteryPiece != MasteryPieces.Pawn) {
                _currentMasteryPiece = (MasteryPieces)((int)_currentMasteryPiece - 1);
            }
            else _currentMasteryPiece = MasteryPieces.King; //warp to end
        }
        UpdateMasterySelection(); //update mastery selection
        _mastery.SetCurrentPiece((int)_currentMasteryPiece); //set the mastery piece
        PlayButtonFeedback(); //play the button feedback
    }

    void OnMasteryApplyButtonPressed() {
        PlayButtonFeedback();
        _mastery.SaveMasterySelection();
        _masteryCanvas.enabled = false;
    }

    //mastery close button pressed
    void OnMateryCloseButtonPressed() {
        _masteryCanvas.enabled = false; //close the canvas
        PlayButtonFeedback(); //play the button feedback
        _mastery.OnMasteryClose();
    }

    //update the mastery selection choice
    void UpdateMasterySelection() {
        _masteryPieceImage.sprite = _masteryPieceSprites[(int)_currentMasteryPiece]; //update the current mastery piece image
    }
    
    void OnPrimaryCycleButtonPressed() {
        _theme.SetPrimaryColor(_theme.GetNextColor(_theme.PrimaryColor));
        _theme.UpdateTMPColors();
        CheckAndSetThemeErrorMessage(); 
        PlayButtonFeedback(); //play the button feedback
    }

    void OnSecondaryCycleButtonPressed() {
        _theme.SetSecondaryColor(_theme.GetNextColor(_theme.SecondaryColor));
        _theme.UpdateTMPColors();
        CheckAndSetThemeErrorMessage(); 
        PlayButtonFeedback(); //play the button feedback
    }

    void DisableThemeApplyButton() {
        _themeApplyButton.interactable = false;
        _themeApplyButton.transform.parent.transform.GetChild(1).GetComponent<Image>().enabled = true;
    }

    void EnableThemeApplyButton() {
        _themeApplyButton.interactable = true;
        _themeApplyButton.transform.parent.transform.GetChild(1).GetComponent<Image>().enabled = false;
    }
    
    void PlayButtonFeedback() {
        SoundManager.PlaySound("button"); //play the button sound
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact); //play vibration
    }

    void CheckAndSetThemeErrorMessage() {
        bool areColorsTheSame = false, areBothColorsUnlocked = true;

        if (_theme.PrimaryColor.Equals(_theme.SecondaryColor)) areColorsTheSame = true; //both colors are the same
        if (_kingSlays < _theme.GetRequirementAtIndex(_theme.ConvertColorToIndex(_theme.PrimaryColor)) || _kingSlays < _theme.GetRequirementAtIndex(_theme.ConvertColorToIndex(_theme.SecondaryColor)))
            areBothColorsUnlocked = false; //atleast one color is not unlocked

        _themeErrorText.text = ""; //reset text before updating error
        
        if (areColorsTheSame) _themeErrorText.text += ("Your theme must have two different colors.\n");
        if (!areBothColorsUnlocked) _themeErrorText.text += ("At least one of the selected themes are not unlocked!\nThe next theme will be unlocked after "+_theme.GetNextRequirement(_kingSlays)+ " more king slays!");
        
        if (_themeErrorText.text == "") EnableThemeApplyButton();
        else DisableThemeApplyButton();
    }
}
