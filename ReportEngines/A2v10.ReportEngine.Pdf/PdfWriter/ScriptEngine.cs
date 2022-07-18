﻿// Copyright © 2022 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;

using Jint;
using Jint.Native;

namespace A2v10.ReportEngine.Pdf;

internal class ScriptEngine
{
	private readonly Engine _engine;
	private readonly CultureInfo _culture;

	public ScriptEngine(ExpandoObject model, CultureInfo cultureInfo, String? code)
	{
		_culture = cultureInfo;
		_engine = new Engine(opts =>
		{
			opts.Strict = true;
			//opts.Debugger.Enabled = true;
			opts.LocalTimeZone(TimeZoneInfo.Utc);
			opts.Culture(cultureInfo);
		});
		if (!String.IsNullOrEmpty(code))
			_engine.Evaluate(code);

		// all properties as Root objects
		foreach (var item in model)
			_engine.SetValue(item.Key, item.Value);

		_engine.SetValue("spellMoney", SpellMoney);
		_engine.SetValue("formatDate", FormatDate);
	}

	public IList<ExpandoObject> EvaluateCollection(String expression)
	{
		var list = _engine.Evaluate(expression).ToObject();
		if (list is IList<ExpandoObject> listExp)
			return listExp;
		throw new InvalidOperationException($"'{expression}' is not a collection");
	}

	public Object? EvaluateValue(String? expression)
	{
		if (expression == null)
			return null;
		return _engine.Evaluate(expression).ToObject();
	}

	public JsValue CreateAccessFunction(String expression)
	{
		return _engine.Evaluate($"_elem_ => _elem_.{expression}");
	}

	public Object Invoke(JsValue func, ExpandoObject? data)
	{
		return _engine.Invoke(func, data).ToObject();
	}

	String SpellMoney(Object value, String currencyCode)
	{
		if (String.IsNullOrEmpty(currencyCode))
			currencyCode = "980";
		var d = Convert.ToDecimal(value);
		return SpellString.SpellCurrency(d, _culture, currencyCode);
	}

	String FormatDate(Object value, String format)
	{
		if (value is DateTime valDate)
			return valDate.ToString(format, _culture);
		return "Invalid date";
	}
}