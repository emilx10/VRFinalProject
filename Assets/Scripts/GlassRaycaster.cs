using System.Collections;
using UnityEngine;

public class GlassRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 50f;
    [SerializeField] private float rayRadius = 0.3f;
    [SerializeField] private LayerMask hitLayer; // include BOTH glass + ammo

    [Header("Gun Settings")]
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireInterval = 0.2f;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    private void Start()
    {
        currentAmmo = magazineSize;
    }

    public void ShootRay()
    {
        if (isReloading)
            return;

        if (Time.time < nextFireTime)
            return;

        // IMPORTANT: allow shooting even at 0 to hit ammo targets
        nextFireTime = Time.time + fireInterval;

        RaycastHit hit;
        Vector3 origin = rayOrigin.position;
        Vector3 direction = rayOrigin.forward;
        Vector3 halfExtents = new Vector3(rayRadius, rayRadius, 0.01f);

        if (Physics.BoxCast(
                origin,
                halfExtents,
                direction,
                out hit,
                rayOrigin.rotation,
                rayDistance,
                hitLayer,
                QueryTriggerInteraction.Collide))
        {
            //  PRIORITY: Ammo target
            AmmoTarget ammoTarget = hit.collider.GetComponent<AmmoTarget>();
            if (ammoTarget != null)
            {
                ammoTarget.Hit(this);
                Debug.Log("<Color=Cyan>Ammo Target Hit!</Color>");
                return;
            }

            //  Only consume ammo if NOT ammo target
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            currentAmmo--;

            ShootableNote note = hit.collider.GetComponent<ShootableNote>();

            if (note != null)
            {
                bool hitSuccess = note.TryHit();

                if (hitSuccess)
                    Debug.Log("<Color=Green>Rhythm Hit!</Color>");
                else
                    Debug.Log("<Color=Yellow>Too Early!</Color>");
            }
        }

        Debug.Log($"Ammo: {currentAmmo}/{magazineSize}");
    }

    //  ADD AMMO (CORE FIX)
    public void AddAmmo(int amount)
    {
        if (isReloading)
            isReloading = false; // cancel reload

        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, magazineSize);

        Debug.Log($"<Color=Cyan>Picked Ammo! {currentAmmo}/{magazineSize}</Color>");
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload Complete!");
    }

    private void OnDrawGizmos()
    {
        if (rayOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance);
    }
}