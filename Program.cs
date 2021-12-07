using System;
using System.IO;
using BF_Compiler;

namespace BrainFuck_Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (!File.Exists(args[0]))
            //{
            //    throw new Exception($"File doesnt exist at path! : {args[0]}");
            //}
            //if (File.Exists(args[1]))
            //{
            //    throw new Exception($"File already exists ar path! : {args[1]}");
            //}
            //if (File.Exists(args[2]))
            //{
            //    throw new Exception($"File already exists ar path! : {args[1]}");
            //}



            Compiler compiler = new Compiler(@"C:\Games\programs\c#\BrainFuck_Compiler\bin\Debug\net6.0\brainfucktest.BF");
            //compiler.OptionalSaveCsFile(@"C:\Games\programs\c#\BrainFuck_Compiler\bin\Debug\net6.0\cscodeout.cs");
            //compiler.CompileToExe(@"C:\Games\programs\c#\BrainFuck_Compiler\bin\Debug\net6.0\outapp.exe");
        }
    }
}
