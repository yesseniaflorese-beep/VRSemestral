using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public async Task<bool> Connect(string sessionCode)
    {
        var runner = GetComponent<NetworkRunner>();
        runner.ProvideInput = false;

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionCode,
            PlayerCount = 2,
            SceneManager = GetComponent<NetworkSceneManagerDefault>()
        });

        if (!result.Ok) Debug.LogError("No se pudo conectar: " + result.ShutdownReason);
        return result.Ok;
    }
}