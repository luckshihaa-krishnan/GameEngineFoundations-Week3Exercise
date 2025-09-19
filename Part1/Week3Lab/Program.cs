/*
 * Name: Luckshihaa Krishnan 
 * Student ID: 186418216
 * Section: GAM 531 NSA 
 */

using FirstOpenTK;
using System;
using Week3Lab;

namespace FirstOpenTK
{
    // Main Entry Point for C# Console
    class Program
    {
        static void Main(String[] args)
        {
            using (Game game = new Game())
            {
                game.Run(); //run the program
            }
        }
    }
}