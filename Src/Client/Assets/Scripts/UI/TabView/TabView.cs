using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour {

    public TabButton[] tabButtons;
    public GameObject[] tabPages;

    public UnityAction<int> OnTabSelect;

    public int index = -1;
    
    IEnumerator Start () {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);
    }


    public void SelectTab(int index)
    {
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index); // 更改按钮图标
                tabPages[i].SetActive(i == index); // 切换页面
            }
            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }
}
