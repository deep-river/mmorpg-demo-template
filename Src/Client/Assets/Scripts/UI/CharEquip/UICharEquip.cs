﻿using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UICharEquip : UIWindow {
	public Text title;
	public Text money;

	public GameObject itemPrefab;
	public GameObject itemEquippedPrefab;

	public Transform itemListRoot;
	public List<Transform> slots;

	// Use this for initialization
	void Start () {
		RefreshUI();
		EquipManager.Instance.OnEquipChanged += RefreshUI;
	}

	private void OnDestroy()
	{
		EquipManager.Instance.OnEquipChanged -= RefreshUI;
	}

	void RefreshUI()
	{
		ClearAllEquipList();
		InitAllEquipItems();
		ClearEquippedList();
		InitEquippedItems();
		this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
	}

	void InitAllEquipItems()
	{
		foreach (var kv in ItemManager.Instance.Items)
		{
			if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)
			{
				if (EquipManager.Instance.Contains(kv.Key))
					continue;
				GameObject go = Instantiate(itemPrefab, itemListRoot);
				UIEquipItem ui = go.GetComponent<UIEquipItem>();
				ui.SetEquipItem(kv.Key, kv.Value, this, false);
			}
		}
	}

	void ClearAllEquipList()
	{
		foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
		{
			Destroy(item.gameObject);
		}
	}

	void ClearEquippedList()
	{
		foreach (var item in slots)
		{
			if (item.childCount > 1)
				// 清除slots下的装备图标，注意slot下的装备类型text不要被销毁
				Destroy(item.GetChild(1).gameObject);
		}
	}

	void InitEquippedItems()
	{
		for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
		{
			var item = EquipManager.Instance.Equips[i];
			{
				if (item != null)
				{
					GameObject go = Instantiate(itemEquippedPrefab, slots[i]);
					UIEquipItem ui = go.GetComponent<UIEquipItem>();
					ui.SetEquipItem(i, item, this, true);

                }
			}
		}
	}

	public void DoEquip(Item item)
	{
		EquipManager.Instance.EquipItem(item);
	}

	public void UnEquip(Item item)
	{
		EquipManager.Instance.UnEquipItem(item);
	}
}
