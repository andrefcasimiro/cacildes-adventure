using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBlockingPivot : MonoBehaviour
{
    public GameObject defaultWeaponPivot;

    private void Start()
    {
        HidePivot();
    }

    public void ShowPivot()
    {
        defaultWeaponPivot.gameObject.SetActive(false);
    }

    public void HidePivot()
    {
        defaultWeaponPivot.gameObject.SetActive(true);
    }

}
