using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Util
    {
        public static Vector3 ConvertWorldPositionToScreenPosition(Vector3 worldPosition)
        {
            return Camera.main.WorldToScreenPoint(worldPosition);
        }

        public static Vector3 ConvertScreenPositionToWorldPosition(Vector3 screenPosition)
        {
            return Camera.main.ScreenToWorldPoint(screenPosition);
        }
    }
}
