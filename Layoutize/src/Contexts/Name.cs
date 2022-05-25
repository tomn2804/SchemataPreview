﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Layoutize.Elements;

namespace Layoutize.Contexts;

public static class Name
{
	public static bool IsValid([NotNullWhen(true)] string? value)
	{
		try
		{
			Validate(value);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static string Of(IBuildContext context)
	{
		var name = context.Element.View.Name;
		Debug.Assert(IsValid(name));
		return name;
	}

	public static void Validate([NotNull] string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException(
				$"Attribute value '{nameof(Name)}' is either null, empty, or consists of only white-space characters.",
				nameof(value)
			);
		}
		if (value.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
		{
			throw new ArgumentException(
				$"Attribute value '{nameof(Name)}' contains invalid path characters.",
				nameof(value)
			);
		}
	}
}
