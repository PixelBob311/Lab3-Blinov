using Lab3.Parsing;
using System.Collections.Generic;
using System.Linq;
namespace Lab3.Ast {
	sealed class ProgramNode : INode {
		public readonly SourceFile SourceFile;
		public readonly IReadOnlyList<IStatement> Statements;
		public ProgramNode(SourceFile sourceFile, IReadOnlyList<IStatement> statements) {
			SourceFile = sourceFile;
			Statements = statements;
		}
		public string FormattedString => string.Join("", Statements.Select(x => x.FormattedString));
	}
}
