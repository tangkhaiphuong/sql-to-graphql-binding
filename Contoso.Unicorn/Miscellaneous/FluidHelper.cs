using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Contoso.Collections;
using Contoso.Unicorn.GraphQL.Proxies;
using Fluid;
using Fluid.Values;
using GraphQL.Types.Relay.DataObjects;
using Humanizer;
using BooleanValue = Fluid.Values.BooleanValue;
using DateTimeValue = Fluid.Values.DateTimeValue;
using ObjectValue = Fluid.Values.ObjectValue;
using StringValue = Fluid.Values.StringValue;

namespace Contoso.Unicorn.Miscellaneous
{
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static class FluidHelper
    {
        public static TemplateOptions ConfigureTemplateOptions(this TemplateOptions templateOption)
        {
            templateOption.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.CamelCase;
            templateOption.Filters.AddFilter("get", GetAsync);
            templateOption.Filters.AddFilter("page", PageAsync);
            templateOption.Filters.AddFilter("md5", Md5Async);
            templateOption.Filters.AddFilter("parse", ParseAsync);
            templateOption.Filters.AddFilter("array", ArrayAsync);
            templateOption.Filters.AddFilter("object", ObjectAsync);
            templateOption.Filters.AddFilter("json", JsonAsync);
            templateOption.Filters.AddFilter("push", PushAsync);
            templateOption.Filters.AddFilter("pop", PopAsync);
            templateOption.Filters.AddFilter("shift", ShiftAsync);
            templateOption.Filters.AddFilter("unshift", UnshiftAsync);
            templateOption.Filters.AddFilter("slice", SliceAsync);
            templateOption.Filters.AddFilter("datediff", DateDiffAsync);
            templateOption.Filters.AddFilter("date_to_iso8601", DateToIso8601Async);
            templateOption.ValueConverters.Add(value =>
            {
                switch (value)
                {
                    case JsonElement json:
                        {
                            switch (json.ValueKind)
                            {
                                case JsonValueKind.Null:
                                    return NilValue.Instance;
                                case JsonValueKind.String:
                                    return new StringValue(json.ToString());
                                case JsonValueKind.Number:
                                    return NumberValue.Create(json.GetDecimal());
                                case JsonValueKind.Undefined:
                                    return NilValue.Empty;
                                case JsonValueKind.False:
                                    return BooleanValue.False;
                                case JsonValueKind.True:
                                    return BooleanValue.True;
                            }

                            if (json.ValueKind != JsonValueKind.Array) return new ObjectValue(json);
                            var enumerator = json.EnumerateArray();
                            var linkedList = new LinkedList<FluidValue>();
                            while (enumerator.MoveNext())
                                linkedList.AddLast(FluidValue.Create(enumerator.Current, templateOption));
                            return new ArrayValue(linkedList);
                        }
                    default:
                        return value;
                }
            });

            templateOption.MemberAccessStrategy.Register<Connection<BaseProxy>>();
            templateOption.MemberAccessStrategy.Register<BaseProxy, object>((source, name) => FluidValue.Create(source[name.Pascalize()], templateOption));
            templateOption.MemberAccessStrategy.Register<IDictionary<string, object>, object>((source, name) => FluidValue.Create(source[name], templateOption));
            templateOption.MemberAccessStrategy.Register<IDictionary<object, object>, object>((source, name) => FluidValue.Create(source[name], templateOption));
            templateOption.MemberAccessStrategy.Register<JsonElement, object>((source, name) =>
            {
                return source.ValueKind switch
                {
                    JsonValueKind.Array => int.TryParse(name, out var index) ? source[index] : default(object),
                    JsonValueKind.Object when source.TryGetProperty(name, out var result) => result,
                    _ => default
                };
            });
            return templateOption;
        }

        [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        public static ValueTask<FluidValue> DateDiffAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input?.Type != FluidValues.DateTime) throw new NotSupportedException("Not implement to things other than DateTime");
            var diff = arguments?.At(0);
            var ticks = Convert.ToInt64(input.ToNumberValue());
            switch (diff?.Type)
            {
                case FluidValues.Number:
                    {
                        var diffTicks = Convert.ToInt64(diff.ToNumberValue());
                        var inputDateTimeOffset = (DateTimeOffset)new DateTime(ticks).AddTicks(diffTicks * 10000000);
                        return new DateTimeValue(inputDateTimeOffset);
                    }
                case FluidValues.String:
                    {
                        var diffSpan = TimeSpan.Parse(diff.ToStringValue()).Ticks;
                        var inputDateTimeOffset = (DateTimeOffset)new DateTime(ticks).AddTicks(diffSpan);
                        return new DateTimeValue(inputDateTimeOffset);
                    }
                default:
                    throw new NotSupportedException("Argument is not a valid");
            }
        }

        public static ValueTask<FluidValue> DateToIso8601Async(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var datetime = DateTimeOffset.Parse(input.ToStringValue());
            if (arguments.Count == 0)
            {
                var result = datetime.ToLocalTime().ToString("O");
                return FluidValue.Create(result, context.Options);
            }
            else
            {
                var result = datetime.ToOffset(TimeSpan.Parse(arguments.At(0).ToStringValue())).ToString("O");
                return FluidValue.Create(result, context.Options);
            }
        }

        public static async ValueTask<FluidValue> GetAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            var obj = input.ToObjectValue();

            for (var index = 0; index < arguments.Count; ++index)
            {
                var arg = arguments.At(index);

                obj = await GetAsync(obj, arg, context).ConfigureAwait(false);
            }

            return FluidValue.Create(obj, context.Options);
        }

        private static async ValueTask<object> GetAsync(object target, FluidValue at, TemplateContext context)
        {
            if (at.Type == FluidValues.Array)
            {
                object result = target;
                foreach (var value in at.Enumerate())
                    result = GetAsync(result, value, context).ConfigureAwait(false);
                return result;
            }

            switch (target)
            {
                case IDictionary<object, object> dictionary:
                    return dictionary[at.ToObjectValue()];
                case IReadOnlyDictionary<object, object> readOnlyDictionary:
                    return readOnlyDictionary[at.ToObjectValue()];
                case IList<object> list:
                    return list[(int)at.ToNumberValue()];
                case IReadOnlyList<object> readOnlyList:
                    return readOnlyList[(int)at.ToNumberValue()];
                case JsonElement jsonElement:
                    return jsonElement.GetProperty(at.ToStringValue());
                case FluidValue fluidValue:
                    if (fluidValue.Type == FluidValues.Array)
                    {
                        return await fluidValue.GetIndexAsync(at, context).ConfigureAwait(false);
                    }
                    else if (fluidValue.Type == FluidValues.Dictionary)
                    {
                        return await fluidValue.GetValueAsync(at.ToStringValue(), context).ConfigureAwait(false);
                    }
                    else if (fluidValue.Type == FluidValues.Object)
                    {
                        return await fluidValue.GetValueAsync(at.ToStringValue(), context).ConfigureAwait(false);
                    }
                    else
                    {
                        return null;
                    }
                case IFluidIndexable fluidIndexable:
                    if (at.Type == FluidValues.String || at.Type == FluidValues.Number)
                    {
                        fluidIndexable.TryGetValue(at.ToStringValue(), out var value);
                        return value;
                    }
                    else if (at.Type == FluidValues.Array)
                    {
                        return await GetAsync(target, at, context).ConfigureAwait(false);
                    }
                    else
                        throw new NotSupportedException("Not support argument type " + Enum.GetName(at.Type.GetType(), at.Type));
                default:
                    return target.GetType()
                        .GetProperty(at.ToStringValue(),
                            BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
                        ?.GetValue(target);
            }
        }

        public static ValueTask<FluidValue> Md5Async(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            var value = input.ToStringValue();

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
            using var md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
            var inputBytes = Encoding.ASCII.GetBytes(value);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var t in hashBytes) sb.Append(t.ToString("X2", CultureInfo.InvariantCulture));

            return new StringValue(sb.ToString());
        }

        public static ValueTask<FluidValue> PageAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            if (input.Type != FluidValues.Array) throw new NotSupportedException("Not implement to things other than array");

            var index = (int)arguments.At(0).ToNumberValue();
            var size = (int)arguments.At(1).ToNumberValue();

            return new ArrayValue(input.Enumerate().Skip(index * size).Take(size));
        }

        public static ValueTask<FluidValue> ParseAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null) return NilValue.Empty;

            if (arguments?.At(0).ToStringValue() == "date")
            {
                return input.Type switch
                {
                    FluidValues.String => new DateTimeValue(DateTimeOffset.Parse(input.ToStringValue(), CultureInfo.InvariantCulture)),
                    FluidValues.Number => new DateTimeValue(
                        DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(input.ToNumberValue()))),
                    _ => throw new NotSupportedException("Input is not a valid DateTime")
                };
            }

            if (arguments?.At(0).ToStringValue() != "json") throw new NotSupportedException();

            var json = JsonSerializer.Deserialize<JsonElement>(input.ToStringValue());
            if (json.ValueKind != JsonValueKind.Array)
            {
                switch (json.ValueKind)
                {
                    case JsonValueKind.Undefined:
                        return FluidValue.Create(null, context.Options);
                    case JsonValueKind.String:
                        return FluidValue.Create(json.GetString(), context.Options);
                    case JsonValueKind.Number:
                        return FluidValue.Create(json.GetDouble(), context.Options);
                    case JsonValueKind.True:
                        return BooleanValue.True;
                    case JsonValueKind.False:
                        return BooleanValue.False;
                    case JsonValueKind.Null:
                        return FluidValue.Create(null, context.Options);
                    default:
                        return new ObjectValue(json);
                }
            }
            var enumerator = json.EnumerateArray();
            var linkedList = new LinkedList<FluidValue>();
            while (enumerator.MoveNext()) linkedList.AddLast(FluidValue.Create(enumerator.Current, context.Options));
            return new ArrayValue(linkedList);
        }

        public static ValueTask<FluidValue> PushAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null || input.Type != FluidValues.Array) throw new NotSupportedException("Not implement to things other than array");
            var linkedList = new LinkedList<FluidValue>();

            if (arguments == null) return new ArrayValue(input.Enumerate());
            for (var i = 0; i < arguments.Count; i++)
            {
                linkedList.AddLast(arguments.At(i));
            }
            return new ArrayValue(input.Enumerate().Concat(linkedList.AsEnumerable()));
        }

        public static ValueTask<FluidValue> UnshiftAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null || input.Type != FluidValues.Array) throw new NotSupportedException("Not implement to things other than array");
            var linkedList = new LinkedList<FluidValue>();

            if (arguments == null) return new ArrayValue(input.Enumerate());
            for (var i = 0; i < arguments.Count; i++)
            {
                linkedList.AddLast(arguments.At(i));
            }
            return new ArrayValue(linkedList.AsEnumerable().Concat(input.Enumerate()));
        }

        public static ValueTask<FluidValue> PopAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null || input.Type != FluidValues.Array) throw new NotSupportedException("Not implement to things other than array");
            if (arguments != null && int.TryParse(arguments.At(0).ToStringValue(), out var skip))
                return new ArrayValue(input.Enumerate().SkipLast(skip));
            return new ArrayValue(input.Enumerate().SkipLast(1));
        }

        public static ValueTask<FluidValue> ShiftAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null || input.Type != FluidValues.Array) throw new NotSupportedException("Not implement to things other than array");
            if (arguments != null && int.TryParse(arguments.At(0).ToStringValue(), out var skip))
                return new ArrayValue(input.Enumerate().Skip(skip));

            return new ArrayValue(input.Enumerate().Skip(1));
        }

        [SuppressMessage("ReSharper", "NotResolvedInText")]
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public static ValueTask<FluidValue> SliceAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input == null || input.Type != FluidValues.Array) throw new NotSupportedException("Not implement to things other than array");
            if (arguments == null || arguments.At(0).Type != FluidValues.String
                                  || arguments.At(1).Type != FluidValues.String
                                  || !int.TryParse(arguments.At(0).ToStringValue(), out var from)
                                  || !int.TryParse(arguments.At(1).ToStringValue(), out var to))
                throw new ArgumentNullException("SLICE_FILTER", "FROM or TO offset is not valid");
            return new ArrayValue(input.Enumerate().Where((c, index) => index >= from && index <= to));
        }

        private static ValueTask<FluidValue> ArrayAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            switch (input.Type)
            {
                case FluidValues.Nil:
                    return ArrayValue.Empty;
                case FluidValues.Array:
                    return input;
                case FluidValues.Boolean:
                    return new ArrayValue(new[] { input });
                case FluidValues.Number:
                    return new ArrayValue(new[] { input });
                case FluidValues.String:
                    return new ArrayValue(new[] { input });
                case FluidValues.DateTime:
                    return new ArrayValue(new[] { input });
                default:
                    {
                        var obj = input.ToObjectValue();

                        var key = arguments.Count > 0 ? arguments.At(0).ToStringValue() : null;
                        var value = arguments.Count > 1 ? arguments.At(1).ToStringValue() : null;

                        switch (obj)
                        {
                            case IDictionary<string, object> genericDictionary:
                                return new ArrayValue(genericDictionary.Select(_ => new ArrayValue(new[]
                                {
                        FluidValue.Create(_.Key, context.Options), FluidValue.Create(_.Value, context.Options),
                    })));
                            case IDictionary dictionary:
                                {
                                    var linkedList = new LinkedList<FluidValue>();
                                    foreach (DictionaryEntry item in dictionary)
                                    {
                                        if (key == null && value == null)
                                            linkedList.AddLast(new ArrayValue(new[]
                                            {
                                                FluidValue.Create(item.Key, context.Options), FluidValue.Create(item.Value, context.Options),
                                            }));
                                        else
                                        {
                                            var dic = new Dictionary<string, object>();
                                            if (key != null) dic[key] = item.Key;
                                            if (value != null) dic[value] = item.Value;
                                            linkedList.AddLast(FluidValue.Create(dic, context.Options));
                                        }
                                    }

                                    return new ArrayValue(linkedList);
                                }
                            case IFluidIndexable fluidIndexable:
                                {
                                    var linkedList = new LinkedList<FluidValue>();
                                    foreach (var localKey in fluidIndexable.Keys)
                                    {
                                        if (key == null && value == null && fluidIndexable.TryGetValue(localKey, out var localValue))
                                            linkedList.AddLast(new ArrayValue(new[]
                                            {
                                            FluidValue.Create(localKey, context.Options), FluidValue.Create(localValue, context.Options),
                                        }));
                                        else
                                        {
                                            var dic = new Dictionary<string, object>();
                                            if (key != null) dic[key] = localKey;
                                            if (value != null && fluidIndexable.TryGetValue(localKey, out localValue)) dic[value] = localValue;
                                            linkedList.AddLast(FluidValue.Create(dic, context.Options));
                                        }
                                    }

                                    return new ArrayValue(linkedList);
                                }
                            case IDictionary<object, object> genericDictionary:
                                {
                                    var linkedList = new LinkedList<FluidValue>();
                                    foreach (var item in genericDictionary)
                                    {
                                        if (key == null && value == null)
                                            linkedList.AddLast(new ArrayValue(new[]
                                            {
                                            FluidValue.Create(item.Key, context.Options), FluidValue.Create(item.Value, context.Options),
                                        }));
                                        else
                                        {
                                            var dic = new Dictionary<string, object>();
                                            if (key != null) dic[key] = item.Key;
                                            if (value != null) dic[value] = item.Value;
                                            linkedList.AddLast(FluidValue.Create(dic, context.Options));
                                        }
                                    }

                                    return new ArrayValue(linkedList);
                                }
                            case JsonElement jsonElement:
                                switch (jsonElement.ValueKind)
                                {
                                    case JsonValueKind.Array:
                                        {
                                            var enumerator = jsonElement.EnumerateArray();
                                            var linkedList = new LinkedList<FluidValue>();
                                            while (enumerator.MoveNext()) linkedList.AddLast(FluidValue.Create(enumerator.Current, context.Options));
                                            return new ArrayValue(linkedList);
                                        }
                                    case JsonValueKind.Object:
                                        {
                                            var iterator = jsonElement.EnumerateObject();
                                            var linkedList = new LinkedList<FluidValue>();
                                            while (iterator.MoveNext())
                                            {
                                                if (key == null && value == null)
                                                    linkedList.AddLast(new ArrayValue(new[]
                                                {
                                                    FluidValue.Create(iterator.Current.Name, context.Options),
                                                    FluidValue.Create(iterator.Current.Value, context.Options),
                                                }));
                                                else
                                                {
                                                    var dic = new Dictionary<string, object>();
                                                    if (key != null) dic[key] = iterator.Current.Name;
                                                    if (value != null) dic[value] = iterator.Current.Value;
                                                    linkedList.AddLast(FluidValue.Create(dic, context.Options));
                                                }
                                            }

                                            return new ArrayValue(linkedList);
                                        }
                                    default:
                                        return new ArrayValue(new[] { FluidValue.Create(jsonElement, context.Options) });
                                }

                            default:
                                return new ArrayValue(obj.GetType()
                                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).Select(c =>
                                    {
                                        if (key == null && value == null)
                                            return new ArrayValue(new[]
                                            {
                                                FluidValue.Create(c.Name, context.Options), FluidValue.Create(c.GetValue(obj), context.Options),
                                            });
                                        var dic = new Dictionary<string, object>();
                                        if (key != null) dic[key] = c.Name;
                                        if (value != null) dic[value] = c.GetValue(obj);
                                        return FluidValue.Create(dic, context.Options);
                                    }));
                        }
                    }
            }
        }

        private static ValueTask<FluidValue> ObjectAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            switch (input.Type)
            {
                case FluidValues.Nil:
                    return input;
                case FluidValues.Boolean:
                    return input;
                case FluidValues.Dictionary:
                    return input;
                case FluidValues.Number:
                    return input;
                case FluidValues.Object:
                    return input;
                case FluidValues.String:
                    return input;
                case FluidValues.DateTime:
                    return input;
                default:
                    {
                        var keys = new List<string>();

                        for (var index = 0; index < arguments.Count; index++)
                            keys.Add(arguments.At(index).ToStringValue());

                        var result = Objecting(input.Enumerate().Select(c => c.ToObjectValue()), keys, context);

                        return result;
                    }
            }
        }

        private static IndexableDictionary Objecting(
            IEnumerable<object> source, IReadOnlyCollection<string> args, TemplateContext context)
        {
            if (source == null) return null;

            args ??= Array.Empty<string>();

            var result = new IndexableDictionary(context.Options);

            object GetValue(object target, string key)
            {
                switch (target)
                {
                    case IDictionary<object, object> dic:
                        {
                            return dic[key];
                        }
                    case JsonElement jsonElement:
                        {
                            var property = jsonElement.GetProperty(key);
                            switch (property.ValueKind)
                            {
                                case JsonValueKind.Undefined:
                                    throw new NotSupportedException($"Property {key} not found");
                                case JsonValueKind.Object:
                                    throw new NotSupportedException($"Value of property {key} is object");
                                case JsonValueKind.Array:
                                    throw new NotSupportedException("Value of property {key} is array");
                                case JsonValueKind.String:
                                    return property.GetString();
                                case JsonValueKind.Number:
                                    return property.GetDouble();
                                case JsonValueKind.True:
                                    return property.GetBoolean();
                                case JsonValueKind.False:
                                    return property.GetBoolean();
                                case JsonValueKind.Null:
                                    return null;
                                default:
                                    throw new NotSupportedException($"Not supported type of property {key}");
                            }
                        }
                    case AbstractProxy proxy:
                        {
                            return proxy[key];
                        }
                    default:
                        target.GetType()
                            .GetProperty(key, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
                            ?.GetValue(target);
                        return target.GetType()
                            .GetProperty(key, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
                            ?.GetValue(target);
                }
            }

            foreach (var item in source)
            {
                IDictionary<object, object> temp = result;

                var index = 0;

                foreach (var arg in args)
                {
                    var value = GetValue(item, arg);

                    if (!temp.ContainsKey(value))
                    {
                        if (index == args.Count - 1)
                        {
                            temp[value] = new List<object> { item };
                        }
                        else
                        {
                            var newDic = new IndexableDictionary(context.Options);
                            temp[value] = newDic;
                            temp = newDic;
                        }
                    }
                    else
                    {
                        switch (temp[value])
                        {
                            case List<object> list:
                                list.Add(item);
                                break;
                            case IDictionary<object, object> dic:
                                temp = dic;
                                break;
                        }
                    }

                    ++index;
                }
            }

            return result;
        }

        [SuppressMessage("ReSharper", "IdentifierTypo")]
        public static ValueTask<FluidValue> JsonAsync(
          FluidValue input,
          FilterArguments arguments,
          TemplateContext context)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (input == null) throw new ArgumentNullException(nameof(input));

            var options = new JsonSerializerOptions()
            {
                WriteIndented = arguments.At(0).ToBooleanValue(),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            object ResolveFluidIndexable(FluidValue o)
            {
                if (o is IndexableDictionary indexableDictionary)
                {
                    var dic = new Dictionary<string, object>();
                    foreach (var item in indexableDictionary.Dictionary)
                    {
                        var key = item.Key.ToString();
                        if (key == null) continue;

                        if (item.Value is FluidValue m)
                            dic[key] = ResolveFluidIndexable(m);
                        else
                            dic[key] = item.Value;
                    }

                    return dic;
                }
                else
                {
                    var temp = o.ToObjectValue();

                    if (!(temp is IFluidIndexable fluidIndexable)) return temp;
                    var dic = new Dictionary<string, object>();
                    foreach (var item in fluidIndexable.Keys)
                    {
                        if (fluidIndexable.TryGetValue(item, out var value))
                            dic[item] = ResolveFluidIndexable(value);
                    }

                    return dic;
                }
            }

            switch (input.Type)
            {
                case FluidValues.Nil:
                    return FluidValue.Create("null", context.Options);
                case FluidValues.Array:
                    var ret = JsonSerializer.Serialize(input.Enumerate().Select(ResolveFluidIndexable), options);
                    return new StringValue(ret);
                case FluidValues.Boolean:
                    return BooleanValue.Create(input.ToBooleanValue());
                case FluidValues.Dictionary:
                case FluidValues.Object:
                case FluidValues.DateTime:
                    return new StringValue(JsonSerializer.Serialize(ResolveFluidIndexable(input), options));
                case FluidValues.Number:
                    return NumberValue.Create(input.ToNumberValue());
                case FluidValues.String:
                    return new StringValue(JsonSerializer.Serialize(input.ToStringValue(), options));
                default:
                    throw new NotSupportedException("Unrecognized FluidValue");
            }
        }

        private class IndexableDictionary :
            FluidValue,
            IDictionary<string, object>,
            IDictionary<object, object>,
            IFluidIndexable
        {
            private readonly TemplateOptions _templateOptions;
            public IndexableDictionary(TemplateOptions templateOptions)
            {
                _templateOptions = templateOptions;
            }

            public IDictionary<object, object> Dictionary { get; } = new DefaultDictionary<object, object>();

            IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator() => Dictionary.GetEnumerator();

            IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
            {
                using var temp = Dictionary.GetEnumerator();
                while (temp.MoveNext())
                    yield return new KeyValuePair<string, object>(temp.Current.Key.ToString(), temp.Current.Value);
            }

            void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            {
                Dictionary.Add(item.Key, item.Value);
            }

            public void Add(KeyValuePair<object, object> item)
            {
                Dictionary.Add(item);
            }

            public void Clear()
            {
                Dictionary.Clear();
            }

            public bool Contains(KeyValuePair<object, object> item) => Dictionary.Contains(item);

            public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
            {
                Dictionary.CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<object, object> item) => Dictionary.Remove(item);

            public int Count => Dictionary.Count;

            bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            {
                return Dictionary.Contains(new KeyValuePair<object, object>(item.Key, item.Value));
            }

            void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                foreach (var item in Dictionary)
                {
                    array[arrayIndex++] = new KeyValuePair<string, object>(item.Key.ToString(), item.Value);
                    if (arrayIndex >= array.Length) break;
                }
            }

            bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            {
                return Dictionary.Remove(item);
            }

            int ICollection<KeyValuePair<string, object>>.Count => Dictionary.Count;

            public bool IsReadOnly => Dictionary.IsReadOnly;

            void IDictionary<string, object>.Add(string key, object value)
            {
                Dictionary.Add(key, value);
            }

            bool IDictionary<string, object>.ContainsKey(string key)
            {
                return Dictionary.ContainsKey(key);
            }

            bool IDictionary<string, object>.Remove(string key)
            {
                return Dictionary.Remove(key);
            }

            bool IDictionary<string, object>.TryGetValue(string key, out object value)
            {
                return Dictionary.TryGetValue(key, out value);
            }

            object IDictionary<string, object>.this[string key]
            {
                get => Dictionary[key];
                set => Dictionary[key] = value;
            }

            public void Add(object key, object value)
            {
                Dictionary.Add(key, value);
            }

            public bool ContainsKey(object key)
            {
                return Dictionary.ContainsKey(key);
            }

            public bool Remove(object key)
            {
                return Dictionary.Remove(key);
            }

            public bool TryGetValue(object key, out object value)
            {
                return Dictionary.TryGetValue(key, out value);
            }

            public object this[object key]
            {
                get => Dictionary[key];
                set => Dictionary[key] = value;
            }

            ICollection<string> IDictionary<string, object>.Keys => Dictionary.Keys.Select(c => c.ToString()).ToList();
            public ICollection<object> Keys => Dictionary.Keys;

            public ICollection<object> Values => Dictionary.Values;

            bool IFluidIndexable.TryGetValue(string name, out FluidValue value)
            {
                var result = Dictionary.TryGetValue(name, out var val);
                value = null;
                if (result)
                {
                    value = Create(val, _templateOptions);
                    return true;
                }

                if (!double.TryParse(name, out var numberValue)) return false;

                result = Dictionary.TryGetValue(numberValue, out val);
                value = null;
                if (result) value = Create(val, _templateOptions);
                return result;
            }

            int IFluidIndexable.Count => Dictionary.Count;

            IEnumerable<string> IFluidIndexable.Keys => Dictionary.Keys.Select(c => c.ToString());

            public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
            {

            }

            public override bool Equals(FluidValue other)
            {
                if (other == null)
                    return Dictionary.Count == 0;
                if (other.IsNil())
                    return Dictionary.Count == 0;
                if (other is IndexableDictionary dictionaryValue && Dictionary.Count == dictionaryValue.Dictionary.Count)
                {
                    foreach (string key in Dictionary.Keys)
                    {
                        if (!dictionaryValue.Dictionary.TryGetValue(key, out var other1))
                            return false;
                        if (!Dictionary.TryGetValue(key, out var fluidValue))
                            return false;
                        if (!fluidValue.Equals(other1))
                            return false;
                    }
                }
                return false;
            }

            public override async ValueTask<FluidValue> GetValueAsync(
                string name,
                TemplateContext context)
            {
                await Task.Yield();

                if (name == "size")
                    return NumberValue.Create(Dictionary.Count);

                return !Dictionary.TryGetValue(name, out var result) ? NilValue.Instance : (result is FluidValue value ? value : Create(result, context.Options));
            }

            protected override FluidValue GetIndex(FluidValue index, TemplateContext context)
            {
                return !Dictionary.TryGetValue(index.ToStringValue(), out var fluidValue) ? NilValue.Instance : fluidValue is FluidValue value ? value : Create(fluidValue, context.Options);
            }

            public override bool ToBooleanValue() => true;

            public override decimal ToNumberValue() => 0M;

            public override string ToStringValue() => "";

            public override object ToObjectValue() => Dictionary;

            public override FluidValues Type { get; } = FluidValues.Dictionary;

            public IEnumerator GetEnumerator() => Dictionary.GetEnumerator();
        }
    }
}
