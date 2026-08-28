using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RightClickTools
{
    partial class Program
    {
        // Custom CheckBox with modern appearance
        public class CustomCheckBox : CheckBox
        {
            private bool isHovered = false;

            public CustomCheckBox()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
                Padding = new Padding(0);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                isHovered = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                isHovered = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                OnPaintBackground(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Calculate checkbox box position
                int checkBoxSize = (int)(16 * ScaleFactor);
                int cornerRadius = (int)(4 * ScaleFactor);
                Rectangle checkBoxRect = new Rectangle(0, (Height - checkBoxSize) / 2, checkBoxSize, checkBoxSize);

                if (Checked)
                {
                    Color fillColor = isHovered ? (Dark ? Color.FromArgb(20, 117, 169) : Color.FromArgb(0, 150, 250)) : (Dark ? Color.FromArgb(85, 196, 255) : Color.FromArgb(0, 103, 192));
                    Color borderColor = isHovered ? (Dark ? Color.FromArgb(0, 150, 250) : Color.FromArgb(0, 150, 250)) : (Dark ? Color.FromArgb(0, 150, 250) : Color.FromArgb(0, 103, 192));

                    using (GraphicsPath path = GetRoundedRectangle(checkBoxRect, cornerRadius))
                    {
                        using (SolidBrush fillBrush = new SolidBrush(fillColor))
                        {
                            e.Graphics.FillPath(fillBrush, path);
                        }
                        using (Pen borderPen = new Pen(borderColor, 1.6f))
                        {
                            e.Graphics.DrawPath(borderPen, path);
                        }
                    }

                    // Draw checkmark
                    using (Pen checkPen = new Pen((Dark ? Color.Black : Color.White), 2))
                    {
                        Point[] checkPoints = new Point[]
                        {
                            new Point(checkBoxRect.X + (int)(3 * ScaleFactor), checkBoxRect.Y + (int)(7 * ScaleFactor)),
                            new Point(checkBoxRect.X + (int)(6 * ScaleFactor), checkBoxRect.Y + (int)(11 * ScaleFactor)),
                            new Point(checkBoxRect.X + (int)(13 * ScaleFactor), checkBoxRect.Y + (int)(4 * ScaleFactor))
                        };
                        e.Graphics.DrawLines(checkPen, checkPoints);
                    }
                }
                else
                {
                    Color fillColor = isHovered ? (Dark ? Color.FromArgb(64, 64, 64) : Color.FromArgb(200, 200, 200)) : (Dark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(240, 240, 240));
                    Color borderColor = isHovered ? (Dark ? Color.FromArgb(120, 120, 120) : Color.FromArgb(140, 140, 140)) : (Dark ? Color.FromArgb(120, 120, 120) : Color.FromArgb(140, 140, 140));

                    using (GraphicsPath path = GetRoundedRectangle(checkBoxRect, cornerRadius))
                    {
                        using (SolidBrush fillBrush = new SolidBrush(fillColor))
                        {
                            e.Graphics.FillPath(fillBrush, path);
                        }
                        using (Pen borderPen = new Pen(borderColor, 1.6f))
                        {
                            e.Graphics.DrawPath(borderPen, path);
                        }
                    }
                }

                // Draw text
                int spacing = (int)(6 * ScaleFactor);
                Rectangle textRect = new Rectangle(checkBoxSize + spacing, 0, Width - checkBoxSize - spacing, Height);
                TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }

            private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int diameter = radius * 2;

                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

                path.CloseFigure();
                return path;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
            }

            protected override void OnCheckedChanged(EventArgs e)
            {
                base.OnCheckedChanged(e);
                Invalidate();
            }

            public override Size GetPreferredSize(Size proposedSize)
            {
                int checkBoxSize = (int)(16 * ScaleFactor);
                int spacing = (int)(6 * ScaleFactor);
                Size textSize = TextRenderer.MeasureText(Text, Font);
                int height = base.GetPreferredSize(proposedSize).Height;
                return new Size(checkBoxSize + spacing + textSize.Width, height);
            }
        }

        // Custom RadioButton with modern appearance and hover effect
        public class CustomRadioButton : RadioButton
        {
            private bool isHovered = false;

            public CustomRadioButton()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
                Padding = new Padding(0);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                isHovered = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                isHovered = false;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                OnPaintBackground(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                int size = (int)(16 * ScaleFactor);
                Rectangle circleRect = new Rectangle(0, (Height - size) / 2, size, size);

                if (Checked)
                {
                    Color fillColor = isHovered ? (Dark ? Color.FromArgb(20, 117, 169) : Color.FromArgb(0, 150, 250)) : (Dark ? Color.FromArgb(85, 196, 255) : Color.FromArgb(0, 103, 192));
                    Color borderColor = isHovered ? (Dark ? Color.FromArgb(0, 150, 250) : Color.FromArgb(0, 150, 250)) : (Dark ? Color.FromArgb(0, 150, 250) : Color.FromArgb(0, 103, 192));

                    e.Graphics.FillEllipse(new SolidBrush(fillColor), circleRect);
                    e.Graphics.DrawEllipse(new Pen(borderColor, 1.6f), circleRect);

                    // Draw inner dot
                    int dotSize = (int)(6 * ScaleFactor);
                    Rectangle dotRect = new Rectangle(
                        circleRect.X + (size - dotSize) / 2,
                        circleRect.Y + (size - dotSize) / 2,
                        dotSize, dotSize);
                    e.Graphics.FillEllipse(new SolidBrush(Dark ? Color.Black : Color.White), dotRect);
                }
                else
                {
                    Color fillColor = isHovered ? (Dark ? Color.FromArgb(64, 64, 64) : Color.FromArgb(200, 200, 200)) : (Dark ? Color.FromArgb(32, 32, 32) : Color.FromArgb(240, 240, 240));
                    Color borderColor = isHovered ? (Dark ? Color.FromArgb(120, 120, 120) : Color.FromArgb(100, 100, 100)) : (Dark ? Color.FromArgb(120, 120, 120) : Color.FromArgb(140, 140, 140));

                    e.Graphics.FillEllipse(new SolidBrush(fillColor), circleRect);
                    e.Graphics.DrawEllipse(new Pen(borderColor, 1.6f), circleRect);
                }

                // Draw text
                int spacing = (int)(6 * ScaleFactor);
                Rectangle textRect = new Rectangle(size + spacing, 0, Width - size - spacing, Height);
                TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }

            protected override void OnCheckedChanged(EventArgs e)
            {
                base.OnCheckedChanged(e);
                Invalidate();
            }

            public override Size GetPreferredSize(Size proposedSize)
            {
                int size = (int)(16 * ScaleFactor);
                int spacing = (int)(6 * ScaleFactor);
                Size textSize = TextRenderer.MeasureText(Text, Font);
                int height = base.GetPreferredSize(proposedSize).Height;
                return new Size(size + spacing + textSize.Width, height);
            }
        }
        public class CustomGroupBox : GroupBox
        {
            private Color _borderColor = Color.Gray;
            private Color _titleColor = Color.Black;

            public Color BorderColor
            {
                get { return _borderColor; }
                set
                {
                    _borderColor = value;
                    Invalidate();
                }
            }

            public Color TitleColor
            {
                get { return _titleColor; }
                set
                {
                    _titleColor = value;
                    Invalidate();
                }
            }

            public CustomGroupBox()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.ResizeRedraw | ControlStyles.ContainerControl, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Clear background
                e.Graphics.Clear(BackColor);

                // Measure title text
                SizeF titleSize = e.Graphics.MeasureString(Text, Font);
                int titleHeight = (int)titleSize.Height;
                int titleWidth = (int)titleSize.Width;

                // Calculate border rectangle (starts below the title)
                int borderTop = titleHeight / 2;
                Rectangle borderRect = new Rectangle(
                    0,
                    borderTop,
                    Width - 1,
                    Height - borderTop - 1
                );

                // Draw border with gap for title
                using (Pen borderPen = new Pen(_borderColor, 1))
                {
                    int titleStart = (int)(8 * ScaleFactor);
                    int titleEnd = titleStart + titleWidth + (int)(4 * ScaleFactor);

                    // Top line (with gap for title)
                    e.Graphics.DrawLine(borderPen, 0, borderTop, titleStart, borderTop);
                    e.Graphics.DrawLine(borderPen, titleEnd, borderTop, Width - 1, borderTop);

                    // Other sides
                    e.Graphics.DrawLine(borderPen, 0, borderTop, 0, Height - 1); // Left
                    e.Graphics.DrawLine(borderPen, Width - 1, borderTop, Width - 1, Height - 1); // Right
                    e.Graphics.DrawLine(borderPen, 0, Height - 1, Width - 1, Height - 1); // Bottom
                }

                // Draw title text
                if (!string.IsNullOrEmpty(Text))
                {
                    using (SolidBrush titleBrush = new SolidBrush(_titleColor))
                    {
                        e.Graphics.DrawString(Text, Font, titleBrush, (int)(8 * ScaleFactor), 0);
                    }
                }
            }
        }

        // Custom ComboBox with dark mode support
        public class CustomComboBox : ComboBox
        {
            private bool _isHovered = false;
            public string PlaceholderText { get; set; }
            public int HeaderIndex { get; set; } = -1;

            [StructLayout(LayoutKind.Sequential)]
            private struct TRACKMOUSEEVENT
            {
                public int cbSize;
                public uint dwFlags;
                public IntPtr hwndTrack;
                public uint dwHoverTime;
            }

            [DllImport("user32.dll")]
            private static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

            private const uint TME_LEAVE = 0x00000002;
            private const uint TME_CANCEL = 0x80000000;

            public CustomComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList;
                BackColor = Color.FromArgb(225, 225, 225);

                // Always use custom painting
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
                DrawMode = DrawMode.OwnerDrawFixed;

                if (Dark)
                {
                    FlatStyle = FlatStyle.Flat;
                    BackColor = Color.FromArgb(60, 60, 60);
                    ForeColor = Color.White;
                }

                DropDownClosed += (s, e) =>
                {
                    _isHovered = false;
                    Invalidate();

                    // Force Windows to reset mouse tracking so OnMouseEnter will fire again
                    TRACKMOUSEEVENT tme = new TRACKMOUSEEVENT();
                    tme.cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
                    tme.dwFlags = TME_CANCEL | TME_LEAVE;
                    tme.hwndTrack = Handle;
                    TrackMouseEvent(ref tme);

                    // Immediately restart tracking
                    tme.dwFlags = TME_LEAVE;
                    TrackMouseEvent(ref tme);
                };
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                _isHovered = true;
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _isHovered = false;
                Invalidate();
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                if (e.Index < 0) return;

                bool isDark = Dark;
                bool isHeader = (HeaderIndex >= 0 && e.Index == HeaderIndex);
                bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                Color backColor;
                Color foreColor;

                if (isHeader)
                {
                    backColor = isDark ? Color.FromArgb(50, 50, 50) : SystemColors.Control;
                    foreColor = isDark ? Color.DimGray : SystemColors.GrayText;
                }
                else if (isDark)
                {
                    backColor = isSelected ? Color.Black : Color.FromArgb(60, 60, 60);
                    foreColor = Color.White;
                }
                else
                {
                    backColor = isSelected ? SystemColors.Highlight : SystemColors.Window;
                    foreColor = isSelected ? SystemColors.HighlightText : SystemColors.WindowText;
                }

                e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(foreColor), e.Bounds);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Dark)
                {
                    Color bgColor = _isHovered ? Color.Black : Color.FromArgb(60, 60, 60);

                    // Fill entire control with background
                    e.Graphics.FillRectangle(new SolidBrush(bgColor), 0, 0, Width, Height);

                    // Draw thin border
                    using (Pen borderPen = new Pen(SystemColors.Highlight, 1))
                    {
                        e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
                    }

                    // Draw text on left side (placeholder if no selection, otherwise selected item)
                    Rectangle textBounds = new Rectangle(3, 0, Width - 20, Height);
                    string displayText = (SelectedIndex == -1 && !string.IsNullOrEmpty(PlaceholderText)) ? PlaceholderText : Text;
                    TextRenderer.DrawText(e.Graphics, displayText, Font, textBounds, Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                    // Draw V-shape arrow to match light mode appearance
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    int arrowSize = 4;
                    int arrowX = Width - 13;
                    int arrowY = Height / 2 - 1;
                    Point[] arrowPoints = new Point[]
                    {
                        new Point(arrowX - arrowSize, arrowY - 2),
                        new Point(arrowX, arrowY + 2),
                        new Point(arrowX + arrowSize, arrowY - 2)
                    };
                    using (Pen arrowPen = new Pen(Color.White, 1.5f))
                    {
                        e.Graphics.DrawLines(arrowPen, arrowPoints);
                    }
                }
                else if (SelectedIndex == -1 && !string.IsNullOrEmpty(PlaceholderText))
                {
                    // Light mode with placeholder - use custom painting
                    Color bgColor = _isHovered ? Color.FromArgb(229, 241, 251) : BackColor;
                    Color fgColor = _isHovered ? Color.Black : ForeColor;

                    // Fill entire control with background
                    e.Graphics.FillRectangle(new SolidBrush(bgColor), 0, 0, Width, Height);

                    // Draw border
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);

                    // Draw placeholder text
                    Rectangle textBounds = new Rectangle(3, 0, Width - 20, Height);
                    TextRenderer.DrawText(e.Graphics, PlaceholderText, Font, textBounds, fgColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                    // Draw dropdown arrow
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    int arrowSize = 4;
                    int arrowX = Width - 13;
                    int arrowY = Height / 2 - 1;
                    Point[] arrowPoints = new Point[]
                    {
                        new Point(arrowX - arrowSize, arrowY - 2),
                        new Point(arrowX, arrowY + 2),
                        new Point(arrowX + arrowSize, arrowY - 2)
                    };
                    using (Pen arrowPen = new Pen(fgColor, 1.5f))
                    {
                        e.Graphics.DrawLines(arrowPen, arrowPoints);
                    }
                }
                else
                {
                    // Light mode without placeholder - paint normally with hover effect
                    Color bgColor = _isHovered ? Color.FromArgb(229, 241, 251) : BackColor;
                    Color fgColor = ForeColor;

                    e.Graphics.FillRectangle(new SolidBrush(bgColor), 0, 0, Width, Height);

                    // Draw border
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);

                    // Draw selected text
                    Rectangle textBounds = new Rectangle(3, 0, Width - 20, Height);
                    TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, fgColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                    // Draw dropdown arrow
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    int arrowSize = 4;
                    int arrowX = Width - 13;
                    int arrowY = Height / 2 - 1;
                    Point[] arrowPoints = new Point[]
                    {
                        new Point(arrowX - arrowSize, arrowY - 2),
                        new Point(arrowX, arrowY + 2),
                        new Point(arrowX + arrowSize, arrowY - 2)
                    };
                    using (Pen arrowPen = new Pen(fgColor, 1.5f))
                    {
                        e.Graphics.DrawLines(arrowPen, arrowPoints);
                    }
                }
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // For light mode, handle placeholder text
                if (!Dark && m.Msg == 0x000F) // WM_PAINT
                {
                    if (SelectedIndex == -1 && !string.IsNullOrEmpty(PlaceholderText))
                    {
                        using (Graphics g = CreateGraphics())
                        {
                            // Use appropriate background color based on hover state
                            Color bgColor = _isHovered ? Color.FromArgb(229, 241, 251) : BackColor;
                            Color fgColor = _isHovered ? Color.Black : ForeColor;

                            Rectangle textBounds = new Rectangle(3, 0, Width - 20, Height);
                            TextRenderer.DrawText(g, PlaceholderText, Font, textBounds, fgColor, bgColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        }
                    }
                }
            }
        }

    }
    public enum ScrollBarOrientation
    {
        Horizontal,
        Vertical
    }

    public enum UITheme
    {
        Custom = -1,
        VS2019DarkBlue = 0,
        VS2019LightBlue = 1
    }

    public class FlatScrollBar : Control
    {
        private bool _isDrawing;
        private ScrollBarOrientation _barOrientation = ScrollBarOrientation.Vertical;
        private ScrollOrientation _scrollOrientation = ScrollOrientation.VerticalScroll;

        private Rectangle _rectClickBar;
        private Rectangle _rectThumb;
        private Rectangle _rectTopArrow;
        private Rectangle _rectBottomArrow;
        private Rectangle _rectChannel;

        private bool _isTopArrowClicked;
        private bool _isBottomArrowClicked;
        private bool _isTopBarClicked;
        private bool _isBottomBarClicked;
        private bool _isThumbClicked;

        private ScrollBarState _thumbState = ScrollBarState.Normal;
        private ScrollBarArrowButtonState _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
        private ScrollBarArrowButtonState _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;

        private int _minimum;
        private int _maximum = 100;
        private int _smallChange = 1;
        private int _largeChange = 10;
        private int _value;

        private int _thumbWidth = 10;
        private int _thumbHeight;

        private int _arrowWidth = 18;
        private int _arrowHeight = 18;

        private int _thumbBottomLimitBottom;
        private int _thumbBottomLimitTop;
        private int _thumbTopLimit;
        private int _thumbPosition;

        private int _trackPosition;

        private readonly Timer scrollTimer = new Timer();

        private UITheme _theme = UITheme.VS2019LightBlue;

        private Color _backColor = Color.FromArgb(225, 225, 225);
        private Color _borderColor = Color.FromArgb(225, 225, 225);
        private Color _borderColorDisabled = Color.FromArgb(225, 225, 225);

        private readonly Color[] _thumbColors = new Color[3];
        private readonly Color[] _arrowColors = new Color[3];

        private const int SETREDRAW = 11;
        private const int MINIMUM_SIZE = 10;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

        public event ScrollEventHandler Scroll;

        public FlatScrollBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | 
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, false);

            SetUpScrollBar();

            scrollTimer.Tick += ScrollTimer_Tick;

            _thumbColors[0] = Color.FromArgb(194, 195, 201);
            _thumbColors[1] = Color.FromArgb(104, 104, 104);
            _thumbColors[2] = Color.FromArgb(91, 91, 91);

            _arrowColors[0] = Color.FromArgb(134, 137, 153);
            _arrowColors[1] = Color.FromArgb(70, 181, 255);
            _arrowColors[2] = Color.FromArgb(0, 122, 204);
        }

        [Category("Layout")]
        [Description("Gets or sets the ScrollBar orientation.")]
        [DefaultValue(ScrollBarOrientation.Vertical)]
        public ScrollBarOrientation Orientation
        {
            get { return _barOrientation; }
            set
            {
                if (value != _barOrientation)
                {
                    _barOrientation = value;
                    _scrollOrientation = value == ScrollBarOrientation.Vertical 
                        ? ScrollOrientation.VerticalScroll 
                        : ScrollOrientation.HorizontalScroll;

                    if (DesignMode)
                    {
                        Size = new Size(Height, Width);
                    }

                    SetUpScrollBar();
                }
            }
        }

        [Category("Behavior")]
        [Description("Gets or sets the ScrollBar minimum value.")]
        [DefaultValue(0)]
        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (_minimum == value || value < 0 || value >= _maximum)
                    return;

                _minimum = value;

                if (_largeChange > _maximum - _minimum)
                    _largeChange = _maximum - _minimum;

                SetUpScrollBar();

                if (_value < value)
                {
                    Value = value;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        [Category("Behavior")]
        [Description("Gets or sets the ScrollBar maximum value.")]
        [DefaultValue(100)]
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value == _maximum || value < 1 || value <= _minimum)
                    return;

                _maximum = value;

                if (_largeChange > _maximum - _minimum)
                    _largeChange = _maximum - _minimum;

                SetUpScrollBar();

                if (_value > _maximum)
                {
                    Value = _maximum;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        [Category("Behavior")]
        [Description("Gets or sets the ScrollBar small change value.")]
        [DefaultValue(1)]
        public int SmallChange
        {
            get { return _smallChange; }
            set
            {
                if (value == _smallChange || value < 1 || value >= _largeChange)
                    return;

                _smallChange = value;
                SetUpScrollBar();
            }
        }

        [Category("Behavior")]
        [Description("Gets or sets the ScrollBar large change value.")]
        [DefaultValue(10)]
        public int LargeChange
        {
            get { return _largeChange; }
            set
            {
                if (value == _largeChange || value < _smallChange || value < 2)
                    return;

                _largeChange = value > _maximum - _minimum ? _maximum - _minimum : value;

                SetUpScrollBar();
            }
        }

        [Category("Behavior")]
        [Description("Gets or sets the ScrollBar current value.")]
        [DefaultValue(0)]
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value == value || value < _minimum || value > _maximum)
                    return;

                _value = value;
                ChangeThumbPosition(GetThumbPosition());
                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, -1, _value, _scrollOrientation));
                Refresh();
            }
        }

        [Category("Appearance")]
        [Description("The theme to apply to the Flat ScrollBar control.")]
        [DefaultValue(UITheme.VS2019LightBlue)]
        public UITheme Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;

                if (_theme == UITheme.VS2019DarkBlue)
                {
                    _backColor = Color.FromArgb(62, 62, 66);
                    _borderColor = Color.FromArgb(62, 62, 66);
                    _borderColorDisabled = Color.FromArgb(62, 62, 66);

                    _thumbColors[0] = Color.FromArgb(104, 104, 104);
                    _thumbColors[1] = Color.FromArgb(158, 158, 158);
                    _thumbColors[2] = Color.FromArgb(239, 235, 239);

                    _arrowColors[0] = Color.FromArgb(153, 153, 153);
                    _arrowColors[1] = Color.FromArgb(28, 151, 234);
                    _arrowColors[2] = Color.FromArgb(0, 122, 204);
                }
                else if (_theme == UITheme.VS2019LightBlue)
                {
                    _backColor = Color.FromArgb(245, 245, 245);
                    _borderColor = Color.FromArgb(245, 245, 245);
                    _borderColorDisabled = Color.FromArgb(245, 245, 245);

                    _thumbColors[0] = Color.FromArgb(194, 195, 201);
                    _thumbColors[1] = Color.FromArgb(104, 104, 104);
                    _thumbColors[2] = Color.FromArgb(91, 91, 91);

                    _arrowColors[0] = Color.FromArgb(134, 137, 153);
                    _arrowColors[1] = Color.FromArgb(28, 151, 234);
                    _arrowColors[2] = Color.FromArgb(0, 122, 204);
                }

                Invalidate();
            }
        }

        public void BeginUpdate()
        {
            SendMessage(Handle, SETREDRAW, false, 0);
            _isDrawing = true;
        }

        public void EndUpdate()
        {
            SendMessage(Handle, SETREDRAW, true, 0);
            _isDrawing = false;
            SetUpScrollBar();
            Refresh();
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (e == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackground(g, ClientRectangle);
            DrawThumb(g, _rectThumb, _thumbState);
            DrawArrowButton(g, _rectTopArrow, _topArrowButtonState, true, _barOrientation);
            DrawArrowButton(g, _rectBottomArrow, _bottomArrowButtonState, false, _barOrientation);

            if (_isTopBarClicked)
            {
                if (_barOrientation == ScrollBarOrientation.Vertical)
                {
                    _rectClickBar.Y = _thumbTopLimit;
                    _rectClickBar.Height = _rectThumb.Y - _thumbTopLimit;
                }
                else
                {
                    _rectClickBar.X = _thumbTopLimit;
                    _rectClickBar.Width = _rectThumb.X - _thumbTopLimit;
                }
            }
            else if (_isBottomBarClicked)
            {
                if (_barOrientation == ScrollBarOrientation.Vertical)
                {
                    _rectClickBar.Y = _rectThumb.Bottom + 1;
                    _rectClickBar.Height = _thumbBottomLimitBottom - _rectClickBar.Y + 1;
                }
                else
                {
                    _rectClickBar.X = _rectThumb.Right + 1;
                    _rectClickBar.Width = _thumbBottomLimitBottom - _rectClickBar.X + 1;
                }
            }

            using (Pen p = new Pen(Enabled ? _borderColor : _borderColorDisabled))
            {
                e.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            if (e.Button == MouseButtons.Left)
            {
                Point mouseLocation = e.Location;

                if (_rectThumb.Contains(mouseLocation))
                {
                    _isThumbClicked = true;
                    _thumbPosition = _barOrientation == ScrollBarOrientation.Vertical 
                        ? mouseLocation.Y - _rectThumb.Y 
                        : mouseLocation.X - _rectThumb.X;
                    _thumbState = ScrollBarState.Pressed;
                    Invalidate(_rectThumb);
                }
                else if (_rectTopArrow.Contains(mouseLocation))
                {
                    _isTopArrowClicked = true;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpPressed;
                    Invalidate(_rectTopArrow);
                    ProgressThumb(true);
                }
                else if (_rectBottomArrow.Contains(mouseLocation))
                {
                    _isBottomArrowClicked = true;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownPressed;
                    Invalidate(_rectBottomArrow);
                    ProgressThumb(true);
                }
                else
                {
                    _trackPosition = _barOrientation == ScrollBarOrientation.Vertical ? mouseLocation.Y : mouseLocation.X;

                    if (_trackPosition < (_barOrientation == ScrollBarOrientation.Vertical ? _rectThumb.Y : _rectThumb.X))
                        _isTopBarClicked = true;
                    else
                        _isBottomBarClicked = true;

                    ProgressThumb(true);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                _trackPosition = _barOrientation == ScrollBarOrientation.Vertical ? e.Y : e.X;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                if (_isThumbClicked)
                {
                    _isThumbClicked = false;
                    _thumbState = ScrollBarState.Normal;
                    OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, -1, _value, _scrollOrientation));
                }
                else if (_isTopArrowClicked)
                {
                    _isTopArrowClicked = false;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                    scrollTimer.Stop();
                }
                else if (_isBottomArrowClicked)
                {
                    _isBottomArrowClicked = false;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;
                    scrollTimer.Stop();
                }
                else if (_isTopBarClicked)
                {
                    _isTopBarClicked = false;
                    scrollTimer.Stop();
                }
                else if (_isBottomBarClicked)
                {
                    _isBottomBarClicked = false;
                    scrollTimer.Stop();
                }

                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            RefreshScrollBar();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                if (_isThumbClicked)
                {
                    int oldValue = _value;

                    int pos = _barOrientation == ScrollBarOrientation.Vertical ? e.Location.Y : e.Location.X;

                    if (pos <= (_thumbTopLimit + _thumbPosition))
                    {
                        ChangeThumbPosition(_thumbTopLimit);
                        _value = _minimum;
                    }
                    else if (pos >= (_thumbBottomLimitTop + _thumbPosition))
                    {
                        ChangeThumbPosition(_thumbBottomLimitTop);
                        _value = _maximum;
                    }
                    else
                    {
                        ChangeThumbPosition(pos - _thumbPosition);

                        int pixelRange, thumbPos, arrowSize;

                        if (_barOrientation == ScrollBarOrientation.Vertical)
                        {
                            pixelRange = Height - (2 * _arrowHeight) - _thumbHeight;
                            thumbPos = _rectThumb.Y;
                            arrowSize = _arrowHeight;
                        }
                        else
                        {
                            pixelRange = Width - (2 * _arrowWidth) - _thumbWidth;
                            thumbPos = _rectThumb.X;
                            arrowSize = _arrowWidth;
                        }

                        float perc = 0F;

                        if (pixelRange != 0)
                            perc = (float)(thumbPos - arrowSize) / (float)pixelRange;

                        _value = Convert.ToInt32((perc * (_maximum - _minimum)) + _minimum);
                    }

                    if (oldValue != _value)
                    {
                        OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, oldValue, _value, _scrollOrientation));
                        Refresh();
                    }
                }
            }
            else if (!ClientRectangle.Contains(e.Location))
            {
                RefreshScrollBar();
            }
            else if (e.Button == MouseButtons.None)
            {
                if (_rectTopArrow.Contains(e.Location))
                {
                    _topArrowButtonState = ScrollBarArrowButtonState.UpHot;
                    Invalidate(_rectTopArrow);
                }
                else if (_rectBottomArrow.Contains(e.Location))
                {
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownHot;
                    Invalidate(_rectBottomArrow);
                }
                else if (_rectThumb.Contains(e.Location))
                {
                    _thumbState = ScrollBarState.Hot;
                    Invalidate(_rectThumb);
                }
                else
                {
                    _thumbState = ScrollBarState.Normal;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;

                    Refresh();
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int oldValue = _value;
            ScrollEventType scrollType;

            if (e.Delta >= 0)
            {
                _value = GetScrollValue(false, true);

                if (_value == _minimum)
                {
                    scrollType = ScrollEventType.First;
                    ChangeThumbPosition(_thumbTopLimit);
                }
                else
                {
                    scrollType = ScrollEventType.LargeDecrement;
                    ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
                }
            }
            else
            {
                _value = GetScrollValue(false, false);

                if (_value == _maximum)
                {
                    scrollType = ScrollEventType.Last;
                    ChangeThumbPosition(_thumbBottomLimitTop);
                }
                else
                {
                    scrollType = ScrollEventType.SmallIncrement;
                    ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
                }
            }

            if (oldValue != _value)
            {
                OnScroll(new ScrollEventArgs(scrollType, oldValue, _value, _scrollOrientation));
                Invalidate(_rectChannel);
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (DesignMode)
            {
                if (_barOrientation == ScrollBarOrientation.Vertical)
                {
                    int minHeight = (2 * _arrowHeight) + MINIMUM_SIZE;

                    if (height < minHeight) height = minHeight;
                    width = SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    int minWidth = (2 * _arrowWidth) + MINIMUM_SIZE;

                    if (width < minWidth) width = minWidth;
                    height = SystemInformation.VerticalScrollBarWidth;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);

            if (DesignMode) SetUpScrollBar();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetUpScrollBar();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool isHandled = false;
            int oldValue = _value;
            ScrollEventType scrollType = ScrollEventType.First;

            Keys keyUp = Keys.Up;
            Keys keyDown = Keys.Down;

            if (_barOrientation == ScrollBarOrientation.Horizontal)
            {
                keyUp = Keys.Left;
                keyDown = Keys.Right;
            }

            switch (keyData)
            {
                case Keys.Up:
                case Keys.Left:
                    if (keyData == keyUp)
                    {
                        _value = GetScrollValue(true, true);

                        if (_value == _minimum)
                        {
                            scrollType = ScrollEventType.First;
                            ChangeThumbPosition(_thumbTopLimit);
                        }
                        else
                        {
                            scrollType = ScrollEventType.SmallDecrement;
                            ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
                        }

                        isHandled = true;
                    }
                    break;

                case Keys.Down:
                case Keys.Right:
                    if (keyData == keyDown)
                    {
                        _value = GetScrollValue(true, false);

                        if (_value == _maximum)
                        {
                            scrollType = ScrollEventType.Last;
                            ChangeThumbPosition(_thumbBottomLimitTop);
                        }
                        else
                        {
                            scrollType = ScrollEventType.SmallIncrement;
                            ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
                        }

                        isHandled = true;
                    }
                    break;

                case Keys.PageUp:
                    _value = GetScrollValue(false, true);

                    if (_value == _minimum)
                    {
                        scrollType = ScrollEventType.First;
                        ChangeThumbPosition(_thumbTopLimit);
                    }
                    else
                    {
                        scrollType = ScrollEventType.LargeDecrement;
                        ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
                    }

                    isHandled = true;
                    break;

                case Keys.PageDown:
                    _value = GetScrollValue(false, false);

                    if (_value == _maximum)
                    {
                        scrollType = ScrollEventType.Last;
                        ChangeThumbPosition(_thumbBottomLimitTop);
                    }
                    else
                    {
                        scrollType = ScrollEventType.SmallIncrement;
                        ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
                    }

                    isHandled = true;
                    break;

                case Keys.Home:
                    _value = _minimum;
                    scrollType = ScrollEventType.First;
                    ChangeThumbPosition(_thumbTopLimit);
                    isHandled = true;
                    break;

                case Keys.End:
                    _value = _maximum;
                    scrollType = ScrollEventType.Last;
                    ChangeThumbPosition(_thumbBottomLimitTop);
                    isHandled = true;
                    break;
            }

            if (isHandled && oldValue != _value)
            {
                OnScroll(new ScrollEventArgs(scrollType, oldValue, _value, _scrollOrientation));
                Invalidate(_rectChannel);
            }

            return isHandled || base.ProcessDialogKey(keyData);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (Enabled)
            {
                _thumbState = ScrollBarState.Normal;
                _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;
            }
            else
            {
                _thumbState = ScrollBarState.Disabled;
                _topArrowButtonState = ScrollBarArrowButtonState.UpDisabled;
                _bottomArrowButtonState = ScrollBarArrowButtonState.DownDisabled;
            }

            Refresh();
        }

        protected override Size DefaultSize
        {
            get { return new Size(SystemInformation.VerticalScrollBarWidth, 200); }
        }

        private void SetUpScrollBar()
        {
            if (_isDrawing) return;

            if (_barOrientation == ScrollBarOrientation.Vertical)
            {
                _arrowHeight = 18;
                _arrowWidth = 18;
                _thumbWidth = 9;
                _thumbHeight = GetThumbSize();
                _rectClickBar = ClientRectangle;
                _rectClickBar.Inflate(-1, -1);
                _rectClickBar.Y += _arrowHeight;
                _rectClickBar.Height -= _arrowHeight * 2;
                _rectChannel = _rectClickBar;
                _rectThumb = new Rectangle((ClientRectangle.Right / 2) - (_thumbWidth / 2), ClientRectangle.Y + _arrowHeight, _thumbWidth, _thumbHeight);
                _rectTopArrow = new Rectangle((ClientRectangle.Right / 2) - (_arrowWidth / 2) + 1, ClientRectangle.Y + 1, _arrowWidth, _arrowHeight);
                _rectBottomArrow = new Rectangle((ClientRectangle.Right / 2) - (_arrowWidth / 2), ClientRectangle.Bottom - _arrowHeight - 1, _arrowWidth, _arrowHeight);
                _thumbPosition = _rectThumb.Height / 2;
                _thumbBottomLimitBottom = ClientRectangle.Bottom - _arrowHeight - 2;
                _thumbBottomLimitTop = _thumbBottomLimitBottom - _rectThumb.Height;
                _thumbTopLimit = ClientRectangle.Y + _arrowHeight + 2;
            }
            else
            {
                _arrowHeight = 18;
                _arrowWidth = 18;
                _thumbHeight = 9;
                _thumbWidth = GetThumbSize();
                _rectClickBar = ClientRectangle;
                _rectClickBar.Inflate(-1, -1);
                _rectClickBar.X += _arrowWidth;
                _rectClickBar.Width -= _arrowWidth * 2;
                _rectChannel = _rectClickBar;
                _rectThumb = new Rectangle(ClientRectangle.X + _arrowWidth, (ClientRectangle.Bottom / 2) - (_thumbHeight / 2), _thumbWidth, _thumbHeight);
                _rectTopArrow = new Rectangle(ClientRectangle.X + 2, (ClientRectangle.Bottom / 2) - (_arrowHeight / 2), _arrowWidth, _arrowHeight);
                _rectBottomArrow = new Rectangle(ClientRectangle.Right - _arrowWidth - 2, (ClientRectangle.Bottom / 2) - (_arrowHeight / 2) + 1, _arrowWidth, _arrowHeight);
                _thumbPosition = _rectThumb.Width / 2;
                _thumbBottomLimitBottom = ClientRectangle.Right - _arrowWidth - 3;
                _thumbBottomLimitTop = _thumbBottomLimitBottom - _rectThumb.Width;
                _thumbTopLimit = ClientRectangle.X + _arrowWidth + 3;
            }

            ChangeThumbPosition(GetThumbPosition());
            Refresh();
        }

        private void RefreshScrollBar()
        {
            Point pt = PointToClient(Cursor.Position);

            if (ClientRectangle.Contains(pt))
            {
                if (_rectThumb.Contains(pt))
                {
                    _thumbState = ScrollBarState.Hot;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;
                }
                else if (_rectTopArrow.Contains(pt))
                {
                    _thumbState = ScrollBarState.Normal;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpActive;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;
                }
                else if (_rectBottomArrow.Contains(pt))
                {
                    _thumbState = ScrollBarState.Normal;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownActive;
                }
                else
                {
                    _thumbState = ScrollBarState.Normal;
                    _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                    _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;
                }
            }
            else
            {
                _thumbState = ScrollBarState.Normal;
                _topArrowButtonState = ScrollBarArrowButtonState.UpNormal;
                _bottomArrowButtonState = ScrollBarArrowButtonState.DownNormal;
            }

            _isTopArrowClicked = false;
            _isBottomArrowClicked = false;
            _isTopBarClicked = false;
            _isBottomBarClicked = false;

            scrollTimer.Stop();
            Refresh();
        }

        private int GetScrollValue(bool isSmallChange, bool isDecreaseValue)
        {
            int newValue;

            if (isDecreaseValue)
            {
                newValue = _value - (isSmallChange ? _smallChange : _largeChange);
                if (newValue < _minimum) newValue = _minimum;
            }
            else
            {
                newValue = _value + (isSmallChange ? _smallChange : _largeChange);
                if (newValue > _maximum) newValue = _maximum;
            }

            return newValue;
        }

        private int GetThumbPosition()
        {
            int pixelRange = _barOrientation == ScrollBarOrientation.Vertical ? _rectChannel.Height : _rectChannel.Width;

            int realRange = _maximum - _minimum;
            float perc = 0F;

            if (realRange != 0)
                perc = (float)(_value - _minimum) / realRange;

            return Math.Max(_thumbTopLimit, Math.Min(_thumbBottomLimitTop, Convert.ToInt32(perc * pixelRange)));
        }

        private int GetThumbSize()
        {
            int trackSize = _barOrientation == ScrollBarOrientation.Vertical ? Height : Width;

            if (_maximum == 0 || _largeChange == 0)
                return trackSize;

            float thumbSize = (float)_largeChange * trackSize / _maximum;

            return Convert.ToInt32(Math.Min(trackSize, Math.Max(thumbSize, 10.0F)));
        }

        private void ChangeThumbPosition(int position)
        {
            if (_barOrientation == ScrollBarOrientation.Vertical)
                _rectThumb.Y = position;
            else
                _rectThumb.X = position;

            Point pt = PointToClient(Cursor.Position);

            if (_rectThumb.Contains(pt))
            {
                _thumbState = ScrollBarState.Hot;
                Invalidate(_rectThumb);
            }
        }

        private void ProgressThumb(bool isContinuousScroll)
        {
            int oldValue = _value;
            ScrollEventType type = ScrollEventType.First;
            int thumbSize, thumbPos;

            if (_barOrientation == ScrollBarOrientation.Vertical)
            {
                thumbPos = _rectThumb.Y;
                thumbSize = _rectThumb.Height;
            }
            else
            {
                thumbPos = _rectThumb.X;
                thumbSize = _rectThumb.Width;
            }

            if (_isBottomArrowClicked || (_isBottomBarClicked && (thumbPos + thumbSize) < _trackPosition))
            {
                type = _isBottomArrowClicked ? ScrollEventType.SmallIncrement : ScrollEventType.LargeIncrement;
                _value = GetScrollValue(_isBottomArrowClicked, false);

                if (_value == _maximum)
                {
                    ChangeThumbPosition(_thumbBottomLimitTop);
                    type = ScrollEventType.Last;
                }
                else
                {
                    ChangeThumbPosition(Math.Min(_thumbBottomLimitTop, GetThumbPosition()));
                }
            }
            else if (_isTopArrowClicked || (_isTopBarClicked && thumbPos > _trackPosition))
            {
                type = _isTopArrowClicked ? ScrollEventType.SmallDecrement : ScrollEventType.LargeDecrement;
                _value = GetScrollValue(_isTopArrowClicked, true);

                if (_value == _minimum)
                {
                    ChangeThumbPosition(_thumbTopLimit);
                    type = ScrollEventType.First;
                }
                else
                {
                    ChangeThumbPosition(Math.Max(_thumbTopLimit, GetThumbPosition()));
                }
            }
            else if (!((_isTopArrowClicked && thumbPos == _thumbTopLimit) || (_isBottomArrowClicked && thumbPos == _thumbBottomLimitTop)))
            {
                RefreshScrollBar();
                return;
            }

            if (oldValue != _value)
            {
                OnScroll(new ScrollEventArgs(type, oldValue, _value, _scrollOrientation));
                Invalidate(_rectChannel);

                if (isContinuousScroll) StartScrollTimer();
            }
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            ProgressThumb(true);
        }

        private void StartScrollTimer()
        {
            if (!scrollTimer.Enabled)
            {
                scrollTimer.Interval = 50;
                scrollTimer.Start();
            }
            else
            {
                scrollTimer.Interval = 10;
            }
        }

        private void DrawBackground(Graphics g, Rectangle rect)
        {
            if (g == null || rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
                return;

            using (SolidBrush sb = new SolidBrush(_backColor))
            {
                g.FillRectangle(sb, rect);
            }
        }

        private void DrawThumb(Graphics g, Rectangle rect, ScrollBarState state)
        {
            if (g == null || rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect) || state == ScrollBarState.Disabled)
                return;

            int index = 0;

            switch (state)
            {
                case ScrollBarState.Hot:
                    index = 1;
                    break;
                case ScrollBarState.Pressed:
                    index = 2;
                    break;
            }

            using (SolidBrush sb = new SolidBrush(_thumbColors[index]))
            {
                // Draw thumb with rounded ends for modern appearance
                int radius = _barOrientation == ScrollBarOrientation.Vertical 
                    ? rect.Width / 2 
                    : rect.Height / 2;

                using (GraphicsPath thumbPath = GetRoundedRectanglePath(rect, radius))
                {
                    g.FillPath(sb, thumbPath);
                }
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            if (diameter >= rect.Width || diameter >= rect.Height)
            {
                // If radius is too large, create a capsule/pill shape
                if (rect.Width < rect.Height)
                {
                    // Vertical pill
                    int r = rect.Width / 2;
                    path.AddArc(rect.X, rect.Y, rect.Width, rect.Width, 180, 180);
                    path.AddArc(rect.X, rect.Bottom - rect.Width, rect.Width, rect.Width, 0, 180);
                }
                else
                {
                    // Horizontal pill
                    int r = rect.Height / 2;
                    path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
                    path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
                }
            }
            else
            {
                // Normal rounded rectangle
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            }

            path.CloseFigure();
            return path;
        }

        private void DrawArrowButton(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool isUpArrow, ScrollBarOrientation orient)
        {
            if (g == null || rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
                return;

            if (orient == ScrollBarOrientation.Vertical)
                DrawVerticalArrowButton(g, rect, state, isUpArrow);
            else
                DrawHorizontalArrowButton(g, rect, state, isUpArrow);
        }

        private void DrawVerticalArrowButton(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp)
        {
            using (Image img = GetDownArrowButtonImage(state))
            {
                if (arrowUp) img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                g.DrawImage(img, rect);
            }
        }

        private void DrawHorizontalArrowButton(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp)
        {
            using (Image img = GetDownArrowButtonImage(state))
            {
                if (arrowUp)
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);

                g.DrawImage(img, rect);
            }
        }

        private Image GetDownArrowButtonImage(ScrollBarArrowButtonState state)
        {
            Rectangle rect = new Rectangle(0, 0, _arrowWidth, _arrowHeight);
            Bitmap bitmap = new Bitmap(_arrowWidth, _arrowHeight, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int index = 0;

            switch (state)
            {
                case ScrollBarArrowButtonState.UpHot:
                case ScrollBarArrowButtonState.DownHot:
                    index = 1;
                    break;
                case ScrollBarArrowButtonState.UpActive:
                case ScrollBarArrowButtonState.DownActive:
                    index = 1;
                    break;
                case ScrollBarArrowButtonState.UpPressed:
                case ScrollBarArrowButtonState.DownPressed:
                    index = 2;
                    break;
            }

            using (SolidBrush sb = new SolidBrush(_arrowColors[index]))
            {
                g.FillPolygon(sb, GetDownArrowPos(rect));
            }

            g.Dispose();

            return bitmap;
        }

        private static Point[] GetDownArrowPos(Rectangle r)
        {
            Point middle = new Point(r.Left + (r.Width / 2), r.Top + (r.Height / 2));
            return new Point[]
            {
                new Point(middle.X - 4, middle.Y - 3),
                new Point(middle.X + 4, middle.Y - 2),
                new Point(middle.X, middle.Y + 2)
            };
        }

        private enum ScrollBarArrowButtonState
        {
            UpNormal,
            UpHot,
            UpActive,
            UpPressed,
            UpDisabled,
            DownNormal,
            DownHot,
            DownActive,
            DownPressed,
            DownDisabled
        }

        private enum ScrollBarState
        {
            Normal,
            Hot,
            Active,
            Pressed,
            Disabled
        }
    }

    // Custom time picker: shows "HH:MM:SS AM/PM" with a single up/down button pair.
    // Clicking any segment (hour, minute, second, AM/PM) selects it;
    // the up/down buttons (or mouse wheel / arrow keys) increment/decrement the selected segment.
    public class TimeSpinnerControl : Control
    {
        private int _hour;
        private int _minute;
        private int _second;

        private enum Segment { Hour, Minute, Second, AmPm }
        private Segment _selected = Segment.Hour;

        private bool _upHovered, _upPressed, _downHovered, _downPressed;

        private readonly System.Windows.Forms.Timer _repeatTimer = new System.Windows.Forms.Timer();
        private bool _repeatUp;

        private int _typingBuffer = -1;  // -1 = no pending digit

        private int BtnW  => (int)(14 * Program.ScaleFactor);   // width of each button
        private int Pad   => (int)(2  * Program.ScaleFactor);

        private Color ColBg       => Program.Dark ? Color.FromArgb(45, 45, 45)  : Color.White;
        private Color ColText     => Program.Dark ? Color.White                 : Color.Black;
        private Color ColBorder   => Program.Dark ? Color.FromArgb(100,100,100) : Color.FromArgb(171,173,179);
        private Color ColSelBg    => Color.FromArgb(0, 103, 192);
        private Color ColSelText  => Color.White;
        private Color ColBtnBg    => Program.Dark ? Color.FromArgb(60, 60, 60)  : Color.FromArgb(225, 225, 225);
        private Color ColBtnHover => Program.Dark ? Color.FromArgb(80, 80, 80)  : Color.FromArgb(190, 225, 255);
        private Color ColBtnPress => Color.FromArgb(0, 103, 192);
        private Color ColArrow    => Program.Dark ? Color.FromArgb(200,200,200) : Color.FromArgb(80, 80, 80);

        public TimeSpan Time
        {
            get => new TimeSpan(_hour, _minute, _second);
            set { _hour = value.Hours; _minute = value.Minutes; _second = value.Seconds; Invalidate(); }
        }

        public TimeSpinnerControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable, true);
            DateTime now = DateTime.Now;
            _hour = now.Hour; _minute = now.Minute; _second = now.Second;
            _repeatTimer.Interval = 80;
            _repeatTimer.Tick += (s, e) => Step(_repeatUp);
        }

        private Rectangle[] SegmentRects(Graphics g)
        {
            Font f = Font ?? new Font("Segoe UI", 9);
            string[] samples = { "00", "00", "00", "AM" };
            float[] widths = new float[4];
            for (int i = 0; i < 4; i++)
                widths[i] = g.MeasureString(samples[i], f).Width + 2;

            float colonW = g.MeasureString(":", f).Width;
            float spaceW = g.MeasureString(" ", f).Width;

            float x = Pad;
            var rects = new Rectangle[4];
            for (int i = 0; i < 4; i++)
            {
                rects[i] = new Rectangle((int)x, 1, (int)widths[i], Height - 2);
                x += widths[i];
                if (i == 0 || i == 1) x += colonW;
                if (i == 2) x += spaceW;
            }
            return rects;
        }

        private Segment? HitSegment(Point pt, Graphics g)
        {
            Rectangle[] r = SegmentRects(g);
            for (int i = 0; i < r.Length; i++)
                if (r[i].Contains(pt)) return (Segment)i;
            return null;
        }

        private void Step(bool up)
        {
            int d = up ? 1 : -1;
            switch (_selected)
            {
                case Segment.Hour:   _hour   = (_hour   + d + 24) % 24; break;
                case Segment.Minute: _minute = (_minute + d + 60) % 60; break;
                case Segment.Second: _second = (_second + d + 60) % 60; break;
                case Segment.AmPm:   _hour   = (_hour + 12) % 24;       break;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var b = new SolidBrush(ColBg))
                g.FillRectangle(b, ClientRectangle);
            using (var p = new Pen(ColBorder, 1))
                g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);

            Font f = Font ?? new Font("Segoe UI", 9);
            Rectangle[] rects = SegmentRects(g);

            int h12 = _hour % 12; if (h12 == 0) h12 = 12;
            string[] texts = { h12.ToString("D2"), _minute.ToString("D2"),
                               _second.ToString("D2"), _hour < 12 ? "AM" : "PM" };

            using (var tb = new SolidBrush(ColText))
            {
                g.DrawString(":", f, tb, rects[0].Right - 1, rects[0].Top + (rects[0].Height - f.Height) / 2f);
                g.DrawString(":", f, tb, rects[1].Right - 1, rects[1].Top + (rects[1].Height - f.Height) / 2f);
            }

            for (int i = 0; i < 4; i++)
            {
                bool sel = (int)_selected == i;
                if (sel)
                    using (var sb = new SolidBrush(ColSelBg))
                        g.FillRectangle(sb, rects[i]);

                TextRenderer.DrawText(g, texts[i], f, rects[i],
                    sel ? ColSelText : ColText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }

            DrawSpinButton(g, UpButtonRect(), _upPressed, _upHovered, true);
            DrawSpinButton(g, DownButtonRect(), _downPressed, _downHovered, false);
        }

        private Rectangle UpButtonRect()
        {
            int x = Width - BtnW - Pad;
            return new Rectangle(x, 1, BtnW, Height - 2);
        }

        private Rectangle DownButtonRect()
        {
            int x = Width - (BtnW * 2) - Pad;
            return new Rectangle(x, 1, BtnW, Height - 2);
        }

        private void DrawSpinButton(Graphics g, Rectangle r, bool pressed, bool hovered, bool up)
        {
            Color bg = pressed ? ColBtnPress : (hovered ? ColBtnHover : ColBtnBg);
            using (var b = new SolidBrush(bg))
                g.FillRectangle(b, r);
            using (var p = new Pen(ColBorder, 1))
                g.DrawRectangle(p, r.X, r.Y, r.Width - 1, r.Height - 1);

            int mx = r.X + r.Width / 2;
            int my = r.Y + r.Height / 2;
            int aw = Math.Max(3, (int)(4 * Program.ScaleFactor));
            Point[] pts = up
                ? new[] { new Point(mx - aw, my + 1), new Point(mx + aw, my + 1), new Point(mx, my - aw + 1) }
                : new[] { new Point(mx - aw, my - 1), new Point(mx + aw, my - 1), new Point(mx, my + aw - 1) };

            using (var ab = new SolidBrush(pressed ? Color.White : ColArrow))
                g.FillPolygon(ab, pts);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            if (UpButtonRect().Contains(e.Location))
            {
                _upPressed = true; Step(true); _repeatUp = true;
                _repeatTimer.Start(); Invalidate(); return;
            }
            if (DownButtonRect().Contains(e.Location))
            {
                _downPressed = true; Step(false); _repeatUp = false;
                _repeatTimer.Start(); Invalidate(); return;
            }
            using (Graphics g = CreateGraphics())
            {
                Segment? hit = HitSegment(e.Location, g);
                if (hit.HasValue) { _selected = hit.Value; Invalidate(); }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _repeatTimer.Stop();
            _upPressed = false; _downPressed = false;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool u = UpButtonRect().Contains(e.Location);
            bool d = DownButtonRect().Contains(e.Location);
            if (u != _upHovered || d != _downHovered)
            { _upHovered = u; _downHovered = d; Invalidate(); }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _upHovered = false; _downHovered = false; Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Step(e.Delta > 0);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Up:    _typingBuffer = -1; Step(true);  e.Handled = true; break;
                case Keys.Down:  _typingBuffer = -1; Step(false); e.Handled = true; break;
                case Keys.Left:  _typingBuffer = -1; _selected = (Segment)(((int)_selected + 3) % 4); Invalidate(); e.Handled = true; break;
                case Keys.Right: _typingBuffer = -1; _selected = (Segment)(((int)_selected + 1) % 4); Invalidate(); e.Handled = true; break;
                case Keys.Delete:
                case Keys.Back:
                    _typingBuffer = -1;
                    SetSegmentValue(0);
                    e.Handled = true; break;
            }
        }

        // Direct digit typing — mirrors Windows built-in time picker behaviour.
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            char c = e.KeyChar;

            // A / P toggle AM/PM from any segment
            if (c == 'a' || c == 'A') { _selected = Segment.AmPm; _hour = (_hour % 12);       Invalidate(); e.Handled = true; return; }
            if (c == 'p' || c == 'P') { _selected = Segment.AmPm; _hour = (_hour % 12) + 12;  Invalidate(); e.Handled = true; return; }

            if (_selected == Segment.AmPm) return;   // no digit entry for AM/PM

            if (!char.IsDigit(c)) return;
            e.Handled = true;

            int digit = c - '0';
            int max = (_selected == Segment.Hour) ? 23 : 59;
            int maxFirst = max / 10;  // highest valid first digit (2 for hours, 5 for min/sec)

            int newVal;
            if (_typingBuffer < 0)
            {
                // No pending digit yet — start accumulation
                _typingBuffer = digit;
                newVal = digit;
                // If this digit alone already exceeds maxFirst, it can only be a 1-digit entry
                // e.g. typing "3" for hours when max first digit is 2 → accept as "03" and advance
                if (digit > maxFirst)
                {
                    _typingBuffer = -1;
                    SetSegmentValue(digit);
                    return;
                }
            }
            else
            {
                // Second digit
                newVal = _typingBuffer * 10 + digit;
                if (newVal > max) newVal = max;
                _typingBuffer = -1;
                SetSegmentValue(newVal);
                return;
            }

            SetSegmentValue(newVal);
        }

        private void SetSegmentValue(int value)
        {
            switch (_selected)
            {
                case Segment.Hour:
                    _hour = Math.Max(0, Math.Min(23, value));
                    break;
                case Segment.Minute: _minute = Math.Max(0, Math.Min(59, value)); break;
                case Segment.Second: _second = Math.Max(0, Math.Min(59, value)); break;
            }
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left: case Keys.Right:
                case Keys.Up:   case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _repeatTimer.Dispose();
            base.Dispose(disposing);
        }
    }
}
