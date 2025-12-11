using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenuBar : MonoBehaviour
{
    public RectTransform rect;
    public Button button;
    public Image icon;
    public Text text;
    protected int idTweenScaleIcon = -1;
    protected int idTweenScaleText = -1;
    public void ResetButtonStates()
    {
        // Reset colors for all buttons
        text.color = Color.white;
    }
    public void ActivateButton()
    {

        if (idTweenScaleIcon != -1 && LeanTween.descr(idTweenScaleIcon) != null)
        {
            LeanTween.cancel(idTweenScaleIcon);

        }
        if (idTweenScaleText != -1 && LeanTween.descr(idTweenScaleText) != null)
        {
            LeanTween.cancel(idTweenScaleText);

        }

        idTweenScaleIcon = LeanTween.scale(icon.gameObject, Vector3.one * 0.75f, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            idTweenScaleIcon = -1;
        }).setIgnoreTimeScale(true).id;

        idTweenScaleText = LeanTween.scale(text.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            idTweenScaleText = -1;
        }).setIgnoreTimeScale(true).id;
    }
    public void UnActivateButton()
    {

        if (idTweenScaleIcon != -1 && LeanTween.descr(idTweenScaleIcon) != null)
        {
            LeanTween.cancel(idTweenScaleIcon);

        }
        if (idTweenScaleText != -1 && LeanTween.descr(idTweenScaleText) != null)
        {
            LeanTween.cancel(idTweenScaleText);

        }

        idTweenScaleIcon = LeanTween.scale(icon.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            idTweenScaleIcon = -1;
        }).setIgnoreTimeScale(true).id;

        idTweenScaleText = LeanTween.scale(text.gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            idTweenScaleText = -1;
        }).setIgnoreTimeScale(true).id;
    }
}
