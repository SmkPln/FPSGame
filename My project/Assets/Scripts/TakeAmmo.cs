using UnityEngine;
using UnityEngine.UI;

public class TakeAmmo : MonoBehaviour
{
    [SerializeField] private float takeRange;
    [SerializeField] private KeyCode takeButton;
    [SerializeField] private LayerMask playerMask;

    [Space(10)]
    public WeaponAmmo[] weaponAmmos;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }


    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, takeRange, playerMask))
        {
            if (hit.transform.GetComponent<AmmoItem>())
            {
                AmmoItem currentItem = hit.transform.GetComponent<AmmoItem>();
                if (Input.GetKeyDown(takeButton))
                {
                    for(int i = 0; i<weaponAmmos.Length; i++)
                    {
                        if (currentItem != null)
                        {
                            if (weaponAmmos[i].ammoName == currentItem.ammoName)
                            {
                                weaponAmmos[i].ReserveAmmo += currentItem.ammoAdd;
                                weaponAmmos[i].UpdateAmmoInScreen();
                                Destroy(currentItem.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
}
