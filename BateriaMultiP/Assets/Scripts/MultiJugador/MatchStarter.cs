using UnityEngine;
using UnityEngine.Events;

public class MatchStarter : MonoBehaviour
{
    public GameFlowManager flow;
    public UnityEvent onStart;
    bool started;

    public void PressReady()
    {
        if (PlayerScore.Local != null) PlayerScore.Local.Ready = true;
    }

    void Update()
    {
        if (started) return;
        var all = FindObjectsByType<PlayerScore>(FindObjectsSortMode.None);
        if (all.Length < 2) return;
        foreach (var p in all) if (!p.Ready) return;
        started = true;
        onStart.Invoke();
        flow.BeginGame();
    }
}