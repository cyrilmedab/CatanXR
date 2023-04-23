using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(RealtimeView), typeof(RealtimeTransform))]
public class RequestOwnership : MonoBehaviour
{
    private RealtimeView _realtimeView;
    private RealtimeTransform _realtimeTransform;

    private void Start()
    {
        _realtimeView = GetComponent<RealtimeView>();
        _realtimeTransform = GetComponent<RealtimeTransform>();
    }

    public void RequestObjectOwnership()
    {
        _realtimeView.RequestOwnership();
        _realtimeTransform.RequestOwnership();
    }

}
