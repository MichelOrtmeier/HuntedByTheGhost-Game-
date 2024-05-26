using System;
using UnityEngine;

namespace Assets.Scripts
{
    abstract class ColorChanger : MonoBehaviour
    {
        Color32 startColor;
        Color32 endColor;
        float changeSpeed;
        float currentChangePosition = 1;

        public void ChangeColor(Color32 endColor, float changeSpeed)
        {
            startColor = GetChangeColor();
            this.endColor = endColor;
            this.changeSpeed = changeSpeed;
            currentChangePosition = 0;
        }

        private void Update()
        {
            if (currentChangePosition < 1)
            {
                currentChangePosition += Time.deltaTime * changeSpeed;
                currentChangePosition = Math.Clamp(currentChangePosition, 0, 1);
                SetChangeColor(Color32.Lerp(startColor, endColor, currentChangePosition));
            }
        }

        protected abstract Color32 GetChangeColor();
        protected abstract void SetChangeColor(Color32 color);
    }
}
