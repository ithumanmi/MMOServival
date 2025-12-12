using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using NFramework;
using UnityEngine;

namespace Antada.Libs
{
    public enum FacebookLogintatus
    {
        Success = 0,
        Cancel = 1,
        Error = 2,
    };

    public enum FacebookScope
    {
        PublicProfile = 1,
        UserFriends = 2,
        UserBirthday = 3,
        UserAgeRange = 4,
        PublishActions = 5,
        UserLocation = 6,
        UserHometown = 7,
        UserGender = 8,
        email = 9,
    };

    public class FacebookManager : SingletonMono<FacebookManager>
    {
        private string _AccessToken;
        public Action<bool> OnLoginAction;
        public string CurrentAccessToken
        {
            get { return _AccessToken; }
        }

        public void Init()
        {
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        public void LogOut()
        {
            if (FB.IsLoggedIn)
                FB.LogOut();
        }

        public IEnumerator Login(HashSet<FacebookScope> scopes = null)
        {
            _AccessToken = "Waiting";
            LogOut();
            yield return new WaitForSeconds(1);

            // Use the provided scopes or use default values if none are provided
            HashSet<FacebookScope> loginScopes = scopes ?? new HashSet<FacebookScope> { FacebookScope.PublicProfile, FacebookScope.email };

            this.CallFBLogin(LoginTracking.ENABLED, loginScopes);
            yield return WaitForToken();
        }

        private IEnumerator WaitForToken()
        {
            yield return new WaitUntil(() => _AccessToken != "Waiting");
        }

        private void CallFBLogin(LoginTracking mode, HashSet<FacebookScope> scopes)
        {
            List<string> _scopes = new List<string>();

            if (scopes.Contains(FacebookScope.PublicProfile))
            {
                _scopes.Add("public_profile");
            }
            if (scopes.Contains(FacebookScope.UserFriends))
            {
                _scopes.Add("user_friends");
            }
            if (scopes.Contains(FacebookScope.UserBirthday))
            {
                _scopes.Add("user_birthday");
            }
            if (scopes.Contains(FacebookScope.UserAgeRange))
            {
                _scopes.Add("user_age_range");
            }
            if (scopes.Contains(FacebookScope.UserLocation))
            {
                _scopes.Add("user_location");
            }
            if (scopes.Contains(FacebookScope.UserHometown))
            {
                _scopes.Add("user_hometown");
            }

            if (scopes.Contains(FacebookScope.UserGender))
            {
                _scopes.Add("user_gender");
            }
            if (scopes.Contains(FacebookScope.email))
            {
                _scopes.Add("email");
            }

            FB.LogInWithReadPermissions(_scopes, this.HandleResult);


        }
        private void HandleResult(IResult result)
        {
            if (result == null)
            {
                OnLoginAction?.Invoke(false);
                _AccessToken = string.Empty;
                return;
            }
            if (!string.IsNullOrEmpty(result.Error))
            {
                OnLoginAction?.Invoke(false);
                _AccessToken = FacebookLogintatus.Error.ToString();
                Debug.Log("Error Response:\n" + result.Error);
            }
            else if (result.Cancelled)
            {
                OnLoginAction?.Invoke(false);
                _AccessToken = FacebookLogintatus.Cancel.ToString();
                Debug.Log("Canceled Response:\n" + result.RawResult);
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                OnLoginAction?.Invoke(true);
                _AccessToken = AccessToken.CurrentAccessToken.TokenString;
            }
            else
            {
                OnLoginAction?.Invoke(false);
                _AccessToken = string.Empty;
                Debug.Log("Empty Response\n");
            }
        }
    }
}

