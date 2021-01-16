using System;
using System.Collections.Generic;
using System.IO;
namespace Lab3.Interpreting.Values.Functions {
	sealed class TraceFunction : ICallable, IDumpable, IReferenceEquatable {
		readonly TextWriter Output;
		public TraceFunction(TextWriter output = null) {
			Output = output ?? Console.Out;
		}
		public object Call(IReadOnlyList<object> args) {
			if (args.Count != 1) {
				throw new Exception($"Нужен 1 аргумент, а не {args.Count}: {string.Join(", ", args)}");
			}
			Output.WriteLine($">> {DumpFunction.ValueToString(args[0])}");
			return args[0];
		}
		public string GetDumpString() {
			return "trace";
		}
	}
}
