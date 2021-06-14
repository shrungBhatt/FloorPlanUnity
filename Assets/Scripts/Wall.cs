using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using static Assets.Scripts.Util;

namespace Assets.Scripts
{
    public class Wall : DragObject
    {
        public const string LEFT_WALL = "leftWall";
        public const string RIGHT_WALL = "rightWall";
        public const string TOP_WALL = "topWall";
        public const string BOTTOM_WALL = "bottomWall";


        public GameObject CornerOne;
        public GameObject CornerTwo;
        public bool IsHorizontal;
        // Start is called before the first frame update
        void Start()
        {
            base.IsHorizontalWall = IsHorizontal;
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void OnMouseDrag()
        {
            base.OnMouseDrag();
            if (IsHorizontal)
            {
                var c1Pos = CornerOne.transform.position;
                c1Pos.y = transform.position.y;
                CornerOne.transform.position = c1Pos;

                var c2Pos = CornerTwo.transform.position;
                c2Pos.y = transform.position.y;
                CornerTwo.transform.position = c2Pos;

            }
            else
            {
                var c1Pos = CornerOne.transform.position;
                c1Pos.x = transform.position.x;
                CornerOne.transform.position = c1Pos;

                var c2Pos = CornerTwo.transform.position;
                c2Pos.x = transform.position.x;
                CornerTwo.transform.position = c2Pos;
            }

            //Pass the update callback to the connector
            CornerOne.GetComponent<Connector>()?.OnCornerPositionChanged(name);
            CornerTwo.GetComponent<Connector>()?.OnCornerPositionChanged(name);
        }

        public void UpdateScale(bool isHorizontal, string cornerName, GameObject wall)
        {
            if (CornerOne == null || CornerTwo == null)
            {
                return;
            }

            var corner = CornerOne.name == cornerName ? CornerOne : CornerTwo;

            var c1ScreenPos = ConvertWorldPositionToScreenPosition(CornerOne.transform.position);
            var c2ScreenPos = ConvertWorldPositionToScreenPosition(CornerTwo.transform.position);

            Vector3 wallPosition = new Vector3(0, 0, 0);
            if (isHorizontal)
            {
                wallPosition.x = (c1ScreenPos.x + c2ScreenPos.x) / 2;
                wallPosition.y = c1ScreenPos.y;
                wallPosition.z = c1ScreenPos.z;
            }
            else
            {
                wallPosition.y = (c1ScreenPos.y + c2ScreenPos.y) / 2;
                wallPosition.x = c1ScreenPos.x;
                wallPosition.z = c1ScreenPos.z;
            }


            transform.position = ConvertScreenPositionToWorldPosition(wallPosition);

            var leftWallRectTransform = transform as RectTransform;
            if (leftWallRectTransform != null)
            {
                var scale = leftWallRectTransform.localScale;
                if (isHorizontal)
                {
                    scale.x = (corner.transform.position.x > wall.transform.position.x ? corner.transform.position.x - wall.transform.position.x : wall.transform.position.x - corner.transform.position.x) * 2;
                }
                else
                {
                    scale.y = (corner.transform.position.y > wall.transform.position.y ? corner.transform.position.y - wall.transform.position.y : wall.transform.position.y - corner.transform.position.y) * 2;

                }

                leftWallRectTransform.localScale = scale;
            }
        }

        public GameObject GetOtherCorner(string cornerBeingDragged)
        {
            if (CornerOne.name.Equals(cornerBeingDragged))
            {
                return CornerTwo;
            }
            else
            {
                return CornerOne;
            }
        }
    }
}
