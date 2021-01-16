using Lab3.Ast;
using Lab3.Ast.Expressions;
using Lab3.Ast.Statements;
using Lab3.Interpreting.Values;
using Lab3.Interpreting.Values.Functions;
using Lab3.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static System.Diagnostics.Debug;
namespace Lab3.Interpreting {
	sealed class Interpreter : IStatementVisitor, IExpressionVisitor<object> {
		static readonly object missingVariable = new object();
		SourceFile sourceFile;
		Dictionary<string, object> currentBlockShadowedVariables = null;
		public readonly IDictionary<string, object> Variables;
		public Interpreter() {
			Variables = new Dictionary<string, object> {
				{ "true", true },
				{ "false", false },
				{ "null", null },
				{ "dump", new DumpFunction() },
				{ "trace", new TraceFunction() },
			};
		}
		public static void Run(ProgramNode program) {
			var interpreter = new Interpreter();
			interpreter.RunProgram(program);
		}
		public void RunProgram(ProgramNode program) {
			sourceFile = program.SourceFile;
			try {
				foreach (var statement in program.Statements) {
					Run(statement);
				}
			}
			finally {
				sourceFile = null;
			}
		}
		void RunBlock(Block block) {
			var oldShadowedVariables = currentBlockShadowedVariables;
			currentBlockShadowedVariables = new Dictionary<string, object>();
			foreach (var statement in block.Statements) {
				Run(statement);
			}
			foreach (var kv in currentBlockShadowedVariables) {
				var name = kv.Key;
				var shadowedVariable = kv.Value;
				if (shadowedVariable == missingVariable) {
					Variables.Remove(name);
				}
				else {
					Variables[name] = shadowedVariable;
				}
			}
			currentBlockShadowedVariables = oldShadowedVariables;
		}
		#region statements
		void Run(IStatement statement) {
			statement.Accept(this);
		}
		public void VisitIf(If ifStatement) {
			if (Calc<bool>(ifStatement.Condition)) {
				RunBlock(ifStatement.Body);
			}
		}
		public void VisitWhile(While whileStatement) {
			while (Calc<bool>(whileStatement.Condition)) {
				RunBlock(whileStatement.Body);
			}
		}
		public void VisitExpressionStatement(ExpressionStatement expressionStatement) {
			Calc(expressionStatement.Expr);
		}
		public void VisitVariableDeclaration(VariableDeclaration variableDeclaration) {
			var name = variableDeclaration.VariableName;
			if (currentBlockShadowedVariables != null && !currentBlockShadowedVariables.ContainsKey(name)) {
				if (Variables.TryGetValue(name, out object value)) {
					currentBlockShadowedVariables[name] = value;
				}
				else {
					currentBlockShadowedVariables[name] = missingVariable;
				}
			}
			Variables[name] = Calc(variableDeclaration.Expr);
		}
		public void VisitVariableAssignment(VariableAssignment variableAssignment) {
			if (!Variables.ContainsKey(variableAssignment.VariableName)) {
				throw MakeError(variableAssignment.Expr, $"Присваивание в неизвестную переменную {variableAssignment.VariableName}");
			}
			Variables[variableAssignment.VariableName] = Calc(variableAssignment.Expr);
		}
		#endregion
		#region expressions
		object Calc(IExpression expression) {
			return expression.Accept(this);
		}
		T Calc<T>(IExpression expression) {
			var value = Calc(expression);
			if (!(value is T)) {
				throw MakeError(expression, $"Ожидали {typeof(T)}, получили {value}");
			}
			return (T)value;
		}
		public object VisitBinary(Binary binary) {
			switch (binary.Operator) {
				case BinaryOperator.Addition:
					return CalcAddition(binary);
				case BinaryOperator.Subtraction:
					return CalcSubtraction(binary);
				case BinaryOperator.Multiplication:
					return CalcMultiplication(binary);
				case BinaryOperator.Division:
					return CalcDivision(binary);
				case BinaryOperator.Remainder:
					return CalcReminder(binary);
				case BinaryOperator.Equal:
					return CalcEqual(binary);
				case BinaryOperator.Less:
					return CalcLess(binary);
				default:
					throw MakeError(binary, $"Неизвестная операция {binary.Operator}");
			}
		}
		#region binary operations
		object CalcAddition(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Addition);
			return Calc<int>(binary.Left) + Calc<int>(binary.Right);
		}
		object CalcSubtraction(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Subtraction);
			return Calc<int>(binary.Left) - Calc<int>(binary.Right);
		}
		object CalcMultiplication(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Multiplication);
			return Calc<int>(binary.Left) * Calc<int>(binary.Right);
		}
		object CalcDivision(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Division);
			return Calc<int>(binary.Left) / Calc<int>(binary.Right);
		}
		object CalcReminder(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Remainder);
			return Calc<int>(binary.Left) % Calc<int>(binary.Right);
		}
		object CalcEqual(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Equal);
			var a = Calc(binary.Left);
			var b = Calc(binary.Right);
			if (a == null) {
				return b == null;
			}
			if (b == null) {
				return a == null;
			}
			if (a.GetType() != b.GetType()) {
				return false;
			}
			if (a is int) {
				return (int)a == (int)b;
			}
			if (a is bool) {
				return (bool)a == (bool)b;
			}
			if (a is IReferenceEquatable) {
				return a == b;
			}
			throw MakeError(binary, $"Неподдерживаемые типы операндов {a} {b}");
		}
		object CalcLess(Binary binary) {
			Assert(binary.Operator == BinaryOperator.Less);
			var a = Calc(binary.Left);
			var b = Calc(binary.Right);
			if (a == null && b == null) {
				return false;
			}
			if (a is bool && b is bool) {
				return !(bool)a && (bool)b;
			}
			if (a is int && b is int) {
				return (int)a < (int)b;
			}
			throw MakeError(binary, $"Неподдерживаемые типы операндов {a} {b}");
		}
		#endregion
		public object VisitParentheses(Parentheses parentheses) {
			return Calc(parentheses.Expr);
		}
		public object VisitCall(Call call) {
			var value = Calc(call.Function);
			if (!(value is ICallable function)) {
				throw MakeError(call, $"Вызвали не функцию, а {value}");
			}
			var args = call.Arguments.Select(Calc).ToList();
			return function.Call(args);
		}
		public object VisitNumber(Number number) {
			if (int.TryParse(number.Lexeme, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int value)) {
				return value;
			}
			throw MakeError(number, $"Не удалось преобразовать {number.Lexeme} в int");
		}
		public object VisitIdentifier(Identifier identifier) {
			if (Variables.TryGetValue(identifier.Name, out object value)) {
				return value;
			}
			throw MakeError(identifier, $"Неизвестная переменная {identifier.Name}");
		}
		public object VisitMemberAccess(MemberAccess memberAccess) {
			throw new NotSupportedException();
		}
		#endregion
		Exception MakeError(IExpression expression, string message) {
			return new Exception(sourceFile.MakeErrorMessage(expression.Position, message));
		}
	}
}
