﻿// Copyright © 2021 Alex Kukhtin. All rights reserved.

using System;

using Microsoft.Extensions.DependencyInjection;

using A2v10.Infrastructure;
using System.Collections.Generic;

namespace A2v10.Platform.Web;
public record ViewEngineDescriptor(String Extension, Type EngineType);

public class ViewEngineFactory
{
	private readonly IList<ViewEngineDescriptor> _list = new List<ViewEngineDescriptor>();

	public IList<ViewEngineDescriptor> Engines => _list;

	public void RegisterEngine<T>(String extension)
	{
		_list.Add(new ViewEngineDescriptor(extension, typeof(T)));
	}
}

public class WebViewEngineProvider : IViewEngineProvider
{
	private readonly IList<ViewEngineDescriptor> _engines;
	private readonly IAppCodeProvider _codeProvider;
	private readonly IServiceProvider _serviceProvider;

	public WebViewEngineProvider(IServiceProvider serviceProvider, IList<ViewEngineDescriptor> engines)
	{
		_serviceProvider = serviceProvider;
		_codeProvider = _serviceProvider.GetRequiredService<IAppCodeProvider>();
		_engines = engines;
	}

	public void RegisterEngine(String extension, Type engineType)
	{
		_engines.Add(new ViewEngineDescriptor(extension, engineType));
	}

	public IViewEngineResult FindViewEngine(String viewName)
	{
		foreach (var engine in _engines)
		{
			String fileName = $"{viewName}{engine.Extension}";
			if (_codeProvider.FileExists(fileName))
			{
				if (_serviceProvider.GetService(engine.EngineType) is IViewEngine viewEngine)
				{
					return new ViewEngineResult
					(
						engine: viewEngine,
						fileName: fileName
					);
				}
			}
		}
		throw new InvalidReqestExecption($"View engine not found for {viewName}");
	}
}

