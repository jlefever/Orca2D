﻿using System;

namespace Orca2D
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var game = new MyGame.MyGame())
            {
                game.Run();
            }
        }
    }
}
