using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMultijugador : MonoBehaviour
{
    public TMP_InputField codeInput;
    public TMP_Text statusText;
    public string gameScene = "VRBatteryGame 1";   // nombre exacto de tu escena de juego

    const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public void CreateRoom()
    {
        string code = "";
        for (int i = 0; i < 4; i++) code += Chars[Random.Range(0, Chars.Length)];
        LobbyData.Code = code;
        SceneManager.LoadScene(gameScene);
    }

    public void JoinRoom()
    {
        string code = codeInput.text.Trim().ToUpper();
        if (code.Length < 3) { statusText.text = "Escribe el código"; return; }
        LobbyData.Code = code;
        SceneManager.LoadScene(gameScene);
    }
}