using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, ISelectHandler
{
    /// <summary>
    /// Reference to the required button component
    /// </summary>
    protected Button m_Button;

    /// <summary>
    /// The UI text element that displays the name of the level
    /// </summary>
    public Text titleDisplay;

    public Text description;
    public Image ImageChapter;

    public Sprite starAchieved;

    public Image[] stars;

    protected MouseScroll m_MouseScroll;

    protected LevelItem m_Item;
    public Text comingSoon;

    public void ButtonClicked()
    {
        AudioManager.instance.PlayDefaultButtonSound();
        ChangeScenes();
    }

    public void Initialize(LevelItem item, MouseScroll mouseScroll)
    {
        LazyLoad();
        if (titleDisplay == null)
        {
            return;
        }
        m_Item = item;
        titleDisplay.text = item.name;
        description.text = item.description;
        ImageChapter.sprite = item.imageChapter;
        HasPlayedState();
        m_MouseScroll = mouseScroll;
        if (item.isBlock)
        {
            comingSoon.gameObject.SetActive(true);
        }
        else
        {

            comingSoon.gameObject.SetActive(false);
        }
    }

    protected void HasPlayedState()
    {
        GameManager gameManager = GameManager.instance;
        if (gameManager == null)
        {
            return;
        }
        int starsForLevel = gameManager.GetStarsForLevel(m_Item.id);
        for (int i = 0; i < starsForLevel; i++)
        {
            stars[i].sprite = starAchieved;
        }
    }
    protected void ChangeScenes()
    {
        if (m_Item.isBlock) return;
        CharacterControl.Instance.SafelyUnsubscribe();
        SceneLoaderManager.instance.LoadScene(m_Item.scene);

    }

    protected void LazyLoad()
    {
        if (m_Button == null)
        {
            m_Button = GetComponent<Button>();
        }
    }
    protected void OnDestroy()
    {
        if (m_Button != null)
        {
            m_Button.onClick.RemoveAllListeners();
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        m_MouseScroll.SelectChild(this);
    }
}