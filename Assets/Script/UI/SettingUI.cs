using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingUI : UICanvas, IPointerDownHandler, IDragHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [Header("UI Music")]
    public RectTransform musicBarArea;
    public Image musicFillImage;

    [Header("UI SFX")]
    public RectTransform sfxBarArea;
    public Image sfxFillImage;
    
    private enum DragTarget
    {
        None,
        Music,
        SFX
    }

    private DragTarget currentTarget = DragTarget.None;

    public override void Open()
    {
        base.Open();
        canvasGroup.alpha = 1;
        float musicValue = AudioManager.Instance.GetMusicVolume();
        float sfxValue   = AudioManager.Instance.GetSFXVolume();

        musicFillImage.fillAmount = musicValue;
        sfxFillImage.fillAmount   = sfxValue;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DetectWhichBar(eventData);
        DragUpdate(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragUpdate(eventData);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //---------------------------------------------------------------------
    private void DetectWhichBar(PointerEventData eventData)
    {
        currentTarget = DragTarget.None;

        if (RectTransformUtility.RectangleContainsScreenPoint(musicBarArea, eventData.position))
        {
            currentTarget = DragTarget.Music;
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(sfxBarArea, eventData.position))
        {
            currentTarget = DragTarget.SFX;
        }
    }

    private void DragUpdate(PointerEventData eventData)
    {
        if (currentTarget == DragTarget.None) return;

        RectTransform targetBar =
            currentTarget == DragTarget.Music ? musicBarArea : sfxBarArea;

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetBar, eventData.position, eventData.pressEventCamera, out localPos))
            return;

        float percent = Mathf.InverseLerp(targetBar.rect.xMin, targetBar.rect.xMax, localPos.x);
        percent = Mathf.Clamp01(percent);

        if (currentTarget == DragTarget.Music)
        {
            musicFillImage.fillAmount = percent;
            AudioManager.Instance.SetMusicVolume(percent);
        }
        else
        {
            sfxFillImage.fillAmount = percent;
            AudioManager.Instance.SetSFXVolume(percent);
        }
    }
}
