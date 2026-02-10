using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabMovesLinePointZ : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int linePointIndex = 1;
    [SerializeField] BowArrowShooter bas;

    XRGrabInteractable grab;
    bool isGrabbed;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        bas.SpawnArrow();
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        bas.ReleaseArrow();
    }

    void Update()
    {
        if (!isGrabbed) return;

        Vector3 point = lineRenderer.GetPosition(linePointIndex);

        // Move string with grab
        point.z = transform.position.z;

        lineRenderer.SetPosition(linePointIndex, point);
    }
}
