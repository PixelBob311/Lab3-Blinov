using DynamicRuntime;
using Lab3.Ast;
using Lab3.Ast.Expressions;
using Lab3.Ast.Statements;
using Lab3.Parsing;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
namespace Lab3.DynamicCompiling {
	sealed class Compiler : IStatementVisitor, IExpressionVisitor {
		static readonly VariableDefinition missingVariable = null;
		SourceFile sourceFile;
		readonly MethodDefinition method;
		readonly ModuleDefinition module;
		readonly ILProcessor cil;
		readonly Dictionary<string, VariableDefinition> variables = new Dictionary<string, VariableDefinition>();
		Dictionary<string, VariableDefinition> currentBlockShadowedVariables = null;
		public Compiler(MethodDefinition method) {
			this.method = method;
			module = method.Module;
			cil = method.Body.GetILProcessor();
		}
		public void CompileProgram(ProgramNode program) {
			sourceFile = program.SourceFile;
			try {
				foreach (var field in typeof(BuiltinVariables).GetFields()) {
					if (field.FieldType != typeof(object)) {
						continue;
					}
					var variable = new VariableDefinition(module.TypeSystem.Object);
					method.Body.Variables.Add(variable);
					variables[field.Name] = variable;
					cil.Emit(OpCodes.Ldsfld, module.ImportReference(field));
					cil.Emit(OpCodes.Stloc, variable);
				}
				foreach (var statement in program.Statements) {
					CompileStatement(statement);
				}
				cil.Emit(OpCodes.Ret);
			}
			finally {
				sourceFile = null;
				variables.Clear();
			}
		}
		void EmitRuntimeCall(string methodName) {
			var method = typeof(Op).GetMethod(methodName);
			if (method == null) {
				throw new Exception($"{methodName} не найден");
			}
			var methodReference = module.ImportReference(method);
			cil.Emit(OpCodes.Call, methodReference);
		}
		void CompileBlock(Block block) {
			var oldShadowedVariables = currentBlockShadowedVariables;
			currentBlockShadowedVariables = new Dictionary<string, VariableDefinition>();
			foreach (var statement in block.Statements) {
				CompileStatement(statement);
			}
			foreach (var kv in currentBlockShadowedVariables) {
				var name = kv.Key;
				var shadowedVariable = kv.Value;
				if (shadowedVariable == missingVariable) {
					variables.Remove(name);
				}
				else {
					variables[name] = shadowedVariable;
				}
			}
			currentBlockShadowedVariables = oldShadowedVariables;
		}
		#region statements
		void CompileStatement(IStatement statement) {
			statement.Accept(this);
		}
		public void VisitIf(If ifStatement) {
			throw new NotImplementedException();
		}
		public void VisitWhile(While whileStatement) {
			throw new NotImplementedException();
		}
		public void VisitExpressionStatement(ExpressionStatement expressionStatement) {
			throw new NotImplementedException();
		}
		public void VisitVariableDeclaration(VariableDeclaration variableDeclaration) {
			CompileExpression(variableDeclaration.Expr);
			var name = variableDeclaration.VariableName;
			if (currentBlockShadowedVariables != null && !currentBlockShadowedVariables.ContainsKey(name)) {
				if (variables.TryGetValue(name, out var existingVariable)) {
					currentBlockShadowedVariables[name] = existingVariable;
				}
				else {
					currentBlockShadowedVariables[name] = missingVariable;
				}
			}
			var variable = new VariableDefinition(module.TypeSystem.Object);
			method.Body.Variables.Add(variable);
			variables[name] = variable;
			cil.Emit(OpCodes.Stloc, variable);
		}
		public void VisitVariableAssignment(VariableAssignment variableAssignment) {
			if (!variables.TryGetValue(variableAssignment.VariableName, out var variable)) {
				throw MakeError(variableAssignment.Expr, $"Присваивание в неизвестную переменную {variableAssignment.VariableName}");
			}
			CompileExpression(variableAssignment.Expr);
			cil.Emit(OpCodes.Stloc, variable);
		}
		#endregion
		#region expressions
		void CompileExpression(IExpression expression) {
			expression.Accept(this);
		}
		public void VisitBinary(Binary binary) {
			throw new NotImplementedException();
		}
		public void VisitCall(Call call) {
			throw new NotImplementedException();
		}
		public void VisitParentheses(Parentheses parentheses) {
			throw new NotImplementedException();
		}
		public void VisitNumber(Number number) {
			throw new NotImplementedException();
		}
		public void VisitIdentifier(Identifier identifier) {
			if (!variables.TryGetValue(identifier.Name, out var variable)) {
				throw MakeError(identifier, $"Неизвестная переменная {identifier.Name}");
			}
			cil.Emit(OpCodes.Ldloc, variable);
		}
		public void VisitMemberAccess(MemberAccess memberAccess) {
			throw new NotSupportedException();
		}
		#endregion
		Exception MakeError(IExpression expression, string message) {
			return new Exception(sourceFile.MakeErrorMessage(expression.Position, message));
		}
	}
}
