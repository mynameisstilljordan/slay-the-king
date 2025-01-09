using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

using Lofelt.NiceVibrations;
using TMPro;

public class GameManager : MonoBehaviour{

    public static GameManager Instance; //the instance var

    private Advertisements _advertisements; //the ads script
    private Theme _theme; //the theme script
    private GDPR _gdpr;
    private AppUpdater _appUpdater;
    private AppReview _appReview;
    public bool _hasGameBeenPlayedInThisSession;

    private void Awake() {
        DontDestroyOnLoad(gameObject); //dont destroy the gameobject on load
        //if there is aleady a singleton instance, destroy any new GameManager instances
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        //if there is no singleton instance (everything here is called once)
        else {
            Instance = this; //set this as the instance

            //ADS
            _advertisements = GetComponent<Advertisements>(); //get the reference of the advertisements script from the same gameobject
            _advertisements.InitializeAds(); //initialize the ads

            //THEME
            _theme = GetComponent<Theme>(); //get the reference of the theme from the same gameobject
            _theme.LoadColors(); //load the colors 

            //GDPR
            _gdpr = GetComponent<GDPR>(); //get reference to the attatched gdpr script

            //APP UPDATE
            _appUpdater = GetComponent<AppUpdater>();
            _appUpdater.CheckForAppUpdate(); //check for an app update

            //APP REVIEW
            _appReview = GetComponent<AppReview>(); //app review reference

            //SCENE LISTENER
            SceneManager.sceneLoaded += OnSceneLoaded; //this listener is called whenever a scene is loaded
        }
    }

    private void Start() {
        //everything here is called once at the start of the game

        //haptics control
        if (PlayerPrefs.GetInt("vibration", 1) == 1) HapticController.hapticsEnabled = true;
        else HapticController.hapticsEnabled = false;

        /*
        //if player is at level 100+ and the player can be asked to leave a review
        if (PlayerPrefs.GetInt("level", 0) > 99 && PlayerPrefs.GetInt("canPlayerBeAskedToReview", 1) == 1) {
            PlayerPrefs.SetInt("canPlayerBeAskedToReview",0); //don't ask player for another review
            StartCoroutine(RequestReview()); //request the review
        }   */

        //ShowGDPRPopup(); //show the GDPR popup

        /*
        if (Application.platform == RuntimePlatform.Android) {
            //check if there's an update
            StartCoroutine(CheckForUpdate());
        }
        */
    }

    //this method is called whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        _theme.UpdateTMPColors();

        if (scene.name.Equals("ingame")) {
            if (PlayerPrefs.GetInt("hasTutorialBeenShown",0) == 1) ShowBannerAd(); //if loading the ingame scene, show a banner ad
            _hasGameBeenPlayedInThisSession = true;
        }
        else {
            HideBanner(); //otherwise, hide the banner
            if (_hasGameBeenPlayedInThisSession) {
                _appReview.RequestReview(); //request a review
            }
        }
    }

    //show the banner
    public void ShowBannerAd() {
        _advertisements.ShowBannerAd();
    }

    //hide the banner
    public void HideBanner() {
        _advertisements.HideBanner();
    }

    //the callback method after a rewarded ad is completed
    public void GiveUserReward() {

    }

    //this method shows the GDPR popup
    public void ShowGdprPopup() {
        _gdpr.ShowConsentForm();
    }

    /*
    #region AppUpdates
    IEnumerator CheckForUpdate() {
        _appUpdateManager = new AppUpdateManager(); //set the reference
        
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
            _appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful) {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.

            //give the user options of an immediate app update
            var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
            //start the update with the passed parameters
            StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
        }
        else {
            // Log appUpdateInfoOperation.Error.
        }
    }

    IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfoResult, AppUpdateOptions appUpdateOptions) {
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var startUpdateRequest = _appUpdateManager.StartUpdate(
            // The result returned by PlayAsyncOperation.GetResult().
            appUpdateInfoResult,
            // The AppUpdateOptions created defining the requested in-app update
            // and its parameters.
            appUpdateOptions);
        yield return startUpdateRequest;

        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (for
        // example, by logging result.Error or by displaying a message to the user).
    }
    #endregion

    #region Review
    IEnumerator RequestReview() {
        _reviewManager = new ReviewManager();

        //request a reviewinfo object
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError) {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();

        //launch the inapp review flow
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError) {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
    #endregion
    */
}
