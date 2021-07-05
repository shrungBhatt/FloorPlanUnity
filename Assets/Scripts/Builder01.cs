using Assets.Scripts.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Util;

/// <summary>
/// This script class is used for drawing non-rect rooms
/// </summary>
namespace Assets.Scripts
{
    public class Builder01 : MonoBehaviour
    {

        const float ZOOM_FACTOR = 2f;

        public GameObject ConnectorPrefab;
        public GameObject WallPrefab;
        public GameObject RoomPrefab;
        public GameObject MeasureLinePrefab;
        public TextAsset FloorPlanJson;

        List<GameObject> _connectors = new List<GameObject>();
        Dictionary<Face, List<Point>> _roomCornersDictionary = new Dictionary<Face, List<Point>>();
        static FloorPlan01 _floorPlan;
        DeviceInfoModel _deviceInfoModel;
        List<GameObject> _rooms = new List<GameObject>();

        #region Lifecycle Methods
        private void Start()
        {
            _deviceInfoModel = new DeviceInfoModel
            {
                ScreenWidth = 1440,
                ScreenHeight = 3200,
                ZoomFactor = 120
            };
            //InitRoomDictionary(FloorPlanJson.text);
            //CreateRooms();
        }
        private void Update()
        {
            //var updatedFloorPlan = GetUpdatedFloorPlan();
            Debug.Log("Floor plan generated");

#if UNITY_ANDROID
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
#endif
        }
        #endregion

        #region Methods used for communication

        #region Unity To Android
        public void SendFloorPlanToDevice()
        {
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject javaObject = androidJavaClass?.GetStatic<AndroidJavaObject>("currentActivity");

            javaObject?.Call("OnFloorPlanReceived", GetUpdatedFloorPlan());
        }
        #endregion

        #region Android To Unity
        public void OnFloorPlanReceived(string jsonContent)
        {
            InitRoomDictionary(jsonContent);
            CreateRooms();
        }
        public void SetDeviceInfoParameters(string jsonContent)
        {
            _deviceInfoModel = JsonConvert.DeserializeObject<DeviceInfoModel>(jsonContent);
            if (_deviceInfoModel == null)
            {
                Debug.LogError("The Device Info Model is not set");
            }
        }
        #endregion

        #endregion

        #region UI Builder Methods
        void InitRoomDictionary(string jsonContent)
        {
            _floorPlan = JsonConvert.DeserializeObject<FloorPlan01>(jsonContent);

            //Get all the room in the faces
            var faces = _floorPlan.FloorPlan.EagleViewExport.Structures.Roof.Faces;

            var rooms = faces?.Face?.FindAll(x => (bool)x.Type?.Equals("ROOM"));
            //Get all the walls in the faces
            if (rooms != null)
            {
                foreach (var room in rooms)
                {
                    var corners = new List<Point>();
                    var walls = room.Children.Split(',');
                    if (walls != null)
                    {
                        foreach (var wall in walls)
                        {
                            var wallFace = faces?.Face?.Find(x => x.Id.Equals(wall));
                            if (wallFace != null)
                            {
                                var line = _floorPlan.FloorPlan.EagleViewExport.Structures.Roof.Lines.Line.Find(x => x.Id.Equals(wallFace.Polygon.Path));
                                if (line != null)
                                {
                                    var points = line.Path.Split(',');
                                    if (points != null)
                                    {
                                        foreach (var point in points)
                                        {
                                            var pointCoord = _floorPlan.FloorPlan.EagleViewExport.Structures.Roof.Points.Point.Find(x => x.Id.Equals(point));
                                            if (pointCoord != null)
                                            {
                                                if (!corners.Contains(pointCoord))
                                                    corners.Add(pointCoord);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    _roomCornersDictionary.Add(room, corners);

                }
            }
        }

        void CreateRooms()
        {
            var offsetPostion = Vector3.zero;
            foreach (var key in _roomCornersDictionary.Keys)
            {
                var room = Instantiate(RoomPrefab, Vector3.zero, Quaternion.identity);
                room.name = key.AreaName;
                _connectors = new List<GameObject>();

                var corners = _roomCornersDictionary[key];
                if (corners != null)
                {
                    foreach (var corner in corners)
                    {
                        var coords = Vector3.zero;

                        if (!string.IsNullOrEmpty(corner.DataXY))
                        {
                            coords = Get3DPointFrom2DPoint(_deviceInfoModel.ZoomFactor, Get2DCoordinates(corner.DataXY), _deviceInfoModel.ScreenWidth, _deviceInfoModel.ScreenHeight);
                            //coords = ConvertScreenPositionToWorldPosition(Get2DCoordinates(corner.DataXY));
                        }
                        else
                        {
                            coords = Get3DCoordinates(corner.Data);
                        }

                        var connector = Instantiate(ConnectorPrefab, coords, Quaternion.identity);
                        connector.name = corner.Id;
                        connector.transform.SetParent(room.transform);
                        _connectors.Add(connector);
                    }
                }

                var walls = new List<GameObject>();
                for (int i = 1; i < _connectors.Count + 1; i++)
                {
                    GameObject wall = null;
                    if (i != _connectors.Count)
                    {
                        wall = GenerateWalls(room, _connectors[i - 1],
                                             _connectors[i],
                                             _connectors[(i + 1) == _connectors.Count ? 0 : (i + 1)],
                                             $"Wall{i}");
                    }
                    else //Join the first and the last points
                    {
                        wall = GenerateWalls(room, _connectors[_connectors.Count - 1], _connectors[0], null, $"Wall{i}");
                    }

                    walls.Add(wall);
                }

                var roomScript = room.GetComponent<Room>();
                if (roomScript != null)
                {
                    roomScript.Corners = _connectors;
                    roomScript.Walls = walls;
                    if (key.Id.Equals(_floorPlan.NewRoomId))
                    {
                        roomScript.offsetPosition = offsetPostion;
                    }
                }

                offsetPostion += CalculateOffsetPostion(room);
                _rooms.Add(room);
            }
        }

        GameObject GenerateWalls(GameObject room, GameObject cornerOne, GameObject cornerTwo, GameObject nextCorner, string wallId)
        {
            var wall = GetWall(cornerOne.transform, cornerTwo.transform, true, wallId, room);
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

        void AddMeasureLine(GameObject wall, bool isBelow, bool isLeft)
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
        #endregion

        #region Util Methods
        string GetUpdatedFloorPlan()
        {
            foreach (var room in _rooms)
            {
                var roomScript = room.GetComponent<Room>();
                if (roomScript != null)
                {
                    foreach (var corner in roomScript.Corners)
                    {
                        var planCorner = _floorPlan.FloorPlan.EagleViewExport.Structures.Roof.Points.Point.Find(x => x.Id.Equals(corner.name));
                        if (planCorner != null)
                        {
                            planCorner.Data = Get3DCoordinatesString(corner.transform.position);

                            var coord = Get2DPointFrom3DPoint(_deviceInfoModel.ZoomFactor, corner.transform.position, _deviceInfoModel.ScreenWidth, _deviceInfoModel.ScreenHeight);
                            //var coord = ConvertScreenPositionToWorldPosition(corner.transform.position);
                            planCorner.DataXY = Get2DCoordinatesString(coord);
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(_floorPlan);
        }

        Vector3 Get3DCoordinates(string coordinates)
        {
            var coords = coordinates.Split(',');
            var vectorCoords = Vector3.zero;
            if (coords.Length == 3)
            {
                float x = float.Parse(coords[0]);
                float y = float.Parse(coords[1]);
                float z = float.Parse(coords[2]);
                //In 3D world z-coordinate is treated as the y-coordinate
                vectorCoords = new Vector3(x, z, y);
            }
            else
            {
                Debug.Log("The 3D co-ordinates are invalid");
            }

            return vectorCoords;
        }

        Vector2 Get2DCoordinates(string coordinates)
        {
            var coords = coordinates.Split(',');
            var vectorCoords = Vector3.zero;
            if (coords.Length == 2)
            {
                float x = float.Parse(coords[0]);
                float y = float.Parse(coords[1]);
                //In 3D world z-coordinate is treated as the y-coordinate
                vectorCoords = new Vector2(x, y);
            }
            else
            {
                Debug.Log("The 2D co-ordinates are invalid");
            }

            return vectorCoords;
        }

        string Get3DCoordinatesString(Vector3 coordinates)
        {
            return string.Join(",", new float[] { coordinates.x, coordinates.z, coordinates.y });
        }

        string Get2DCoordinatesString(Vector3 coordinates)
        {
            return string.Join(",", new float[] { coordinates.x, coordinates.y });
        }

        private Vector3 CalculateOffsetPostion(GameObject room)
        {
            float maxXPos = room.transform.position.x + room.transform.localScale.x / 2;
            var position = new Vector3(maxXPos + 3, 0, 0);

            return position;
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

        public Vector2 Get2DPointFrom3DPoint(int zoomFactor, Vector3 point, int screenWidth, int screenHeight)
        {
            var vector2 = new Vector2();

            vector2.x = zoomFactor * point.x + screenWidth / 2;
            vector2.y = screenHeight / 2 - zoomFactor * point.y;

            return vector2;
        }

        public Vector3 Get3DPointFrom2DPoint(int zoomFactor, Vector2 point, int screenWidth, int screenHeight)
        {
            var vector3 = new Vector3();

            vector3.x = (point.x - (screenWidth / 2)) / zoomFactor;
            vector3.y = -(point.y - (screenHeight / 2)) / zoomFactor;

            return vector3;
        }

        public string Get3DPointFrom2DPointString(int zoomFactor, Vector2 point, int screenWidth, int screenHeight)
        {
            var vector3 = new Vector3();

            vector3.x = (point.x - (screenWidth / 2)) / zoomFactor;
            vector3.z = -(point.y - (screenHeight / 2)) / zoomFactor;

            return Get3DCoordinatesString(vector3);
        }
        #endregion
    }


}
