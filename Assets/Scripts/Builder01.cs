using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Util;
using static Assets.Scripts.Connector;
using static Assets.Scripts.Wall;


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

        List<GameObject> connectors = new List<GameObject>();

        private void Start()
        {
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
                        connector = Instantiate(ConnectorPrefab, new Vector3(-2, -2, 2), Quaternion.identity);
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
                        connector = Instantiate(ConnectorPrefab, new Vector3(2, 2, 2), Quaternion.identity);
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

            for (int i = 1; i < connectors.Count + 1; i++)
            {
                if(i != connectors.Count)
                {
                    GenerateWalls(room, connectors[i-1], connectors[i]);
                }
                else //Join the first and the last points
                {
                    GenerateWalls(room, connectors[connectors.Count - 1], connectors[0]);
                }
            }


            return room;

        }

        void GenerateWalls(GameObject room, GameObject cornerOne, GameObject cornerTwo)
        {

            var walls = new List<GameObject>();
            walls.Add(GetWall(cornerOne.transform, cornerTwo.transform, true, TOP_WALL, room));

            var roomScript = room.GetComponent<Room>();
            if (roomScript != null)
            {
                roomScript.Corners = connectors;
                roomScript.Walls = walls;
            }
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
    }
}
