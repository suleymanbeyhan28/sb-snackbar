// Copyright (c) 2026 Süleyman BEYHAN
// Licensed under the MIT License. See the LICENSE file in the project root.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace SBNotepad.UI;

#region Notification_States_Enum
/// <summary>
/// Specifies the visual state and theme color of the snackbar.
/// </summary>
public enum NotificationState
{
    /// <summary>
    /// Blue theme for informational messages.
    /// </summary>
    Info,

    /// <summary>
    /// Green theme for successful operations.
    /// </summary>
    Success,

    /// <summary>
    /// Orange theme for situations that need attention.
    /// </summary>
    Warning,

    /// <summary>
    /// Red theme for failed operations.
    /// </summary>
    Error
}
#endregion

/// <summary>
/// Modern, animated, DPI-aware snackbar (notification bar) component.
/// </summary>
public class Snackbar : IDisposable
{
    #region Fields_And_Components
    private readonly Form _hostForm;
    private readonly Panel _panel;
    private readonly PictureBox _iconBox;

    private readonly Timer _animationTimer;
    private readonly Timer _displayTimer;

    private readonly EventHandler _formResizeHandler;
    private readonly DpiChangedEventHandler _formDpiChangedHandler;
    private readonly FormClosedEventHandler _formClosedHandler;
    private readonly EventHandler _formDisposedHandler;

    private bool _disposed;

    private float _opacity = 0f;
    private bool _animatingIn = false;
    private bool _animatingOut = false;
    private int _displayDuration = 3000;

    private Color _backgroundColor = Color.FromArgb(33, 150, 243);
    private int _targetY;

    private readonly Dictionary<NotificationState, Image> _iconCache = [];

    private float DpiScale => _hostForm.DeviceDpi / 96f;

    internal const int BasePanelHeight = 60;
    internal const int BasePanelSideMargin = 20;
    internal const int BasePanelBottomMargin = 20;
    internal const int BasePanelCornerRadius = 8;
    internal const int BaseLabelLeftPadding = 50;
    internal const int BaseLabelRightPadding = 20;
    internal const int BaseLabelVerticalPadding = 4;
    internal const float BaseFontPixels = 14.67f;

    internal const bool CenterTextVertically = true;

    private const TextFormatFlags TextFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.Left | TextFormatFlags.Top;

    private float _lastDpi = -1f;
    private Font? _font;
    private string _fullMessage = string.Empty;
    private string[] _visibleLines = [string.Empty];
    private int _lineHeight;
    private Color _textColor = Color.White;
    #endregion

    #region Double_Buffered_Panel_Class
    private sealed class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the Snackbar class.
    /// </summary>
    /// <param name="hostForm">The host form the snackbar is drawn on.</param>
    public Snackbar(Form hostForm)
    {
        _hostForm = hostForm ?? throw new ArgumentNullException(nameof(hostForm));

        float dpi = DpiScale;
        int scaledPanelHeight = ScaledPanelHeight(dpi);
        int scaledSideMargin = (int)(BasePanelSideMargin * dpi);

        _panel = new DoubleBufferedPanel
        {
            Height = scaledPanelHeight,
            Width = _hostForm.ClientSize.Width - 2 * scaledSideMargin,
            Left = scaledSideMargin,
            Top = _hostForm.ClientSize.Height,
            BackColor = _backgroundColor,
            Visible = false
        };

        _iconBox = new PictureBox
        {
            Size = new Size((int)(24 * dpi), (int)(24 * dpi)),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent
        };

        _font = new Font("Segoe UI Semibold", BaseFontPixels * dpi, FontStyle.Regular, GraphicsUnit.Pixel);
        _lastDpi = dpi;

        _panel.Controls.Add(_iconBox);
        _panel.Paint += Panel_Paint;
        _hostForm.Controls.Add(_panel);

        ApplyDpiLayout();

        _animationTimer = new Timer { Interval = 15 };
        _animationTimer.Tick += AnimationTimer_Tick;

        _displayTimer = new Timer();
        _displayTimer.Tick += (s, e) =>
        {
            _displayTimer.Stop();
            _animatingOut = true;
            _animationTimer.Start();
        };

        _formResizeHandler = (s, e) => ApplyDpiLayout();
        _formDpiChangedHandler = (s, e) =>
        {
            if (_hostForm == null || _hostForm.IsDisposed || _hostForm.Disposing)
                return;

            if (_hostForm.IsHandleCreated)
            {
                _hostForm.BeginInvoke(ApplyDpiLayout);
            }
            else
            {
                ApplyDpiLayout();
            }
        };
        _formClosedHandler = (s, e) => Dispose();
        _formDisposedHandler = (s, e) => Dispose();

        _hostForm.Resize += _formResizeHandler;
        _hostForm.DpiChanged += _formDpiChangedHandler;
        _hostForm.FormClosed += _formClosedHandler;
        _hostForm.Disposed += _formDisposedHandler;

        _iconCache[NotificationState.Success] = StateIcons.Load("success.png");
        _iconCache[NotificationState.Error] = StateIcons.Load("error.png");
        _iconCache[NotificationState.Warning] = StateIcons.Load("warning.png");
        _iconCache[NotificationState.Info] = StateIcons.Load("info.png");
    }
    #endregion

    #region Public_API_Show_Notification
    /// <summary>
    /// Shows the notification on screen with an animation.
    /// </summary>
    /// <param name="message">The notification text to show.</param>
    /// <param name="state">The notification type (Success, Error, Warning, Info).</param>
    /// <param name="displayDuration">How long the bar stays visible, in milliseconds.</param>
    public void Show(string message, NotificationState state = NotificationState.Info, int displayDuration = 3000)
    {
        if (_disposed || _hostForm == null || _hostForm.IsDisposed || _hostForm.Disposing)
            return;

        if (_hostForm.InvokeRequired)
        {
            if (_hostForm.IsHandleCreated)
            {
                _hostForm.BeginInvoke(() => Show(message, state, displayDuration));
            }
            return;
        }

        _displayTimer.Stop();
        _animationTimer.Stop();

        _displayDuration = displayDuration;
        _fullMessage = message;

        switch (state)
        {
            case NotificationState.Success:
                _backgroundColor = Color.FromArgb(46, 125, 50);
                _iconBox.Image = _iconCache[NotificationState.Success];
                PlaySound(SystemSounds.Asterisk);
                break;

            case NotificationState.Error:
                _backgroundColor = Color.FromArgb(160, 35, 35);
                _iconBox.Image = _iconCache[NotificationState.Error];
                PlaySound(SystemSounds.Hand);
                break;

            case NotificationState.Warning:
                _backgroundColor = Color.FromArgb(190, 98, 0);
                _iconBox.Image = _iconCache[NotificationState.Warning];
                PlaySound(SystemSounds.Exclamation);
                break;

            case NotificationState.Info:
            default:
                _backgroundColor = Color.FromArgb(21, 101, 192);
                _iconBox.Image = _iconCache[NotificationState.Info];
                PlaySound(SystemSounds.Beep);
                break;
        }

        _panel.BackColor = _backgroundColor;

        ApplyDpiLayout();

        _opacity = 0f;
        _panel.Visible = true;
        _panel.BringToFront();

        _animatingIn = true;
        _animatingOut = false;

        _animationTimer.Start();
    }

    /// <summary>
    /// Tries to play a system sound safely; continues silently if no audio device exists or an error occurs.
    /// </summary>
    private static void PlaySound(SystemSound sound)
    {
        try
        {
            sound.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Snackbar] Could not play system sound: {ex.Message}");
        }
    }
    #endregion

    #region DPI_Adaptation_And_Sizing
    private void ApplyDpiLayout()
    {
        if (_disposed || _hostForm == null || _hostForm.IsDisposed || _hostForm.Disposing || _hostForm.WindowState == FormWindowState.Minimized)
            return;

        float dpi = DpiScale;
        if (!IsLayoutApplicable(_hostForm.ClientSize, dpi))
            return;

        bool fontChanged = false;
        if (Math.Abs(dpi - _lastDpi) > 0.001f || _font == null)
        {
            Font newFont = new("Segoe UI Semibold", BaseFontPixels * dpi, FontStyle.Regular, GraphicsUnit.Pixel);
            Font? oldFont = _font;
            _font = newFont;
            oldFont?.Dispose();
            _lastDpi = dpi;
            fontChanged = true;
        }

        int iconSize = (int)(24 * dpi);
        _iconBox.Size = new Size(iconSize, iconSize);

        int width = PanelWidth(_hostForm.ClientSize.Width, dpi);
        int height = ScaledPanelHeight(dpi);
        int leftMargin = (int)(BasePanelSideMargin * dpi);

        bool sizeChanged = (_panel.Width != width || _panel.Height != height);
        _panel.Width = width;
        _panel.Height = height;
        _panel.Left = leftMargin;

        int iconLeftMargin = (int)(16 * dpi);
        _iconBox.Location = new Point(iconLeftMargin, (height - _iconBox.Height) / 2);

        if (sizeChanged || _panel.Region == null)
        {
            Region? oldRegion = _panel.Region;
            _panel.Region = CreateRoundedRegion(width, height, (int)(BasePanelCornerRadius * dpi));
            oldRegion?.Dispose();
        }

        _targetY = TargetY(_hostForm.ClientSize.Height, height, dpi);

        if (!_panel.Visible)
        {
            _panel.Top = _hostForm.ClientSize.Height;
        }
        else if (!_animatingIn && !_animatingOut)
        {
            _panel.Top = _targetY;
        }

        int textLeftPadding = (int)(BaseLabelLeftPadding * dpi);
        int textWidth = Math.Max(1, width - textLeftPadding - (int)(BaseLabelRightPadding * dpi));
        int measureLimit = Math.Max(1, textWidth - (int)(2 * dpi));

        if (fontChanged || _lineHeight == 0)
        {
            _lineHeight = TextRenderer.MeasureText("Ag", _font, new Size(int.MaxValue, int.MaxValue), TextFlags).Height;
        }

        int maxLines = MaxLineCount(LabelHeight(dpi), _lineHeight);
        string[] newLines = CalculateVisibleLines(
            _fullMessage,
            s => TextRenderer.MeasureText(s, _font, new Size(int.MaxValue, int.MaxValue), TextFlags).Width,
            measureLimit,
            maxLines);

        if (!_visibleLines.SequenceEqual(newLines))
        {
            _visibleLines = newLines;
            _panel.Invalidate();
        }
    }
    #endregion

    #region Panel_Painting
    private void Panel_Paint(object? sender, PaintEventArgs e)
    {
        if (_visibleLines == null || _visibleLines.Length == 0 || _font == null)
            return;

        float dpi = DpiScale;
        int labelHeight = LabelHeight(dpi);
        int textLeftPadding = (int)(BaseLabelLeftPadding * dpi);
        int textWidth = Math.Max(1, _panel.Width - textLeftPadding - (int)(BaseLabelRightPadding * dpi));
        int topY = (int)(BaseLabelVerticalPadding * dpi) + TextBlockTopY(labelHeight, _lineHeight, _visibleLines.Length, CenterTextVertically);

        for (int i = 0; i < _visibleLines.Length; i++)
        {
            Rectangle rect = new(textLeftPadding, topY + i * _lineHeight, textWidth, _lineHeight);
            TextRenderer.DrawText(e.Graphics, _visibleLines[i], _font, rect, _textColor, TextFlags);
        }
    }
    #endregion

    #region Geometry_And_Layout_Calculation_Methods
    internal static int PanelWidth(int clientWidth, float dpi)
    {
        return Math.Max(0, clientWidth - 2 * (int)(BasePanelSideMargin * dpi));
    }

    internal static int ScaledPanelHeight(float dpi)
    {
        return (int)(BasePanelHeight * dpi);
    }

    internal static int LabelHeight(float dpi)
    {
        return ScaledPanelHeight(dpi) - 2 * (int)(BaseLabelVerticalPadding * dpi);
    }

    internal static int TargetY(int clientHeight, int panelHeight, float dpi)
    {
        return Math.Max(0, clientHeight - panelHeight - (int)(BasePanelBottomMargin * dpi));
    }

    internal static bool IsLayoutApplicable(Size client, float dpi)
    {
        return client.Width >= (int)(150 * dpi) && client.Height >= (int)(100 * dpi);
    }

    internal static int MaxLineCount(int labelHeight, int lineHeight)
    {
        return Math.Max(1, labelHeight / Math.Max(1, lineHeight));
    }

    internal static int TextBlockTopY(int labelHeight, int lineHeight, int lineCount, bool center)
    {
        return center ? Math.Max(0, (labelHeight - lineCount * lineHeight) / 2) : 0;
    }

    internal static string[] CalculateVisibleLines(string? message, Func<string, int> widthMeasurer, int maxWidth, int maxLines)
    {
        if (maxWidth < 1) maxWidth = 1;
        if (maxLines < 1) maxLines = 1;

        if (string.IsNullOrEmpty(message))
        {
            return [string.Empty];
        }

        string normalized = message.Replace("\r\n", "\n").Replace('\r', '\n');
        string[] rawLines = normalized.Split('\n');
        List<string> validLines = [];

        foreach (string s in rawLines)
        {
            string t = s.Trim();
            if (!string.IsNullOrEmpty(t))
            {
                validLines.Add(t);
            }
        }

        if (validLines.Count == 0)
        {
            return [string.Empty];
        }

        if (validLines.Count >= 2)
        {
            int take = Math.Min(validLines.Count, maxLines);
            string[] result = new string[take];

            for (int i = 0; i < take; i++)
            {
                if (i < take - 1)
                {
                    result[i] = FitSingleLine(validLines[i], widthMeasurer, maxWidth);
                }
                else
                {
                    if (validLines.Count > take)
                    {
                        result[i] = FitSingleLine(validLines[i] + "...", widthMeasurer, maxWidth);
                    }
                    else
                    {
                        result[i] = FitSingleLine(validLines[i], widthMeasurer, maxWidth);
                    }
                }
            }

            return result;
        }

        return FitWrapped(validLines[0], widthMeasurer, maxWidth, maxLines);
    }

    private static string FitSingleLine(string s, Func<string, int> widthMeasurer, int maxWidth)
    {
        if (widthMeasurer(s) <= maxWidth)
        {
            return s;
        }

        int lo = 0;
        int hi = s.Length;
        int largestN = -1;

        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (widthMeasurer(Truncate(s, mid)) <= maxWidth)
            {
                largestN = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }

        if (largestN < 0)
        {
            return Truncate(s, 0);
        }

        return Truncate(s, largestN);
    }

    private static string Truncate(string m, int n)
    {
        if (string.IsNullOrEmpty(m)) return "...";
        n = Math.Clamp(n, 0, m.Length);
        if (n > 0 && char.IsHighSurrogate(m[n - 1]))
        {
            n--;
        }

        return m[..n].TrimEnd() + "...";
    }

    private static string[] FitWrapped(string s, Func<string, int> widthMeasurer, int maxWidth, int maxLines)
    {
        List<string> lines = [];
        string rest = s;

        for (int i = 0; i < maxLines; i++)
        {
            if (string.IsNullOrEmpty(rest))
            {
                break;
            }

            if (i == maxLines - 1)
            {
                lines.Add(FitSingleLine(rest, widthMeasurer, maxWidth));
                break;
            }

            if (widthMeasurer(rest) <= maxWidth)
            {
                lines.Add(rest);
                break;
            }

            int lo = 1;
            int hi = rest.Length;
            int k = 1;

            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                if (widthMeasurer(rest[..mid]) <= maxWidth)
                {
                    k = mid;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            int cut = k;
            if (k < rest.Length && rest[k] != ' ')
            {
                int lastSpace = rest[..k].LastIndexOf(' ');
                if (lastSpace > 0)
                {
                    cut = lastSpace;
                }
            }

            if (cut > 0 && char.IsHighSurrogate(rest[cut - 1]))
            {
                cut--;
            }

            lines.Add(rest[..cut].TrimEnd());
            rest = rest[cut..].TrimStart();
        }

        return lines.Count > 0 ? [.. lines] : [string.Empty];
    }
    #endregion

    #region Rounded_Corners
    private static Region CreateRoundedRegion(int width, int height, int radius)
    {
        if (width <= 0 || height <= 0)
        {
            return new Region(Rectangle.Empty);
        }

        int effectiveRadius = Math.Min(radius, Math.Min(width, height));
        if (effectiveRadius < 2)
        {
            return new Region(new Rectangle(0, 0, width, height));
        }

        using GraphicsPath path = new();
        path.AddArc(0, 0, effectiveRadius, effectiveRadius, 180, 90);
        path.AddArc(width - effectiveRadius, 0, effectiveRadius, effectiveRadius, 270, 90);
        path.AddArc(width - effectiveRadius, height - effectiveRadius, effectiveRadius, effectiveRadius, 0, 90);
        path.AddArc(0, height - effectiveRadius, effectiveRadius, effectiveRadius, 90, 90);
        path.CloseFigure();
        return new Region(path);
    }
    #endregion

    #region Animation_Tick_Handler
    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        if (_disposed || _hostForm == null || _hostForm.IsDisposed || _hostForm.Disposing || !_hostForm.IsHandleCreated)
        {
            _animationTimer.Stop();
            _displayTimer.Stop();
            return;
        }

        int step = (int)(4 * DpiScale);

        if (_animatingIn)
        {
            _opacity += (1f - _opacity) * 0.12f;

            if (_panel.Top > _targetY)
                _panel.Top = Math.Max(_panel.Top - step, _targetY);
            else if (_panel.Top < _targetY)
                _panel.Top = _targetY;

            if (_opacity >= 0.98f && _panel.Top <= _targetY)
            {
                _animatingIn = false;
                _opacity = 1f;
                _panel.Top = _targetY;
                _displayTimer.Interval = _displayDuration;
                _displayTimer.Start();
                _animationTimer.Stop();
            }
        }
        else if (_animatingOut)
        {
            _opacity -= _opacity * 0.12f;

            if (_panel.Top < _hostForm.ClientSize.Height)
                _panel.Top = Math.Min(_panel.Top + step, _hostForm.ClientSize.Height);

            if (_opacity <= 0.02f)
            {
                _animatingOut = false;
                _opacity = 0f;
                _panel.Visible = false;
                _animationTimer.Stop();
            }
        }

        int alpha = Math.Max(0, Math.Min(255, (int)(_opacity * 255)));
        _panel.BackColor = Color.FromArgb(alpha, _backgroundColor);
        Color newTextColor = Color.FromArgb(alpha, Color.White);
        if (_textColor != newTextColor)
        {
            _textColor = newTextColor;
            _panel.Invalidate();
        }
    }
    #endregion

    #region IDisposable_Implementation
    /// <summary>
    /// Releases the managed and unmanaged resources used by the snackbar.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Stops the timers, unsubscribes from events and cleans up graphics objects when the object is disposed.
    /// </summary>
    /// <param name="disposing">Whether managed resources are being released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _displayTimer.Stop();
            _animationTimer.Stop();
            _displayTimer.Dispose();
            _animationTimer.Dispose();

            if (_hostForm != null)
            {
                _hostForm.Resize -= _formResizeHandler;
                _hostForm.DpiChanged -= _formDpiChangedHandler;
                _hostForm.FormClosed -= _formClosedHandler;
                _hostForm.Disposed -= _formDisposedHandler;
            }

            _font?.Dispose();
            _font = null;

            if (_panel != null)
            {
                _panel.Paint -= Panel_Paint;
                _panel.Region?.Dispose();
                _panel.Region = null;
                _panel.Dispose();
            }

            _iconBox?.Dispose();
        }

        _disposed = true;
    }
    #endregion
}
