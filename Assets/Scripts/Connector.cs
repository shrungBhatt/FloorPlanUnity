using Assets.Scripts;
using System;
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
        public bool EnableFreeMovement;
        // Start is called before the first frame update
        void Start()
        {
            enableFreeMovement = EnableFreeMovement;
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
                        var midpoint = new Vector3((c1Pos.x + c2Pos.x) / 2, (c1Pos.y + c2Pos.y) / 2, c1Pos.z);


                        Debug.Log($"horizontal: {EnableFreeMovement}");
                        Debug.Log($"c1Pos: {transform.position}");
                        //Find the angle between both the corners
                        //Vector3 dir = c2Pos - c1Pos;
                        //dir = otherCorner.transform.InverseTransformDirection(dir);
                        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        //float angle = CalculateAngle(transform.position, otherCorner.transform.position);

                        //Convert the screen coordinates to world co-ordinates.
                        var midPointScreenPos = ConvertScreenPositionToWorldPosition(midpoint);

                        //Translate the wall transform to the midpoint found in above step
                        wall.transform.position = new Vector3(midPointScreenPos.x, midPointScreenPos.y, wall.transform.position.z);


                        var degree = AngleInDeg(c1Pos, c2Pos);

                        //Rotate the wall transform by that angle.
                        wall.transform.rotation = Quaternion.Euler(0, 0, degree);

                        Debug.Log($"angle: {degree}");
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

        //This returns the angle in radians
        public static float AngleInRad(Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        //This returns the angle in degrees
        public static float AngleInDeg(Vector3 vec1, Vector3 vec2)
        {
            return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
        }

        static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            {
                x =
                    (float)
                    (cosTheta * (pointToRotate.x - centerPoint.x) -
                    sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
                y =
                    (float)
                    (sinTheta * (pointToRotate.x - centerPoint.x) +
                    cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y)
            };
        }

        protected override void OnMouseUp()
        {
            base.OnMouseUp();
        }
    }
}