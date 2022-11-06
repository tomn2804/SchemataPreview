﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Layoutize.Contexts;
using Layoutize.Layouts;
using Layoutize.Utils;
using Layoutize.Views;

namespace Layoutize.Elements;

internal abstract class Element : IBuildContext, IComparable<Element>
{
	public int CompareTo(Element? other)
	{
		return string.Compare(Name.Of(this), other != null ? Name.Of(other) : null, StringComparison.Ordinal);
	}

	[MemberNotNull(nameof(View))]
	public void MountTo(Element? parent)
	{
		Debug.Assert(!IsMounted);
		Parent = parent;
		Cleanup = Mount();
		Debug.Assert(IsMounted);
	}

	protected abstract Action? Mount();

	public void Unmount()
	{
		Debug.Assert(IsMounted);
		Cleanup?.Invoke();
		Parent = null;
		Debug.Assert(!IsMounted);
	}

	private Action? Cleanup { get; set; }

	public abstract IView? View { get; }

	[MemberNotNullWhen(true, nameof(Parent), nameof(View))]
	public virtual bool IsMounted
	{
		get
		{
			if (View != null)
			{
				Debug.Assert(Parent != null);
				Debug.Assert(View.Exists);
				return true;
			}
			Debug.Assert(Parent == null);
			return false;
		}
	}

	public Layout Layout
	{
		get
		{
			Debug.Assert(Model.IsValid(_layout));
			return _layout;
		}
		[MemberNotNull(nameof(Parent), nameof(View))]
		set
		{
			Debug.Assert(Model.IsValid(value));
			Debug.Assert(IsMounted);
			OnLayoutUpdating(EventArgs.Empty);
			_layout = value;
			OnLayoutUpdated(EventArgs.Empty);
			Debug.Assert(Layout == value);
		}
	}

	public virtual Element? Parent { get; private set; }

	public event EventHandler? LayoutUpdated;

	public event EventHandler? LayoutUpdating;

	protected Element(Layout layout)
	{
		Debug.Assert(Model.IsValid(layout));
		_layout = layout;
		Debug.Assert(Layout == layout);
		Debug.Assert(!IsMounted);
	}

	[MemberNotNull(nameof(Parent), nameof(View))]
	protected virtual void OnLayoutUpdated(EventArgs e)
	{
		Debug.Assert(IsMounted);
		LayoutUpdated?.Invoke(this, e);
	}

	[MemberNotNull(nameof(Parent), nameof(View))]
	protected virtual void OnLayoutUpdating(EventArgs e)
	{
		Debug.Assert(IsMounted);
		LayoutUpdating?.Invoke(this, e);
	}

	Element IBuildContext.Element => this;

	private Layout _layout;
}
