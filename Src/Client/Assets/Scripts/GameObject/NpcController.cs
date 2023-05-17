using Common.Data;
using Entities;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCController : MonoBehaviour {

	public int npcID;

	public float interactiveDistance = 5f;

	private bool inInteractive = false;

    Color orignColor;

    SkinnedMeshRenderer renderer;

	Animator anim;

	NpcDefine npc;

	NpcQuestStatus questStatus;

	// Use this for initialization
	void Start () {
		renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		anim = this.gameObject.GetComponent<Animator>();
		orignColor = renderer.sharedMaterial.color;
		npc = NPCManager.Instance.GetNpcDefine(npcID);
		this.StartCoroutine(Actions());
		RefreshNpcStatus();
		QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
	}

	void OnQuestStatusChanged(Quest quest)
	{
		this.RefreshNpcStatus();
    }

	void RefreshNpcStatus()
	{
		questStatus = QuestManager.Instance.GetQuestStatusByNpc(npcID);
		UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
	}

	void OnDestroy()
	{
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
		if (UIWorldElementManager.Instance != null)
			UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
    }


    IEnumerator Actions()
	{
		while (true)
		{
			if (inInteractive)
				yield return new WaitForSeconds(2f);
			else
				yield return new WaitForSeconds(Random.Range(5f, 10f));
			this.Relax();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Relax()
	{
		anim.SetTrigger("Relax");
	}

	void Interactive()
	{
		if (!inInteractive)
		{
			inInteractive= true;
			StartCoroutine(DoInteractive());
		}
	}

	IEnumerator DoInteractive()
	{
		yield return FaceToPlayer();
		if (NPCManager.Instance.Interactive(npc))
			anim.SetTrigger("Talk");
		yield return new WaitForSeconds(2f);
		inInteractive = false;
	}

	IEnumerator FaceToPlayer()
	{
		Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
		while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
		{
			this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
			yield return null;
		}
	}


    void OnMouseDown()
	{
		if (IsTouchedUI())
			return;

		float npcDistance = Get2DDistanceFromPlayer();
		if (npcDistance <= interactiveDistance)
			Interactive();
	}

	private void OnMouseOver()
	{
		Highlight(true);

    }

	private void OnMouseEnter()
	{
        Highlight(true);
    }

	private void OnMouseExit()
	{
        Highlight(false);
    }

	void Highlight(bool highlight)
	{
		if (highlight)
		{
			if (renderer.sharedMaterial.color != Color.white)
				renderer.sharedMaterial.color = Color.white;

        }
		else
		{
            if (renderer.sharedMaterial.color != orignColor)
                renderer.sharedMaterial.color = orignColor;
        }
    }

	private float Get2DDistanceFromPlayer()
	{
		if (User.Instance.CurrentCharacter != null)
		{
            Character character = CharacterManager.Instance.Characters[User.Instance.CurrentCharacter.Id];
            Vector2 character2DPosition = new Vector2(character.position.x / 100, character.position.y / 100);
			Vector2 npc2DPosition = new Vector2(this.transform.position.x, this.transform.position.z);
			// Debug.LogFormat("character pos:{0}, npc pos:{1}", character2DPosition, npc2DPosition);
            return Vector2.Distance(character2DPosition, npc2DPosition);
        }
        return -1f;
	}

    bool IsTouchedUI()
    {
        bool touchedUI = false;
        if (Application.isMobilePlatform)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                touchedUI = true;
            }
        }
        else if (EventSystem.current.IsPointerOverGameObject())
        {
            touchedUI = true;
        }
        return touchedUI;
    }
}
