﻿// Copyright © 2021 Alex Kukhtin. All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;

using A2v10.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace A2v10.Platform.Web
{
	public record ReportEngineDescriptor(String Name, Type EngineType);

	public class ReportEngineFactory
	{
		private readonly IList<ReportEngineDescriptor> _list = new List<ReportEngineDescriptor>();

		public IList<ReportEngineDescriptor> Engines => _list;

		public void RegisterEngine<T>(String name)
		{
			_list.Add(new ReportEngineDescriptor(name, typeof(T)));
		}
	}

	public class WebReportEngineProvider : IReportEngineProvider
	{
		private readonly IList<ReportEngineDescriptor> _engines;
		private readonly IServiceProvider _serviceProvider;

		public WebReportEngineProvider(IServiceProvider serviceProvider, IList<ReportEngineDescriptor> engines)
		{
			_serviceProvider = serviceProvider;
			_engines = engines;
		}

		public void RegisterEngine(String name, Type engineType)
		{
			_engines.Add(new ReportEngineDescriptor(name, engineType));
		}

		public IReportEngine FindReportEngine(String name)
		{
			var engine = _engines.FirstOrDefault(x => x.Name == name);
			if (engine == null)
				throw new InvalidReqestExecption($"Report engine for '{name}' not found");
			var rs = _serviceProvider.GetRequiredService(engine.EngineType);
			if (rs is IReportEngine re)
				return re;
			throw new InvalidReqestExecption($"Report engine '{engine.EngineType}' is not an IReportEngine");
		}
	}
}
