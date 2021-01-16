using Lab3.Ast;
using Lab3.DynamicCompiling;
using Lab3.Interpreting;
using Lab3.Parsing;
using System;
namespace Lab3 {
	public static class Program {
		static ProgramNode CheckedParse(SourceFile sourceFile) {
			var programNode = Parser.Parse(sourceFile);
			var code2 = programNode.FormattedString;
			var programNode2 = Parser.Parse(SourceFile.FromString(code2));
			var code3 = programNode2.FormattedString;
			if (code2 != code3) {
				Console.WriteLine(code2);
				Console.WriteLine(code3);
				throw new Exception($"Кривой парсер или {nameof(INode.FormattedString)} у узлов");
			}
			return programNode;
		}
		public static void Main(string[] args) {
			Interpreter.Run(CheckedParse(SourceFile.Read("../../code.txt")));
			CompilingInterpreter.Run(CheckedParse(SourceFile.Read("../../code.txt")));
		}
	}
}
