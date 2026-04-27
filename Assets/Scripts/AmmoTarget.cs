using UnityEngine;

public class AmmoTarget : MonoBehaviour
{
    [SerializeField] private int ammoGiven = 10;

    private bool isHit = false;

    public void Hit(GlassRaycaster gun)
    {
        if (isHit) return;

        isHit = true;

        gun.AddAmmo(ammoGiven);

        gameObject.SetActive(false); // return to pool
    }

    private void OnEnable()
    {
        isHit = false;
    }
}