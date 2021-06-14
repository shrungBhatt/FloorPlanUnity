using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Util;
using static Assets.Scripts.Connector;
using static Assets.Scripts.Wall;

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
            CreateRoom("Test room", 2, 1);
        }

        private void Update()
        {
            
        }

        GameObject CreateRoom(string id, int corners, int walls)
        {
            var room = Instantiate(RoomPrefab, Vector3.zero, Quaternion.identity);
            room.name = id;
            for (int i = 0; i < corners; i++)
            {
                GameObject connector;
                switch (i)
                {
                    
                    case 0:
                        connector = Instantiate(ConnectorPrefab, new Vector3(-2, 2, 2), Quaternion.identity);
                        connector.name = TOP_LEFT_CORNER;
                        break;
                    case 1:
                        connector = Instantiate(ConnectorPrefab, new Vector3(2, 2, 2), Quaternion.identity);
                        connector.name = TOP_RIGHT_CORNER;
                        break;
                    default:
                        connector = new GameObject("defaultConnector");
                        break;
                }
                connector.transform.SetParent(room.transform);
                connectors.Add(connector);
            }

            GenerateWalls(room, 4, connectors);

            return room;

        }

        void GenerateWalls(GameObject room, int noOfWalls, List<GameObject> connectors)
        {
            var topLeftCorner = connectors.Find(x => x.name.Equals(TOP_LEFT_CORNER));

            var topRightCorner = connectors.Find(x => x.name.Equals(TOP_RIGHT_CORNER));

            var walls = new List<GameObject>();
            walls.Add(GetWall(topLeftCorner.transform, topRightCorner.transform, true, TOP_WALL, room));

            var roomScript = room.GetComponent<Room>();
            if (roomScript != null)
            {
                roomScript.Corners = connectors;
                roomScript.Walls = walls;
            }
        }

        GameObject GetWall(Transform cornerOneRectTransform, Transform cornerTwoRectTransform, bool isHorizontal, string wallId, GameObject parent)
        {
            var c1ScreenPos = ConvertWorldPositionToScreenPosition(cornerOneRectTransform.position);
            var c2ScreenPos = ConvertWorldPositionToScreenPosition(cornerTwoRectTransform.position);

            var wall = Instantiate(WallPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            wall.name = wallId;
            var wallScript = wall.GetComponent<Wall>();
            if (wallScript != null)
            {
                wallScript.CornerOne = cornerOneRectTransform.gameObject;
                wallScript.CornerTwo = cornerTwoRectTransform.gameObject;
                wallScript.IsHorizontal = isHorizontal;
            }

            //Add the wall reference in the corners
            var c1ConnectorScript = cornerOneRectTransform.gameObject.GetComponent<Connector>();
            if (c1ConnectorScript != null)
            {
                c1ConnectorScript.Walls.Add(wall);
            }

            var c2ConnectorScript = cornerTwoRectTransform.gameObject.GetComponent<Connector>();
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

            var leftWallRectTransform = wall.transform as RectTransform;
            if (leftWallRectTransform != null)
            {
                var scale = leftWallRectTransform.localScale;
                if (isHorizontal)
                {
                    scale.x = cornerOneRectTransform.position.x > wall.transform.position.x ? cornerOneRectTransform.position.x - wall.transform.position.x : wall.transform.position.x - cornerOneRectTransform.position.x;
                }
                else
                {
                    scale.y = cornerOneRectTransform.position.y > wall.transform.position.y ? cornerOneRectTransform.position.y - wall.transform.position.y : wall.transform.position.y - cornerOneRectTransform.position.y;
                }

                leftWallRectTransform.localScale = scale * 2;
            }


            return wall;
        }
    }
}
