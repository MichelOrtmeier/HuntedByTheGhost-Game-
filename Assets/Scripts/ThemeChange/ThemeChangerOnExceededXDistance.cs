
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ThemeChangerOnExceededXDistance : ExecutorOnExceededXDistance
{
    [SerializeField] ThemeSO[] themes;
    [SerializeField] int startThemeIndex = 0;

    ThemeSO currentTheme;
    bool isFirstThemeChange = true;

    protected override void Start()
    {
        SelectFirstCurrentTheme();
        ChangeAllChangeOnThemeChangeObjects();
        base.Start();
    }

    private void SelectFirstCurrentTheme()
    {
        if (isFirstThemeChange)
        {
            currentTheme = themes[startThemeIndex];
            isFirstThemeChange = false;
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnXDistanceIsExceeded()
    {
        ChangeTheme();
    }

    private void ChangeTheme()
    {
        SelectFirstCurrentTheme();
        currentTheme = GetNextTheme();
        ChangeAllChangeOnThemeChangeObjects();
    }

    private void ChangeAllChangeOnThemeChangeObjects()
    {
        foreach (ChangeOnThemeChange themeObject in FindObjectsOfType<ChangeOnThemeChange>())
        {
            themeObject.ChangeTheme(currentTheme);
        }
    }

    private ThemeSO GetNextTheme()
    {
        ThemeSO nextTheme = currentTheme;
        if(themes.Length > 1)
        {
            do
            {
                nextTheme = themes[Random.Range(0, themes.Length)];
            } while (nextTheme == currentTheme);
        }
        return nextTheme;
    }
}