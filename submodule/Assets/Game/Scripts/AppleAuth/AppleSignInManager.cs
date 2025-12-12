#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using NFramework;
using UnityEngine;
using System.Text;
using System;
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Extensions;


namespace Antada.Libs
{
    public class AppleSignInManager : SingletonMono<AppleSignInManager>
    {
        // Static reference to the instance
        private string _identityToken;
        private IAppleAuthManager appleAuthManager;
        public Action<bool> OnAppleLoginAction;

        public string IdentityToken
        {
            get { return _identityToken; }
        }

        public void Init()
        {
            // If the current platform is supported
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                this.appleAuthManager = new AppleAuthManager(deserializer);
            }

        }

        void Update()
        {
            if (this.appleAuthManager != null)
            {
                this.appleAuthManager.Update();
            }
        }

        public IEnumerator Login()
        {
            yield return SignInWithApple();
        }

        private IEnumerator SignInWithApple()
        {
            _identityToken = "Pending";
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            this.appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                    if (appleIdCredential != null)
                    {

                    // Identity token
                    _identityToken = Encoding.UTF8.GetString(
                                    appleIdCredential.IdentityToken,
                                    0,
                                    appleIdCredential.IdentityToken.Length);
                        OnAppleLoginAction?.Invoke(true);
                    }
                    else
                    {
                        OnAppleLoginAction?.Invoke(false);
                        Debug.Log("appleIdCredential is null!");
                        _identityToken = string.Empty;
                    }
                },
                error =>
                {
                    OnAppleLoginAction?.Invoke(false);
                    _identityToken = string.Empty;
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.Log("failed Set authorizationErrorCode:" + authorizationErrorCode.ToString());
                });
            yield return new WaitUntil(() => _identityToken != "Pending");
        }

    }

}
#endif