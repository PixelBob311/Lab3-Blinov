namespace Lab3.Ast.Statements {
	sealed class If : IStatement {
		public readonly IExpression Condition;
		public readonly Block Body;
		public readonly INode _else;
		public string FormattedString {
			get {
				if(_else==null)
					return $"if ({Condition.FormattedString}) {Body.FormattedString}";
				else {
					return $"if ({Condition.FormattedString}) {Body.FormattedString} else {_else.FormattedString}";
				}
			}
		}
		public If(IExpression condition, Block body, INode elseIf) {
			Condition = condition;
			Body = body;
			this._else = elseIf;
		}
		public void Accept(IStatementVisitor visitor) => visitor.VisitIf(this);
		public T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitIf(this);
	}
}
