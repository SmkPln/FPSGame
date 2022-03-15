using UnityEngine;
using UnityEngine.UI;

public class WeaponAmmo : MonoBehaviour
{
    public Text AmmoText { get; set;}
    public int ClipSize { get; set; }
    public int Ammo { get; set; }
    public int ReserveAmmo { get; set; }

    public void AddAmmo()
    {
        int amountNeeded = ClipSize - Ammo;
        if (amountNeeded >= ReserveAmmo)
        {
            Ammo += ReserveAmmo;
            ReserveAmmo -= amountNeeded;
        }
        else
        {
            Ammo = ClipSize;
            ReserveAmmo -= amountNeeded;
        }

        UpdateAmmoInScreen();
    }

    public void UpdateAmmoInScreen()
    {
        AmmoText.text = Ammo + "/" + ReserveAmmo;
        if (Ammo <= 0) Ammo = 0;
        if (ReserveAmmo <= 0) ReserveAmmo = 0;
    }
}
