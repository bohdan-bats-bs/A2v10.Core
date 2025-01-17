﻿// Copyright © 2021 Alex Kukhtin. All rights reserved.

using System;

using A2v10.Runtime.Interfaces;

namespace A2v10.Infrastructure
{
	public enum InvokeScope
	{
		Singleton,
		Scoped,
		Transient
	}

	public interface IInvokeEngineProvider
	{
		IRuntimeInvokeTarget FindEngine(String name);
	}
}
