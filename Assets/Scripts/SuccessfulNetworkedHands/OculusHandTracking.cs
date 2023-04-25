using System.Collections.Generic;
using UnityEngine;

public class OculusHandTracking : MonoBehaviour
{
    public enum Handedness
    {
        None = 0,
        Left = 1,
        Right = 2
    };

    [SerializeField] private Handedness handedness = Handedness.None;
    [SerializeField] Transform rootTrackedTransform = null;

    public bool isHandTrackingConfidenceHigh = true;

    private Dictionary<OVRSkeleton.BoneId, Transform> _boneIDMapping = new();
    private OVRCameraRig _ovrRig = null;
    private OVRSkeleton _skeleton = null;
    
    #region HandBone references

    private Transform _handRoot = null;
    private Transform _handWrist = null;

    private Transform _handIndex1 = null;
    private Transform _handIndex2 = null;
    private Transform _handIndex3 = null;

    private Transform _handMiddle1 = null;
    private Transform _handMiddle2 = null;
    private Transform _handMiddle3 = null;

    private Transform _handRing1 = null;
    private Transform _handRing2 = null;
    private Transform _handRing3 = null;

    private Transform _handPinky0 = null;
    private Transform _handPinky1 = null;
    private Transform _handPinky2 = null;
    private Transform _handPinky3 = null;

    private Transform _handThumb0 = null;
    private Transform _handThumb1 = null;
    private Transform _handThumb2 = null;
    private Transform _handThumb3 = null;

    #endregion

    private OVRSkeleton.SkeletonType GetSkeletonTypeFromHandedness(Handedness handedness)
    {
        switch (handedness)
        {
            case Handedness.Left:
                return OVRSkeleton.SkeletonType.HandLeft;
            case Handedness.Right:
                return OVRSkeleton.SkeletonType.HandRight;
            default:
                return OVRSkeleton.SkeletonType.None;
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!InitializeHandHierarchy())
        {
            Debug.LogError("Failed to init hierarchy for " + handedness);
            return;
        }

        if (!Application.isPlaying) return;

        InitializeRuntime();
        UpdateBones();
        isHandTrackingConfidenceHigh = _skeleton.IsDataHighConfidence;
    }

    private void InitializeRuntime()
    {
        if (!Application.isPlaying || _ovrRig != null) return;

        _ovrRig = FindObjectOfType<OVRCameraRig>();
        _ovrRig.EnsureGameObjectIntegrity();

        foreach (var skelly in _ovrRig.GetComponentsInChildren<OVRSkeleton>())
        {
            if (skelly.GetSkeletonType() != GetSkeletonTypeFromHandedness(handedness)) continue;

            _skeleton = skelly;
            break;
        }
    }

    private void UpdateBones()
    {
        if (_skeleton == null) return;

        foreach (OVRBone bone in _skeleton.Bones)
        {
            if (!_boneIDMapping.TryGetValue(bone.Id, out Transform boneTransform)) continue;

            if (bone.Id == OVRSkeleton.BoneId.Hand_WristRoot)
            {
                boneTransform.position = bone.Transform.position;
                boneTransform.rotation = bone.Transform.rotation;
                continue;
            }

            if (boneTransform == null) continue;

            boneTransform.localRotation = bone.Transform.localRotation;
        }
    }

    private bool InitializeHandHierarchy()
    {
        if (_handRoot != null) return true;

        OVRSkeleton.SkeletonType skellyType = GetSkeletonTypeFromHandedness(handedness);
        if (skellyType == OVRSkeleton.SkeletonType.None) return false;

        string handSignifier = skellyType == OVRSkeleton.SkeletonType.HandLeft ? "l" : "r";
        string handStructure = "b_" + handSignifier;

        _handRoot = transform;
        
        #region Wrist
        string wristString = handStructure + "_wrist";
        _handWrist = _handRoot.Find(wristString);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_WristRoot, rootTrackedTransform);
        Debug.Log("Found: " + _handWrist);
        #endregion
        
        #region Index
        
        string indexString = handStructure + "_index";

        string index1 = indexString + "1";
        _handIndex1 = _handWrist.Find(index1);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Index1, _handIndex1);
        Debug.Log("Found: " + _handIndex1);
        
        string index2 = indexString + "2";
        _handIndex2 = _handIndex1.Find(index2);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Index2, _handIndex2);
        Debug.Log("Found: " + _handIndex2);
        
        string index3= indexString + "3";
        _handIndex3 = _handIndex2.Find(index3);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Index3, _handIndex3);
        Debug.Log("Found: " + _handIndex3);
        
        #endregion
        
        #region Middle
        
        string middleString = handStructure + "_middle";

        string middle1 = middleString + "1";
        _handMiddle1 = _handWrist.Find(middle1);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Middle1, _handMiddle1);

        string middle2 = middleString + "2";
        _handMiddle2 = _handMiddle1.Find(middle2);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Middle2, _handMiddle2);

        string middle3 = middleString + "3";
        _handMiddle3 = _handMiddle2.Find(middle3);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Middle3, _handMiddle3);
        
        #endregion
        
        #region Pinky
        
        string pinkyString = handStructure + "_pinky";

        string pinky0 = pinkyString + "0";
        _handPinky0 = _handWrist.Find(pinky0);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Pinky0, _handPinky0);
        
        string pinky1 = pinkyString + "1";
        _handPinky1 = _handPinky0.Find(pinky1);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Pinky1, _handPinky1);
        
        string pinky2 = pinkyString + "2";
        _handPinky2 = _handPinky1.Find(pinky2);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Pinky2, _handPinky2);
        
        string pinky3 = pinkyString + "3";
        _handPinky3 = _handPinky2.Find(pinky3);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Pinky3, _handPinky3);
        
        #endregion

        #region Ring
        
        string ringString = handStructure + "_ring";

        string ring1 = ringString + "1";
        _handRing1 = _handWrist.Find(ring1);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Ring1, _handRing1);

        string ring2 = ringString + "2";
        _handRing2 = _handRing1.Find(ring2);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Ring2, _handRing2);

        string ring3 = ringString + "3";
        _handRing3 = _handRing2.Find(ring3);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Ring3, _handRing3);
        
        #endregion
        
        #region Thumb
        
        string thumbString = handStructure + "_thumb";

        string thumb0 = thumbString + "0";
        _handThumb0 = _handWrist.Find(thumb0);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Thumb0, _handThumb0);

        string thumb1 = thumbString + "1";
        _handThumb1 = _handThumb0.Find(thumb1);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Thumb1, _handThumb1);
        
        string thumb2 = thumbString + "2";
        _handThumb2 = _handThumb1.Find(thumb2);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Thumb2, _handThumb2);
        
        string thumb3 = thumbString + "3";
        _handThumb3 = _handThumb2.Find(thumb3);
        _boneIDMapping.Add(OVRSkeleton.BoneId.Hand_Thumb3, _handThumb3);

        #endregion

        Debug.Log($"Hand {handedness} initialized");

        return true;
    }
}
