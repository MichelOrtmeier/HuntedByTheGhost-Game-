using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ChangeOnComponentPropertyChange : MonoBehaviour
{
    public abstract void ChangeTheme(ComponetPropertySO newTheme);
}