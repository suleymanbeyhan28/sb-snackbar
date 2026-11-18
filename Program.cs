// Copyright (c) 2026 Süleyman BEYHAN
// Licensed under the MIT License. See the LICENSE file in the project root.

using System;
using System.Windows.Forms;

namespace SBNotepad.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
