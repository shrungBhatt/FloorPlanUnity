using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class AngleCalculator
    {

    }

    

    class Anchor
    {
        public string Id { get; set; }
    }

    class LineSegment
    {
        public string Id { get; set; }
        public Anchor Anchor1 { get; set; }
        public Anchor Anchor2 { get; set; }
        public double Length { get; set; }
    }

    class Triangle1
    {
        LineSegment LineSegmentA { get; set; }
        LineSegment LineSegmentB { get; set; }
        LineSegment LineSegmentC { get; set; }
        public double AngleA { get; set; }
        public double AngleB { get; set; }
        public double AngleC { get; set; }

        Triangle1(LineSegment lineSegmentA, LineSegment lineSegmentB, LineSegment lineSegmentC)
        {
            LineSegmentA = lineSegmentA;
            LineSegmentB = lineSegmentB;
            LineSegmentC = lineSegmentC;
            GetAngleA();
            GetAngleB();
            GetAngleC();
        }

        double GetAngleA()
        {
            var a = LineSegmentA.Length;
            var b = LineSegmentB.Length;
            var c = LineSegmentC.Length;
            AngleA = Math.Acos((b * b + c * c - a * a) / (2 * b * c));
            return AngleA;
        }

        double GetAngleB()
        {
            var a = LineSegmentA.Length;
            var b = LineSegmentB.Length;
            var c = LineSegmentC.Length;
            AngleB = Math.Acos((a * a + c * c - b * b) / (2 * a * c));
            return AngleB;
        }

        double GetAngleC()
        {
            var a = LineSegmentA.Length;
            var b = LineSegmentB.Length;
            var c = LineSegmentC.Length;
            AngleC = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
            return AngleC;
        }



    }

    internal class Polygon
    {
        List<Vector3> _points = new List<Vector3>();
        List<Triangle> _triangles = new List<Triangle>();
        

        internal Polygon(List<Vector3> points)
        {
            _points = points;
        }


    }
}
