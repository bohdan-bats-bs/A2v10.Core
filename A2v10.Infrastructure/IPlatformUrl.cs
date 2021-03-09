﻿// Copyright © 2015-2021 Alex Kukhtin. All rights reserved.

using System;
using System.Dynamic;

namespace A2v10.Infrastructure
{
	public enum UrlKind
	{
		Undefined,
		Page,
		Dialog,
		Popup,
		Image,
		Report
	}

	public interface IPlatformUrl
	{
		String LocalPath { get; }
		String BaseUrl { get; }
		UrlKind Kind { get; }
		String Action { get; }
		String Id { get; }

		ExpandoObject Query { get; }
		void Redirect(String path);
	}
}
