using UnityEngine;

public class KamehamehaXR : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform headCamera;
    public float intentionalHoldTime = 0.25f;
    private float holdTimer;

    [Header("Prefabs")]
    public GameObject energyBallPrefab;
    public GameObject beamPrefab;

    [Header("Gesture Settings")]
    public float handCloseDistance = 0.15f;
    public float fireDistance = 0.35f;
    public float chargeTime = 2f;

    [Header("Beam Settings")]
    public float beamSpeed = 25f;

    private GameObject energyBallInstance;

    private float chargeTimer;
    private float previousHandDistance;

    private bool charging;
    private bool charged;

    void Start()
    {
        // Initialize distance so it doesn�t instantly trigger
        previousHandDistance = Vector3.Distance(leftHand.position, rightHand.position);
    }

    void Update()
    {
        float currentHandDistance = Vector3.Distance(leftHand.position, rightHand.position);

        // === START CHARGING (gesture-based) ===
        if (!charging && currentHandDistance <= handCloseDistance)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= intentionalHoldTime)
            {
                StartCharging();
            }
        }
        else
        {
            holdTimer = 0f;
        }

        // === CHARGING ===
        if (charging)
        {
            chargeTimer += Time.deltaTime;

            // Follow hands
            energyBallInstance.transform.position =
                (leftHand.position + rightHand.position) / 2f;

            float scale = Mathf.Lerp(0.1f, 0.5f, chargeTimer / chargeTime);
            energyBallInstance.transform.localScale = Vector3.one * scale;

            // Fully charged
            if (chargeTimer >= chargeTime)
            {
                charged = true;
            }

            // Released too early ? cancel
            if (currentHandDistance > handCloseDistance && !charged)
            {
                CancelCharge();
            }
        }

        // === FIRE ===
        if (charged && currentHandDistance > fireDistance)
        {
            FireBeam();
        }

        previousHandDistance = currentHandDistance;
    }

    void StartCharging()
    {
        charging = true;
        charged = false;
        chargeTimer = 0f;

        Vector3 pos = (leftHand.position + rightHand.position) / 2f;
        energyBallInstance = Instantiate(energyBallPrefab, pos, Quaternion.identity);
    }

    void CancelCharge()
    {
        charging = false;
        charged = false;
        chargeTimer = 0f;

        if (energyBallInstance != null)
            Destroy(energyBallInstance);
    }

    void FireBeam()
    {
        charging = false;
        charged = false;

        Vector3 firePos = energyBallInstance.transform.position;
        Destroy(energyBallInstance);

        Vector3 dir = headCamera.forward;

        GameObject beam = Instantiate(beamPrefab, firePos, Quaternion.LookRotation(dir));
        Rigidbody rb = beam.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * beamSpeed;
    }
}