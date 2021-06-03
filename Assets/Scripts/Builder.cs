using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Util;

public class Builder : MonoBehaviour
{

    const string BOTTOM_LEFT_CORNER = "bottomLeftCorner";
    const string TOP_LEFT_CORNER = "topLeftCorner";
    const string TOP_RIGHT_CORNER = "topRightCorner";
    const string BOTTOM_RIGHT_CORNER = "bottomRightCorner";

    public GameObject ConnectorPrefab;
    public GameObject WallPrefab;
    public GameObject LineRenderer;

    List<GameObject> _rooms = new List<GameObject>();
    List<GameObject> _corners = new List<GameObject>();
    LineRenderer _lineRendererComponent;

    // Start is called before the first frame update
    void Start()
    {
        #region Development

        //AddRoomButton.onClick.AddListener(GenerateRoom);

        //DeleteRoomButton.onClick.AddListener(DeleteRoom);
        #endregion

        _lineRendererComponent = Instantiate(LineRenderer, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<LineRenderer>();

        _rooms.Add(CreateRoom("Main Room", 4, 4));
    }

    private void DeleteRoom()
    {
        foreach (var room in _rooms)
        {
            Destroy(room);
        }
    }

    private void GenerateRoom()
    {
        _rooms.Add(CreateRoom("Main Room", 4, 4));
    }

    GameObject CreateRoom(string id, int corners, int walls)
    {
        GameObject room = new GameObject(id);
        for (int i = 0; i < corners; i++)
        {
            GameObject connector;
            switch (i)
            {
                case 0:
                    connector = Instantiate(ConnectorPrefab, new Vector3(-2, -2, 2), Quaternion.identity);
                    connector.name = BOTTOM_LEFT_CORNER;
                    break;
                case 1:
                    connector = Instantiate(ConnectorPrefab, new Vector3(-2, 2, 2), Quaternion.identity);
                    connector.name = TOP_LEFT_CORNER;
                    break;
                case 2:
                    connector = Instantiate(ConnectorPrefab, new Vector3(2, 2, 2), Quaternion.identity);
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
            _corners.Add(connector);
        }

        if (_lineRendererComponent != null)
        {
            _lineRendererComponent.positionCount = _corners.Count;
        }
        //GenerateWalls(room, 4, _corners);

        return room;

    }

    void Update()
    {
        if (_lineRendererComponent != null)
        {
            if (_corners.Count > 0)
            {
                for (int i = 0; i < _corners.Count; i++)
                {
                    _lineRendererComponent.SetPosition(i, _corners[i].transform.position);
                }
            }
        }
    }

    void GenerateWalls(GameObject room, int walls, List<GameObject> connectors)
    {
        var bottomLeftCorner = connectors.Find(x => x.name.Equals(BOTTOM_LEFT_CORNER));
        var topLeftCorner = connectors.Find(x => x.name.Equals(TOP_LEFT_CORNER));

        var topRightCorner = connectors.Find(x => x.name.Equals(TOP_RIGHT_CORNER));
        var bottomRightCorner = connectors.Find(x => x.name.Equals(BOTTOM_RIGHT_CORNER));


        AddWall(bottomLeftCorner.transform, topLeftCorner.transform, false, "leftWall", room);
        AddWall(topLeftCorner.transform, topRightCorner.transform, true, "topWall", room);
        AddWall(topRightCorner.transform, bottomRightCorner.transform, false, "rightWall", room);
        AddWall(bottomLeftCorner.transform, bottomRightCorner.transform, true, "bottomWall", room);

    }

    void AddWall(Transform cornerOneRectTransform, Transform cornerTwoRectTransform, bool isHorizontal, string wallId, GameObject parent)
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
                wallCollider.size = new Vector2(1, 4);
            }
            else
            {
                wallCollider.size = new Vector2(4, 1);
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
    }

    // Update is called once per frame


    #region For development purpose only

    //public Button AddRoomButton;
    //public Button DeleteRoomButton;


    #endregion


}
