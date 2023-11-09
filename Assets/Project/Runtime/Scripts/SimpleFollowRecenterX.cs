using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;

public class SimpleFollowRecenterX : MonoBehaviour
{
    [HideInInspector] public bool recenter;
    [SerializeField] float recenterTime;

    CinemachineFreeLook vcam;
    CinemachineVirtualCamera[] rigs = new CinemachineVirtualCamera[3];
    CinemachineOrbitalTransposer[] orbital = new CinemachineOrbitalTransposer[3];
    Transform target;

    void Awake()
    {
        vcam = GetComponent<CinemachineFreeLook>();
        for (int i = 0; vcam != null && i < 3; ++i)
        {
            rigs[i] = vcam.GetRig(i);
            orbital[i] = rigs[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }
        target = vcam != null ? vcam.Follow : null;
    }

    void Update()
    {
        // Disable the transposers while recentering
        for (int i = 0; i < 3; ++i)
            orbital[i].enabled = !recenter;

        if (recenter)
        {
            // How far away from centered are we?
            Vector3 up = vcam.State.ReferenceUp;
            Vector3 back = vcam.transform.position - target.position;
            float angle = UnityVectorExtensions.SignedAngle(
                back.ProjectOntoPlane(up), -target.forward.ProjectOntoPlane(up), up);
            if (Mathf.Abs(angle) < 0.1)
                recenter = false; // done!

            // Do the recentering on all 3 rigs
            angle = Damper.Damp(angle, recenterTime, Time.deltaTime);
            for (int i = 0; recenter && i < 3; ++i)
            {
                Vector3 pos = rigs[i].transform.position - target.position;
                pos = Quaternion.AngleAxis(angle, up) * pos;
                rigs[i].transform.position = pos + target.position;
            }
        }
    }
}
