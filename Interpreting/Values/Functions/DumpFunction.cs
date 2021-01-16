using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
namespace Lab3.Interpreting.Values.Functions {
	sealed class DumpFunction : ICallable, IDumpable, IReferenceEquatable {
		readonly TextWriter Output;
		public DumpFunction(TextWriter output = null) {
			Output = output ?? Console.Out;
		}
		public object Call(IReadOnlyList<object> args) {
			Output.WriteLine(string.Join(" ", args.Select(ValueToString)));
			return this;
		}
		public string GetDumpString() {
			return "dump";
		}
		public static string ValueToString(object value) {
			if (value == null) {
				return "null";
			}
			if (value is bool) {
				return (bool)value ? "true" : "false";
			}
			if (value is int) {
				return ((int)value).ToString(NumberFormatInfo.InvariantInfo);
			}
			if (value is IDumpable) {
				return ((IDumpable)value).GetDumpString();
			}
			throw new Exception($"Неизвестный тип значения {value}");
		}
	}
}
