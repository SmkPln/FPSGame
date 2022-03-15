using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField] private _GunType gunType;

    [Space(10)]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float weaponRange;
    [SerializeField] private float reloadTime;

    [Header("Animation")]
    [SerializeField] private string shootAnimationName;
    [SerializeField] private string reloadAnimationTriggerName; 

    [Header("Sounds")]
    [SerializeField] private AudioClip empty;
    [SerializeField] private AudioClip reload;

    [Space(10)]
    [SerializeField] private AudioClip[] shoots;
    

    [Header("Muzzle")]
    [SerializeField] private bool haveMuzzle;
    [SerializeField] private Transform muzzlePosition;
    [SerializeField] private float scaleFactor;
    [SerializeField] private float timeToDestroy;

    [Header("Ammo")]
    [SerializeField] string ammoObjectName;
    [SerializeField] private int ammo;
    [SerializeField] private int reserveAmmo;

    [Space(10)]
    [SerializeField] private GameObject[] muzzle; 

    [Header("Keyboard")]
    [SerializeField] private KeyCode fireButton;
    [SerializeField] private KeyCode reloadButton;

    private enum _GunType
    {
        Auto,
        Semi
    }
    private bool _canShoot;
    private float _nextFire;
    private Camera _cam;
    private AudioSource _audioSource;
    private AudioSource _audioShootSource;
    private Animator _animator;
    private Text ammoText;
    private WeaponAmmo _weaponAmmo;


    private void Awake()
    {
        _cam = Camera.main;
        _audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        _audioShootSource = gameObject.GetComponent<AudioSource>();
        _animator = gameObject.GetComponent<Animator>();
        ammoText = GameObject.FindGameObjectWithTag("AmmoTextObject").GetComponent<Text>();
        _weaponAmmo = GameObject.Find(ammoObjectName).GetComponent<WeaponAmmo>();
    }

    private void Start()
    {
        _canShoot = true;

        _weaponAmmo.Ammo = ammo;
        _weaponAmmo.ClipSize = ammo;
        _weaponAmmo.AmmoText = ammoText;
        _weaponAmmo.ReserveAmmo = reserveAmmo;
    }

    private void Update()
    {
        if (_canShoot == true)
        {
            if (_weaponAmmo.Ammo >= 1)
            {
                if (Time.time - _nextFire > 1 / fireRate)
                {
                    if (gunType == _GunType.Auto)
                    {
                        if (Input.GetKey(fireButton))
                        {
                            Shoot();
                        }
                    }
                    else if (gunType == _GunType.Semi)
                    {
                        if (Input.GetKeyDown(fireButton))
                        {
                            Shoot();
                        }
                    }
                }
            }
            
        }

        if (Input.GetKeyDown(fireButton))
        {
            if (_weaponAmmo.Ammo <= 0)
            {
                _audioSource.PlayOneShot(empty);
            }
        }

        if(Input.GetKeyDown(reloadButton))
        {
            if (_weaponAmmo.ReserveAmmo > 0 && _weaponAmmo.Ammo < ammo)
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void Shoot()
    {
        _nextFire = Time.time;

        PlayShootSound();
        SpawnMuzzle();
        _animator.Play(shootAnimationName);
        _weaponAmmo.Ammo--;
        _weaponAmmo.UpdateAmmoInScreen();
    }

    private void PlayShootSound()
    {
        AudioClip clip = null;
        clip = shoots[Random.Range(0, shoots.Length)];
        _audioShootSource.clip = clip;
        _audioShootSource.Play();
    }

    private void SpawnMuzzle()
    {
        if (haveMuzzle == true)
        {
            GameObject currentMuzzle = muzzle[0];
            GameObject spawnedMuzzle = Instantiate(currentMuzzle, muzzlePosition.position, muzzlePosition.rotation);
            spawnedMuzzle.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            Destroy(spawnedMuzzle, timeToDestroy);
        }

    }
    private IEnumerator ReloadCoroutine()
    {
        _canShoot = false;
        _animator.SetTrigger(reloadAnimationTriggerName);
        _audioSource.PlayOneShot(reload);
        yield return new WaitForSeconds(reloadTime);
        _weaponAmmo.AddAmmo();
        _weaponAmmo.UpdateAmmoInScreen();
        _canShoot = true;
    }
}
