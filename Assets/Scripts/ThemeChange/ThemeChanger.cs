
using UnityEngine;

public class ThemeChanger : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] ThemeSO[] themes;
    [SerializeField] int startThemeIndex = 0;

    ThemeSO currentTheme;
    bool isFirstThemeChange = true;

    private void Start()
    {
        SelectFirstCurrentTheme();
        ChangeAllChangeOnThemeChangeObjects();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         Debug.Log("Entered OnTriggerEnter");
        if(player == collision.gameObject)
        {
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