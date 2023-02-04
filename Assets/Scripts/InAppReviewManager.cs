//using System.Collections;
//using UnityEngine;

//public static class InAppReviewManager 
//{
//    public static IEnumerator RequestReview()
//    {
//        yield return null;

//#if UNITY_IPHONE
//        UnityEngine.iOS.Device.RequestStoreReview();
//        yield break;

//#elif UNITY_ANDROID
//        Google.Play.Review.ReviewManager reviewManager = new Google.Play.Review.ReviewManager();
//        var requestFlowOperation = reviewManager.RequestReviewFlow();
//        yield return requestFlowOperation;
//        if(requestFlowOperation.Error != Google.Play.Review.ReviewErrorCode.NoError)
//        {
//            yield break;
//        }

//        var playReviewInfo = requestFlowOperation.GetResult();
//        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
//        playReviewInfo = null;
//        if(launchFlowOperation.Error != Google.Play.Review.ReviewErrorCode.NoError)
//        {
//            yield break;
//        }
//#else
//        Debug.Log("requestRevie not supported.");
//#endif


//    }
//}
