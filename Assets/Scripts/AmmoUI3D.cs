using UnityEngine;
using TMPro;

public class AmmoUI3D : MonoBehaviour
{
    [SerializeField] private GlassRaycaster gun;
    [SerializeField] private TextMeshPro textMesh;

    private void Update()
    {
        if (gun == null || textMesh == null) return;

        if (gun.IsReloading)
        {
            textMesh.text = "Reloading...";
        }
        else
        {
            textMesh.text = $"Ammo: {gun.CurrentAmmo}/{gun.MagazineSize}";
        }
    }
}