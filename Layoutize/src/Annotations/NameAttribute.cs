﻿using System.ComponentModel.DataAnnotations;
using Layoutize.Contexts;

namespace Layoutize.Annotations;

public class NameAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext context)
	{
		try
		{
			Name.Validate((string?)value);
		}
		catch (ValidationException e)
		{
			return new(e.Message);
		}
		return null;
	}
}
