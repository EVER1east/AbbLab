# AbbLab

This is a series of libraries, that I'm doing mainly for myself.

### AbbLab.SemanticVersioning

This library provides classes, structures and methods to create, modify, compare and whatever semantic versions, strictly adhering to the [v2.0.0 specification](https://semver.org). At the moment it performs several times better than the alternatives (yep, I love optimizing stuff).

Loose parsing mode provides quite a lot of options, like allowing leading, trailing and inner whitespace, leading zeroes, omitting minor and/or patch version components, unseparated pre-release syntax and etc.

There's also a `SemanticVersionBuilder`, that you can use to create semantic versions step-by-step or modify existing versions without allocating memory for every single step of the transformation. Also provides `Implement` methods.

Planning on implementing version ranges (partial versions, partial version components, comparators and etc.) from [`node-semver`](https://github.com/npm/node-semver).

### AbbLab.Extensions

This library adds some useful utility and extension methods:

- `ArrayExtensions` - adds methods from the `System.Array` type as extensions, plus adds `Contains` and `Cast`;
- `EnumerableExtensions` - adds `WithMin<T>`, `WithMax<T>` and `WithDistinct<T>`;
- `ReadOnlyCollection.Empty<T>()` - works similarly to `Array.Empty<T>()`;
- `TypeExtensions` - adds methods for searching both public and non-public fields/properties/methods/constructors and also `MakeInstance()` that initializes an instance using the type's parameterless constructor.

### AbbLab.Parsing

This library adds a `StringParser` structure, that you can use to parse various strings.

`StringParser` is allocated on-stack, and, when used to loosely parse semantic versions, is just 20% slower than storing the string's length and parser's position directly on the stack (as local variables) and using for-loops and a lot of if statements to conditionally read sequences of characters.

```cs
StringParser parser = new StringParser(text);
if (parser.SkipString("Hello") || parser.SkipString("Hi"))
{
    char punctuation = parser.SkipAny(',', '.', ':', out char read) ? read : default;

    parser.SkipWhitespaces();
    static bool WordEnd(int c) => c == ' ';
    ReadOnlySpan<char> nameSpan = parser.ReadUntil(&WordEnd);
    // Note: Uses unsafe function pointers, to avoid allocating extra memory for predicates
    
    bool exclamation = false;
    if (parser.Skip('!'))
        exclamation = true;
    
    ShowGreeting(new string(nameSpan), punctuation, exclamation);
}
```
