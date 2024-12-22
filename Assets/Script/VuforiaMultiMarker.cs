using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Events;

public class VuforiaMultiMarker : MonoBehaviour
{
    [System.Serializable]
    public class CTargetMarker
    {
        public string MarkerName;
        public bool MarkerStatus;
    }

    [Header("Target Markers")]
    public List<CTargetMarker> TargetMarker;

    [Header("Tracked Event")]
    public UnityEvent OnTrackedFound;
    public UnityEvent OnTrackedLost;

    public void SetTrackedTrue(int aIndex)
    {
        TargetMarker[aIndex].MarkerStatus = true;
    }

    public void SetTrackedFalse(int aIndex)
    {
        TargetMarker[aIndex].MarkerStatus = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool AllTrackedFound()
    {
        bool result = true;
        for (int i = 0; i < TargetMarker.Count; i++)
        {
            if (TargetMarker[i].MarkerStatus == false)
            {
                result = false;
            }
        }
        return result;
    }

    bool AllTrackedLost()
    {
        bool result = false;
        for (int i = 0; i < TargetMarker.Count; i++)
        {
            if (TargetMarker[i].MarkerStatus == true)
            {
                result = true;
            }
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        if (AllTrackedFound())
        {
            OnTrackedFound.Invoke();
        }
        else if (!AllTrackedLost())
        {
            OnTrackedLost.Invoke();
        }
    }
}
