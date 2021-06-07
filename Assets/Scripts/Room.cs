using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Connector;
using static Assets.Scripts.Util;

namespace Assets.Scripts
{
    public class Room : MonoBehaviour
    {

        public List<GameObject> Corners = new List<GameObject>();
        public List<GameObject> Walls = new List<GameObject>();
        public Texture BackgroundImage;

        BoxCollider2D Collider;

        private void Start()
        {
            Collider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            
        }

        private void OnGUI()
        {
            if(Corners.Count > 0)
            {
                var position = Vector2.zero;
                var topLeftCornerPos = Vector2.zero;
                var bottomRightCornerPos = Vector2.zero;

                foreach(var corner in Corners)
                {
                    switch (corner.name)
                    {
                        case BOTTOM_LEFT_CORNER:
                            var screenPos = ConvertWorldPositionToScreenPosition(corner.transform.position);
                            position.x = screenPos.x;
                            position.y = screenPos.y;
                            break;
                        case BOTTOM_RIGHT_CORNER:
                            bottomRightCornerPos = ConvertWorldPositionToScreenPosition(corner.transform.position);
                            break;
                        case TOP_LEFT_CORNER:
                            topLeftCornerPos = ConvertWorldPositionToScreenPosition(corner.transform.position);
                            break;
                    }
                }

                
                var rect = new Rect(position.x, position.y, bottomRightCornerPos.x - position.x, topLeftCornerPos.y - position.y);
                GUI.DrawTexture(rect, BackgroundImage, ScaleMode.ScaleAndCrop);
                Debug.Log($"Postion of X:{rect.x}, Y:{rect.y} ###### Width:{rect.width}, Height:{rect.height}");
            } 
        }

    }
}
