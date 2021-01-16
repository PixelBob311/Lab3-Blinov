using Lab3.Ast.Statements;
namespace Lab3.Ast {
	interface IStatementVisitor {
		void VisitIf(If ifStatement);
		void VisitWhile(While whileStatement);
		void VisitExpressionStatement(ExpressionStatement expressionStatement);
		void VisitVariableDeclaration(VariableDeclaration variableDeclaration);
		void VisitVariableAssignment(VariableAssignment variableAssignment);
	}
	interface IStatementVisitor<T> {
		T VisitIf(If ifStatement);
		T VisitWhile(While whiteStatement);
		T VisitExpressionStatement(ExpressionStatement expressionStatement);
		T VisitVariableDeclaration(VariableDeclaration variableDeclaration);
		T VisitVariableAssignment(VariableAssignment variableAssignment);
	}
}
