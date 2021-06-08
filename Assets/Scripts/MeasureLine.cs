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
            if (Wall != null)
            {
                DrawText();
            }
        }

        private GUIStyle currentStyle = null;

        void DrawText()
        {

            var distanceVector = Length();
            var distanceInFeetAndInch = ToFeetInches(distanceVector * 39.37f);

            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var text = $"{distanceInFeetAndInch.Key}′ {distanceInFeetAndInch.Value}′′";
            var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.contentColor = Color.blue;
            if (IsLeftWall || IsRightWall)
            {
                var rect = new Rect(pos.x - textSize.y / 1.5f, (Screen.height - pos.y) + textSize.y / 0.9f, textSize.x + 20, textSize.y + 10);
                InitStyles((int)rect.width, (int)rect.height);
                var matrixBackup = GUI.matrix;
                GUIUtility.RotateAroundPivot(270, new Vector2(rect.x, rect.y));
                GUI.contentColor = Color.black;
                GUI.Label(rect, text, currentStyle);
                GUI.matrix = matrixBackup;
            }
            else if (IsTopWall || IsBottomWall)
            {
                var rect = new Rect(pos.x - textSize.x / 1.5f, (Screen.height - pos.y) - textSize.y / 1.5f, textSize.x + 20, textSize.y + 10);
                InitStyles((int)rect.width, (int)rect.height);
                GUI.contentColor = Color.black;
                GUI.Label(rect, text, currentStyle);
            }


        }

        float Length()
        {
            var length = 0.0f;
            if (IsLeftWall || IsRightWall)
            {
                length = Wall.transform.localScale.y;
            }
            else if (IsTopWall || IsBottomWall)
            {
                length = Wall.transform.localScale.x;
            }

            return length;
        }

        static KeyValuePair<int, double> ToFeetInches(float inches)
        {
            return new KeyValuePair<int, double>((int)inches / 12, (int)inches % 12);
        }

        private void InitStyles(int width, int height)
        {
            if (currentStyle == null)
            {
                currentStyle = new GUIStyle(GUI.skin.box);
                var color = Color.white;
                currentStyle.fontSize = 18;
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


    }
}
