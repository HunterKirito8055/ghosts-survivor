using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateScreen : MonoBehaviour
{
    [SerializeField] private List<LevelSlotUI> levelSlotList = new List<LevelSlotUI>();

    public void InitializeSlots(WeaponBase[] weaponsToUpgrade, object[] upgradeInstructions)
    {
        if (weaponsToUpgrade == null || weaponsToUpgrade.Length == 0)
        {
            foreach (LevelSlotUI slot in levelSlotList)
            {
                slot.Hide();
            }
            return;
        }
        int index = 0;
        foreach (LevelSlotUI slot in levelSlotList)
        {
            WeaponBase weapon = weaponsToUpgrade[index];
            bool cooldown = (bool)upgradeInstructions[index];
            string content = cooldown ? $"Decrease Cooldown by 10%" : $"Increase damage by 10%";
            slot.SetLevelSlotUI(weapon.weaponData.weaponName, content, null, $"Level : {weapon.currentWeaponLevel}", weapon.OnLevelUp, cooldown);
            index++;
        }
    }
}
