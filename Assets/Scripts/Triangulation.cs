﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Triangulation : MonoBehaviour
    {
        private void Start()
        {
            var samplePoints = new List<Vertex>
            {
                new Vertex(new Vector3(0.5103803f, -0.42641863f, 0.031951603f), 0),
                new Vertex(new Vector3(0.8607999f, -0.42641854f, -0.35582685f), 1),
                new Vertex(new Vector3(0.049748395f, -0.4264186f, -1.0802422f), 2),
                new Vertex(new Vector3(-0.03956592f, -0.42641863f, -0.48409465f), 3)
            };



            //Step 1 - Calculate the triangles from the polygon.
            var hullPoints = GetConvexHull(samplePoints);
            var triangles = TriangulateConvexPolygon(hullPoints);

            //Step 2 - Calculate the angles of each triangles
            foreach(var triangle in triangles)
            {
                triangle.e1 = new Edge(triangle.v1, triangle.v2, oppositeVertex: triangle.v3) ;
                triangle.e2 = new Edge(triangle.v2, triangle.v3, oppositeVertex: triangle.v1);
                triangle.e3 = new Edge(triangle.v1, triangle.v3, oppositeVertex: triangle.v2);
                triangle.CalculateAngles();

                foreach(var key in triangle.EdgeAnglePair.Keys)
                {
                    var vertex = hullPoints.Find(x => x.id == key.id);
                    if(vertex != null)
                    {
                        vertex.angle += triangle.EdgeAnglePair[key];
                    }
                    else
                    {
                        Debug.LogError("Vertex not found");
                    }
                }
            }


            hullPoints = hullPoints.OrderBy(x => x.id).ToList();

            foreach(var point in hullPoints)
            {
                Debug.Log($"The vertex {point.id} has angle {point.angle}");
            }

            Debug.Log("");
        }


        public List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints)
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 2; i < convexHullpoints.Count; i++)
            {
                Vertex a = convexHullpoints[0];
                Vertex b = convexHullpoints[i - 1];
                Vertex c = convexHullpoints[i];

                triangles.Add(new Triangle(a, b, c));
            }

            return triangles;
        }

        #region Convex Hull
        public List<Vertex> GetConvexHull(List<Vertex> points)
        {
            //If we have just 3 points, then they are the convex hull, so return those
            if (points.Count == 3)
            {
                //These might not be ccw, and they may also be colinear
                return points;
            }

            //If fewer points, then we cant create a convex hull
            if (points.Count < 3)
            {
                return null;
            }



            //The list with points on the convex hull
            List<Vertex> convexHull = new List<Vertex>();

            //Step 1. Find the vertex with the smallest x coordinate
            //If several have the same x coordinate, find the one with the smallest z
            Vertex startVertex = points[0];

            Vector3 startPos = startVertex.position;

            for (int i = 1; i < points.Count; i++)
            {
                Vector3 testPos = points[i].position;

                //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
                if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.z < startPos.z))
                {
                    startVertex = points[i];

                    startPos = startVertex.position;
                }
            }

            //This vertex is always on the convex hull
            convexHull.Add(startVertex);

            points.Remove(startVertex);



            //Step 2. Loop to generate the convex hull
            Vertex currentPoint = convexHull[0];

            //Store colinear points here - better to create this list once than each loop
            List<Vertex> colinearPoints = new List<Vertex>();

            int counter = 0;

            while (true)
            {
                //After 2 iterations we have to add the start position again so we can terminate the algorithm
                //Cant use convexhull.count because of colinear points, so we need a counter
                if (counter == 2)
                {
                    points.Add(convexHull[0]);
                }

                System.Random rnd = new System.Random();

                //Pick next point randomly
                Vertex nextPoint = points[rnd.Next(0, points.Count)];

                //To 2d space so we can see if a point is to the left is the vector ab
                Vector2 a = currentPoint.GetPos2D_XZ();

                Vector2 b = nextPoint.GetPos2D_XZ();

                //Test if there's a point to the right of ab, if so then it's the new b
                for (int i = 0; i < points.Count; i++)
                {
                    //Dont test the point we picked randomly
                    if (points[i].Equals(nextPoint))
                    {
                        continue;
                    }

                    Vector2 c = points[i].GetPos2D_XZ();

                    //Where is c in relation to a-b
                    // < 0 -> to the right
                    // = 0 -> on the line
                    // > 0 -> to the left
                    float relation = IsAPointLeftOfVectorOrOnTheLine(a, b, c);

                    //Colinear points
                    //Cant use exactly 0 because of floating point precision issues
                    //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
                    float accuracy = 0.00001f;

                    if (relation < accuracy && relation > -accuracy)
                    {
                        colinearPoints.Add(points[i]);
                    }
                    //To the right = better point, so pick it as next point on the convex hull
                    else if (relation < 0f)
                    {
                        nextPoint = points[i];

                        b = nextPoint.GetPos2D_XZ();

                        //Clear colinear points
                        colinearPoints.Clear();
                    }
                    //To the left = worse point so do nothing
                }



                //If we have colinear points
                if (colinearPoints.Count > 0)
                {
                    colinearPoints.Add(nextPoint);

                    //Sort this list, so we can add the colinear points in correct order
                    colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n.position - currentPoint.position)).ToList();

                    convexHull.AddRange(colinearPoints);

                    currentPoint = colinearPoints[colinearPoints.Count - 1];

                    //Remove the points that are now on the convex hull
                    for (int i = 0; i < colinearPoints.Count; i++)
                    {
                        points.Remove(colinearPoints[i]);
                    }

                    colinearPoints.Clear();
                }
                else
                {
                    convexHull.Add(nextPoint);

                    points.Remove(nextPoint);

                    currentPoint = nextPoint;
                }

                //Have we found the first point on the hull? If so we have completed the hull
                if (currentPoint.Equals(convexHull[0]))
                {
                    //Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
                    convexHull.RemoveAt(convexHull.Count - 1);

                    break;
                }

                counter += 1;
            }

            return convexHull;
        }

        float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p)
        {
            float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

            return determinant;
        }
        #endregion
    }



    #region Data Structure
    public class Vertex
    {
        public Vector3 position;

        //The outgoing halfedge (a halfedge that starts at this vertex)
        //Doesnt matter which edge we connect to it
        public HalfEdge halfEdge;

        //Which triangle is this vertex a part of?
        public Triangle triangle;

        //The previous and next vertex this vertex is attached to
        public Vertex prevVertex;
        public Vertex nextVertex;

        //Properties this vertex may have
        //Reflex is concave
        public bool isReflex;
        public bool isConvex;
        public bool isEar;

        public int id;

        public double angle;

        public Vertex(Vector3 position, int id)
        {
            this.position = position;
            this.id = id;
        }


        //Get 2d pos of this vertex
        public Vector2 GetPos2D_XZ()
        {
            Vector2 pos_2d_xz = new Vector2(position.x, position.z);

            return pos_2d_xz;
        }
    }

    public class HalfEdge
    {
        //The vertex the edge points to
        public Vertex v;

        //The face this edge is a part of
        public Triangle t;

        //The next edge
        public HalfEdge nextEdge;
        //The previous
        public HalfEdge prevEdge;
        //The edge going in the opposite direction
        public HalfEdge oppositeEdge;

        //This structure assumes we have a vertex class with a reference to a half edge going from that vertex
        //and a face (triangle) class with a reference to a half edge which is a part of this face 
        public HalfEdge(Vertex v)
        {
            this.v = v;
        }
    }

    public class Triangle
    {
        //Corners
        public Vertex v1;
        public Vertex v2;
        public Vertex v3;

        //Angles
        public double a1;
        public double a2;
        public double a3;

        //Edges
        public Edge e1;
        public Edge e2;
        public Edge e3;

        public Dictionary<Vertex, double> EdgeAnglePair = new Dictionary<Vertex, double>();

        //If we are using the half edge mesh structure, we just need one half edge
        public HalfEdge halfEdge;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Triangle(HalfEdge halfEdge)
        {
            this.halfEdge = halfEdge;
        }

        //Change orientation of triangle from cw -> ccw or ccw -> cw
        public void ChangeOrientation()
        {
            Vertex temp = this.v1;

            this.v1 = this.v2;

            this.v2 = temp;
        }

        public void CalculateAngles()
        {
            GetAngleA();
            GetAngleB();
            GetAngleC();
        }

        double GetAngleA()
        {
            var a = e1.Length;
            var b = e2.Length;
            var c = e3.Length;
            a1 = Math.Acos((b * b + c * c - a * a) / (2 * b * c)) * 180/Math.PI;
            EdgeAnglePair.Add(e1.oppositeVertex, a1);
            return a1;
        }

        double GetAngleB()
        {
            var a = e1.Length;
            var b = e2.Length;
            var c = e3.Length;
            a2 = Math.Acos((a * a + c * c - b * b) / (2 * a * c)) * 180 / Math.PI;
            EdgeAnglePair.Add(e2.oppositeVertex, a2);
            return a2;
        }

        double GetAngleC()
        {
            var a = e1.Length;
            var b = e2.Length;
            var c = e3.Length;
            a3 = Math.Acos((a * a + b * b - c * c) / (2 * a * b)) * 180 / Math.PI;
            EdgeAnglePair.Add(e3.oppositeVertex, a3);
            return a3;
        }
    }

    public class Edge
    {
        public Vertex v1;
        public Vertex v2;
        public Vertex oppositeVertex;

        public double Length;

        //Is this edge intersecting with another edge?
        public bool isIntersecting = false;

        public bool isDiagonal = false;


        public Edge(Vertex v1, Vertex v2, Vertex oppositeVertex)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.oppositeVertex = oppositeVertex;
            Length = Vector3.Magnitude(v2.position - v1.position);
        }

        //Get vertex in 2d space (assuming x, z)
        public Vector2 GetVertex2D(Vertex v)
        {
            return new Vector2(v.position.x, v.position.z);
        }

        //Flip edge
        public void FlipEdge()
        {
            Vertex temp = v1;

            v1 = v2;

            v2 = temp;
        }
    }
    #endregion

    class Point3d
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
