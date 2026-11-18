// Copyright (c) 2026 Süleyman BEYHAN
// Licensed under the MIT License. See the LICENSE file in the project root.

using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SBNotepad.UI;

/// <summary>
/// Loads the state icons (PNG files in the Icons folder) that are embedded in the assembly.
/// </summary>
internal static class StateIcons
{
    public static Image Load(string fileName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Icons." + fileName)
            ?? throw new InvalidOperationException($"Embedded icon not found: {fileName}");
        using Image source = Image.FromStream(stream);

        // Copy into a standalone bitmap so the stream can be closed safely.
        return new Bitmap(source);
    }
}
