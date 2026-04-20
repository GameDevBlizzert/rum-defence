using System;
using Microsoft.Xna.Framework;

namespace RumDefence
{

    public class LinePieceCollider : Collider, IEquatable<LinePieceCollider>
    {

        public Vector2 Start;
        public Vector2 End;

        /// <summary>
        /// The length of the LinePiece, changing the length moves the end vector to adjust the length.
        /// </summary>
        public float Length 
        { 
            get { 
                return (End - Start).Length(); 
            } 
            set {
                End = Start + GetDirection() * value; 
            }
        }

        /// <summary>
        /// The A component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardA
        {
            get
            {
                // TODO: Implement
                return End.Y - Start.Y;
            }
        }

        /// <summary>
        /// The B component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardB
        {
            get
            {
                // TODO: Implement
                return Start.X - End.X;
            }
        }

        /// <summary>
        /// The C component from the standard line formula Ax + By + C = 0
        /// </summary>
        public float StandardC
        {
            get
            {
                // TODO: Implement
                return End.X * Start.Y - Start.X * End.Y;
            }
        }

        public LinePieceCollider(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        
        public LinePieceCollider(Vector2 start, Vector2 direction, float length)
        {
            Start = start;
            End = start + direction * length;
        }

        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
        public static float GetAngle(Vector2 direction)
        {
            // TODO: Implement
            return (float)Math.Atan2(direction.Y, direction.X) + MathHelper.ToRadians(90);
        }


        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Vector2 point1, Vector2 point2)
        {
            // TODO Implement, currently pointing up.
            Vector2 direction = point2 - point1;
            direction.Normalize();
            return direction;
        }


        /// <summary>
        /// Gets whether or not the Line intersects another Line
        /// </summary>
        /// <param name="other">The Line to check for intersection</param>
        /// <returns>true there is any overlap between the Circle and the Line.</returns>
        public override bool Intersects(LinePieceCollider other)
        {
            // TODO Implement.
            float denominator = StandardA * other.StandardB - other.StandardA * StandardB;
            if (denominator == 0)
            {
                return false;
            }
            float intersectX = StandardB * other.StandardC - other.StandardB * StandardC;
            float intersectY = StandardC * other.StandardA - other.StandardC * StandardA;

            Vector2 intersectPoint = new Vector2(intersectX, intersectY);

            return Contains(intersectPoint) && other.Contains(intersectPoint);
        }


        /// <summary>
        /// Gets whether or not the line intersects a Circle.
        /// </summary>
        /// <param name="other">The Circle to check for intersection.</param>
        /// <returns>true there is any overlap between the two Circles.</returns>
        public override bool Intersects(CircleCollider other)
        {
            // TODO Implement hint, you can use the NearestPointOnLine function defined below.
            Vector2 nearest = NearestPointOnLine(other.Center);
            return (nearest - other.Center).LengthSquared() <= other.Radius * other.Radius;
        }

        /// <summary>
        /// Gets whether or not the Line intersects the Rectangle.
        /// </summary>
        /// <param name="other">The Rectangle to check for intersection.</param>
        /// <returns>true there is any overlap between the Circle and the Rectangle.</returns>
        public override bool Intersects(RectangleCollider other)
        {
            // TODO Implement
            if (other.Contains(Start) || other.Contains(End))
                return true;

            Rectangle r = other.shape;
            LinePieceCollider top = new LinePieceCollider(new Vector2(r.Left, r.Top), new Vector2(r.Right, r.Top));
            LinePieceCollider bottom = new LinePieceCollider(new Vector2(r.Left, r.Bottom), new Vector2(r.Right, r.Bottom));
            LinePieceCollider left = new LinePieceCollider(new Vector2(r.Left, r.Top), new Vector2(r.Left, r.Bottom));
            LinePieceCollider right = new LinePieceCollider(new Vector2(r.Right, r.Top), new Vector2(r.Right, r.Bottom));

            return Intersects(top) || Intersects(bottom) || Intersects(left) || Intersects(right);
        }

        /// <summary>
        /// Calculates the intersection point between 2 lines.
        /// </summary>
        /// <param name="Other">The line to intersect with</param>
        /// <returns>A Vector2 with the point of intersection.</returns>
        public Vector2 GetIntersection(LinePieceCollider Other)
        {
            // TODO Implement
            float det = StandardA * Other.StandardB - Other.StandardA * StandardB;
            if (Math.Abs(det) < 0.0001f)
                return Vector2.Zero;

            float x = (StandardB * Other.StandardC - Other.StandardB * StandardC) / det;
            float y = (Other.StandardA * StandardC - StandardA * Other.StandardC) / det;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Finds the nearest point on a line to a given vector, taking into account if the line is .
        /// </summary>
        /// <param name="other">The Vector you want to find the nearest point to.</param>
        /// <returns>The nearest point on the line.</returns>
        public Vector2 NearestPointOnLine(Vector2 other)
        {
            // TODO Implement
            Vector2 lineDir = End - Start;
            float lengthSquared = lineDir.LengthSquared();
            if (lengthSquared == 0)
                return Start;

            float t = Vector2.Dot(other - Start, lineDir) / lengthSquared;
            t = MathHelper.Clamp(t, 0f, 1f);
            return Start + t * lineDir;
        }

        /// <summary>
        /// Returns the enclosing Axis Aligned Bounding Box containing the control points for the line.
        /// As an unbound line has infinite length, the returned bounding box assumes the line to be bound.
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            Point topLeft = new Point((int)Math.Min(Start.X, End.X), (int)Math.Min(Start.Y, End.Y));
            Point size = new Point((int)Math.Max(Start.X, End.X), (int)Math.Max(Start.Y, End.X)) - topLeft;
            return new Rectangle(topLeft,size);
        }


        /// <summary>
        /// Gets whether or not the provided coordinates lie on the line.
        /// </summary>
        /// <param name="coordinates">The coordinates to check.</param>
        /// <returns>true if the coordinates are within the circle.</returns>
        public override bool Contains(Vector2 coordinates)
        {
            // TODO Implement
            Vector2 nearest = NearestPointOnLine(coordinates);
            return (nearest - coordinates).LengthSquared() == 0;
        }

        public bool Equals(LinePieceCollider other)
        {
            return other.Start == this.Start && other.End == this.End;
        }

        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public static Vector2 GetDirection(Point point1, Point point2)
        {
            return GetDirection(point1.ToVector2(), point2.ToVector2());
        }


        /// <summary>
        /// Calculates the normalized vector pointing from point1 to point2
        /// </summary>
        /// <returns> A Vector2 containing the direction from point1 to point2. </returns>
        public Vector2 GetDirection()
        {
            return GetDirection(Start, End);
        }


        /// <summary>
        /// Should return the angle between a given direction and the up vector.
        /// </summary>
        /// <param name="direction">The Vector2 pointing out from (0,0) to calculate the angle to.</param>
        /// <returns> The angle in radians between the the up vector and the direction to the cursor.</returns>
        public float GetAngle()
        {
            return GetAngle(GetDirection());
        }
    }
}
