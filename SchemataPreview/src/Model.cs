﻿using System;
using System.IO;
using System.Management.Automation;

namespace SchemataPreview
{
	public abstract partial class Model
	{
		public Model(ImmutableSchema schema)
		{
			Schema = schema;
			PipeAssembly = new(this);
		}

		public abstract ModelSet? Children { get; }

		public abstract bool Exists { get; }

		public string Name => (string)Schema["Path"];

		public Model? Parent => (Model?)Schema.TryGetValue("Parent");

		public bool PassThru => (bool?)Schema.TryGetValue("PassThru") ?? false;

		public PipeAssembly PipeAssembly { get; }

		public PipelineTraversalOption Traversal => (PipelineTraversalOption?)Schema.TryGetValue("Traversal") ?? PipelineTraversalOption.PreOrder;

		public string FullName => Path.Combine(Parent?.FullName ?? (string)Schema["Path"], Name);

		public string RelativeName => Path.Combine(Parent?.RelativeName ?? string.Empty, Name);

		public static implicit operator string(Model rhs)
		{
			return rhs.FullName;
		}

		public Action CaptureContext(ScriptBlock script, params object[] args)
		{
			script = script.GetNewClosure();
			return () => script.InvokeWithContext(null, new()
			{
				{ new PSVariable("this", this) },
				{ new PSVariable("_", Schema) }
			}, args);
		}

		protected ImmutableSchema Schema { get; }
	}

	public abstract partial class Model : IComparable<Model>
	{
		public int CompareTo(Model? other)
		{
			if (other != null)
			{
				return ((int?)Schema.TryGetValue("Priority"))?.CompareTo(other.Schema.TryGetValue("Priority")) ?? Name.CompareTo(other.Name);
			}
			return 1;
		}
	}
}
