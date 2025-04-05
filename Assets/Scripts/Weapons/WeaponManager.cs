using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    public float changeWeaponAfter = 30f;
    public float timer;
    public List<WeaponBase> weapons = new List<WeaponBase>();
    public List<Toggle> toggles;
    public List<Text> statusTexts;
    public int index = 0;
    public BoolEvent OnLevelup;
    private void Awake()
    {
        OnLevelup.AddListener(LevelUP);
    }
    private void Start()
    {
        timer = changeWeaponAfter;
        //   ActiveWeapon(index);
        UpdateStatus();
    }
    private void Update()
    {
        // timer -= Time.deltaTime;
        // if (timer <= 0)
        // {
        //     timer = changeWeaponAfter;
        //     ActiveWeapon(++index);
        //     if (index >= weapons.Count - 1)
        //     {
        //         index = -1;
        //     }
        // }
        if (Input.GetKeyDown(KeyCode.U))
        {
            LevelUP(true);
        }
    }
    private void ActiveWeapon(int n)
    {
        foreach (WeaponBase VARIABLE in weapons)
        {
            VARIABLE.gameObject.SetActive(false);
        }
        weapons[n].gameObject.SetActive(true);
    }

    public void LevelUP(bool val)
    {
        if (!val)
        {
            return;
        }
        int[] ints = new int[3];
        WeaponBase[] weaponBases = new WeaponBase[3];
        object[] objs = new object[3];
        for (int i = 0; i < 3; i++)
        {
            weaponBases[i] = GetUniqueWeapon(ref weaponBases);
            int rand = Random.Range(0, 2);
            objs[i] = rand == 0;
        }
        GameManager.Instance.updateScreen.InitializeSlots(weaponBases, objs);
    }
    WeaponBase GetUniqueWeapon(ref WeaponBase[] array)
    {
        WeaponBase weapon = null;
        int count = weapons.FindAll(x => x.currentWeaponLevel == 10).Count;
        if (count == weapons.Count)
        {
            return null;
        }
        while (weapon == null || array.Contains(weapon) || weapon.currentWeaponLevel == 10)
        {
            weapon = weapons[Random.Range(0, weapons.Count)];
        }
        return weapon;
    }
    public void UpdateStatus()
    {
        for (int i = 0; i < 10; i++)
        {
            WeaponBase weaponBase = weapons[i];
            statusTexts[i].text = $"{weaponBase.weaponData.weaponName} Level {weaponBase.currentWeaponLevel} \n CD = {weaponBase.stats.CoolDown} | AP = {weaponBase.stats.Damage}";
            toggles[i].onValueChanged.RemoveAllListeners();
            toggles[i].onValueChanged.AddListener((x) => weaponBase.gameObject.SetActive(x));
        }
    }
}
