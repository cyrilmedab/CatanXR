using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusControllerMapper : MonoBehaviour
{
    [SerializeField]
    private OculusHandTracking.Handedness _handedness = OculusHandTracking.Handedness.None;
    
    private OVRCameraRig _cameraRig = null;

    private bool _initialized = false;
    private Transform _controllerAnchor = null;

    private bool InitializeTrackingReference()
    {
        if (_initialized) return true;
        if (_handedness == OculusHandTracking.Handedness.None) return false;
        
        _cameraRig = FindObjectOfType<OVRCameraRig>();
        _initialized = _cameraRig != null;
        if (_initialized)
        {
            _cameraRig.EnsureGameObjectIntegrity();
            _controllerAnchor = _handedness == OculusHandTracking.Handedness.Left ? 
                _cameraRig.leftControllerAnchor : _cameraRig.rightControllerAnchor;
        }

        return _initialized;
    }
    
    private void Update()
    {
        if (!InitializeTrackingReference())
            return;

        transform.position = _controllerAnchor.position;
        transform.rotation = _controllerAnchor.rotation;
    }
}
