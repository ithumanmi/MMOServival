using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static ShopItemTypeIcon;
using Lean;
using UnityEngine.UI;

public class CoinMagnetNotify : MonoBehaviour
{
    public Text text;
    public Image icon;
    int idTweenMove = -1;
    int idTweenalpha = -1;
    public CanvasGroup canvasGroup;
    public void Move()
    {
        if (idTweenMove != -1 && LeanTween.descr(idTweenMove) != null)
        {
            LeanTween.cancel(idTweenMove);

        }
        if (idTweenalpha != -1 && LeanTween.descr(idTweenalpha) != null)
        {
            LeanTween.cancel(idTweenalpha);

        }
        canvasGroup.alpha = 1;
        idTweenMove = LeanTween.moveY(gameObject, gameObject.transform.position.y + 500f, 1).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            idTweenMove = -1;
        }).setIgnoreTimeScale(true).id;
        idTweenalpha = LeanTween.alphaCanvas(canvasGroup, 0f, 0.5f).setDelay(1f).setEase(LeanTweenType.easeOutBack).setOnComplete(() =>
        {
            idTweenalpha = -1;
        }).setIgnoreTimeScale(true).id;
    }
}