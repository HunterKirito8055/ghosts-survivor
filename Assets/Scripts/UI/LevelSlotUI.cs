using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LevelSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotName;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private Image image;
    [SerializeField]
    private Button button;
    private bool forCooldown;
    public void SetLevelSlotUI(string _name, string _content, Sprite _sprite, string _level, Action<bool> action, bool cooldown)
    {
        slotName.text = _name;
        content.text = _content;
        image.sprite = _sprite;
        level.gameObject.SetActive(_level != null);
        level.text = _level;
        button.onClick.RemoveAllListeners();
        forCooldown = cooldown;
        if (action != null)
        {
            button.onClick.AddListener(() => action.Invoke(forCooldown));
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
