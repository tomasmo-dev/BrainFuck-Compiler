using CodePresets;
using Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Exceptions
{
    class InvalidCharException : Exception
    {
        public InvalidCharException()
        { }

        public InvalidCharException(string message)
            : base(message)
        { }

        public InvalidCharException(string message, Exception ex)
            : base(message, ex)
        { }
    }
    class PointerOutOfRangeException : Exception
    {
        public PointerOutOfRangeException() { }

        public PointerOutOfRangeException(string message) : base(message) { }

        public PointerOutOfRangeException(string message, Exception ex) : base(message, ex) { }
    }
    class StackOverFlowException : Exception
    {
        public StackOverFlowException() : base() { }
        public StackOverFlowException(string message) : base(message) { }
        public StackOverFlowException(string message, Exception ex) : base(message, ex) { }
    }
}

namespace CodePresets
{
    class Printer
    {
        public static string CS_WRITE = "Console.Write({0});\n";
        public static string BF_WRITE = "Console.Write({0});\n";

        public static string CS_NEWLINE = "Console.Write('\\n');\n";
        public static string BF_NEWLINE = "Console.Write('\\n');\n";
    }
    class Reader
    {
        public static string CS_READLINE = "int {0} = int.Parse(Console.Readline());\n";
        public static string BF_READLINE = "int {0} = int.Parse(Console.Readline());\n";
    }
}

namespace BF_Compiler
{
    static class Extensions
    {
        public static bool isValidChar(this char ch)
        {
            bool isValid = true;

            if (!Compiler.ValidChars.Contains(ch))
            {
                isValid = false;
            }
            return isValid;
        }
        public static void AddLineToString(this string converted, ref string codeOut)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(converted);
            sb.Append("\n");
            codeOut = sb.ToString();
        }

        public static string JoinArray(this string[] arr)
        {
            string temp = "";

            for (int i = 0; i < arr.Length; i++)
            {
                temp += arr[i].ToString();
            }
            return temp;
        }

    }

    class Compiler
    {
        private string compilePath;
        private byte[] stack = new byte[30000];

        public readonly byte stackMaxVal = byte.MaxValue;
        public readonly byte stackMinVal = 0;

        private uint stackPointer = 0;

        private readonly char commentFlag;
        private readonly bool suppress_StackOverFlowError;

        private List<string> code = new();  // Each member of array is one line of code

        public static readonly char[] ValidChars = { '+', '-', '[', ']', ',', '.', '<', '>', ';', ':' };

        private string csCodeConverted;

        private bool runtime;
        private bool printAscii;


        public Compiler(string path, bool suppress_StackOverFlow = false, char commentChar = ';', bool run_in_runtime = true, bool print_ascii = false)
        {
            compilePath = path;
            commentFlag = commentChar;
            suppress_StackOverFlowError = suppress_StackOverFlow;

            printAscii = print_ascii;

            runtime = run_in_runtime;

            LoadProgram();
            Compile();
        }

        struct LoopInfo
        {
            public int lstart;
            public int lexnd;

            public ulong id;

            public uint stackPointer;
        }

        private void LoadProgram()
        {
            string LoadedText = File.ReadAllText(compilePath);
            LoadedText = LoadedText.Replace("\r\n", "\n");
            string[] lines = LoadedText.Split('\n');

            uint lineNumber = 1;

            foreach (string line in lines)
            {
                foreach (char ch in line)
                {
                    if (!ch.isValidChar())
                    {
                        throw new InvalidCharException($"Error on line : {lineNumber} \nIn Char : {ch}");
                    }
                }
                lineNumber++;
            }
            code = lines.ToList();

        }

        private void Compile()
        {
            StringBuilder sb = new StringBuilder();

            string Ocode = code.ToArray().JoinArray();
            List<LoopInfo> info = new();

            List<int> lastloopINdex = new();

            bool inLoop = false;

            for (int character = 0; character < Ocode.Length; character++)
            {
                char selectedChar = Ocode[character];

                switch (selectedChar)
                {
                    case '+':
                        if (suppress_StackOverFlowError)
                        {
                            if (stack[stackPointer] + 1 != stackMaxVal)
                            {
                                stack[stackPointer] += 1;

                            }
                            else
                            {
                                throw new StackOverflowException($"StackOverFlow+ error at line : ?\nPointer at : {stackPointer}\nWith value : {stack[stackPointer]}");
                            }

                        }
                        else
                        {
                            stack[stackPointer] += 1;

                        }
                        break;

                    case '-':
                        if (suppress_StackOverFlowError)
                        {
                            stack[stackPointer] -= 1;

                        }
                        else
                        {
                            stack[stackPointer] -= 1;
                        }
                        break;

                    case '[':
                        // loop

                        inLoop = true;

                        if (stack[stackPointer] != 0)
                        {
                            info.Add(new LoopInfo() { id = (ulong)character, lstart = character, lexnd = GetEndLoop(character), stackPointer = stackPointer});
                            lastloopINdex.Add(character);
                        }
                        else if (stack[stackPointer] == 0)
                        {
                            inLoop = false;
                            character = GetEndLoop(character);
                        }

                        break;

                    case ']':
                        // loop end

                        if (stack[stackPointer] != 0 )
                        {
                            character = lastloopINdex[lastloopINdex.Count - 1];
                        }
                        else if (stack[stackPointer] == 0)
                        {
                            inLoop = false;
                            lastloopINdex.RemoveAt(lastloopINdex.Count - 1);
                        }

                        break;

                    case '<':
                        if (stackPointer != 0)
                        {
                            stackPointer--;
                        }
                        else
                        {
                            throw new PointerOutOfRangeException($"Tried to decrement pointer below 0 at line : {character}");
                        }
                        break;

                    case '>':
                        if (stackPointer != stack.Length - 1)
                        {
                            stackPointer++;
                        }
                        else
                        {
                            throw new PointerOutOfRangeException($"Tried to increment pointer above max value at line : {character}");
                        }
                        break;

                    case ',':
                        if (!runtime)
                        {
                            sb.Append(string.Format(Reader.CS_READLINE, $"_Line{character}_Char{character}"));

                        }
                        else
                        {
                            stack[stackPointer] = (byte)int.Parse(Console.ReadLine());

                        }


                        break;

                    case '.':
                        if (!runtime)
                        {
                            sb.Append(string.Format(Printer.CS_WRITE, stack[stackPointer])); // use ascii

                        }
                        else
                        {
                            if (printAscii)
                            {
                                Console.WriteLine((char)stack[stackPointer]);

                            }
                            else
                            {
                                Console.WriteLine(stack[stackPointer]);
                            }

                        }

                        break;

                    case ':':
                        if (!runtime)
                        {
                            // make something
                        }

                        else
                        {
                            if (printAscii)
                            {
                                File.AppendAllText("bf-file.txt", Encoding.ASCII.GetString(new byte[] { stack[stackPointer] }));
                            }
                        }

                        break;

                }


            }

            int GetEndLoop(int indexStart)
            {
                int endindex = 0;

                int skip = 0;

                for (int i = indexStart; i < Ocode.Length; i++)
                {
                    if (Ocode[i] == '[')
                    {
                        skip++;
                    }
                    if (Ocode[i] == ']' && skip == 0)
                    {
                        endindex = i;
                    }

                    if (Ocode[i] == ']' && skip > 0)
                    {
                        skip--;
                        continue;
                    }
                }

                return endindex;
            }


        }

    }



}


