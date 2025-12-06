# Advent of Code

This repository contains my solutions for [Advent Of Code](https://adventofcode.com/).

Beware, inside are spoilers, so I'd recommend trying to solve them yourself before checking other's implementations. Ultimately the goal is to have fun and learn some new things!

The implementations are written in C#, and I have updated all scripts to use .NET's [file based applications](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/overview#file-based-apps). These can be run directly without the need for a project file:
```shell
dotnet run 01.cs
```

Additionally, I have created a number of helper classes for common tasks, importing and loading input files as well as timing and asserting the expected results. These are combined into a helper assembly in the `Helpers` folder.
