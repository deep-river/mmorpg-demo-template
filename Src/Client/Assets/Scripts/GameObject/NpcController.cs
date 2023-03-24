using Common.Data;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour {

	public int npcID;

	Animator anim;

	NpcDefine npc;

	// Use this for initialization
	void Start () {
		anim = this.gameObject.GetComponent<Animator>();
		npc = NPCManager.Instance.GetNpcDefine(npcID);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
