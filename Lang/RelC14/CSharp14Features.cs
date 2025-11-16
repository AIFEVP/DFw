using System;
using System.Collections.Generic;
using System.Linq;

public class CSharp14Features
{
    public static void Main(string[] args)
    {
        // 1. Extension Members
        Console.WriteLine("1. Extension Members");
        List<int> numbers = new List<int> { 1, 2, 3 };
        Console.WriteLine($"Is numbers empty? {numbers.IsEmpty}");
        var combined = EnumerableExtensions.Combine(numbers, new int[] { 4, 5, 6 });
        Console.WriteLine($"Combined: {string.Join(", ", combined)}");
        var identity = Enumerable.Empty<int>();
        Console.WriteLine($"Identity is empty: {identity.IsEmpty}");
        Console.WriteLine();

        // 2. The `field` Keyword
        Console.WriteLine("2. The `field` Keyword");
        var person = new PersonWithMessage();
        person.Message = "Hello";
        Console.WriteLine($"Person's message: {person.Message}");
        person.Message = null;
        Console.WriteLine($"Person's message is now null.");
        Console.WriteLine();

        // 3. Implicit Span Conversions
        Console.WriteLine("3. Implicit Span Conversions");
        int[] array = { 1, 2, 3, 4, 5 };
        Span<int> span = array;
        Console.WriteLine("Span from array:");
        for (int i = 0; i < span.Length; i++)
        {
            Console.Write($"{span[i]} ");
        }
        Console.WriteLine("\n");

        // 4. Unbound Generic Types and `nameof`
        Console.WriteLine("4. Unbound Generic Types and `nameof`");
        Console.WriteLine($"nameof(List<>) = {nameof(List<>)}");
        Console.WriteLine();

        // 5. Simple Lambda Parameters with Modifiers
        Console.WriteLine("5. Simple Lambda Parameters with Modifiers");
        TryParse<int> parse = (text, out result) => int.TryParse(text, out result);
        if (parse("123", out int val))
        {
            Console.WriteLine($"Parsed value: {val}");
        }
        Console.WriteLine();

        // 6. More Partial Members
        Console.WriteLine("6. More Partial Members");
        var partial = new PartialClass("test");
        partial.MyEvent += (sender, e) => Console.WriteLine("MyEvent triggered!");
        partial.RaiseEvent();
        Console.WriteLine();

        // 7. User Defined Compound Assignment
        Console.WriteLine("7. User Defined Compound Assignment");
        var point = new Point(1, 2);
        point += new Point(3, 4);
        Console.WriteLine($"Point after += : ({point.X}, {point.Y})");
        Console.WriteLine();

        // 8. Null-Conditional Assignment
        Console.WriteLine("8. Null-Conditional Assignment");
        Customer customer = null;
        customer?.Order = new Order(); // Does not throw
        Console.WriteLine("Null-conditional assignment on null object did not throw.");
        customer = new Customer();
        customer.Order = new Order();
        Console.WriteLine("Order assigned to non-null customer.");
    }
}

// 1. Extension Members
public static class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        public bool IsEmpty => !source.Any();
    }

    extension<TSource>(IEnumerable<TSource>)
    {
        public static IEnumerable<TSource> Combine(IEnumerable<TSource> first, IEnumerable<TSource> second) => first.Concat(second);
        public static IEnumerable<TSource> Identity => Enumerable.Empty<TSource>();
    }
}

// 2. The `field` Keyword
public partial class PersonWithMessage
{
    public string? Message
    {
        get;
        set => field = value;
    }
}

// 5. Simple Lambda Parameters with Modifiers
delegate bool TryParse<T>(string text, out T result);

// 6. More Partial Members
public partial class PartialClass
{
    public partial PartialClass(string name);
    public partial event EventHandler MyEvent;

    public void RaiseEvent()
    {
        _myEvent?.Invoke(this, EventArgs.Empty);
    }
}

public partial class PartialClass
{
    public partial PartialClass(string name)
    {
        Console.WriteLine($"Partial constructor implemented with name: {name}");
    }

    private event EventHandler? _myEvent;
    public partial event EventHandler? MyEvent
    {
        add
        {
            Console.WriteLine("Add accessor for MyEvent");
            _myEvent += value;
        }
        remove
        {
            Console.WriteLine("Remove accessor for MyEvent");
            _myEvent -= value;
        }
    }
}

// 7. User Defined Compound Assignment
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point operator +(Point left, Point right)
    {
        return new Point(left.X + right.X, left.Y + right.Y);
    }
}

// 8. Null-Conditional Assignment
public class Customer
{
    public Order? Order { get; set; }
}

public class Order { }
