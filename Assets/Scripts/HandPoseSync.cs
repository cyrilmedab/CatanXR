using UnityEngine;
using Normal.Realtime;

public class HandPoseSync : RealtimeComponent<GenericStringModel>
{
    private HandSkeletonSerializer _handSkeletonSerializer;

    private void Awake() => _handSkeletonSerializer = GetComponent<HandSkeletonSerializer>();

    protected override void OnRealtimeModelReplaced(GenericStringModel previousModel, GenericStringModel currentModel)
    {
        base.OnRealtimeModelReplaced(previousModel, currentModel);
        if (previousModel != null) previousModel.stringValueDidChange -= ReceivedData;

        if (currentModel != null)
        {
            if (!currentModel.isFreshModel) ReceivedData(currentModel, currentModel.stringValue);
            currentModel.stringValueDidChange += ReceivedData;
        }
    }

    private void ReceivedData(GenericStringModel model, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        _handSkeletonSerializer.DeserializeSkeletalData(value);
    }

    public void SendData(string data) => model.stringValue = data;

}
