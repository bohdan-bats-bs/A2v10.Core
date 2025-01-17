﻿// Copyright © 2015-2023 Oleksandr Kukhtin. All rights reserved.


namespace A2v10.Xaml;

public enum TabPosition
{
	Top,
	Bottom
}

[ContentProperty("Tabs")]
public class TabPanel : UIElement
{
	public Object? ItemsSource { get; set; }

	public Object? Header { get; set; }

	public Boolean Border { get; set; }

	public Boolean FullPage { get; set; }
	public Length? MinHeight { get; set; }
	public Boolean Overflow { get; set; }
	public Length? Width { get; set; }
	public Length? Height { get; set; }

	public TabPosition TabPosition { get; set; }

	public TabCollection Tabs { get; set; } = new TabCollection();

	static String ReplaceScope(String path)
	{
		return path.Replace("tabitem.item.$Index", "tabitem.index").
				 Replace("tabitem.item.$Number", "tabitem.number");
	}

	public override void RenderElement(RenderContext context, Action<TagBuilder>? onRender = null)
	{
		if (SkipRender(context))
			return;
		var panel = new TagBuilder("a2-tab-panel", null, IsInGrid);
		onRender?.Invoke(panel);
		MergeAttributes(panel, context);
		panel.AddCssClassBool(Border, "bordered");
		panel.AddCssClassBool(FullPage, "full-page");
		panel.AddCssClassBool(Overflow, "overflow");
		panel.AddCssClass($"tab-pos-{TabPosition.ToString().ToLowerInvariant()}");

		if (MinHeight != null)
			panel.MergeStyleUnit("min-height", MinHeight.Value);

		if (Width != null)
			panel.MergeStyle("width", Width.Value);

		if (Height != null)
			panel.MergeStyleUnit("height", Height.Value);


		var isBind = GetBinding(nameof(ItemsSource));
		if (isBind != null)
		{
			panel.MergeAttribute(":items", isBind.GetPath(context));
			if (Tabs.Count != 1)
				throw new XamlException("If ItemsSource is specified, then only one Tab allowed in the collection");
			panel.RenderStart(context);
			var tml = new TagBuilder("template");
			tml.MergeAttribute("slot", "items");
			tml.MergeAttribute("slot-scope", "tabitem");
			tml.RenderStart(context);
			using (var cts = new ScopeContext(context, "tabitem.item", isBind.Path, ReplaceScope))
			{
				Tabs[0].RenderTemplate(context);
			}
			tml.RenderEnd(context);
			RenderHeaderTemplate(context);
			RenderHeader(context);
			panel.RenderEnd(context);
		}
		else
		{
			panel.RenderStart(context);
			RenderTabs(context);
			RenderHeader(context);
			panel.RenderEnd(context);
		}
	}

	void RenderHeaderTemplate(RenderContext context)
	{
		if (Tabs[0].Header is not UIElementBase tabHeader)
			return;
		var tml = new TagBuilder("template");
		tml.MergeAttribute("slot", "header");
		tml.MergeAttribute("slot-scope", "tabitem");
		tml.RenderStart(context);
		using (var cts = new ScopeContext(context, "tabitem.item", null, ReplaceScope))
		{
			tabHeader.RenderElement(context);
		}
		tml.RenderEnd(context);
	}


	void RenderTabs(RenderContext context)
	{
		foreach (var tab in Tabs)
			tab.RenderElement(context);
	}

	void RenderHeader(RenderContext context)
	{
		var hBind = GetBinding(nameof(Header));
		if (hBind != null || Header != null)
		{
			var ht = new TagBuilder("template");
			ht.MergeAttribute("slot", "title");
			ht.RenderStart(context);
			var tag = new TagBuilder("div", "pane-header");
			if (hBind != null)
				tag.MergeAttribute("v-text", hBind.GetPath(context));
			tag.RenderStart(context);
			if (Header != null)
				RenderContent(context, Header);
			tag.RenderEnd(context);
			ht.RenderEnd(context);
		}
	}

	protected override void OnEndInit()
	{
		base.OnEndInit();
		foreach (var tab in Tabs)
			tab.SetParent(this);
	}

	public override void OnSetStyles(RootContainer root)
	{
		base.OnSetStyles(root);
		foreach (var tab in Tabs)
			tab.OnSetStyles(root);
	}
}
