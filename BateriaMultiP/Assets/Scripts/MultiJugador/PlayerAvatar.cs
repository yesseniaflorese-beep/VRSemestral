using Fusion;
using UnityEngine;
using Unity.XR.CoreUtils;

public class PlayerAvatar : NetworkBehaviour
{
    public Transform head, leftHand, rightHand;
    public Renderer[] hideForOwner;

    Transform rigHead, rigLeft, rigRight;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        var rig = FindFirstObjectByType<XROrigin>();
        rigHead = rig.Camera.transform;
        rigLeft = Find(rig.transform, "Left Controller");
        rigRight = Find(rig.transform, "Right Controller");

        foreach (var r in hideForOwner) r.enabled = false;
    }

    void LateUpdate()
    {
        if (!HasStateAuthority || rigHead == null) return;
        head.SetPositionAndRotation(rigHead.position, rigHead.rotation);
        if (rigLeft) leftHand.SetPositionAndRotation(rigLeft.position, rigLeft.rotation);
        if (rigRight) rightHand.SetPositionAndRotation(rigRight.position, rigRight.rotation);
    }

    Transform Find(Transform root, string n)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>())
            if (t.name == n) return t;
        Debug.LogWarning("No encontré " + n);
        return null;
    }
}