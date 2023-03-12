using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

	public GameObject nameBarPrefab;

	private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddCharacterNameBar(Transform owner, Character character)
	{
		// 初始化姓名条，加入管理字典
		GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
		goNameBar.name = "NameBar" + character.entityId;
		goNameBar.GetComponent<UIWorldElement>().owner = owner;
		goNameBar.GetComponent<UINameBar>().character = character;
		goNameBar.SetActive(true);

		this.elements[owner] = goNameBar;
    }

	public void RemoveCharacterNameBar(Transform owner)
	{
		if (this.elements.ContainsKey(owner))
		{
			// 销毁元素，移除出管理字典
			Destroy(this.elements[owner]);
            this.elements.Remove(owner);
        }
		
	}
}
