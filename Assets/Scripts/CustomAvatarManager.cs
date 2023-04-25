using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.XR;

public class CustomAvatarManager : MonoBehaviour
{
    [SerializeField] private GameObject _localAvatarPrefab;
    [SerializeField] private RealtimeAvatar.LocalPlayer _localPlayer;
    public OVRSkeleton localLeftSkeleton;
    public OVRSkeleton localRightSkeleton;
    
    public GameObject LocalAvatarPrefab {
        get { return _localAvatarPrefab; }
        set { SetLocalAvatarPrefab(value); }
    }
    
    public RealtimeAvatar LocalAvatar { get; private set; }
    public Dictionary<int, RealtimeAvatar> Avatars { get; private set; }

    public delegate void AvatarCreatedDestroyed(CustomAvatarManager avatarManager, RealtimeAvatar avatar,
        bool isLocalAvatar);

    public event AvatarCreatedDestroyed AvatarCreated;
    public event AvatarCreatedDestroyed AvatarDestroyed;

    private Realtime _realtime;

    private void Awake()
    {
        _realtime = GetComponent<Realtime>();
        _realtime.didConnectToRoom += DidConnectToRoom;

        if (_localPlayer == null) _localPlayer = new RealtimeAvatar.LocalPlayer();

        Avatars = new Dictionary<int, RealtimeAvatar>();
    }

    private void OnEnable()
    {
        if (_realtime.connected) CreateAvatarIfNeeded();
    }
    private void OnDisable() => DestroyAvatarIfNeeded();
    private void OnDestroy() => _realtime.didConnectToRoom -= DidConnectToRoom;
    
    private void DidConnectToRoom(Realtime room)
    {
        if (!gameObject.activeInHierarchy || !enabled) return;
        CreateAvatarIfNeeded();
    }
    
    
    public static RealtimeAvatar.DeviceType GetRealtimeAvatarDeviceTypeForLocalPlayer() {
        switch (XRSettings.loadedDeviceName) {
            case "OpenVR":
                return RealtimeAvatar.DeviceType.OpenVR;
            case "Oculus":
                return RealtimeAvatar.DeviceType.Oculus;
            default:
                return RealtimeAvatar.DeviceType.Unknown;
        }
    }

    public void _RegisterAvatar(int clientID, RealtimeAvatar avatar)
    {
        if (Avatars.ContainsKey(clientID))
        {
            Debug.LogError("RealtimeAvatar registered more than once for the same clientID (" + clientID + "). This is a bug!");
        }

        Avatars[clientID] = avatar;
        if (AvatarCreated != null)
        {
            try
            {
                AvatarCreated(this, avatar, clientID == _realtime.clientID);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    public void _UnregisterAvatar(RealtimeAvatar avatar)
    {
        bool isLocalAvatar = false;

        List<KeyValuePair<int, RealtimeAvatar>> matchingAvatars =
            Avatars.Where(keyValuePair => keyValuePair.Value == avatar).ToList();
        foreach (KeyValuePair<int, RealtimeAvatar> matchingAvatar in matchingAvatars)
        {
            int avatarClientID = matchingAvatar.Key;
            Avatars.Remove(avatarClientID);
            isLocalAvatar = isLocalAvatar || avatarClientID == _realtime.clientID;
        }

        if (AvatarDestroyed != null)
        {
            try
            {
                AvatarDestroyed(this, avatar, isLocalAvatar);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    private void SetLocalAvatarPrefab(GameObject localAvatarPrefab)
    {
        if (localAvatarPrefab == _localAvatarPrefab) return;

        _localAvatarPrefab = localAvatarPrefab;
        
        // Replaces the existing avatar if we've already instantiated the old prefab
        if (LocalAvatar != null)
        {
            DestroyAvatarIfNeeded();
            CreateAvatarIfNeeded();
        }
    }

    public void CreateAvatarIfNeeded()
    {
        if (!_realtime.connected)
        {
            Debug.LogError("RealtimeAvatarManager: Unable to create avatar. Realtime is not connected to a room.");
            return;
        }

        if (LocalAvatar != null) return;
        
        if (_localAvatarPrefab == null) {
            Debug.LogWarning("Realtime Avatars local avatar prefab is null. No avatar prefab will be instantiated for the local player.");
            return;
        }
        
        GameObject avatarGameObject = Realtime.Instantiate(_localAvatarPrefab.name, new Realtime.InstantiateOptions
        {
            ownedByClient               = true,
            preventOwnershipTakeover    = true,
            destroyWhenOwnerLeaves      = true,
            destroyWhenLastClientLeaves = true,
            useInstance                 = _realtime
        });
        
        if (avatarGameObject == null) {
            Debug.LogError("RealtimeAvatarManager: Failed to instantiate RealtimeAvatar prefab for the local player.");
            return;
        }
        
        /*
        LocalAvatar = avatarGameObject.GetComponent<RealtimeAvatar>();
        if (LocalAvatar == null) {
            Debug.LogError("RealtimeAvatarManager: Successfully instantiated avatar prefab, but could not find the RealtimeAvatar component.");
            return;
        }
        */
        
        // next line is a custom addition
        avatarGameObject.GetComponent<PlayerAvatar>()
            .LinkWithLocal(_localPlayer.head, localLeftSkeleton, localRightSkeleton);
        
        //LocalAvatar.localPlayer = _localPlayer;
        //LocalAvatar.deviceType = GetRealtimeAvatarDeviceTypeForLocalPlayer();
    }

    public void DestroyAvatarIfNeeded()
    {
        if (LocalAvatar == null) return;

        Realtime.Destroy(LocalAvatar.gameObject);
        LocalAvatar = null;
    }
}
