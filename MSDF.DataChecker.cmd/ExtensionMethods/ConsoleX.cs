// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MSDF.DataChecker.cmd.ExtensionMethods
{
    public static class ConsoleX
    {
        public static void Write(string message)
        {
            Console.Write(message);
        }

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void WriteInfo(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
            Console.ForegroundColor = previousColor;
        }

        public static void WriteSuccess(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ForegroundColor = previousColor;
        }

        public static void WriteWarning(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ForegroundColor = previousColor;
        }

        public static void WriteError(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
            Console.ForegroundColor = previousColor;
        }

        public static void WriteLineInfo(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("  Info ");
            Console.ForegroundColor = previousColor;
            Console.WriteLine($" - {message}");
        }

        public static void WriteLineSuccess(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Success");
            Console.ForegroundColor = previousColor;
            Console.WriteLine($" - {message}");
        }

        public static void WriteLineWarning(string message)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Warning");
            Console.ForegroundColor = previousColor;
            Console.WriteLine($" - {message}");
        }

        public static void WriteLineError(string message)
        {
            var previousForegroundColorColor = Console.ForegroundColor;
            var previousBackgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Error ");
            Console.ForegroundColor = previousForegroundColorColor;
            Console.BackgroundColor = previousBackgroundColor;
            Console.WriteLine($" - {message}");
        }
    }
}
