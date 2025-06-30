using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PasswordManager : MonoBehaviour
{
    [SerializeField] GameObject passwordPrefab;
    public bool passwordScreen = false;
    string password = "pawn";

#if UNITY_WEBGL_API || UNITY_WEB || UNITY_WEBGL_API
    private void Start()
    {
        passwordScreen = true;
        passwordPrefab.SetActive(true);
    }
#endif

    public void CheckPassword(string passwordAttempt)
    {
        if(passwordAttempt == password)
        {
            //Debug.Log("Password Correct!");
            passwordScreen = false;
            passwordPrefab.SetActive(false);
        }
    }

}
