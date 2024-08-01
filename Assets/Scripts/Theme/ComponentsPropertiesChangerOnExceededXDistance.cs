using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ComponentsPropertiesChangerOnExceededXDistance : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] ComponentsPropertiesSO[] themes;
    [SerializeField] int startThemeIndex = 0;
    [SerializeField] int xDistanceExceededByPlayerBetweenKeySpawns = 50;
    [SerializeField] int xDistanceVariation = 10;

    ComponentsPropertiesSO currentTheme;
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
        foreach (ChangeOnComponentPropertyChange themeObject in FindObjectsOfType<ChangeOnComponentPropertyChange>())
        {
            themeObject.ChangeTheme(currentTheme);
        }
    }

    private ComponentsPropertiesSO GetNextTheme()
    {
        ComponentsPropertiesSO nextTheme = currentTheme;
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