using Normal.Realtime;
using UnityEngine;

public class HandSyncModelController : RealtimeComponent<HandSyncRealtimeModel>
{
    [Header("Controller")]
    [SerializeField] private GameObject rightControllerModelRoot = null;
    [SerializeField] private GameObject leftControllerModelRoot = null;

    [Header("Hands")]
    [SerializeField] private GameObject rightHandModelRoot = null;
    [SerializeField] private GameObject leftHandModelRoot = null;
        
    [Header("Hands Sync Helpers")]
    [SerializeField] private OculusHandTracking rightHandSyncController = null;
    [SerializeField] private OculusHandTracking leftHandSyncController = null;
    
    bool _isOwnershipInitialized = false;
    
    private void InitalizeLocalSystems()
    {
        if(_isOwnershipInitialized || !realtime.connected)
            return;
            
        _isOwnershipInitialized = true;
            
        leftHandSyncController.enabled = true;
        rightHandSyncController.enabled = true;

        leftControllerModelRoot.GetComponent<OculusControllerMapper>().enabled = true;
        rightControllerModelRoot.GetComponent<OculusControllerMapper>().enabled = true;

        foreach (var view in GetComponentsInChildren<RealtimeView>())
        {
            view.RequestOwnership();
        }
        foreach (var realtimeTransform in GetComponentsInChildren<RealtimeTransform>())
        {
            realtimeTransform.RequestOwnership();
        }
    }
    
    private void Update()
    {
        if (isOwnedLocallyInHierarchy)
        {
            InitalizeLocalSystems();
            
            bool isHandTrackingActive = OVRPlugin.GetHandTrackingEnabled();
            model.isHandTrackingActive = isHandTrackingActive;
            model.isRightHandTrackingReliable = rightHandSyncController.isHandTrackingConfidenceHigh;
            model.isLeftHandTrackingReliable = leftHandSyncController.isHandTrackingConfidenceHigh;
                
            rightControllerModelRoot.SetActive(!isHandTrackingActive);
            rightHandModelRoot.SetActive(isHandTrackingActive);
                
            leftControllerModelRoot.SetActive(!isHandTrackingActive);
            leftHandModelRoot.SetActive(isHandTrackingActive);
        }
        else
        {
            bool isHandTrackingActive = model.isHandTrackingActive;
                
            rightControllerModelRoot.SetActive(!isHandTrackingActive);
            rightHandModelRoot.SetActive(isHandTrackingActive && model.isRightHandTrackingReliable);
                
            leftControllerModelRoot.SetActive(!isHandTrackingActive);
            leftHandModelRoot.SetActive(isHandTrackingActive && model.isLeftHandTrackingReliable);
        }
    }
}
