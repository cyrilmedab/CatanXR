using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DisableMeshIfLocallyOwned : MonoBehaviour
{
    [SerializeField] private RealtimeView _realtimeView = null;
    private Renderer _renderer = null;

    private void Awake() => _renderer = GetComponent<Renderer>();

    // Update is called once per frame
    private void Update()
    {
        if (_realtimeView != null && _realtimeView.realtime.connected)
        {
            _renderer.enabled = !_realtimeView.isOwnedLocallyInHierarchy;
            enabled = false;
        }
    }
}
