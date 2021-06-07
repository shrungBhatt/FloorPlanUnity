using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Connector : MonoBehaviour
    {

        public const string BOTTOM_LEFT_CORNER = "bottomLeftCorner";
        public const string TOP_LEFT_CORNER = "topLeftCorner";
        public const string TOP_RIGHT_CORNER = "topRightCorner";
        public const string BOTTOM_RIGHT_CORNER = "bottomRightCorner";

        public List<GameObject> Walls = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnCornerPositionChanged(string wallToIgnoreUpdate)
        {
            if (Walls != null)
            {
                foreach (var wall in Walls)
                {
                    var component = wall.GetComponent<Wall>();
                    if (!string.IsNullOrEmpty(wallToIgnoreUpdate) && wallToIgnoreUpdate.Equals(component.name))
                    {
                        continue;
                    }
                    else
                    {
                        component?.UpdateScale(component.IsHorizontal, name, component.gameObject);
                    }

                }
            }
        }
    }
}