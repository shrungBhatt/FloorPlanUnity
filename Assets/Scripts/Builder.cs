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
    public Texture LabelBackgroundTexture;

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

    private void OnGUI()
    {
        if (connectors != null)
        {
            if (connectors.Count > 0)
            {
                DrawText();
            }
        }
    }

    //float rotAngle = 0;
    //Vector2 pivotPoint;

    //void OnGUI()
    //{
    //    pivotPoint = new Vector2(Screen.width / 2, Screen.height / 2);
    //    GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
    //    if (GUI.Button(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 25, 50, 50), "Rotate"))
    //    {
    //        rotAngle += 10;
    //    }
    //}

    private GUIStyle currentStyle = null;

    void DrawText()
    {

        for (int i = 0; i < connectors.Count; i++)
        {
            var pos = Camera.main.WorldToScreenPoint(connectors[i].transform.position);
            var text = connectors[i].transform.position.ToString();
            var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.contentColor = Color.blue;
            var rect = new Rect(pos.x, Screen.height - pos.y, textSize.x, textSize.y);
            InitStyles((int)rect.width, (int)rect.height);
            var matrixBackup = GUI.matrix;
            GUIUtility.RotateAroundPivot(270, new Vector2(rect.x, rect.y));
            GUI.Label(rect, text, currentStyle);
            GUI.matrix = matrixBackup;
        }
    }

    private void InitStyles(int width, int height)
    {
        if (currentStyle == null)
        {
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(width, height, Color.white);
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
            connectors.Add(connector);
        }

        GenerateWalls(room, 4, connectors);

        return room;

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
                wallCollider.size = new Vector2(1, 4);
            }
            else
            {
                wallCollider.size = new Vector2(4,1);
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
    void Update()
    {

    }

    #region For development purpose only

    //public Button AddRoomButton;
    //public Button DeleteRoomButton;


    #endregion


}
