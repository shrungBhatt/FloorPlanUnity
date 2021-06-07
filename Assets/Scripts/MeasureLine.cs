using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Util;

namespace Assets.Scripts
{
    public class MeasureLine : MonoBehaviour
    {

        public GameObject Wall;
        public bool IsHorizontal;
        public bool IsLeftWall;
        public bool IsRightWall;
        public bool IsTopWall;
        public bool IsBottomWall;

        private void Start()
        {

        }


        private void Update()
        {

        }

        private void OnGUI()
        {
            //if (CornerOne != null && CornerTwo != null)
            //{
            //    DrawText();
            //}
        }

        private GUIStyle currentStyle = null;

        void DrawText()
        {


            //var measureLinePoints = ConvertWorldPositionToScreenPosition(transform.position);

            //var distanceVector = new Vector3();
            //if (IsHorizontal)
            //{
            //    distanceVector = Corner
            //}


            //for (int i = 0; i < connectors.Count; i++)
            //{
            //    var pos = Camera.main.WorldToScreenPoint(connectors[i].transform.position);
            //    var text = "Random Text";
            //    var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
            //    GUI.contentColor = Color.blue;
            //    var rect = new Rect(pos.x, Screen.height - pos.y, textSize.x, textSize.y);
            //    InitStyles((int)rect.width, (int)rect.height);
            //    var matrixBackup = GUI.matrix;
            //    GUIUtility.RotateAroundPivot(270, new Vector2(rect.x, rect.y));
            //    GUI.Label(rect, text, currentStyle);
            //    GUI.matrix = matrixBackup;
            //}
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


    }
}
