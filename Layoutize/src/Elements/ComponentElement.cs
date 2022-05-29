﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Layoutize.Layouts;
using Layoutize.Views;

namespace Layoutize.Elements;

internal abstract class ComponentElement : Element
{
	[DisallowNull]
	public Element? Child
	{
		get => _child;
		protected set
		{
			Debug.Assert(IsMounted);
			_child.Unmount();
			_child = value;
			_child.Mount(this);
			Debug.Assert(IsMounted);
		}
	}

	[MemberNotNullWhen(true, nameof(_child), nameof(Child), nameof(View))]
	public override bool IsMounted
	{
		get
		{
			var result = base.IsMounted;
			if (result)
			{
				Debug.Assert(Parent != null);
				Debug.Assert(_child != null);
				Debug.Assert(Child != null);
				Debug.Assert(Child.IsMounted);
				Debug.Assert(Child.Parent == this);
			}
			else
			{
				Debug.Assert(Parent == null);
				Debug.Assert(Child == null);
			}
			return result;
		}
	}

	public override View? View => Child?.View;

	protected ComponentElement(ComponentLayout layout)
		: base(layout)
	{
	}

	protected abstract Layout Build();

	[MemberNotNull(nameof(_child), nameof(Child), nameof(View))]
	protected override void OnLayoutUpdated(EventArgs e)
	{
		Debug.Assert(IsMounted);
		Child.Layout = Build();
		Debug.Assert(IsMounted);
		base.OnLayoutUpdated(e);
	}

	[MemberNotNull(nameof(_child), nameof(Child), nameof(View))]
	protected override void OnMounting(EventArgs e)
	{
		base.OnMounting(e);
		Debug.Assert(!IsMounted);
		_child = Build().CreateElement();
		_child.Mount(this);
		Debug.Assert(IsMounted);
	}

	protected override void OnUnmounting(EventArgs e)
	{
		base.OnUnmounting(e);
		Debug.Assert(IsMounted);
		_child.Unmount();
		_child = null;
		Debug.Assert(!IsMounted);
	}

	private Element? _child;
}
