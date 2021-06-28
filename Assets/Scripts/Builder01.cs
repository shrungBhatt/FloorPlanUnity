using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Util;
using static Assets.Scripts.Connector;
using static Assets.Scripts.Wall;
using Assets.Scripts.Models;
using Newtonsoft.Json;

/// <summary>
/// This script class is used for drawing non-rect rooms
/// </summary>
namespace Assets.Scripts
{
    public class Builder01 : MonoBehaviour
    {
        public GameObject ConnectorPrefab;
        public GameObject WallPrefab;
        public GameObject RoomPrefab;
        public GameObject MeasureLinePrefab;
        public TextAsset FloorPlanJson;

        List<GameObject> connectors = new List<GameObject>();

        private void Start()
        {

            var model = JsonConvert.DeserializeObject<FloorPlan>(FloorPlanJson.text);

            CreateRoom("Test room", 5);
        }

        private void Update()
        {

        }

        GameObject CreateRoom(string id, int corners)
        {
            var room = Instantiate(RoomPrefab, Vector3.zero, Quaternion.identity);
            room.name = id;
            for (int i = 0; i < corners; i++)
            {
                GameObject connector;
                switch (i)
                {
                    case 0:
                        connector = Instantiate(ConnectorPrefab, new Vector3(-2, -3, 2), Quaternion.identity);
                        connector.name = "corner0";
                        break;
                    case 1:
                        connector = Instantiate(ConnectorPrefab, new Vector3(-2, 2, 2), Quaternion.identity);
                        connector.name = "corner1";
                        break;
                    case 2:
                        connector = Instantiate(ConnectorPrefab, new Vector3(0, 4, 2), Quaternion.identity);
                        connector.name = "corner2";
                        break;
                    case 3:
                        connector = Instantiate(ConnectorPrefab, new Vector3(2, 3, 2), Quaternion.identity);
                        connector.name = "corner3";
                        break;
                    case 4:
                        connector = Instantiate(ConnectorPrefab, new Vector3(2, -2, 2), Quaternion.identity);
                        connector.name = "corner4";
                        break;
                    default:
                        connector = new GameObject("defaultConnector");
                        break;
                }
                connector.transform.SetParent(room.transform);
                connectors.Add(connector);
            }

            var walls = new List<GameObject>();
            for (int i = 1; i < connectors.Count + 1; i++)
            {
                GameObject wall = null;
                if (i != connectors.Count)
                {
                    wall = GenerateWalls(room, connectors[i - 1], connectors[i], connectors[(i + 1) == connectors.Count ? 0 : (i + 1)]);
                }
                else //Join the first and the last points
                {
                    wall = GenerateWalls(room, connectors[connectors.Count - 1], connectors[0], null);
                }

                walls.Add(wall);
            }

            var roomScript = room.GetComponent<Room>();
            if (roomScript != null)
            {
                roomScript.Corners = connectors;
                roomScript.Walls = walls;
            }
            return room;

        }

        GameObject GenerateWalls(GameObject room, GameObject cornerOne, GameObject cornerTwo, GameObject nextCorner)
        {
            var wall = GetWall(cornerOne.transform, cornerTwo.transform, true, TOP_WALL, room);
            //Add the measure line
            AddMeasureLine(wall, false, false);

            return wall;
        }

        GameObject GetWall(Transform c1Transform, Transform c2Transform, bool isHorizontal, string wallId, GameObject parent)
        {
            var c1ScreenPos = ConvertWorldPositionToScreenPosition(c1Transform.position);
            var c2ScreenPos = ConvertWorldPositionToScreenPosition(c2Transform.position);

            var wall = Instantiate(WallPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            wall.name = wallId;
            var wallScript = wall.GetComponent<Wall>();
            if (wallScript != null)
            {
                wallScript.CornerOne = c1Transform.gameObject;
                wallScript.CornerTwo = c2Transform.gameObject;
                wallScript.IsHorizontal = isHorizontal;
            }

            //Add the wall reference in the corners
            var c1ConnectorScript = c1Transform.gameObject.GetComponent<Connector>();
            if (c1ConnectorScript != null)
            {
                c1ConnectorScript.Walls.Add(wall);
            }

            var c2ConnectorScript = c2Transform.gameObject.GetComponent<Connector>();
            if (c2ConnectorScript != null)
            {
                c2ConnectorScript.Walls.Add(wall);
            }

            //Set the size of the collider
            var wallCollider = wall.GetComponent<BoxCollider2D>();
            if (wallCollider != null)
            {
                if (isHorizontal)
                {
                    wallCollider.size = new Vector2(1, 5);
                }
                else
                {
                    wallCollider.size = new Vector2(5, 1);
                }
            }

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


            wall.transform.position = ConvertScreenPositionToWorldPosition(wallPosition);
            wall.transform.SetParent(parent.transform);

            var cornerScript = c2Transform.gameObject.GetComponent<Connector>();
            if (cornerScript != null)
            {
                cornerScript.UpdateWallPosition(wall);
                wallScript.ChangeWallScale(wall.transform, wall, c2Transform.gameObject);
            }
            else
            {
                Debug.Log("The corner script is null");
            }

            return wall;
        }

        private void AddMeasureLine(GameObject wall, bool isBelow, bool isLeft)
        {
            var measureLine = Instantiate(MeasureLinePrefab, Vector3.zero, Quaternion.identity);

            var wallScript = wall.GetComponent<Wall>();
            if (wallScript != null)
            {
                wallScript.MeasureLine = measureLine;
            }

            if (measureLine != null)
            {
                var measureLineScript = measureLine.GetComponent<MeasureLine>();
                measureLineScript.Wall = wall;
                var offsetPostion = Vector3.zero;
                const float offset = 20f;

                measureLine.transform.position = wall.transform.position;
                measureLine.transform.localScale = wall.transform.localScale;
                measureLine.transform.localRotation = wall.transform.rotation;

                var measureLineScreenPosition = ConvertWorldPositionToScreenPosition(measureLine.transform.position);
                offsetPostion = ConvertScreenPositionToWorldPosition(new Vector3(measureLineScreenPosition.x, measureLineScreenPosition.y, measureLineScreenPosition.z));
                measureLine.transform.position = offsetPostion;// + new Vector3(MeasureLine.OFFSET, 0, 0);

                measureLine.transform.SetParent(wall.transform);


            }


        }

        public bool IsLeft(Vector3 currentPoint, Vector3 nextPoint)
        {
            //Create two points for a line that is parallel to Y - Axis
            var pointA = new Vector2();
            pointA.x = currentPoint.x;
            pointA.y = currentPoint.z + 50;

            var pointB = new Vector2();
            pointB.x = currentPoint.x;
            pointB.y = currentPoint.z - 50;

            var pointToCheck = new Vector2();
            pointToCheck.x = nextPoint.x;
            pointToCheck.y = nextPoint.z;


            return ((pointB.x - pointA.x) * (pointToCheck.y - pointA.y) - (pointB.y - pointA.y) * (pointToCheck.x - pointA.x)) < 0;
        }

        public bool IsBelow(Vector3 currentPoint, Vector3 nextPoint)
        {
            //Create two points for a line that is parallel to X - Axis
            var pointA = new Vector2();
            pointA.x = currentPoint.x + 50;
            pointA.y = currentPoint.z;

            var pointB = new Vector2();
            pointB.x = currentPoint.x - 50;
            pointB.y = currentPoint.z;

            var pointToCheck = new Vector2();
            pointToCheck.x = nextPoint.x;
            pointToCheck.y = nextPoint.z;


            return ((pointB.x - pointA.x) * (pointToCheck.y - pointA.y) - (pointB.y - pointA.y) * (pointToCheck.x - pointA.x)) > 0;
        }
    }


}
