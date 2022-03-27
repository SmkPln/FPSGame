using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{
    public int selectedWeapon = 0;
    public GameObject currentWeaponInHand;
    public KeyCode switchButton;
    public bool canSwitch;

    private int _childs;
    private bool _startSwitch;

    public void Start()
    {
        _startSwitch = true;
        SelectWeapon();
    }

    public void Update()
    {
        int prevSelectedWeapon = selectedWeapon;
        _childs = gameObject.transform.childCount;

        if (_startSwitch == true && _childs >= 1 && Input.GetKeyDown(switchButton))
        {
            _startSwitch = false;
            if (canSwitch == true) SelectWeapon();
        }

        if (Input.GetKeyDown(switchButton))
        {
            if (canSwitch == true)
            {
                if (selectedWeapon >= transform.childCount - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon++;
                }
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (canSwitch == true)
            {
                if (selectedWeapon >= transform.childCount - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon++;
                }
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (canSwitch == true)
            {
                if (selectedWeapon <=0)
                {
                    selectedWeapon = transform.childCount - 1;
                }
                else
                {
                    selectedWeapon--;
                }
            }
        }

        if (prevSelectedWeapon != selectedWeapon)
        {
            if (canSwitch == true) SelectWeapon();
        }
    }

    public void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            currentWeaponInHand = gameObject.transform.GetChild(selectedWeapon).gameObject;
            i++;
        }
    }
}
