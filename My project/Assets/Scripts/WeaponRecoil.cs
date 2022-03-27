using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponRecoil : MonoBehaviour
{
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float returnSpeed;
    public float snappiness;

    public static Action OnRecoil;
    public static Action<float, float, float, float, float> OnSetRecoil;

    private Vector3 targetRotation;
    private Vector3 currentRotation;

    private void Start()
    {
        OnRecoil += Recoil;
        OnSetRecoil += SetRecoil;
    }

    private void OnEnable()
    {
        OnRecoil += Recoil;
        OnSetRecoil += SetRecoil;
    }

    private void OnDisable()
    {
        OnRecoil -= Recoil;
        OnSetRecoil -= SetRecoil;
    }

    private void OnDestroy()
    {
        OnRecoil -= Recoil;
        OnSetRecoil -= SetRecoil;
    }

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    private void Recoil()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    private void SetRecoil(float recX, float recY, float recZ, float returnS, float snapp)
    {
        recoilX = recX;
        recoilY = recY;
        recoilZ = recZ;
        returnSpeed = returnS;
        snappiness = snapp;
    }
}
