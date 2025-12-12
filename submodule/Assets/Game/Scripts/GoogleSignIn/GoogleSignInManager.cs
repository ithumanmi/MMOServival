using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using NFramework;
using UnityEngine;

namespace Antada.Libs
{
    public interface IGoogleSignInSettingProvider
    {
        public string WebClientId();
    }

    public class GoogleSignInManager : SingletonMono<GoogleSignInManager>
    {
        public IGoogleSignInSettingProvider Delegate;
        private bool isInitialized;
        private string _idToken = "Waiting";
        private string _authCode = "";
        public Action<bool> OnGoogleLoginAction;

        public string IDToken
        {
            get { return _idToken; }
        }

        public string AuthCode => _authCode;

        public void Init()
        {
            if (this.isInitialized)
            {
                return;
            }
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = Delegate.WebClientId(),
                RequestIdToken = true,
                RequestEmail = true,
                RequestAuthCode = true,
            };
            this.isInitialized = true;
        }

        public void LogOut()
        {
            if (!this.isInitialized)
            {
                return;
            }
            GoogleSignIn.DefaultInstance.SignOut();
        }

        public IEnumerator Login()
        {
            _idToken = "Waiting";
            GoogleSignIn.DefaultInstance.SignOut();
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
            yield return new WaitUntil(() => _idToken != "Waiting");
        }

        private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error =
                                (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
                    }
                    OnGoogleLoginAction?.Invoke(false);
                    _idToken = string.Empty;

                }
            }
            else if (task.IsCanceled)
            {
                OnGoogleLoginAction?.Invoke(false);
                _idToken = string.Empty;
            }
            else
            {
                OnGoogleLoginAction?.Invoke(true);
                _idToken = task.Result.IdToken;
                _authCode = task.Result.AuthCode;
            }
        }
    }
}

