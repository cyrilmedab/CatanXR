using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Normal.Realtime;
using Unity.VisualScripting;
using static OVRSkeleton;
using UnityEngine;

public class HandSkeletonSerializer : MonoBehaviour
{
    public Transform skeletonRoot;

    private RealtimeView _realtimeView;
    private HandPoseSync _handPoseSync;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private List<Transform> _allBones = new List<Transform>();

    private OVRSkeleton _ovrSkeleton;
    private IOVRSkeletonDataProvider _ovrSkeletonDataProvider;

    private StringBuilder _stringBuilder;
    private string[] _incomingDataArray;

    private bool _isInitialized = false;

    private void Awake()
    {
        _realtimeView = GetComponent<RealtimeView>();
        _handPoseSync = GetComponent<HandPoseSync>();
        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        _stringBuilder = new StringBuilder();

        transform.eulerAngles = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        
    }

    private void Update()
    {
        if (!IsOnline()) return;
        if (!_isInitialized) Initialize();
        if (_realtimeView.isOwnedLocallyInHierarchy) LocalUpdate();
    }
    
    private bool IsOnline()
    {
        return _realtimeView != null && _realtimeView.realtime != null && _realtimeView.realtime.connected;
    }

    private void LocalUpdate() => _handPoseSync.SendData(SerializeSkeletalData());

    public void AssignLocalSkeleton(OVRSkeleton skeleton)
    {
        _ovrSkeleton = skeleton;
        _ovrSkeletonDataProvider = _ovrSkeleton.GetComponent<IOVRSkeletonDataProvider>();
    }
    
    // here's where major changes happen
    public void Initialize()
    {
        _allBones.Clear();
        _allBones = ListOfBoneTransforms(skeletonRoot);
        
        _skinnedMeshRenderer.enabled = true;
        _skinnedMeshRenderer.bones = _allBones.ToArray();
        _isInitialized = true;
    }

    private List<Transform> ListOfBoneTransforms(Transform obj)
    {
        List<Transform> bonePositions = new List<Transform>();
        //List<Transform> tipsOfBones = new List<Transform>();
        if (obj == null) return bonePositions;

        var stack = new Stack<Transform>();
        stack.Push(obj);

        while (stack.Count != 0)
        {
            Transform curr = stack.Pop();
            bonePositions.Add(curr);
            
            for (int i = 0; i < curr.childCount; ++i)
            {
                var child = curr.GetChild(i);
                if (child == null) continue;
                
                //if (child.name.Contains("_null")) tipsOfBones.Add(child);
                //else
                stack.Push(child);
            }
        }

        //bonePositions.AddRange(tipsOfBones);
        return bonePositions;
    }

    private string SerializeSkeletalData()
    {
        SkeletonPoseData data = _ovrSkeletonDataProvider.GetSkeletonPoseData();
        _stringBuilder.Clear();

        if (!data.IsDataValid || !data.IsDataHighConfidence)
        {
            _skinnedMeshRenderer.enabled = false;
            _stringBuilder.Append("0|");
        }
        else
        {
            _skinnedMeshRenderer.enabled = true;
            _stringBuilder.Append("1|");

            for (int i = 0; i < _allBones.Count; i++)
            {
                _allBones[i].transform.localRotation = data.BoneRotations[i].FromFlippedZQuatf();

                _stringBuilder.Append(_allBones[i].transform.localEulerAngles.x);
                _stringBuilder.Append("|");
                _stringBuilder.Append(_allBones[i].transform.localEulerAngles.y);
                _stringBuilder.Append("|");
                _stringBuilder.Append(_allBones[i].transform.localEulerAngles.z);
                _stringBuilder.Append("|");
            }
        }

        return _stringBuilder.ToString();
    }

    public void DeserializeSkeletalData(string netHandData)
    {
        if (string.IsNullOrEmpty(netHandData) || !IsOnline() || _realtimeView.isOwnedLocallyInHierarchy) return;

        _incomingDataArray = netHandData.Split("|");

        if (_incomingDataArray[0] == "0")
        {
            _skinnedMeshRenderer.enabled = false;
            return;
        }

        _skinnedMeshRenderer.enabled = true;
        
        for (int i = 0; i < _allBones.Count; i++)
        {
            int adjIndex = i * 3;

            _allBones[i].transform.localEulerAngles = new Vector3(
                float.Parse(_incomingDataArray[adjIndex + 1], CultureInfo.InvariantCulture),
                float.Parse(_incomingDataArray[adjIndex + 2], CultureInfo.InvariantCulture),
                float.Parse(_incomingDataArray[adjIndex + 3], CultureInfo.InvariantCulture));
        }
    }
    
    
}
