﻿// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using A2v10.Infrastructure;

namespace A2v10.Xaml
{
	public class CardBody : Container
	{
		public override void RenderElement(RenderContext context, Action<TagBuilder>? onRender = null)
		{
			var body = new TagBuilder("div", "card-body");
			MergeAttributes(body, context);
			body.RenderStart(context);
			RenderChildren(context);
			body.RenderEnd(context);
		}
	}

	public class Card : Container, ITableControl
	{
		public Object? Header { get; set; }
		public Object? Footer { get; set; }
		public Object? Body { get; set; }

		public Length? Height { get; set; }
		public Length? Width { get; set; }

		public BackgroundStyle Background { get; set; }
		public ShadowStyle DropShadow { get; set; }
		public Boolean Compact { get; set; }

		public TextAlign Align { get; set; }

		public Length? MaxHeight { get; set; }

		public Popover? Hint { get; set; }

		Boolean HasHeader => GetBinding(nameof(Header)) != null || Header != null;
		Boolean HasFooter => GetBinding(nameof(Footer)) != null || Footer != null;

		public override void RenderElement(RenderContext context, Action<TagBuilder>? onRender = null)
		{
			if (SkipRender(context))
				return;
			var card = new TagBuilder("div", "card", IsInGrid);
			MergeAttributes(card, context);
			if (Background != BackgroundStyle.Default)
				card.AddCssClass("background-" + Background.ToString().ToKebabCase());
			if (MaxHeight != null)
				card.MergeStyle("max-height", MaxHeight.Value);
			if (DropShadow != ShadowStyle.None)
			{
				card.AddCssClass("drop-shadow");
				card.AddCssClass(DropShadow.ToString().ToLowerInvariant());
			}
			if (Align != TextAlign.Left)
				card.AddCssClass("text-" + Align.ToString().ToLowerInvariant());
			card.AddCssClassBool(Compact, "compact");

			if (Height != null)
				card.MergeStyle("height", Height.Value);
			if (Width != null)
				card.MergeStyle("width", Width.Value);

			card.RenderStart(context);
			RenderHeader(context);
			RenderChildren(context);
			RenderFooter(context);
			card.RenderEnd(context);
		}

		void RenderHeader(RenderContext context)
		{
			if (!HasHeader)
				return;
			var h = new TagBuilder("div", "card-header");
			h.RenderStart(context);
			var hb = GetBinding(nameof(Header));
			if (hb != null)
			{
				var s = new TagBuilder("span");
				s.MergeAttribute(":text", hb.GetPathFormat(context));
				s.Render(context);
			}
			else if (Header is UIElementBase hUiElem)
			{
				hUiElem.RenderElement(context);
			}
			else if (Header is String hStr)
			{
				context.Writer.Write(context.LocalizeCheckApostrophe(hStr));
			}
			RenderHint(context);
			h.RenderEnd(context);
		}

		void RenderHint(RenderContext context)
		{
			if (Hint == null)
				return;
			if (Hint.Icon == Icon.NoIcon)
				Hint.Icon = Icon.Help;
			Hint.RenderElement(context, (t) =>
			{
				t.AddCssClass("hint");
			});
		}

		void RenderFooter(RenderContext context)
		{
			if (!HasFooter)
				return;
			var f = new TagBuilder("div", "card-footer");
			f.RenderStart(context);
			var fb = GetBinding(nameof(Footer));
			if (fb != null)
			{
				var s = new TagBuilder("span");
				s.MergeAttribute(":text", fb.GetPathFormat(context));
				s.Render(context);
			}
			else if (Footer is UIElementBase fUiElem)
			{
				fUiElem.RenderElement(context);
			}
			else if (Footer is String fStr)
			{
				context.Writer.Write(context.LocalizeCheckApostrophe(fStr));
			}
			f.RenderEnd(context);
		}
	}
}
