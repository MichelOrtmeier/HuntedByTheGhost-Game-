using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ChangeOnThemeChange : MonoBehaviour
{
    public abstract void ChangeTheme(ThemeSO newTheme);
}