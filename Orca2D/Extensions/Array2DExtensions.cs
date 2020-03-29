namespace Orca2D.Extensions
{
    public static class Array2DExtensions
    {
        public static int GetHeight<T>(this T[,] array)
        {
            return array.GetLength(1);
        }

        public static int GetWidth<T>(this T[,] array)
        {
            return array.GetLength(0);
        }
    }
}
