using Antada.Libs;
using NFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingPopupSub : BaseUIView
{
    [SerializeField] private List<GameObject> _starFills = new List<GameObject>();
    [SerializeField] private List<Button> _starButtons = new List<Button>();
    [SerializeField] private Button _remindLaterButton;
    private void Awake()
    {
        _remindLaterButton.onClick.AddListener(OnRemindLaterButtonClicked);
        for (int i = 0; i < _starButtons.Count; i++)
        {
            var index = i;
            _starButtons[i].onClick.AddListener(() => OnStarButtonClicked(index));
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        _starFills.ForEach(star => star.SetActive(false));
    }

    private void OnStarButtonClicked(int index)
    {
        for (int i = 0; i <= index; i++)
        {
            _starFills[i].SetActive(true);
        }

        UIManager.I.DisableInteract(this);
        this.InvokeDelay(0.25f, () =>
        {
            UIManager.I.EnableInteract(this);
            if (index == 4)
            {
                NFramework.Logger.Log("ShowNativeRatingPopup");
                ReviewClientManager.I.ShowNativeRatingPopup();
            }

            CloseSelf();
        });
    }

    private void OnRemindLaterButtonClicked()
    {
        CloseSelf();
    }
}
