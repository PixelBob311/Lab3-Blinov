using Lab3.Ast;
using Mono.Cecil;
using System;
using System.IO;
namespace Lab3.DynamicCompiling {
	sealed class CompilingInterpreter {
		public string ClassName = "Program";
		public string MethodName = "Main";
		public string BinPath = "out.exe";
		public static void Run(ProgramNode program, string binPath = "out.exe") {
			new CompilingInterpreter() {
				BinPath = binPath,
			}.RunProgram(program);
		}
		public void RunProgram(ProgramNode program) {
			Compile(program);
			RunCompiled();
		}
		public void Compile(ProgramNode program) {
			var module = ModuleDefinition.CreateModule(
				Path.GetFileName(BinPath),
				ModuleKind.Console
			);
			var mainClass = new TypeDefinition(
				"", ClassName,
				TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit,
				module.TypeSystem.Object
			);
			module.Types.Add(mainClass);
			var method = new MethodDefinition(
				MethodName,
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static,
				module.TypeSystem.Void
			);
			mainClass.Methods.Add(method);
			var compiler = new Compiler(method);
			compiler.CompileProgram(program);
			module.EntryPoint = method;
			module.Write(BinPath);
		}
		public void RunCompiled() {
			var method = System.Reflection.Assembly.LoadFrom(BinPath).GetType(ClassName).GetMethod(MethodName);
			method.Invoke(null, Array.Empty<object>());
		}
	}
}
