using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Util;

namespace Assets.Scripts
{
    public class Connector : DragObject
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

        protected override void OnMouseDrag()
        {
            base.OnMouseDrag();
            //For each wall conencted to this point
            foreach (var wall in Walls)
            {
                //Find the other corner of the wall
                var wallScript = wall.GetComponent<Wall>();
                if (wallScript != null)
                {
                    var otherCorner = wallScript.GetOtherCorner(name);
                    if (otherCorner != null)
                    {
                        var c1Pos = ConvertWorldPositionToScreenPosition(transform.position);
                        var c2Pos = ConvertWorldPositionToScreenPosition(otherCorner.transform.position);

                        //Find the midpoint between these two corners, this point should be in screen coordinates
                        var midpoint = new Vector2((c1Pos.x + c2Pos.x) / 2, (c1Pos.y + c2Pos.y) / 2);

                        //Find the angle between both the corners
                        Vector3 dir = c2Pos - c1Pos;
                        dir = otherCorner.transform.InverseTransformDirection(dir);
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        //float angle = CalculateAngle(transform.position, otherCorner.transform.position);

                        //Convert the screen coordinates to world co-ordinates.
                        var midPointScreenPos = ConvertScreenPositionToWorldPosition(midpoint);

                        //Translate the wall transform to the midpoint found in above step
                        wall.transform.position = new Vector3(midPointScreenPos.x, midPointScreenPos.y, wall.transform.position.z);


                        //Rotate the wall transform by that angle.
                        wall.transform.rotation = Quaternion.Euler(0, 0, angle);

                        Debug.Log($"angle: {angle}");
                    }
                    else
                    {
                        Debug.LogWarning("Other corner is null");
                    }
                }
                else
                {
                    Debug.LogWarning("Wall script is null");
                }


            }

        }

        public static float CalculateAngle(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }

        protected override void OnMouseUp()
        {
            base.OnMouseUp();
        }
    }
}