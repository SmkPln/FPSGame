using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField] private _GunType gunType;
    [SerializeField] private GameObject enemyImpact;

    [Space(10)]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float weaponRange;
    [SerializeField] private float reloadTime;
    [SerializeField] private LayerMask playerMask;

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
    [SerializeField] private KeyCode scopeButton;

    [Header("Sway")]
    [SerializeField] private Vector3 initialSwayPosition;
    [SerializeField] private float swayAmount;
    [SerializeField] private float maxSwayAmount;
    [SerializeField] private float swaySmoothing;
    [SerializeField] private bool canSway;

    [Header("Scope")]
    [SerializeField] private float normalFOV;
    [SerializeField] private float scopedFOV;
    [SerializeField] private float normalSens;
    [SerializeField] private float scopedSens;
    [SerializeField] private float FOVSmoothing;

    [Header("Aiming")]
    [SerializeField] private bool canScope;
    [SerializeField] private Vector3 normalLocalPosition;
    [SerializeField] private Vector3 aimingLocalPosition;
    [SerializeField] private float aimSmoothing;

    [Header("Spread")]
    [SerializeField] private float maxSpread;

    [Header("Recoil")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float snappiness;

    private enum _GunType
    {
        Auto,
        Semi
    }
    private bool _canShoot;
    private bool _inScope;
    private float _nextFire;
    private Camera _cam;
    private Camera _gunCam;
    private AudioSource _audioSource;
    private AudioSource _audioShootSource;
    private Animator _animator;
    private Text ammoText;
    private WeaponAmmo _weaponAmmo;
    private PlayerController _playerController;
    private Transform _startShootPosition;



    private void Awake()
    {
        _cam = Camera.main;
        _gunCam = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        //_audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioShootSource = gameObject.GetComponent<AudioSource>();
        _animator = gameObject.GetComponent<Animator>();
        ammoText = GameObject.FindGameObjectWithTag("AmmoTextObject").GetComponent<Text>();
        _weaponAmmo = GameObject.Find(ammoObjectName).GetComponent<WeaponAmmo>();
        _playerController = FindObjectOfType<PlayerController>();
        _startShootPosition = GameObject.FindGameObjectWithTag("StartShootPosition").transform;

        WeaponRecoil.OnSetRecoil?.Invoke(recoilX, recoilY, recoilZ, returnSpeed, snappiness);
    }

    private void Start()
    {
        _canShoot = true;

        _weaponAmmo.AmmoText = ammoText;
        _weaponAmmo.Ammo = ammo;
        _weaponAmmo.ClipSize = ammo;
        _weaponAmmo.ReserveAmmo += reserveAmmo;
        _weaponAmmo.UpdateAmmoInScreen();

        WeaponRecoil.OnSetRecoil?.Invoke(recoilX, recoilY, recoilZ, returnSpeed, snappiness);
    }

    private void OnEnable()
    {
        _weaponAmmo.UpdateAmmoInScreen();

        WeaponRecoil.OnSetRecoil?.Invoke(recoilX, recoilY, recoilZ, returnSpeed, snappiness);
    }

    private void LateUpdate()
    {
        if (canSway == true)
        {
            float mX = -Input.GetAxis("Mouse X") * swayAmount;
            float mY = -Input.GetAxis("Mouse Y") * swayAmount;
            mX = Mathf.Clamp(mX, -maxSwayAmount, maxSwayAmount);
            mY = Mathf.Clamp(mY, -maxSwayAmount, maxSwayAmount);
            Vector3 finalSwayPosition = new Vector3(mX, mY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalSwayPosition + initialSwayPosition, Time.deltaTime * swaySmoothing);
        }
    }

    private void Update()
    {
        Scope();
        KeyCodes();
        Aiming();
    }

    private void KeyCodes()
    {
        if (_canShoot == true)
        {
            if (_weaponAmmo.Ammo > 0)
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

        if (Input.GetKeyDown(reloadButton))
        {
            if (_weaponAmmo.ReserveAmmo > 0 && _weaponAmmo.Ammo < ammo)
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void Scope()
    {
        if (_inScope == true)
        {
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, scopedFOV, FOVSmoothing * Time.deltaTime);
            _gunCam.fieldOfView = Mathf.Lerp(_gunCam.fieldOfView, scopedFOV, FOVSmoothing * Time.deltaTime);
        }
        else if (_inScope == false)
        {
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, normalFOV, FOVSmoothing * Time.deltaTime);
            _gunCam.fieldOfView = Mathf.Lerp(_gunCam.fieldOfView, normalFOV, FOVSmoothing * Time.deltaTime);
        }
    }

    private void Aiming()
    {
        if (canScope == true)
        {
            Vector3 target = normalLocalPosition;

            if (Input.GetKey(scopeButton))
            {
                initialSwayPosition = aimingLocalPosition;
                target = aimingLocalPosition;
                _inScope = true;
            }
            else if (Input.GetKeyUp(scopeButton))
            {
                initialSwayPosition = normalLocalPosition;
                _inScope = false;
            }

            Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSmoothing);

            transform.localPosition = desiredPosition;
        }
    }

    private void Shoot()
    {
        _nextFire = Time.time;

        if (_inScope == true)
        {
            Ray ray = new Ray(_startShootPosition.position, _startShootPosition.forward);
            RayCasting(ray);
        }
        else
        {
            Ray ray = new Ray(_startShootPosition.position, GenerateVectorToSpread());
            RayCasting(ray);
            _startShootPosition.localRotation = Quaternion.Euler(0, 0, 0);
        }
       
        PlayShootSound();
        SpawnMuzzle();
        WeaponRecoil.OnRecoil?.Invoke();
        _animator.Play(shootAnimationName);
        _weaponAmmo.Ammo--;
        _weaponAmmo.UpdateAmmoInScreen();
    }

    private void RayCasting(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, weaponRange, playerMask))
        {
            GameObject particle = null;
            if (hit.transform.GetComponent<ImpactType>())
            {
                ImpactType currentImpact = hit.transform.GetComponent<ImpactType>();
                if (currentImpact != null)
                {
                    Instantiate(currentImpact.currentImpact, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                IDamagable hitedEnemy = hit.collider.GetComponent<IDamagable>();
                hitedEnemy.TakeDamage(damage);

                GameObject impact = Instantiate(enemyImpact, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 5f);
            }

            Destroy(particle, timeToDestroy);
        }
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

    private Vector3 GenerateVectorToSpread()
    {
        float currentSpread = maxSpread / 10;
        float x = _startShootPosition.localRotation.x + Random.Range(-currentSpread, currentSpread);
        float y = _startShootPosition.localRotation.y + Random.Range(-currentSpread, currentSpread);
        float z = _startShootPosition.localRotation.z + Random.Range(-currentSpread, currentSpread);

        _startShootPosition.localRotation = Quaternion.Euler(x, y, z);

        Vector3 forward = _startShootPosition.TransformDirection(Vector3.forward);
        return forward;
    }
}


