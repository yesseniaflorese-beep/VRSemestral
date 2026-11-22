using System.Collections;
using UnityEngine;

public class StationPlacer : MonoBehaviour
{
    public Transform xrOrigin, station;
    public Vector3 sideBOffset = new Vector3(6, 0, 0);

    IEnumerator Start()
    {
        while (PlayerScore.Local == null) yield return null;

        int slot = PlayerScore.Local.Object.StateAuthority.PlayerId % 2;
        if (slot == 1)
        {
            xrOrigin.position += sideBOffset;
            station.position += sideBOffset;
        }
    }
}