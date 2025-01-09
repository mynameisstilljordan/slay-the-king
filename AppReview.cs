using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;

public class AppReview : MonoBehaviour
{
    //app review
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    
    public void RequestReview() {
#if !UNITY_EDITOR
        //if player is at level 100+ and the player can be asked to leave a review
        if (PlayerPrefs.GetInt("level", 0) > 99 && PlayerPrefs.GetInt("canPlayerBeAskedToReview", 1) == 1) {
            PlayerPrefs.SetInt("canPlayerBeAskedToReview",0); //don't ask player for another review
            StartCoroutine(RequestAppReview()); //request the review
        }
#endif
    }

    IEnumerator RequestAppReview() {
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
}
