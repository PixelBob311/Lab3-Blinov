namespace Lab3.Ast.Statements {
	sealed class VariableDeclaration : IStatement {
		public readonly string VariableName;
		public readonly IExpression Expr;
		public string FormattedString => $"var {VariableName} = {Expr.FormattedString};\n";
		public VariableDeclaration(string variableName, IExpression expr) {
			VariableName = variableName;
			Expr = expr;
		}
		public void Accept(IStatementVisitor visitor) => visitor.VisitVariableDeclaration(this);
		public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitVariableDeclaration(this);
	}
}
