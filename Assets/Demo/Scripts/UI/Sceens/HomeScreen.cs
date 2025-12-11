using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : MenuScreen
{

    public LevelSelectButton selectionPrefab;
    public LayoutGroup layout;
    public Transform rightBuffer;
    public MouseScroll mouseScroll;
    protected LevelList m_LevelList;
    protected List<Button> m_Buttons = new List<Button>();

    protected virtual void Start()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        m_LevelList = GameManager.instance.levelList;
        if (layout == null || selectionPrefab == null || m_LevelList == null)
        {
            return;
        }

        int amount = m_LevelList.Count;
        for (int i = 0; i < amount; i++)
        {
            LevelSelectButton button = CreateButton(m_LevelList[i]);
            button.transform.SetParent(layout.transform);
            button.transform.localScale = Vector3.one;
            m_Buttons.Add(button.GetComponent<Button>());
        }
        if (rightBuffer != null)
        {
            rightBuffer.SetAsLastSibling();
        }
        for (int index = 1; index < m_Buttons.Count - 1; index++)
        {
            Button button = m_Buttons[index];
            SetUpNavigation(button, m_Buttons[index - 1], m_Buttons[index + 1]);
        }



        SetUpNavigation(m_Buttons[m_Buttons.Count - 1], m_Buttons[m_Buttons.Count - 2], null);

        mouseScroll.SetHasRightBuffer(rightBuffer != null);
    }

    protected LevelSelectButton CreateButton(LevelItem item)
    {
        LevelSelectButton button = Instantiate(selectionPrefab);
        button.Initialize(item, mouseScroll);
        return button;
    }

    void SetUpNavigation(Selectable selectable, Selectable left, Selectable right)
    {
        Navigation navigation = selectable.navigation;
        navigation.selectOnLeft = left;
        navigation.selectOnRight = right;
        selectable.navigation = navigation;
    }

}
