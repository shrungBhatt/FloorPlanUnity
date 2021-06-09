using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Connector;
using static Assets.Scripts.Util;
using static Assets.Scripts.Wall;

namespace Assets.Scripts
{
    public class Room : DragObject
    {

        public List<GameObject> Corners = new List<GameObject>();
        public List<GameObject> Walls = new List<GameObject>();
        public GameObject BackgroundPrefab;

        BoxCollider2D _collider;
        GameObject _backgroundGrid;

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
            _backgroundGrid = Instantiate(BackgroundPrefab, Vector3.zero, Quaternion.identity);
            _backgroundGrid.name = "background";
            _backgroundGrid.transform.SetParent(transform);
        }

        private void Update()
        {
            if(Walls.Count > 0)
            {
                var leftWallPos = Vector3.zero;
                var topWallPos = Vector3.zero;
                var rightWallPos = Vector3.zero;
                var bottomWallPos = Vector3.zero;

                var leftWallScale = Vector3.zero;
                var topWallScale = Vector3.zero;

                foreach(var wall in Walls)
                {
                    switch (wall.name)
                    {
                        case LEFT_WALL:
                            leftWallPos = ConvertWorldPositionToScreenPosition(wall.transform.position);
                            leftWallScale = wall.transform.localScale;
                            break;
                        case TOP_WALL:
                            topWallPos = ConvertWorldPositionToScreenPosition(wall.transform.position);
                            topWallScale = wall.transform.localScale;
                            break;
                        case RIGHT_WALL:
                            rightWallPos = ConvertWorldPositionToScreenPosition(wall.transform.position);
                            break;
                        case BOTTOM_WALL:
                            bottomWallPos = ConvertWorldPositionToScreenPosition(wall.transform.position);
                            break;
                    }
                }

                //Set the background prefab transform position
                // X = (rightWall.x + leftWall.x)/2, Y = rightWall.y, Z = rightWall.z
                var position = new Vector3((rightWallPos.x + leftWallPos.x)/2, ((topWallPos.y + bottomWallPos.y))/2, topWallPos.z);

                //Convert the position from Screen -> World position
                var screenPos = ConvertScreenPositionToWorldPosition(position);
                _backgroundGrid.transform.position = screenPos;
                _collider.offset = new Vector2(screenPos.x - _collider.transform.position.x, screenPos.y - _collider.transform.position.y);

                //_collider.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
                //_collider.bounds.center.Set(screenPos.x, screenPos.y, screenPos.z);


                //Change the local scale
                // X = topWallScale.x, Y = leftWallScale.y
                _backgroundGrid.transform.localScale = new Vector3(topWallScale.x/6f, leftWallScale.y/6f, topWallScale.z);
                _collider.size = new Vector2(topWallScale.x - 1.5f, leftWallScale.y - 1.5f);

            }
            
        }

        protected override void OnMouseDrag()
        {
            HighlightWalls(true);
            base.OnMouseDrag();
            _collider.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        private void HighlightWalls(bool flag)
        {
            if(Walls.Count > 0)
            {
                foreach(var wall in Walls)
                {
                    var renderer = wall.GetComponent<SpriteRenderer>();
                    if(renderer != null)
                    {
                        if (flag)
                        {
                            renderer.color = Color.green;
                        }
                        else
                        {
                            renderer.color = Color.black;
                        }
                    }
                }
            }
        }

        protected override void OnMouseUp()
        {
            HighlightWalls(false);
            base.OnMouseUp();
        }
    }
}
