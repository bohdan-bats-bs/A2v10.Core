﻿// Copyright © 2015-2017 Alex Kukhtin. All rights reserved.

namespace A2v10.Xaml
{
	public class GridGroup : Container
	{
		public override void RenderElement(RenderContext context, Action<TagBuilder>? onRender = null)
		{
			throw new XamlException("A GridGroup can only be a direct child of the Grid");
		}
	}
}
