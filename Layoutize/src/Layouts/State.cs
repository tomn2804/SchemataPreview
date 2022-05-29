﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Layoutize.Elements;

namespace Layoutize.Layouts;

public abstract class State
{
	[MemberNotNull(nameof(Element))]
	protected internal abstract Layout Build(IBuildContext context);

	[MemberNotNull(nameof(Element))]
	protected void SetState(Action action)
	{
		OnStateUpdating(EventArgs.Empty);
		action.Invoke();
		Validate();
		OnStateUpdated(EventArgs.Empty);
	}

	[MemberNotNullWhen(true, nameof(Element))]
	internal virtual bool IsValid()
	{
		try
		{
			Validate();
		}
		catch
		{
			return false;
		}
		return true;
	}

	[MemberNotNull(nameof(Element))]
	internal virtual void Validate()
	{
		Validator.ValidateObject(this, new(this));
		if (Element == null) throw new ValidationException("Element is uninitialized.");
	}

	internal event EventHandler? StateUpdated;

	internal event EventHandler? StateUpdating;

	[Required]
	[DisallowNull]
	internal StatefulElement? Element; // TODO: Replace this with ViewModel

	[MemberNotNull(nameof(Element))]
	private protected virtual void OnStateUpdated(EventArgs e)
	{
		Debug.Assert(IsValid());
		StateUpdated?.Invoke(this, e);
		Debug.Assert(IsValid());
	}

	[MemberNotNull(nameof(Element))]
	private protected virtual void OnStateUpdating(EventArgs e)
	{
		Debug.Assert(IsValid());
		StateUpdating?.Invoke(this, e);
		Debug.Assert(IsValid());
	}
}

public abstract class State<T> : State where T : StatefulLayout
{
	protected T Layout
	{
		get
		{
			Debug.Assert(IsValid());
			return (T)Element.Layout;
		}
	}
}
