<div align="center">

# SB Windows Forms Snackbar

Modern & Animated Notification Component for Windows Forms

Replace standard, blocking message boxes with a sleek, DPI-aware, and non-intrusive Snackbar UI that smoothly animates from the bottom of your forms.

<br>

<div align="center">

<img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=sharp&logoColor=white" />  
<img src="https://img.shields.io/badge/.NET-WinForms-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />  
<img src="https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white" />  
<img src="https://img.shields.io/badge/License-MIT-2ea44f?style=for-the-badge&logo=opensourceinitiative&logoColor=white" />

</div>

</div>

<br>

---

## 🔔 Why SB WinForms Snackbar?

Standard Windows Forms `MessageBox` dialogs pause the application's execution thread and force the user to interact with them before continuing. They are outdated and break the fluidity of modern user experiences.

**SB WinForms Snackbar** is designed to provide seamless, non-blocking feedback to the user. It integrates directly into your existing Windows Forms projects as a lightweight, single-class solution.

---

## 🚀 Features

### Visual States & Design
* ✅ **Success** – Green background with an acknowledgment sound.
* ❌ **Error** – Red background with an alert sound.
* ⚠️ **Warning** – Orange background with an exclamation sound.
* ℹ️ **Info** – Blue background with a standard beep.

### Technical Capabilities
* **Smooth Animations:** Fluid slide-in from the bottom and fade-in/fade-out effects.
* **Smart Positioning:** Recalculates its position automatically when the parent form is resized or the DPI changes.
* **Flicker-Free:** Uses a double-buffered panel for smooth rendering.
* **Multi-Line Messages:** Long text is wrapped, and anything that does not fit is truncated with an ellipsis.
* **Thread-Safe Calls:** `Show()` can be called from any thread.
* **Automatic Cleanup:** Timers and event handlers are released when the host form closes.
* **Customizable Duration:** Define exactly how many milliseconds the message should stay visible.

---

## 🖥️ System Requirements

* **Operating System:** Windows 10 / Windows 11
* **Framework:** Modern .NET with Windows Forms. The demo targets **.NET 10** (`net10.0-windows`).
* **Language:** C# 12 or later (the component uses collection expressions and file-scoped namespaces).
* **DPI Awareness:** Enable per-monitor DPI awareness (the demo sets `ApplicationHighDpiMode` to `PerMonitorV2`) for optimal scaling.

---

## 🛠️ Installation & Usage

### 1. Integration
Copy these into your WinForms project:

* `Snackbar.cs` – the component
* `StateIcons.cs` – loads the state icons
* `Icons/` – the four PNG icons (`success.png`, `error.png`, `warning.png`, `info.png`)

The icons are embedded in the assembly, so add this to your `.csproj`:

```xml
<ItemGroup>
  <EmbeddedResource Include="Icons\*.png" LogicalName="Icons.%(Filename)%(Extension)" />
</ItemGroup>
```

To use your own icons, replace the PNG files with images of the same names.

### 2. Implementation
Initialize the Snackbar in your form and trigger it via the `Show()` method:

```csharp
// Instantiate in your form's constructor or load event
Snackbar snackbar = new Snackbar(this);

// Trigger a notification
snackbar.Show("Process completed successfully.", NotificationState.Success, 3000);
```

### 3. API

```csharp
void Show(string message,
          NotificationState state = NotificationState.Info,
          int displayDuration = 3000);
```

| Parameter | Description |
|-----------|-------------|
| `message` | Text to show. Use `\n` for multiple lines. |
| `state` | `Success`, `Error`, `Warning` or `Info`. |
| `displayDuration` | How long the bar stays visible, in milliseconds. |

### 4. Running the Demo
Open `SnackbarDemo.slnx` (Visual Studio 2022 17.13+) or run `dotnet run` inside the `SnackbarDemo` folder. The demo is a single form with four buttons, one for each notification state.

---

## 📁 Project Structure

```
sb-snackbar/
├── SnackbarDemo.slnx        Solution file
├── SnackbarDemo.csproj      net10.0-windows, Windows Forms
├── Program.cs               Entry point
├── MainForm.cs              Button click handlers
├── MainForm.Designer.cs     Designer-generated layout
├── Snackbar.cs              The snackbar component
├── StateIcons.cs            Loads the embedded state icons
├── Icons/                   success.png, error.png, warning.png, info.png
├── AppIcon.ico              Application icon
└── LICENSE                  MIT License
```

> `ImplicitUsings` is disabled in the project on purpose, so `System.Windows.Forms.Timer` does not clash with `System.Threading.Timer`.

---

## ⚖️ License

This project is licensed under the [MIT License](LICENSE).

Copyright (c) 2026 Süleyman BEYHAN

You are free to use, copy, modify, merge, publish, distribute, sublicense and sell copies of this software, in personal and commercial projects, as long as the copyright notice and the license text are included in all copies or substantial portions of the software.

---

## ☕ Say Thanks

If this project was useful to you, you can support it with a Bitcoin donation.

**Bitcoin (BTC) address:**

```text
bc1qraj6jdvnz0wyge42mrtc7jr72n74cttzqukcw6
```

---

## 📩 Contact

**🌐 Website / Blog**
[https://suleymanbprojects.blogspot.com/](https://suleymanbprojects.blogspot.com)

**📧 Email**
[sbprojects.requests@gmail.com](mailto:sbprojects.requests@gmail.com)
