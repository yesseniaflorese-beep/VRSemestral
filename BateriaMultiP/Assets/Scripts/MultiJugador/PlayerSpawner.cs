using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public NetworkObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
            Runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
    }
}