using Fusion;

public class PlayerScore : NetworkBehaviour
{
    public static PlayerScore Local;

    [Networked] public int Points { get; set; }
    [Networked] public int Cables { get; set; }
    [Networked] public float TimeLeft { get; set; }
    [Networked] public NetworkBool Finished { get; set; }
    [Networked] public NetworkBool Ready { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority) Local = this;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Local == this) Local = null;
    }

    public void Report(int points, int cables, float timeLeft, bool finished)
    {
        if (!HasStateAuthority) return;
        Points = points; Cables = cables; TimeLeft = timeLeft; Finished = finished;
    }
}