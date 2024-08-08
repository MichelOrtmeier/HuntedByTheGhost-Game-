
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ThemeOnExceededXDistanceChanger : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] ThemeSO[] themes;
    [SerializeField] int startThemeIndex = 0;
    [SerializeField] int xDistanceExceededByPlayerBetweenKeySpawns = 50;
    [SerializeField] int xDistanceVariation = 10;

    ThemeSO currentTheme;
    bool isFirstThemeChange = true;
    Vector3 lastPlayerPosition;
    int nextXDistance;

    private void Start()
    {
        SelectFirstCurrentTheme();
        ChangeAllChangeOnThemeChangeObjects();
        ResetNextXDistanceVariables();
    }

    private void ResetNextXDistanceVariables()
    {
        nextXDistance = GetNextXDistance();
        lastPlayerPosition.x = player.position.x;
    }

    private int GetNextXDistance()
    {
        return Random.Range(xDistanceExceededByPlayerBetweenKeySpawns - xDistanceVariation, xDistanceExceededByPlayerBetweenKeySpawns + xDistanceVariation);
    }

    private void SelectFirstCurrentTheme()
    {
        if (isFirstThemeChange)
        {
            currentTheme = themes[startThemeIndex];
            isFirstThemeChange = false;
        }
    }

    void Update()
    {
        if (player.position.x - lastPlayerPosition.x > nextXDistance)
        {
            ChangeTheme();
            ResetNextXDistanceVariables();
        }
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