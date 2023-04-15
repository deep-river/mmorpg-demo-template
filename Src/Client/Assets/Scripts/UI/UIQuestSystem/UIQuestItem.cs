using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    public Text title;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public Quest quest;

    void Start()
    {

    }

    bool isEquipped = false;

    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
        {
            this.title.text = this.quest.Define.Name;
        }
    }
}
