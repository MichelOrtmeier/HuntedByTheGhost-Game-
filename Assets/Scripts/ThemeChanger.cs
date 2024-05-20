
using UnityEngine;

public class ThemeChanger : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] ThemeSO[] themes;
    [SerializeField] int startThemeIndex = 0;

    ThemeSO currentTheme;
    bool isFirstThemeChange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(player == collision.gameObject)
        {
            SelectFirstCurrentTheme();
            ChangeTheme();
        }
    }

    private void SelectFirstCurrentTheme()
    {
        if (isFirstThemeChange)
        {
            currentTheme = themes[startThemeIndex];
            isFirstThemeChange = false;
        }
    }

    private void ChangeTheme()
    {
        currentTheme = GetNextTheme();
        foreach(ChangeOnThemeChange themeObject in FindObjectsOfType<ChangeOnThemeChange>())
        {
            themeObject.ChangeTheme(currentTheme);
        }
    }

    private ThemeSO GetNextTheme()
    {
        ThemeSO nextTheme;
        do
        {
            nextTheme = themes[Random.Range(0, themes.Length)];
        } while (currentTheme != null && nextTheme == currentTheme && themes.Length > 1);
        return nextTheme;
    }
}