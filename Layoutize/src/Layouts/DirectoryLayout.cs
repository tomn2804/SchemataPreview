﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Layoutize.Contexts;
using Layoutize.Elements;
using Layoutize.Views;
using Path = System.IO.Path;

namespace Layoutize.Layouts;

public class DirectoryLayout : ViewGroupLayout
{
	public new object Children
	{
		get => base.Children;
		init => base.Children = value switch
		{
			IEnumerable<object> children => children.Cast<Layout>(),
			_ => new[] { (Layout)value },
		};
	}

	internal override DirectoryElement CreateElement()
	{
		var element = new DirectoryElement(this);
		Debug.Assert(IsValid());
		return element;
	}

	internal override DirectoryView CreateView(IBuildContext context)
	{
		Debug.Assert(IsValid());
		var fullName = Path.Combine(Contexts.Path.Of(context), Name);
		Debug.Assert(FullName.IsValid(fullName));
		return new(new(fullName));
	}
}
