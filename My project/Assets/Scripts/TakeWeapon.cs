using UnityEngine;
using UnityEngine.UI;

public class TakeWeapon : MonoBehaviour
{
    [SerializeField] private float takeRange;
    [SerializeField] private KeyCode takeButton;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private Text tooltipText;

    private Camera _cam;

    void Start()
    {
        tooltipText.text = null;
        _cam = Camera.main;
    }


    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, takeRange, playerMask))
        {
            WeaponItem currentItem = hit.transform.GetComponent<WeaponItem>();

            if (currentItem != null) tooltipText.text = "Press " + takeButton.ToString() + "\n to select a " + currentItem.weaponName;

            if (hit.transform.GetComponent<WeaponItem>())
            {
                if (Input.GetKeyDown(takeButton))
                {


                    if (currentItem != null)
                    {
                        GameObject spawnedWeapon = Instantiate(currentItem.weaponPrefab, weaponHolder.transform.position, weaponHolder.transform.rotation);
                        spawnedWeapon.transform.SetParent(weaponHolder.transform);
                        Destroy(currentItem.gameObject);
                    }
                }
            }
            else tooltipText.text = null;
        }
        else tooltipText.text = null;
    }
}
