using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UIElements;

public abstract class MenuScreen : MonoBehaviour
{
    public Canvas canvas;
    [Tooltip("Set the Main Menu here explicitly (or get automatically from current GameObject).")]
    [SerializeField] protected MainMenuUIManager m_MainMenuUIManager;


    protected virtual void Awake()
    {
        RegisterButtonCallbacks();
    }

    // Thêm callback cho các nút trong menu screen
    protected virtual void RegisterButtonCallbacks()
    {
        // Override trong các lớp con để thêm logic cho các nút
    }
    public void ShowVisualElement(bool state)
    {
        if (canvas == null)
            return;
        canvas.enabled = state;
    }


    // Hiển thị màn hình
    public virtual void ShowScreen()
    {
        AudioManager.instance.PlayAltButtonSound();
        ShowVisualElement(true);

    }

    // Ẩn màn hình
    public virtual void HideScreen()
    {
        ShowVisualElement(false);

    }
}