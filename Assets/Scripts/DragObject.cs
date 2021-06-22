using UnityEngine;

namespace Assets.Scripts
{
    public class DragObject : MonoBehaviour
    {
        private Vector3 mOffset;
        private float mZCoord;
        protected bool IsHorizontalWall;
        protected bool enableFreeMovement;
        Vector3 cachedPosition;

        void OnMouseDown()
        {
            PanZoom.DisablePanZoom = true;
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            cachedPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            // Store offset = gameobject world pos - mouse world pos
            mOffset = gameObject.transform.position - GetMouseAsWorldPoint(mZCoord);
        }

        protected virtual void OnMouseUp()
        {
            PanZoom.DisablePanZoom = false;
        }

        protected virtual Vector3 GetMouseAsWorldPoint(float zCoord)
        {

            // Pixel coordinates of mouse (x,y)
            Vector3 mousePoint = Input.mousePosition;
            if (!enableFreeMovement)
            {
                if (IsHorizontalWall)
                {
                    mousePoint.y = Input.mousePosition.y;
                    mousePoint.x = cachedPosition.x;
                }
                else
                {
                    mousePoint.y = cachedPosition.y;
                    mousePoint.x = Input.mousePosition.x;
                }
            }
            

            // z coordinate of game object on screen
            mousePoint.z = zCoord;

            // Convert it to world points
            return Camera.main.ScreenToWorldPoint(mousePoint);

        }

        protected virtual void OnMouseDrag()
        {
            transform.position = GetMouseAsWorldPoint(mZCoord) + mOffset;
        }

    }
}
