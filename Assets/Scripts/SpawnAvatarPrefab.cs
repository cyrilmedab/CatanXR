using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
//using Oculus.Interaction;

public class SpawnAvatarPrefab : MonoBehaviour
{
    
    public GameObject avatar; //!
    public bool ownedByClient = true;
    public bool preventOwnershipTakeover = true;
    public bool destroyWhenOwnerOrLastClientLeaves = true;

    // should change all of these to localPlayer???
    public Transform localHeadReference;
    public OVRCustomSkeleton localLeftSkeleton;
    public OVRCustomSkeleton localRightSkeleton;

    private Realtime _realtime;

    
    private void Awake() => _realtime = GetComponent<Realtime>();
    
    private void OnEnable()
    {
        if (_realtime.connected) RealtimeConnected(_realtime);
        else _realtime.didConnectToRoom += RealtimeConnected;
    }

    private void OnDisable() => _realtime.didConnectToRoom -= RealtimeConnected;

    //protected virtual void RealtimeConnected(Realtime realtime) => Realtime.Instantiate(avatar.name, ownedByClient,
    //    preventOwnershipTakeover, destroyWhenOwnerOrLastClientLeaves);

    protected virtual void RealtimeConnected(Realtime realtime)
    {
        var player_avatar = Realtime.Instantiate(avatar.name, ownedByClient,
            preventOwnershipTakeover, destroyWhenOwnerOrLastClientLeaves);

        //player_avatar.GetComponent<PlayerAvatar>()
        //    .LinkWithLocal(localHeadReference, localLeftSkeleton, localRightSkeleton);

    }
}
