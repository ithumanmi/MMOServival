using NFramework;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Antada.Libs
{
    public interface PlayFabSettingProvider
    {
        string TitleId();
        string DevelopmentSecretKey();
        string ProductionEnvironmentUrl();
    }

    public class PlayFabManager : SingletonMono<PlayFabManager>
    {
        public PlayFabSettingProvider Delegate;

        public void Init()
        {
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            {
                PlayFabSettings.staticSettings.TitleId = Delegate.TitleId();
            }
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.DeveloperSecretKey))
            {
                PlayFabSettings.staticSettings.DeveloperSecretKey = Delegate.DevelopmentSecretKey();
            }
            if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.ProductionEnvironmentUrl))
            {
                PlayFabSettings.staticSettings.ProductionEnvironmentUrl = Delegate.ProductionEnvironmentUrl();
            }
        }

        public void LoginFacebook(string accessToken, Action<LoginResult> onLoginCompleted = null, Action<PlayFabError> onLoginFailed = null)
        {
            PlayFabClientAPI.LoginWithFacebook(
                new LoginWithFacebookRequest
                {
                    CreateAccount = true,
                    AccessToken = accessToken
                }, resultCallback =>
                {
                    Debug.Log($"[PlayFab] Login Facebook complete");
                    onLoginCompleted?.Invoke(resultCallback);
                }, error =>
                {
                    Debug.Log($"[PlayFab] Login Facebook error: {error.GenerateErrorReport()}");
                    onLoginFailed?.Invoke(error);
                });
        }

        public void LinkFacebookAccount(string accessToken, Action<LinkFacebookAccountResult> onLinkCompleted = null, Action<PlayFabError> onLinkFailed = null)
        {
            PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest
            {
                AccessToken = accessToken,
                ForceLink = true
            }, resultCallback =>
            {
                Debug.Log($"[PlayFab] Link Facebook complete");
                onLinkCompleted?.Invoke(resultCallback);
            }, error =>
            {
                Debug.Log($"[PlayFab] Link Facebook error: {error.GenerateErrorReport()}");
                onLinkFailed?.Invoke(error);
            });
        }

        public void LoginGoogle(string authCode, Action<LoginResult> onLoginCompleted = null, Action<PlayFabError> onLoginFailed = null)
        {
            PlayFabClientAPI.LoginWithGoogleAccount(
                new LoginWithGoogleAccountRequest
                {
                    CreateAccount = true,
                    ServerAuthCode = authCode,
                }, resultCallback =>
                {
                    Debug.Log($"[PlayFab] Login Google complete");
                    onLoginCompleted?.Invoke(resultCallback);
                }, error =>
                {
                    Debug.Log($"[PlayFab] Login Google error: {error.GenerateErrorReport()}");
                    onLoginFailed?.Invoke(error);
                });
        }

        public void LoginApple(string identityToken, Action<LoginResult> onLoginCompleted = null, Action<PlayFabError> onLoginFailed = null)
        {
            PlayFabClientAPI.LoginWithApple(
                new LoginWithAppleRequest
                {

                    CreateAccount = true,
                    IdentityToken = identityToken
                }, resultCallback =>
                {
                    Debug.Log($"[PlayFab] Login Apple complete");
                    onLoginCompleted?.Invoke(resultCallback);
                }, error =>
                {
                    Debug.Log($"[PlayFab] Login Apple error: {error.GenerateErrorReport()}");
                    onLoginFailed?.Invoke(error);
                });
        }

        public void SetUserData(Dictionary<string, string> data, Action<UpdateUserDataResult> onSetUserDataCompleted = null)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = data
            }, result =>
            {
                Debug.Log($"[PlayFab] Successfully updated user data");
                onSetUserDataCompleted?.Invoke(result);
            }, error =>
            {
                Debug.Log($"[PlayFab] Got error setting user data: {error.GenerateErrorReport()}");
            });
        }

        public void GetUserData(string playFabId, Action<GetUserDataResult> onGetUserDataCompleted)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = playFabId,
                Keys = null
            }, result =>
            {
                Debug.Log($"[PlayFab] Successfully get user data");
                onGetUserDataCompleted?.Invoke(result);
            }, error =>
            {
                Debug.Log($"[PlayFab] Got error retrieving user data: {error.GenerateErrorReport()}");
            });
        }

        public bool IsClientLoggedIn() => PlayFabClientAPI.IsClientLoggedIn();
    }

}
