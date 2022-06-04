﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Layoutize.Elements;

namespace Layoutize.Contexts;

public static class Path
{
	public static string Of(IBuildContext context)
	{
		string? path;
		void VisitParent(Element element)
		{
			var parent = element.Parent;
			Debug.Assert(parent != null);
			switch (parent)
			{
				case ViewElement viewElement:
					path = viewElement.ViewContext!.FullName;
					return;

				default:
					VisitParent(parent);
					return;
			}
		}
		var element = context.Element;
		Debug.Assert(element is not RootDirectoryElement);
		VisitParent(element);
		Debug.Assert(IsValid(path));
		return path;
	}

	internal static bool IsValid([NotNullWhen(true)] string? value)
	{
		try
		{
			Validate(value);
			return true;
		}
		catch
		{
			return false;
		}
	}

	internal static void Validate([NotNull] string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ValidationException(
				$"Property value '{nameof(Path)}' is either null, empty, or consists of only white-space characters."
			);
		}
		if (value.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1)
		{
			throw new ValidationException($"Property value '{nameof(Path)}' contains invalid characters.");
		}
		if (!string.IsNullOrWhiteSpace(value) && !System.IO.Path.IsPathFullyQualified(value))
		{
			throw new ValidationException($"Property value '{nameof(Path)}' is not an absolute path.");
		}
	}
}
