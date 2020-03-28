using Microsoft.Xna.Framework;

namespace Orca2D.Game
{
    /// <summary>
    /// Represents a 2D circle.
    /// </summary>
    public struct Circle
    {
        /// <summary>
        /// Center position of the circle.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Constructs a new circle.
        /// </summary>
        public Circle(Vector2 position, float radius)
        {
            Center = position;
            Radius = radius;
        }

        /// <summary>
        /// Determines if a circle intersects a rectangle.
        /// </summary>
        /// <returns>True if the circle and rectangle overlap. False otherwise.</returns>
        public bool Intersects(Rectangle rectangle)
        {
            var x = MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right);
            var y = MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom);

            var direction = Center - new Vector2(x, y);
            var distanceSquared = direction.LengthSquared();

            return distanceSquared > 0 && distanceSquared < Radius * Radius;
        }
    }
}
