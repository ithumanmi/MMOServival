using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuthManager instance;
    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    // Login Variables
    [Space]
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public InputField nameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField confirmPasswordRegisterField;

    public Text textError;
    private void Awake()
    {
        if(instance == null) { 
            instance = this;    
        }
    }
    private void Start()
    {
        textError.text = string.Empty;
        StartCoroutine(CheckAndFixDependenciesAsync());  
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(()=>dependencyTask.IsCompleted);

        dependencyStatus = dependencyTask.Result;

        if(dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogError("Could not resolve all firebase dependencies " + dependencyStatus);
        }
    }
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        // Configure the emulator (only in development environment)
        //#if UNITY_EDITOR
        //        auth.UseAuthEmulator("localhost", 9099); // Replace "localhost" and "9099" with your emulator's host and port
        //        Debug.Log("Using Firebase Authentication Emulator.");
        //#else
        //    Debug.Log("Using live Firebase services.");
        //#endif
    }
    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);

                SceneLoaderManager.instance.LoadScene(GlobalEnum.SceneIndex.LoginScene);
                ClearLoginInputFieldText();
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    private IEnumerator CheckForAutoLogin()
    {
        if(user != null) 
        {
            var reloadUserTask = user.ReloadAsync();

            yield return new WaitUntil(() => reloadUserTask.IsCompleted);
            AutoLogin();
        }
        else
        {
            LoginSceneManager.instance.OpenLoginPanel();
        }
    }

    private void AutoLogin()
    {
        if(auth != null)
        {

            LoginSceneManager.instance.GoHomeScene();
        }
        else
        {
            LoginSceneManager.instance.OpenLoginPanel();
        }
    }
    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
    }

    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {

        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Login Failed! Because ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    break;
                default:
                    failedMessage = "Login Failed";
                    break;
            }
            textError.text = failedMessage;
            Debug.Log(failedMessage);
        }
        else
        {
            textError.text = "You Are Successfully Logged In";
            var authResult = loginTask.Result; // This is of type AuthResult
            user = authResult.User; // Extract the FirebaseUser from AuthResult
            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);

            LoginSceneManager.instance.GoHomeScene();
        }
    }
    public void Register()
    {
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            Debug.LogError("User Name is empty");
        }
        else if (email == "")
        {
            Debug.LogError("email field is empty");
        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            Debug.LogError("Password does not match");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Becuase ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;
                }

                Debug.Log(failedMessage);
            }
            else
            {
                var authResult = registerTask.Result; // This is of type AuthResult
                user = authResult.User; // Extract the FirebaseUser from AuthResult


                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if user update failed
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Profile update Failed! Becuase ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Profile update Failed";
                            break;
                    }

                    Debug.Log(failedMessage);
                }
                else
                {
                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    Debug.Log(LoginSceneManager.instance);
                    LoginSceneManager.instance.OpenLoginPanel();
                }
            }
        }
    }

    public void Logout()
    {
        Debug.Log("Logout");
        if(auth != null && user != null)
        {
            auth.SignOut();
          
        }
    }

    private void ClearLoginInputFieldText()
    {
        emailLoginField.text = string.Empty;
        passwordLoginField.text = string.Empty;
    }
}
