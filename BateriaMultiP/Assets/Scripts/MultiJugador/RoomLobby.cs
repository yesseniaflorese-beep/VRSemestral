using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomLobby : MonoBehaviour
{
    public ConnectionManager connection;
    public GameObject canvasRoot, waitPanel, errorPanel;
    public TMP_Text waitCodeText, playersText;
    public string menuScene = "InicioMultiplayer";

    bool placedAfterSpawn;

    async void Start()
    {
        // Si pruebas esta escena sin pasar por el menú, usa el código 0000
        string code = string.IsNullOrEmpty(LobbyData.Code) ? "0000" : LobbyData.Code;

        waitPanel.SetActive(true);
        errorPanel.SetActive(false);
        waitCodeText.text = "Código: " + code + "\nConectando...";
        Invoke(nameof(PlaceCanvas), 0.5f);

        bool ok = await connection.Connect(code);
        if (!ok)
        {
            waitPanel.SetActive(false);
            errorPanel.SetActive(true);
            return;
        }
        waitCodeText.text = "Código: " + code;
    }

    void Update()
    {
        // Cuando ya apareció tu jugador (y StationPlacer te movió), vuelve a colocar el panel
        if (!placedAfterSpawn && PlayerScore.Local != null)
        {
            placedAfterSpawn = true;
            Invoke(nameof(PlaceCanvas), 0.3f);
        }

        if (!waitPanel.activeSelf) return;
        int n = FindObjectsByType<PlayerScore>(FindObjectsSortMode.None).Length;
        playersText.text = "Jugadores: " + n + "/2";
    }

    // Pone el panel a 2 m frente a tu cabeza, a la altura de tus ojos
    void PlaceCanvas()
    {
        var camera = Camera.main;
        if (camera == null) return;

        Vector3 fwd = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
        if (fwd == Vector3.zero) fwd = Vector3.forward;

        canvasRoot.transform.position = camera.transform.position + fwd * 2f;
        canvasRoot.transform.rotation = Quaternion.LookRotation(fwd);
    }

    public void HideAll() { canvasRoot.SetActive(false); }

    public void BackToMenu()
    {
        var runner = connection.GetComponent<NetworkRunner>();
        if (runner != null && runner.IsRunning) _ = runner.Shutdown();
        SceneManager.LoadScene(menuScene);
    }
}