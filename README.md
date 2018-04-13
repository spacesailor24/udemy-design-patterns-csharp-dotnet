# Design Patterns C# and .NET

[**Udemy Course Link**](https://www.udemy.com/design-patterns-csharp-dotnet/learn/v4/content) - https://www.udemy.com/design-patterns-csharp-dotnet/learn/v4/content

## Table of Contents

Lecture Topic | Link
--- | ---
General Notes | [General Notes](#general-notes)
**SECTION 1** | [**Section 1**](#section-1)
Single Responsibility Principle | [Lecture 3](#section-1-lecture-3)

## General Notes

<!-- ################################################################################################################ -->
<!--                                                     SECTION 1                                                    -->
<!-- ################################################################################################################ -->

## SECTION 1

### Section 1 Lecture 3

#### Single Responsibility Principle

- States that every `module` or `class` should have **responsibility** over a **single** part of the functionality provided by the program e.g.:

```csharp
public class
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace singleResponsibilityPrinciple
{
    // Stores and Removes Journal Entries
    public class Journal
    {
        private readonly List<string> entries = new List<string>();

        private static int count = 0;

        // Adds Journal entries
        public int AddEntry(string text)
        {
            entries.Add($"{++count}: {text}");
            return count; // Memento Design Pattern!
        }

        // Removes Journal entries
        // **NOTE** Bad design pattern, because removing by index will cause all
        // other existing entries' indexes to be updated
        public void RemoveEntry(int index)
        {
            entries.RemoveAt(index);
        }

        // Overrides existing ToString methods to add formatting
        public override string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }

        // **NOTE** The following methods in this class break the Single Responsibility Principle
        // because they now provide additional responsibilities to the class Journal beyond
        // just adding and removing entries

        public void Save(string filename, bool overwrite = false)
        {
            // Saves to filename
        }

        public void Load(string filename)
        {
            // Loads filename
        }
    }

    // **NOTE** Saveing and Loading SHOULD be put into their own class that will have the sole responsibility of reading and // writing to files

    // Manages Writing of Journal file
    public class Persistence
    {
        // Saves journal using filename with option to overwrite filename if overwrite = true
        public void SaveEntryToJournal(Journal journal, string filename, bool overwrite = false)
        {
            if (overwrite || !File.Exists(filename))
                File.WriteAllText(filename, journal.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var j = new Journal();
            j.AddEntry("Hello World!");
            j.AddEntry("Goodbye World!");

            var p = new Persistence();
            var filename = @"journal.txt";

            p.SaveEntryToJournal(j, filename);
            Process.Start(filename);
        }
    }
}
```