using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace TestPerformance;

public sealed class TestRow
{
    public TestRow(object[] arguments)
    {
        this.Arguments = arguments;
    }

    public object[] Arguments { get; }

    public override string ToString() => string.Join(", ", this.Arguments.Select(a => a?.ToString() ?? "null"));
}

public static class TestInvoker
{
    public static MethodInfo Method(Type type, string name, int parameterCount)
        => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(m => m.Name == name && m.GetParameters().Length == parameterCount);

    public static Task Invoke(MethodInfo method, object instance, object[] arguments)
    {
        try
        {
            return method.Invoke(instance, arguments) as Task ?? Task.CompletedTask;
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    public static IEnumerable<TestRow> Data(MethodInfo method)
    {
        var count = 0;
        var parameters = method.GetParameters();
        foreach (var attribute in method.GetCustomAttributes<DataAttribute>(true))
        {
            foreach (var arguments in attribute.GetData(method))
            {
                count++;
                var supplied = arguments ?? new object[] { null! };
                if (supplied.Length > parameters.Length)
                {
                    throw new InvalidOperationException($"Too many arguments for {method.Name}");
                }

                var normalized = new object[parameters.Length];
                Array.Copy(supplied, normalized, supplied.Length);
                for (var i = supplied.Length; i < normalized.Length; i++)
                {
                    if (!parameters[i].HasDefaultValue)
                    {
                        throw new InvalidOperationException($"Missing argument {parameters[i].Name} for {method.Name}");
                    }

                    normalized[i] = parameters[i].DefaultValue;
                }

                yield return new TestRow(normalized);
            }
        }

        if (count == 0)
        {
            throw new InvalidOperationException($"No data for {method.DeclaringType}.{method.Name}");
        }
    }

    public static T Argument<T>(object value)
    {
        if (value is T result)
        {
            return result;
        }

        if (value == null)
        {
            return default!;
        }

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)(type.IsEnum ? Enum.ToObject(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
    }

    public static async Task Run<T>(MethodInfo method, Func<T, Task> testBody)
        where T : new()
    {
        var instance = new T();
        var hooks = typeof(T).GetCustomAttributes<BeforeAfterTestAttribute>(true)
            .Concat(method.GetCustomAttributes<BeforeAfterTestAttribute>(true)).ToArray();
        var entered = new Stack<BeforeAfterTestAttribute>();
        try
        {
            if (instance is IAsyncLifetime lifetime)
            {
                await lifetime.InitializeAsync();
            }

            foreach (var hook in hooks)
            {
                hook.Before(method);
                entered.Push(hook);
            }

            await testBody(instance);
        }
        finally
        {
            try
            {
                foreach (var hook in entered)
                {
                    hook.After(method);
                }
            }
            finally
            {
                if (instance is IAsyncLifetime lifetime)
                {
                    await lifetime.DisposeAsync();
                }

                (instance as IDisposable)?.Dispose();
            }
        }
    }
}
