using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Valve.VR.InteractionSystem;

public class RealTimeThrowable : Throwable
{
    private RealtimeTransform rtTransform;
    RealtimeView rtView;
    public int Ownership = -1;
    // Start is called before the first frame update
    void Start()
    {
        rtTransform = gameObject.GetComponent<RealtimeTransform>();
        rtView = gameObject.GetComponent<RealtimeView>();
    }
    protected override void OnHandHoverBegin(Hand hand)
    {
        GrabTypes bestGrabType = hand.GetBestGrabbingType();

        if (bestGrabType != GrabTypes.None && rtTransform.ownerID == -1)
        {
            //if (rigidbody.velocity.magnitude >= catchingThreshold)
            {
                hand.AttachObject(gameObject, bestGrabType, attachmentFlags);
            }
        }

    }
    
    public void Grabbed()
    {
        rtTransform.RequestOwnership();
        Ownership = rtTransform.ownerID;
    }
}
