﻿// Copyright © 2015-2022 Oleksandr Kukhtin. All rights reserved.

using System.Collections.Generic;

namespace A2v10.Xaml;

[ContentProperty("Content")]
public class ComboBoxItem : UIElementBase
{
	public String? Content { get; set; }
	public Object? Value { get; set; }

	public override void RenderElement(RenderContext context, Action<TagBuilder>? onRender = null)
	{
		if (SkipRender(context))
			return;
		var option = new TagBuilder("option");
		MergeAttributes(option, context, MergeAttrMode.Visibility);
		if (Value != null)
		{
			if (Value is IJavaScriptSource valJsSource)
			{
				var jsValue = valJsSource.GetJsValue(context);
				option.MergeAttribute(":value", jsValue);
			}
			else
				option.MergeAttribute("value", Value?.ToString());
		}
		else
		{
			option.MergeAttribute(":value", "null"); // JS null value
		}
		option.RenderStart(context);
		if (Content != null)
			context.Writer.Write(context.LocalizeCheckApostrophe(Content));
		option.RenderEnd(context);
	}
}

public class ComboBoxItems : List<ComboBoxItem>
{

}

public enum ComboBoxStyle
{
	Default,
	Hyperlink
}

[ContentProperty("Children")]
public class ComboBox : ValuedControl, ITableControl
{
	public Object? ItemsSource { get; set; }
	public String? DisplayProperty { get; set; }
	public Boolean ShowValue { get; set; }
	public TextAlign Align { get; set; }
	public ComboBoxStyle Style { get; set; }
	public String? GroupBy { get; set; }

	ComboBoxItems? _children;

	public ComboBoxItems Children
	{
		get
		{
			_children ??= new ComboBoxItems();
			return _children;
		}
		set
		{
			_children = value;
		}
	}

	public override void RenderElement(RenderContext context, Action<TagBuilder>? onRender = null)
	{
		if (CheckDisabledModel(context))
			return;
		var combo = new TagBuilder("select", null, IsInGrid);
		onRender?.Invoke(combo);
		combo.MergeAttribute("is", "combobox");
		combo.MergeAttribute("v-cloak", String.Empty);
		combo.MergeAttribute("name-prop", DisplayProperty);
		if (Style != ComboBoxStyle.Default)
			combo.AddCssClass($"combo-{Style.ToString().ToLowerInvariant()}");
		if (!String.IsNullOrEmpty(GroupBy))
			combo.MergeAttribute("groupby", GroupBy);
		MergeAttributes(combo, context);
		MergeAlign(combo, context, Align);
		MergeBoolAttribute(combo, context, nameof(ShowValue), ShowValue);
		SetSize(combo, nameof(ComboBox));
		MergeDisabled(combo, context);
		var isBind = GetBinding(nameof(ItemsSource));
		if (isBind != null)
		{
			combo.MergeAttribute(":items-source", isBind.GetPath(context));
			if (_children != null)
			{
				if (Children.Count != 1)
				{
					throw new XamlException("The ComboBox with the specified ItemsSource must have only one ComboBoxItem element");
				}
				var elem = Children[0];
				var contBind = elem.GetBinding("Content");
				if (contBind == null)
					throw new XamlException("ComboBoxItem. Content binging must be specified");
				combo.MergeAttribute(":name-prop", $"'{contBind.Path}'"); /*without context!*/
				var valBind = elem.GetBinding("Value");
				if (valBind == null)
					throw new XamlException("ComboBoxItem. Value binging must be specified");
				combo.MergeAttribute(":value-prop", $"'{valBind.Path}'");  /*without context!*/
			}
		}
		MergeValue(combo, context);
		combo.RenderStart(context);
		if (_children != null && isBind == null)
		{
			foreach (var ch in Children)
				ch.RenderElement(context);
		}
		RenderPopover(context);
		RenderHint(context);
		RenderLink(context);
		combo.RenderEnd(context);
	}

	protected override void OnEndInit()
	{
		base.OnEndInit();
		if (_children != null)
			foreach (var ch in Children)
				ch.SetParent(this);
	}

	public override void OnSetStyles(RootContainer root)
	{
		base.OnSetStyles(root);
		if (_children != null)
			foreach (var ch in Children)
				ch.OnSetStyles(root);
	}
}
