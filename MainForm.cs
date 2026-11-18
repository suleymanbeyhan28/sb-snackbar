// Copyright (c) 2026 Süleyman BEYHAN
// Licensed under the MIT License. See the LICENSE file in the project root.

using System;
using System.Windows.Forms;

namespace SBNotepad.UI;

public partial class MainForm : Form
{
    private readonly Snackbar _snackbar;

    public MainForm()
    {
        InitializeComponent();
        _snackbar = new Snackbar(this);
    }

    private void buttonSuccess_Click(object? sender, EventArgs e)
        => _snackbar.Show("File saved successfully.", NotificationState.Success);

    private void buttonError_Click(object? sender, EventArgs e)
        => _snackbar.Show("Something went wrong. The file could not be saved.", NotificationState.Error);

    private void buttonInfo_Click(object? sender, EventArgs e)
        => _snackbar.Show("A new version is available.", NotificationState.Info);

    private void buttonWarning_Click(object? sender, EventArgs e)
        => _snackbar.Show("Your changes have not been saved yet.", NotificationState.Warning);
}
