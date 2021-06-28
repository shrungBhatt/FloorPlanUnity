using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Util;
using static Assets.Scripts.Connector;
using static Assets.Scripts.Wall;

public class Builder : MonoBehaviour
{
    public GameObject ConnectorPrefab;
    public GameObject WallPrefab;
    public GameObject MeasureLinePrefab;
    public GameObject RoomPrefab;

    List<GameObject> rooms = new List<GameObject>();
    List<GameObject> connectors = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        #region Development

        //AddRoomButton.onClick.AddListener(GenerateRoom);

        //DeleteRoomButton.onClick.AddListener(DeleteRoom);
        #endregion
        rooms.Add(CreateRoom("Main Room", 4, 4));
    }

    
    private void DeleteRoom()
    {
        foreach(var room in rooms)
        {
            Destroy(room);
        }
    }

    private void GenerateRoom()
    {
        rooms.Add(CreateRoom("Main Room", 4, 4));
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
                    connector = Instantiate(ConnectorPrefab, new Vector3(-2, -3, 2), Quaternion.identity);
                    connector.name = BOTTOM_LEFT_CORNER;
                    break;
                case 1:
                    connector = Instantiate(ConnectorPrefab, new Vector3(-2, 2, 2), Quaternion.identity);
                    connector.name = TOP_LEFT_CORNER;
                    break;
                case 2:
                    connector = Instantiate(ConnectorPrefab, new Vector3(3, 2, 2), Quaternion.identity);
                    connector.name = TOP_RIGHT_CORNER;
                    break;
                case 3:
                    connector = Instantiate(ConnectorPrefab, new Vector3(2, -2, 2), Quaternion.identity);
                    connector.name = BOTTOM_RIGHT_CORNER;
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
        var bottomLeftCorner = connectors.Find(x => x.name.Equals(BOTTOM_LEFT_CORNER));
        var topLeftCorner = connectors.Find(x => x.name.Equals(TOP_LEFT_CORNER));

        var topRightCorner = connectors.Find(x => x.name.Equals(TOP_RIGHT_CORNER));
        var bottomRightCorner = connectors.Find(x => x.name.Equals(BOTTOM_RIGHT_CORNER));

        var walls = new List<GameObject>();
        walls.Add(GetWall(bottomLeftCorner.transform, topLeftCorner.transform, false, LEFT_WALL, room));
        walls.Add(GetWall(topLeftCorner.transform, topRightCorner.transform, true, TOP_WALL, room));
        walls.Add(GetWall(topRightCorner.transform, bottomRightCorner.transform, false, RIGHT_WALL, room));
        walls.Add(GetWall(bottomLeftCorner.transform, bottomRightCorner.transform, true, BOTTOM_WALL, room));

        var roomScript = room.GetComponent<Room>();
        if(roomScript != null)
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
        if(wallScript != null)
        {
            wallScript.CornerOne = cornerOneRectTransform.gameObject;
            wallScript.CornerTwo = cornerTwoRectTransform.gameObject;
            wallScript.IsHorizontal = isHorizontal;
        }

        //Add the wall reference in the corners
        var c1ConnectorScript = cornerOneRectTransform.gameObject.GetComponent<Connector>();
        if(c1ConnectorScript != null)
        {
            c1ConnectorScript.Walls.Add(wall);
        }

        var c2ConnectorScript = cornerTwoRectTransform.gameObject.GetComponent<Connector>();
        if(c2ConnectorScript != null)
        {
            c2ConnectorScript.Walls.Add(wall);
        }

        //Set the size of the collider
        var wallCollider = wall.GetComponent<BoxCollider2D>();
        if(wallCollider != null)
        {
            if (isHorizontal)
            {
                wallCollider.size = new Vector2(1, 5);
            }
            else
            {
                wallCollider.size = new Vector2(5,1);
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

        AddMeasureLine(wall);

        return wall;
    }

    private void AddMeasureLine(GameObject wall)
    {
        var measureLine = Instantiate(MeasureLinePrefab, Vector3.zero, Quaternion.identity);

        if(measureLine != null)
        {
            var measureLineScript = measureLine.GetComponent<MeasureLine>();
            measureLineScript.Wall = wall;
            var offsetPostion = Vector3.zero;
            const float offset = 50f;

            measureLine.transform.position = wall.transform.position;
            var measureLineScreenPosition = ConvertWorldPositionToScreenPosition(measureLine.transform.position);

            switch (wall.name)
            {
                case LEFT_WALL:
                    measureLine.transform.localScale = new Vector3(measureLine.transform.localScale.x, wall.transform.localScale.y, wall.transform.localScale.z);
                    measureLineScript.IsLeftWall = true;

                    measureLineScreenPosition = ConvertWorldPositionToScreenPosition(measureLine.transform.position);
                    offsetPostion = ConvertScreenPositionToWorldPosition(new Vector3(measureLineScreenPosition.x - offset, measureLineScreenPosition.y, measureLineScreenPosition.z));
                    measureLine.transform.position = offsetPostion;
                    break;
                case TOP_WALL:
                    measureLine.transform.position = wall.transform.position;
                    measureLine.transform.localScale = new Vector3(wall.transform.localScale.x, measureLine.transform.localScale.y, wall.transform.localScale.z);
                    measureLineScript.IsTopWall = true;

                    offsetPostion = ConvertScreenPositionToWorldPosition(new Vector3(measureLineScreenPosition.x , measureLineScreenPosition.y + offset, measureLineScreenPosition.z));
                    measureLine.transform.position = offsetPostion;
                    break;
                case RIGHT_WALL:
                    measureLine.transform.position = wall.transform.position;
                    measureLine.transform.localScale = new Vector3(measureLine.transform.localScale.x, wall.transform.localScale.y, wall.transform.localScale.z);
                    measureLineScript.IsRightWall = true;
                    
                    offsetPostion = ConvertScreenPositionToWorldPosition(new Vector3(measureLineScreenPosition.x + offset, measureLineScreenPosition.y, measureLineScreenPosition.z));
                    measureLine.transform.position = offsetPostion;
                    break;
                case BOTTOM_WALL:
                    measureLine.transform.position = wall.transform.position;
                    measureLine.transform.localScale = new Vector3(wall.transform.localScale.x, measureLine.transform.localScale.y, wall.transform.localScale.z);
                    measureLineScript.IsBottomWall = true;

                    offsetPostion = ConvertScreenPositionToWorldPosition(new Vector3(measureLineScreenPosition.x, measureLineScreenPosition.y - offset, measureLineScreenPosition.z));
                    measureLine.transform.position = offsetPostion;
                    break;
            }
            measureLine.transform.SetParent(wall.transform);
        }
        

    }

   

    // Update is called once per frame
    void Update()
    {

    }

    #region For development purpose only

    //public Button AddRoomButton;
    //public Button DeleteRoomButton;


    #endregion


}
