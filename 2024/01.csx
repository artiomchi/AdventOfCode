#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
#r "nuget: System.Numerics.Tensors, 9.0.0"
using FluentAssertions;
using FluentAssertions.Execution;
using System.Buffers;
using System.Numerics.Tensors;

var input = ReadInputLines("01.real.txt")
    .Select(line => line.Trim()
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => Convert.ToInt32(s))
        .ToArray())
    .ToArray();
int[] list1 = new int[input.Length], list2 = new int[input.Length];
foreach (var (i, values) in input.Index())
{
    list1[i] = values[0];
    list2[i] = values[1];
}
list1 = list1.Order().ToArray();
list2 = list2.Order().ToArray();

// Part 1
var distance = Enumerable.Range(0, list1.Length)
    .Aggregate(
        0, 
        (s, i) => s + Math.Abs(list1[i] - list2[i]))
    .Dump("distance");

// Part 1 v2 using Tensors
using (var result = MemoryPool<int>.Shared.Rent(list1.Length))
{
    TensorPrimitives.Subtract<int>(list1, list2, result.Memory.Span);
    TensorPrimitives.Abs<int>(result.Memory.Span, result.Memory.Span);
    distance = TensorPrimitives.Sum<int>(result.Memory.Span);
}

// Part 2
var similarity = Enumerable.Range(0, list1.Length)
    .Aggregate(
        0, 
        (s, i) => s + list1[i] * list2.Count(j => list1[i] == j))
    .Dump("similarity");

// Assert
using (var _ = new AssertionScope())
{
    distance.Should().BeOneOf(11, 1830467);
    similarity.Should().BeOneOf(31, 26674158);
}
