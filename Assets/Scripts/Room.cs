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
        public Vector3 offsetPosition;

        PolygonCollider2D _collider;
        GameObject _backgroundGrid;

        private void Start()
        {
            _collider = GetComponent<PolygonCollider2D>();
            _backgroundGrid = Instantiate(BackgroundPrefab, Vector3.zero, Quaternion.identity);
            _backgroundGrid.name = "background";
            enableFreeMovement = true;
            _backgroundGrid.transform.SetParent(transform);
            var centroid = GetCentroidOfPolygon(Corners.Select(x => x.transform.position).ToList());
            PivotTo(centroid, transform);
            
            AlignTheRoom();
            SetupColliderPoints();
        }

        public void PivotTo(Vector3 position, Transform transform)
        {
            Vector3 offset = transform.position - position;
            foreach (Transform child in transform)
                child.transform.position += offset;
            transform.position = position;
        }

        private void AlignTheRoom()
        {
            if(Corners != null)
            {
                if(Corners.Count > 2)
                {
                    var firstCorner = Corners[0];
                    var secondCorner = Corners[1];

                    var slope = (secondCorner.transform.position.y - firstCorner.transform.position.y) / (secondCorner.transform.position.x - firstCorner.transform.position.x);
                    bool isPositiveSlope = slope >= 0;
                    var degree = Math.Atan(slope) * 180 / Math.PI;

                    var offsetAngle = (float) (isPositiveSlope ? 360 - degree : (180 - degree));

                    transform.rotation = Quaternion.Euler(0, 0, isPositiveSlope ? offsetAngle : offsetAngle - 180);
                }
                
            }
        }

        private void Update()
        {
            if (Walls.Count > 0)
            {
                var leftWallPos = Vector3.zero;
                var topWallPos = Vector3.zero;
                var rightWallPos = Vector3.zero;
                var bottomWallPos = Vector3.zero;

                var leftWallScale = Vector3.zero;
                var topWallScale = Vector3.zero;

                foreach (var wall in Walls)
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
                //var position = transform.position;

                ////Convert the position from Screen -> World position
                var screenPos = transform.position;
                _backgroundGrid.transform.position = screenPos;
                var bounds = _collider.bounds;
                _collider.offset = new Vector2(screenPos.x - (_collider.transform.position.x * 2), screenPos.y - (_collider.transform.position.y * 2));

                SetupColliderPoints();
                
                //Change the local scale
                // X = topWallScale.x, Y = leftWallScale.y
                _backgroundGrid.transform.localScale = new Vector3(topWallScale.x / 6f, leftWallScale.y / 6f, topWallScale.z);

            }

        }

        protected override void OnMouseDrag()
        {
            HighlightWalls(true);
            base.OnMouseDrag();
            _collider.transform.position = transform.position;
        }

        private void HighlightWalls(bool flag)
        {
            if (Walls.Count > 0)
            {
                foreach (var wall in Walls)
                {
                    var renderer = wall.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        if (flag)
                        {
                            renderer.color = Color.grey;
                        }
                        else
                        {
                            renderer.color = Color.black;
                        }
                    }
                }
            }
        }

        public void SetupColliderPoints()
        {
            var points = new List<Vector2>();
            foreach (var corner in Corners)
            {
                points.Add(new Vector2(corner.transform.position.x, corner.transform.position.y));
            }

            _collider.points = points.ToArray();
        }

        protected override void OnMouseUp()
        {
            HighlightWalls(false);
            base.OnMouseUp();
        }

        private void OnGUI()
        {
            DrawText();
        }

        private GUIStyle currentStyle = null;

        void DrawText()
        {

            var pos = GetCentroidOfPolygon(Corners.Select(x => ConvertWorldPositionToScreenPosition(x.transform.position)).ToList());
            var text = $"{name}";
            var textSize = GUI.skin.label.CalcSize(new GUIContent(text));

            var rect = new Rect(pos.x - textSize.x / 1.5f, (Screen.height - pos.y) - textSize.y / 1.5f, textSize.x + 25, textSize.y + 10);
            InitStyles((int)rect.width, (int)rect.height);
            GUI.contentColor = Color.black;
            GUI.Label(rect, text, currentStyle);
        }

        private void InitStyles(int width, int height)
        {
            if (currentStyle == null)
            {
                currentStyle = new GUIStyle(GUI.skin.box);
                var color = Color.white;
                currentStyle.fontSize = 14;
                currentStyle.normal.textColor = Color.black;
                currentStyle.normal.background = MakeTex(width, height, color);
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public Vector3 GetCentroidOfPolygon(List<Vector3> vertices)
        {
            var centroid = new Vector3();

            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = vertices.Count - 1; i < vertices.Count; j = i++)
            {
                float temp = vertices[i].x * vertices[j].y - vertices[j].x * vertices[i].y;
                accumulatedArea += temp;
                centerX += (vertices[i].x + vertices[j].x) * temp;
                centerY += (vertices[i].y + vertices[j].y) * temp;
            }

            //The area should not be zero.
            if (Math.Abs(accumulatedArea) < 1E-7f)
                return centroid;

            accumulatedArea *= 3f;
            centroid.x = centerX / accumulatedArea;
            centroid.y = centerY / accumulatedArea;

            return centroid;
        }
    }
}
