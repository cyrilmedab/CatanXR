using Normal.Realtime;
using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    public RealtimeTransform headRT;
    public HandSkeletonSerializer leftHand;
    public HandSkeletonSerializer rightHand;

    private Transform _localHead;
    private Transform _localLeftHand;
    private Transform _localRightHand;

    private RealtimeTransform _leftHandRT;
    private RealtimeTransform _rightHandRT;

    private RealtimeView _realtimeView;

    private void Awake()
    {
        _realtimeView = GetComponent<RealtimeView>();
        _leftHandRT = GetComponent<RealtimeTransform>();
        _rightHandRT = GetComponent<RealtimeTransform>();
    }

    private void Update()
    {
        if (!_realtimeView.isOwnedLocallySelf) return;

        MatchTransform(_localHead, headRT.transform);
        MatchTransform(_localLeftHand, leftHand.transform);
        MatchTransform(_localRightHand, rightHand.transform);
    }

    public void LinkWithLocal(Transform head, OVRSkeleton left, OVRSkeleton right)
    {
        headRT.RequestOwnership();
        _leftHandRT.RequestOwnership();
        _rightHandRT.RequestOwnership();

        _localHead = head;
        _localLeftHand = left.transform.parent;
        _localRightHand = right.transform.parent;

        leftHand.AssignLocalSkeleton(left);
        rightHand.AssignLocalSkeleton(right);
    }

    private void MatchTransform(Transform source, Transform target)
    {
        target.localPosition = source.localPosition;
        target.localRotation = source.localRotation;
        target.localScale = source.localScale;
    }
}
