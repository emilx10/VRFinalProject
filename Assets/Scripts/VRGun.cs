using UnityEngine;
using UnityEngine.InputSystem;

public class VRGun : MonoBehaviour
{
    public Transform muzzle;
    public float range = 30f;
    public LayerMask enemyLayer;
    public InputActionProperty fireAction;

    void Update()
    {
        if (fireAction.action.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, enemyLayer))
        {
            Destroy(hit.collider.gameObject);
        }
    }
}