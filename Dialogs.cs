using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace RightClickTools
{
    partial class Program
    {
        // Wraps a raw HWND as an IWin32Window so it can be passed as a dialog owner
        private class WindowWrapper : IWin32Window
        {
            private readonly IntPtr _hwnd;
            public WindowWrapper(IntPtr hwnd) { _hwnd = hwnd; }
            public IntPtr Handle => _hwnd;
        }

        // Dialog for simple OK messages
        public class CustomMessageBox : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;

            public CustomMessageBox(string message, string caption)
            {
                message = $"\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(350 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                messageLabel.Padding = new Padding(0, 0, (int)(26 * ScaleFactor), 0);

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }
                Controls.Add(buttonHelp);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var customMessageBox = new CustomMessageBox(message, caption))
                {
                    return customMessageBox.ShowDialog();
                }
            }

        }

        // Dialog for Take Ownership
        public class TakeOwnDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;

            public TakeOwnDialog(string message, string caption)
            {
                message = $"\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(400 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                messageLabel.Padding = new Padding((int)(26 * ScaleFactor), 0, (int)(26 * ScaleFactor), 0);

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }
                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                Stop = false;
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    // Check if path exceeds MAX_PATH (260 characters) and convert to short path if needed
                    if (newFolder.Length > 260)
                    {
                        newFolder = GetShortPath(newFolder);
                    }

                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);

                    string sMsg = validateTakeOwnPath();

                    string updatedMessage = $"\n{sMsg}\n\n{StartDirectory}\n\n";
                    messageLabel.Text = updatedMessage;

                    // Recalculate dialog height based on new message size
                    using (Graphics g = CreateGraphics())
                    {
                        SizeF size = g.MeasureString(updatedMessage, new Font("Segoe UI", 10), Width);
                        Height = Math.Max((int)(150 * ScaleFactor), (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                    }

                    // Reposition OK button
                    buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                }
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var takeOwnDialog = new TakeOwnDialog(message, caption))
                {
                    return takeOwnDialog.ShowDialog();
                }
            }

            public static DialogResult Show(string message, string caption, bool showFolderPicker)
            {
                using (var takeOwnDialog = new TakeOwnDialog(message, caption))
                {
                    return takeOwnDialog.ShowDialog();
                }
            }

        }

        // Dialog for Unblock Files
        public class UnblockHereDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;

            public UnblockHereDialog(string message, string caption)
            {
                message = $"\n\n\n\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(400 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(75 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                messageLabel.Padding = new Padding((int)(26 * ScaleFactor), 0, (int)(26 * ScaleFactor), 0);

                checkboxUnblockAdmin = new CustomCheckBox();
                checkboxUnblockAdmin.Font = new Font("Segoe UI", 10);
                checkboxUnblockAdmin.Text = sAdministrator;
                checkboxUnblockAdmin.Checked = false;
                checkboxUnblockAdmin.AutoSize = true;
                checkboxUnblockAdmin.Location = new Point((int)(8 * ScaleFactor), (int)(38 * ScaleFactor));

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }
                Controls.Add(checkboxUnblockAdmin);
                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                Stop = false;
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    if (newFolder.Length > 260)
                    {
                        newFolder = GetShortPath(newFolder);
                    }

                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);

                    string updatedMessage = $"\n\n\n\n{sUnblockHere}?\n\n{StartDirectory}\n\n";
                    messageLabel.Text = updatedMessage;

                    using (Graphics g = CreateGraphics())
                    {
                        SizeF size = g.MeasureString(updatedMessage, new Font("Segoe UI", 10), Width);
                        Height = Math.Max((int)(150 * ScaleFactor), (int)(size.Height * 1.1 + (int)(75 * ScaleFactor)));
                    }

                    buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                }
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var unblockHereDialog = new UnblockHereDialog(message, caption))
                {
                    return unblockHereDialog.ShowDialog();
                }
            }

        }

        // Dialog for install/Remove
        public class TwoChoiceBox : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Button buttonYes;
            private Button buttonNo;
            private Image helpImageNormal;
            private Image helpImageHover;

            public TwoChoiceBox(string message, string caption, string button1, string button2)
            {
                int b2Width = (int)(bwidth * ScaleFactor);
                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(button2, new Font("Segoe UI", 9));
                    b2Width = Math.Max((int)size.Width, b2Width);
                }
                message = $"\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                Text = caption;
                Width = (int)(350 * ScaleFactor);
                int h = 150; if (AnyInstall) h = 174; if (Win11Install) h = 194;
                Height = (int)(h * ScaleFactor);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                messageLabel.Padding = new Padding(0, 0, (int)(26 * ScaleFactor), 0);

                checkboxTask = new CustomCheckBox();
                checkboxTask.Font = new Font("Segoe UI", 10);
                checkboxTask.Text = sInstallTask;
                checkboxTask.Checked = true;
                checkboxTask.AutoSize = true;
                checkboxTask.Location = new Point((int)(12 * ScaleFactor), (int)(60 * ScaleFactor));

                checkboxCCM = new CustomCheckBox();
                checkboxCCM.Font = new Font("Segoe UI", 10);
                checkboxCCM.Text = sCCM;
                checkboxCCM.Checked = Win10ContextMenu;
                checkboxCCM.AutoSize = true;
                checkboxCCM.Location = new Point((int)(12 * ScaleFactor), (int)(86 * ScaleFactor));

                buttonYes = new Button();
                buttonYes.Text = button1;
                buttonYes.Font = new Font("Segoe UI", 9);
                buttonYes.MinimumSize = new Size((int)(bwidth * ScaleFactor), (int)(26 * ScaleFactor));
                buttonYes.Left = (int)(10 * ScaleFactor);
                buttonYes.Top = ClientSize.Height - buttonYes.Height - (int)(12 * ScaleFactor);
                buttonYes.DialogResult = DialogResult.Yes;

                buttonNo = new Button();
                buttonNo.Text = button2;
                buttonNo.Font = new Font("Segoe UI", 9);
                buttonNo.MinimumSize = new Size((int)(bwidth * ScaleFactor), (int)(26 * ScaleFactor));
                buttonNo.Left = ClientSize.Width - b2Width - (int)(16 * ScaleFactor);
                buttonNo.Top = ClientSize.Height - buttonNo.Height - (int)(12 * ScaleFactor);
                buttonNo.DialogResult = DialogResult.No;

                if (Dark)
                {
                    buttonYes.FlatStyle = FlatStyle.Flat;
                    buttonYes.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonYes.FlatAppearance.BorderSize = 1;
                    buttonYes.BackColor = Color.FromArgb(60, 60, 60);
                    buttonYes.FlatAppearance.MouseOverBackColor = Color.Black;
                    buttonNo.FlatStyle = FlatStyle.Flat;
                    buttonNo.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonNo.FlatAppearance.BorderSize = 1;
                    buttonNo.BackColor = Color.FromArgb(60, 60, 60);
                    buttonNo.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                if (AnyInstall) Controls.Add(checkboxTask);
                if (Win11Install) Controls.Add(checkboxCCM);
                Controls.Add(buttonHelp);
                Controls.Add(buttonYes);
                Controls.Add(buttonNo);
                Controls.Add(messageLabel);

                int x = 50; if (Win11Install) x = 40;
                Location = GetDialogPosition(this, -(int)(x * ScaleFactor));
            }

            public static DialogResult Show(string message, string caption, string button1, string button2)
            {
                using (var TwoChoiceBox = new TwoChoiceBox(message, caption, button1, button2))
                {
                    return TwoChoiceBox.ShowDialog();
                }
            }

        }

        // Dialog for Setup
        public class SetupDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private ToggleSwitch toggleTask;
            private Label labelTask;
            private ToggleSwitch toggleClassicContextMenu;
            private Label labelClassicContextMenu;
            private ToggleSwitch toggleCustomContextMenu;
            private Label labelCustomContextMenu;
            private ToggleSwitch toggleWin11ClassicMenu;
            private Label labelWin11ClassicMenu;

            // Public properties to access toggle states
            public bool InstallTask => toggleTask?.Checked ?? false;
            public bool InstallClassicContextMenu => toggleClassicContextMenu.Checked;
            public bool InstallCustomContextMenu => toggleCustomContextMenu?.Checked ?? false;
            public bool EnableWin11ClassicMenu => toggleWin11ClassicMenu?.Checked ?? false;

            public SetupDialog(string message, string caption, bool showCCM, bool showWin11Toggle)
            {
                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                Text = caption;
                Width = (int)(380 * ScaleFactor);
                int baseHeight = 200;
                if (isAdmin) baseHeight += 30;
                if (showCCM) baseHeight += 30;
                if (showWin11Toggle) baseHeight += 30;
                Height = (int)(baseHeight * ScaleFactor);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                // Help button (top right)
                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                // Message label (title area, not docked)
                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.AutoSize = false;
                messageLabel.Location = new Point((int)(10 * ScaleFactor), (int)(35 * ScaleFactor));
                messageLabel.Width = ClientSize.Width - (int)(20 * ScaleFactor);
                messageLabel.Height = (int)(25 * ScaleFactor);

                int yPos = (int)(70 * ScaleFactor);

                // Classic context menu toggle (always shown)
                toggleClassicContextMenu = new ToggleSwitch();
                toggleClassicContextMenu.Checked = IsClassicContextMenuInstalled();
                toggleClassicContextMenu.Location = new Point((int)(12 * ScaleFactor), yPos + (int)(3 * ScaleFactor));

                labelClassicContextMenu = new Label();
                labelClassicContextMenu.Text = sClassicContextMenu;
                labelClassicContextMenu.Font = new Font("Segoe UI", 10);
                labelClassicContextMenu.AutoSize = true;
                labelClassicContextMenu.Location = new Point((int)(60 * ScaleFactor), yPos);
                labelClassicContextMenu.Cursor = Cursors.Hand;
                labelClassicContextMenu.Click += (s, e) => toggleClassicContextMenu.Checked = !toggleClassicContextMenu.Checked;
                yPos += (int)(32 * ScaleFactor);

                // Privilege elevation task toggle (only if admin)
                if (isAdmin)
                {
                    toggleTask = new ToggleSwitch();
                    toggleTask.Checked = TaskExists();
                    toggleTask.Location = new Point((int)(12 * ScaleFactor), yPos + (int)(3 * ScaleFactor));

                    labelTask = new Label();
                    labelTask.Text = sInstallTask;
                    labelTask.Font = new Font("Segoe UI", 10);
                    labelTask.AutoSize = true;
                    labelTask.Location = new Point((int)(60 * ScaleFactor), yPos);
                    labelTask.Cursor = Cursors.Hand;
                    labelTask.Click += (s, e) => toggleTask.Checked = !toggleTask.Checked;
                    yPos += (int)(32 * ScaleFactor);
                }

                // Custom Context Menu toggle (only if CCM third-party app is installed)
                if (showCCM)
                {
                    toggleCustomContextMenu = new ToggleSwitch();
                    toggleCustomContextMenu.Checked = IsCustomContextMenuInstalled();
                    toggleCustomContextMenu.Location = new Point((int)(12 * ScaleFactor), yPos + (int)(3 * ScaleFactor));

                    labelCustomContextMenu = new Label();
                    labelCustomContextMenu.Text = sCustomContextMenu;
                    labelCustomContextMenu.Font = new Font("Segoe UI", 10);
                    labelCustomContextMenu.AutoSize = true;
                    labelCustomContextMenu.Location = new Point((int)(60 * ScaleFactor), yPos);
                    labelCustomContextMenu.Cursor = Cursors.Hand;
                    labelCustomContextMenu.Click += (s, e) => toggleCustomContextMenu.Checked = !toggleCustomContextMenu.Checked;
                    yPos += (int)(32 * ScaleFactor);
                }

                // Windows 11 classic menu toggle (only if Win11)
                if (showWin11Toggle)
                {
                    toggleWin11ClassicMenu = new ToggleSwitch();
                    toggleWin11ClassicMenu.Checked = Win10ContextMenu;
                    toggleWin11ClassicMenu.Location = new Point((int)(12 * ScaleFactor), yPos + (int)(3 * ScaleFactor));

                    labelWin11ClassicMenu = new Label();
                    labelWin11ClassicMenu.Text = sWin11ClassicMenu;
                    labelWin11ClassicMenu.Font = new Font("Segoe UI", 10);
                    labelWin11ClassicMenu.AutoSize = true;
                    labelWin11ClassicMenu.Location = new Point((int)(60 * ScaleFactor), yPos);
                    labelWin11ClassicMenu.Cursor = Cursors.Hand;
                    labelWin11ClassicMenu.Click += (s, e) => toggleWin11ClassicMenu.Checked = !toggleWin11ClassicMenu.Checked;
                    yPos += (int)(32 * ScaleFactor);
                }

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.MinimumSize = new Size((int)(bwidth * ScaleFactor), (int)(26 * ScaleFactor));
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(12 * ScaleFactor);
                buttonOK.DialogResult = DialogResult.OK;

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                Controls.Add(buttonHelp);
                Controls.Add(messageLabel);
                Controls.Add(toggleClassicContextMenu);
                Controls.Add(labelClassicContextMenu);
                if (isAdmin)
                {
                    Controls.Add(toggleTask);
                    Controls.Add(labelTask);
                }
                if (showCCM)
                {
                    Controls.Add(toggleCustomContextMenu);
                    Controls.Add(labelCustomContextMenu);
                }
                if (showWin11Toggle)
                {
                    Controls.Add(toggleWin11ClassicMenu);
                    Controls.Add(labelWin11ClassicMenu);
                }
                Controls.Add(buttonOK);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            public static SetupDialog Show(string message, string caption, bool showCCM, bool showWin11Toggle)
            {
                var setupDialog = new SetupDialog(message, caption, showCCM, showWin11Toggle);
                setupDialog.ShowDialog();
                return setupDialog;
            }

        }

        // Dialog for User/Administrator/TrustedInstaller options
        public class ThreeChoiceBox : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button ButtonUser;
            private Button ButtonAdministrator;
            private Button ButtonTrustedInstaller;
            private Image folderImageNormal;
            private Image folderImageHover;
            private Image helpImageNormal;
            private Image helpImageHover;

            public ThreeChoiceBox(string message, string caption, string button1, string button2, string button3, bool showTrustedInstaller = true)
            {
                // Calculate maximum button width needed
                int maxButtonWidth = (int)(bwidth * ScaleFactor);
                using (Graphics g = CreateGraphics())
                {
                    SizeF size1 = g.MeasureString(button1, new Font("Segoe UI", 9));
                    SizeF size2 = g.MeasureString(button2, new Font("Segoe UI", 9));
                    SizeF size3 = showTrustedInstaller ? g.MeasureString(button3, new Font("Segoe UI", 9)) : SizeF.Empty;
                    maxButtonWidth = Math.Max(maxButtonWidth, (int)Math.Max(Math.Max(size1.Width, size2.Width), showTrustedInstaller ? size3.Width : 0) + 20);
                }

                message = $"\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                Text = caption;
                Width = (int)(350 * ScaleFactor);
                Height = showTrustedInstaller ? (int)(180 * ScaleFactor) : (int)(160 * ScaleFactor);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    int baseHeight = showTrustedInstaller ? 180 : 160;
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(baseHeight * ScaleFactor)));
                }

                // Folder picker button (upper left)
                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                // Help button (upper right)
                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                messageLabel.Padding = new Padding((int)(26 * ScaleFactor), 0, (int)(26 * ScaleFactor), 0);

                int buttonHeight = (int)(26 * ScaleFactor);
                int buttonSpacing = (int)(8 * ScaleFactor);
                int numButtons = showTrustedInstaller ? 3 : 2;
                int startY = ClientSize.Height - (numButtons * buttonHeight + (numButtons - 1) * buttonSpacing) - (int)(12 * ScaleFactor);
                int centerX = (ClientSize.Width - maxButtonWidth) / 2;

                // User button (top)
                ButtonUser = new Button();
                ButtonUser.Text = button1;
                ButtonUser.Font = new Font("Segoe UI", 9);
                ButtonUser.Width = maxButtonWidth;
                ButtonUser.Height = buttonHeight;
                ButtonUser.Left = centerX;
                ButtonUser.Top = startY;
                ButtonUser.DialogResult = DialogResult.OK;

                // Administrator button (middle)
                ButtonAdministrator = new Button();
                ButtonAdministrator.Text = button2;
                ButtonAdministrator.Font = new Font("Segoe UI", 9);
                ButtonAdministrator.Width = maxButtonWidth;
                ButtonAdministrator.Height = buttonHeight;
                ButtonAdministrator.Left = centerX;
                ButtonAdministrator.Top = startY + buttonHeight + buttonSpacing;
                ButtonAdministrator.DialogResult = DialogResult.Yes;

                // TrustedInstaller button (bottom) - only if showTrustedInstaller is true
                if (showTrustedInstaller)
                {
                    ButtonTrustedInstaller = new Button();
                    ButtonTrustedInstaller.Text = button3;
                    ButtonTrustedInstaller.Font = new Font("Segoe UI", 9);
                    ButtonTrustedInstaller.Width = maxButtonWidth;
                    ButtonTrustedInstaller.Height = buttonHeight;
                    ButtonTrustedInstaller.Left = centerX;
                    ButtonTrustedInstaller.Top = startY + 2 * (buttonHeight + buttonSpacing);
                    ButtonTrustedInstaller.DialogResult = DialogResult.No;
                }

                if (Dark)
                {
                    ButtonUser.FlatStyle = FlatStyle.Flat;
                    ButtonUser.FlatAppearance.BorderColor = SystemColors.Highlight;
                    ButtonUser.FlatAppearance.BorderSize = 1;
                    ButtonUser.BackColor = Color.FromArgb(60, 60, 60);
                    ButtonUser.FlatAppearance.MouseOverBackColor = Color.Black;

                    ButtonAdministrator.FlatStyle = FlatStyle.Flat;
                    ButtonAdministrator.FlatAppearance.BorderColor = SystemColors.Highlight;
                    ButtonAdministrator.FlatAppearance.BorderSize = 1;
                    ButtonAdministrator.BackColor = Color.FromArgb(60, 60, 60);
                    ButtonAdministrator.FlatAppearance.MouseOverBackColor = Color.Black;

                    if (showTrustedInstaller)
                    {
                        ButtonTrustedInstaller.FlatStyle = FlatStyle.Flat;
                        ButtonTrustedInstaller.FlatAppearance.BorderColor = SystemColors.Highlight;
                        ButtonTrustedInstaller.FlatAppearance.BorderSize = 1;
                        ButtonTrustedInstaller.BackColor = Color.FromArgb(60, 60, 60);
                        ButtonTrustedInstaller.FlatAppearance.MouseOverBackColor = Color.Black;
                    }

                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(ButtonUser);
                Controls.Add(ButtonAdministrator);
                if (showTrustedInstaller)
                {
                    Controls.Add(ButtonTrustedInstaller);
                }
                Controls.Add(messageLabel);

                int x = 15; if (Win11Install) x = 40;
                Location = GetDialogPosition(this, -(int)(x * ScaleFactor));
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);
                }
            }

            private static Image CreateTransparentImage(Image original, float opacity)
            {
                Bitmap transparentBitmap = new Bitmap(original.Width, original.Height);
                using (Graphics g = Graphics.FromImage(transparentBitmap))
                {
                    ColorMatrix colorMatrix = new ColorMatrix();
                    colorMatrix.Matrix33 = opacity;
                    ImageAttributes imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                return transparentBitmap;
            }

            public static DialogResult Show(string message, string caption, string button1, string button2, string button3, bool showTrustedInstaller = true, bool topmost = false, Action<DialogResult> onResult = null)
            {
                using (var ThreeChoiceBox = new ThreeChoiceBox(message, caption, button1, button2, button3, showTrustedInstaller))
                {
                    if (topmost && explorerHwnd != IntPtr.Zero)
                    {
                        ThreeChoiceBox.FormClosing += (s, e) =>
                        {
                            if (ThreeChoiceBox.DialogResult != DialogResult.Cancel)
                            {
                                // Hide the form before invoking the action for Admin/TrustedInstaller
                                // so the dialog disappears before any UAC prompt appears.
                                // For User (OK), keep the form visible so the launched app can take foreground.
                                if (ThreeChoiceBox.DialogResult != DialogResult.OK)
                                    ThreeChoiceBox.Hide();
                                onResult?.Invoke(ThreeChoiceBox.DialogResult);
                                Program.GrantForegroundRights();
                            }
                        };
                        return ThreeChoiceBox.ShowDialog(new WindowWrapper(explorerHwnd));
                    }
                    if (topmost) ThreeChoiceBox.TopMost = true;
                    return ThreeChoiceBox.ShowDialog();
                }
            }

        }
        static void ButtonHelp_Click(object sender, EventArgs e)
        {
            string helpURL = "https://lesferch.github.io/RightClickTools#";
            try
            {
                string regValue = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "helpURL", "");
                if (!string.IsNullOrEmpty(regValue)) helpURL = regValue;
            }
            catch { }
            Process.Start(helpURL + helpPage);
        }

        // Dialog for Search Helper
        public class SearchHelperDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;

            private CustomComboBox combo1;
            private TextBox text1;
            private CustomComboBox kindPresetsCombo;
            private CustomComboBox combo2;
            private TextBox text2;
            private CustomComboBox sizePresetsCombo;
            private CustomComboBox combo3;
            private TextBox text3;
            private CustomComboBox combo4;
            private TextBox text4;
            private CustomComboBox combo5;
            private TextBox text5;
            private CustomComboBox datePresetsCombo;
            private Button pickDateButton;
            private Button dateRangeButton;
            private Label folderPathLabel;
            private FlatScrollBar folderPathScrollBar;
            private Label tipLabel;
            private TextBox queryTextBox;
            private ToggleSwitch customToggle;
            private Label customLabel;
            private TextBox customTextBox;
            private Button copyButton;
            private Button moreButton;
            private Button editHistoryButton;
            private bool customFieldManuallyEdited = false;
            private Panel historyPanel;
            private System.Collections.Generic.List<string> searchHistory = new System.Collections.Generic.List<string>();
            private int hoveredLineIndex = -1;
            private string historyFilePath;
            private int historyScrollOffset = 0;
            private System.IO.FileSystemWatcher historyFileWatcher;
            private bool autoClose = false;

            public SearchHelperDialog(string message, string caption)
            {
                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.Sizable;
                Text = caption;
                this.DoubleBuffered = true;
                int baseWidth = 550;
                int dialogHeight = (int)(590 * ScaleFactor);

                // Initialize history file path
                historyFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RightClickTools", "Searches.txt");

                // Read AutoClose setting from [SearchHere] section in RightClickTools.ini
                autoClose = ReadString(myIniFile, "SearchHere", "AutoClose", "0") == "1";

                // Load search history from file
                try
                {
                    if (System.IO.File.Exists(historyFilePath))
                    {
                        string[] lines = System.IO.File.ReadAllLines(historyFilePath);
                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                searchHistory.Add(line.Trim());
                            }
                        }
                    }
                }
                catch { }

                // Set up file watcher to reload history when file changes
                try
                {
                    string directory = System.IO.Path.GetDirectoryName(historyFilePath);
                    string fileName = System.IO.Path.GetFileName(historyFilePath);

                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }

                    historyFileWatcher = new System.IO.FileSystemWatcher(directory, fileName);
                    historyFileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size;
                    historyFileWatcher.Changed += (s, ev) =>
                    {
                        // Reload history on file change
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                try
                                {
                                    searchHistory.Clear();
                                    if (System.IO.File.Exists(historyFilePath))
                                    {
                                        string[] lines = System.IO.File.ReadAllLines(historyFilePath);
                                        foreach (string line in lines)
                                        {
                                            if (!string.IsNullOrWhiteSpace(line))
                                            {
                                                searchHistory.Add(line.Trim());
                                            }
                                        }
                                    }
                                    historyPanel.Invalidate();
                                }
                                catch { }
                            }));
                        }
                    };
                    historyFileWatcher.EnableRaisingEvents = true;
                }
                catch { }

                bool customToggleWasOn = false;

                // Load saved width and height from registry
                try
                {
                    using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\RightClickTools"))
                    {
                        if (key != null)
                        {
                            object savedWidth = key.GetValue("SearchHereWidth");
                            if (savedWidth != null)
                            {
                                int width = (int)savedWidth;
                                if (width >= (int)(455 * ScaleFactor))
                                {
                                    baseWidth = (int)(width / ScaleFactor);
                                }
                            }

                            object savedHeight = key.GetValue("SearchHereHeight");
                            if (savedHeight != null)
                            {
                                int height = (int)savedHeight;
                                if (height >= (int)(590 * ScaleFactor))
                                {
                                    dialogHeight = height;
                                }
                            }

                            object savedCustomToggle = key.GetValue("SearchHereCustomToggle");
                            if (savedCustomToggle != null)
                            {
                                customToggleWasOn = (int)savedCustomToggle != 0;
                            }
                        }
                    }
                }
                catch { }

                Width = (int)(baseWidth * ScaleFactor);
                Height = dialogHeight;
                MinimumSize = new Size((int)(488 * ScaleFactor), (int)(590 * ScaleFactor));
                MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                MaximizeBox = false;
                MinimizeBox = false;

                // Save width and height to registry on close
                FormClosing += (s, e) =>
                {
                    try
                    {
                        using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\RightClickTools"))
                        {
                            key.SetValue("SearchHereWidth", Width, Microsoft.Win32.RegistryValueKind.DWord);
                            key.SetValue("SearchHereHeight", Height, Microsoft.Win32.RegistryValueKind.DWord);
                            key.SetValue("SearchHereCustomToggle", customToggle.Checked ? 1 : 0, Microsoft.Win32.RegistryValueKind.DWord);
                        }
                    }
                    catch { }

                    // Dispose file watcher
                    if (historyFileWatcher != null)
                    {
                        historyFileWatcher.EnableRaisingEvents = false;
                        historyFileWatcher.Dispose();
                    }
                };

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                // Message label for title (centered between folder and help icons)
                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.MiddleCenter;
                messageLabel.AutoSize = false;
                messageLabel.Location = new Point((int)(35 * ScaleFactor), (int)(5 * ScaleFactor));
                messageLabel.Width = ClientSize.Width - (int)(70 * ScaleFactor);
                messageLabel.Height = (int)(20 * ScaleFactor);
                messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                int comboWidth = (int)(100 * ScaleFactor);
                int textWidth = (int)((baseWidth - 137) * ScaleFactor);
                int controlHeight = (int)(24 * ScaleFactor);
                int spacing = (int)(30 * ScaleFactor);
                int xCombo = (int)(10 * ScaleFactor);
                int xText = xCombo + comboWidth + (int)(5 * ScaleFactor);

                // Folder path display at the top
                int folderPathLabelHeight = controlHeight;
                folderPathLabel = new Label();
                folderPathLabel.Font = new Font("Segoe UI", 9);
                folderPathLabel.Location = new Point(xCombo, (int)(40 * ScaleFactor));
                folderPathLabel.Width = comboWidth + textWidth + (int)(5 * ScaleFactor);
                folderPathLabel.Height = folderPathLabelHeight;
                folderPathLabel.AutoSize = false;
                folderPathLabel.BorderStyle = BorderStyle.None;
                folderPathLabel.Padding = new Padding(2, 2, 2, 2);
                folderPathLabel.BackColor = SystemColors.Control;
                folderPathLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                // Custom paint to ensure text is always vertically centered
                folderPathLabel.Paint += (s, pe) =>
                {
                    Label lbl = s as Label;
                    if (lbl != null)
                    {
                        pe.Graphics.Clear(lbl.BackColor);

                        if (!string.IsNullOrEmpty(lbl.Text))
                        {
                            Rectangle textRect = new Rectangle(
                                lbl.Padding.Left, 
                                0, 
                                lbl.Width - lbl.Padding.Left - lbl.Padding.Right, 
                                lbl.Height);
                            TextRenderer.DrawText(
                                pe.Graphics, 
                                lbl.Text, 
                                lbl.Font, 
                                textRect, 
                                lbl.ForeColor, 
                                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                        }

                        // Always draw thin border
                        Color borderColor = Dark ? Color.FromArgb(100, 100, 100) : Color.FromArgb(171, 173, 179);
                        using (Pen borderPen = new Pen(borderColor, 1))
                        {
                            pe.Graphics.DrawRectangle(borderPen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                        }
                    }
                };

                // Add horizontal scrollbar for folder path
                folderPathScrollBar = new FlatScrollBar();
                folderPathScrollBar.Orientation = ScrollBarOrientation.Horizontal;
                folderPathScrollBar.Location = new Point(xCombo, (int)(40 * ScaleFactor) + folderPathLabelHeight - 1);
                folderPathScrollBar.Width = comboWidth + textWidth + (int)(5 * ScaleFactor);
                folderPathScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
                folderPathScrollBar.Minimum = 0;
                folderPathScrollBar.SmallChange = 5;
                folderPathScrollBar.LargeChange = 20;
                folderPathScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                folderPathScrollBar.Scroll += (s, ev) =>
                {
                    int offset = folderPathScrollBar.Value;
                    folderPathLabel.Text = StartDirectory.Length > offset ? StartDirectory.Substring(offset) : "";
                };

                // Calculate scrollbar range based on text length
                using (Graphics g = CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                    int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                    folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                    folderPathScrollBar.Visible = maxScroll > 0;
                }

                int yPos = (int)(40 * ScaleFactor) + folderPathLabelHeight + (folderPathScrollBar.Visible ? SystemInformation.HorizontalScrollBarHeight : 0) + (int)(10 * ScaleFactor);

                // Add tip label for query syntax
                tipLabel = new Label();
                tipLabel.Text = sSearchHelperHint;
                tipLabel.Font = new Font("Segoe UI", 8.25f, FontStyle.Italic);
                tipLabel.ForeColor = SystemColors.GrayText;
                tipLabel.AutoSize = false;
                tipLabel.Location = new Point(xCombo, yPos);
                tipLabel.Width = comboWidth + textWidth + (int)(5 * ScaleFactor);
                tipLabel.Height = (int)(32 * ScaleFactor);
                tipLabel.TextAlign = ContentAlignment.MiddleLeft;
                tipLabel.Padding = new Padding((int)(4 * ScaleFactor), (int)(2 * ScaleFactor), (int)(4 * ScaleFactor), (int)(2 * ScaleFactor));
                tipLabel.BackColor = Color.FromArgb(242, 242, 235);
                tipLabel.BorderStyle = BorderStyle.FixedSingle;
                tipLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += (int)(42 * ScaleFactor);

                combo1 = new CustomComboBox();
                combo1.Items.AddRange(new string[] { sKindLabel, sExtLabel });
                combo1.SelectedIndex = 0;
                combo1.SelectedIndexChanged += UpdateQuery;
                combo1.SelectedIndexChanged += (s, ev) =>
                {
                    kindPresetsCombo.Visible = combo1.SelectedItem?.ToString() == sKindLabel;
                };
                combo1.Font = new Font("Segoe UI", 9);
                combo1.Location = new Point(xCombo, yPos);
                combo1.Width = comboWidth;
                combo1.Height = controlHeight;

                text1 = new TextBox();
                text1.Font = new Font("Segoe UI", 9);
                text1.Location = new Point(xText, yPos);
                text1.Width = textWidth;
                text1.Height = controlHeight;
                text1.TextChanged += UpdateQuery;
                text1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += spacing;

                // Kind Presets combobox
                kindPresetsCombo = new CustomComboBox();
                kindPresetsCombo.PlaceholderText = sKindPresets;
                kindPresetsCombo.Items.AddRange(new string[] { sKindText, sKindDocument, sKindPicture, sKindMusic, sKindVideo, sKindFolder });
                kindPresetsCombo.SelectedIndex = -1; // Show placeholder
                kindPresetsCombo.SelectedIndexChanged += KindPresetsCombo_SelectedIndexChanged;
                kindPresetsCombo.Font = new Font("Segoe UI", 9);
                kindPresetsCombo.Location = new Point(xText, yPos);
                kindPresetsCombo.Width = (int)(160 * ScaleFactor);
                kindPresetsCombo.Height = controlHeight;
                kindPresetsCombo.DropDownStyle = ComboBoxStyle.DropDownList;

                // Add tooltip for Kind presets
                ToolTip kindPresetsToolTip = new ToolTip();
                kindPresetsToolTip.InitialDelay = 100;
                kindPresetsToolTip.AutoPopDelay = 1000;
                kindPresetsToolTip.SetToolTip(kindPresetsCombo, "Hold Shift to append");

                yPos += spacing;

                combo2 = new CustomComboBox();
                combo2.Items.AddRange(new string[] { sSizeLabel, sWidthLabel, sHeightLabel, sDimensionsLabel });
                combo2.SelectedIndex = 0;
                combo2.SelectedIndexChanged += UpdateQuery;
                combo2.SelectedIndexChanged += (s, ev) =>
                {
                    sizePresetsCombo.Visible = combo2.SelectedItem?.ToString() == "Size:";
                };
                combo2.Font = new Font("Segoe UI", 9);
                combo2.Location = new Point(xCombo, yPos);
                combo2.Width = comboWidth;
                combo2.Height = controlHeight;

                text2 = new TextBox();
                text2.Font = new Font("Segoe UI", 9);
                text2.Location = new Point(xText, yPos);
                text2.Width = textWidth;
                text2.Height = controlHeight;
                text2.TextChanged += UpdateQuery;
                text2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += spacing;

                // Size Presets combobox
                sizePresetsCombo = new CustomComboBox();
                sizePresetsCombo.PlaceholderText = sSizePresets;
                sizePresetsCombo.Items.AddRange(new string[] { sSizeEmpty, sSizeTiny, sSizeSmall, sSizeMedium, sSizeLarge, sSizeHuge, sSizeGigantic });
                sizePresetsCombo.SelectedIndex = -1; // Show placeholder
                sizePresetsCombo.SelectedIndexChanged += SizePresetsCombo_SelectedIndexChanged;
                sizePresetsCombo.Font = new Font("Segoe UI", 9);
                sizePresetsCombo.Location = new Point(xText, yPos);
                sizePresetsCombo.Width = (int)(160 * ScaleFactor);
                sizePresetsCombo.Height = controlHeight;
                sizePresetsCombo.DropDownStyle = ComboBoxStyle.DropDownList;

                yPos += spacing;

                combo3 = new CustomComboBox();
                combo3.Items.AddRange(new string[] { sModifiedLabel, sCreatedLabel, sDateLabel, sDateTakenLabel });
                combo3.SelectedIndex = 0;
                combo3.SelectedIndexChanged += UpdateQuery;
                combo3.Font = new Font("Segoe UI", 9);
                combo3.Location = new Point(xCombo, yPos);
                combo3.Width = comboWidth;
                combo3.Height = controlHeight;

                text3 = new TextBox();
                text3.Font = new Font("Segoe UI", 9);
                text3.Location = new Point(xText, yPos);
                text3.Width = textWidth;
                text3.Height = controlHeight;
                text3.TextChanged += UpdateQuery;
                text3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += spacing;

                // Date Presets combobox
                datePresetsCombo = new CustomComboBox();
                datePresetsCombo.PlaceholderText = sDatePresets;
                datePresetsCombo.Items.AddRange(new string[] { sDateToday, sDateYesterday, sDateThisWeek, sDateLastWeek, sDateThisMonth, sDateLastMonth, sDateThisYear, sDateLastYear });
                datePresetsCombo.SelectedIndex = -1; // Show placeholder
                datePresetsCombo.SelectedIndexChanged += DatePresetsCombo_SelectedIndexChanged;
                datePresetsCombo.Font = new Font("Segoe UI", 9);
                datePresetsCombo.Location = new Point(xText, yPos);
                datePresetsCombo.Width = (int)(160 * ScaleFactor);
                datePresetsCombo.Height = controlHeight;
                datePresetsCombo.DropDownStyle = ComboBoxStyle.DropDownList;

                // Pick a date button
                pickDateButton = new Button();
                pickDateButton.Text = sPickADate;
                pickDateButton.Font = new Font("Segoe UI", 9);
                pickDateButton.Width = (int)(90 * ScaleFactor);
                pickDateButton.Height = controlHeight;
                pickDateButton.Click += PickDateButton_Click;
                // Center-align with combobox
                int buttonOffset = (datePresetsCombo.Height - pickDateButton.Height) / 2;
                pickDateButton.Location = new Point(xText + (int)(167 * ScaleFactor), yPos + buttonOffset);

                // Date range button
                dateRangeButton = new Button();
                dateRangeButton.Text = sDateRange;
                dateRangeButton.Font = new Font("Segoe UI", 9);
                dateRangeButton.Width = (int)(90 * ScaleFactor);
                dateRangeButton.Height = controlHeight;
                dateRangeButton.Click += DateRangeButton_Click;
                // Center-align with combobox
                dateRangeButton.Location = new Point(xText + (int)(262 * ScaleFactor), yPos + buttonOffset);

                yPos += spacing;

                combo4 = new CustomComboBox();
                combo4.Items.AddRange(new string[] { sContentsLabel, sTagsLabel });
                combo4.SelectedIndex = 0;
                combo4.SelectedIndexChanged += UpdateQuery;
                combo4.Font = new Font("Segoe UI", 9);
                combo4.Location = new Point(xCombo, yPos);
                combo4.Width = comboWidth;
                combo4.Height = controlHeight;

                text4 = new TextBox();
                text4.Font = new Font("Segoe UI", 9);
                text4.Location = new Point(xText, yPos);
                text4.Width = textWidth;
                text4.Height = controlHeight;
                text4.TextChanged += UpdateQuery;
                text4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += spacing;

                combo5 = new CustomComboBox();
                combo5.Items.AddRange(new string[] { sNameLabel, sTitleLabel });
                combo5.SelectedIndex = 0;
                combo5.SelectedIndexChanged += UpdateQuery;
                combo5.Font = new Font("Segoe UI", 9);
                combo5.Location = new Point(xCombo, yPos);
                combo5.Width = comboWidth;
                combo5.Height = controlHeight;

                text5 = new TextBox();
                text5.Font = new Font("Segoe UI", 9);
                text5.Location = new Point(xText, yPos);
                text5.Width = textWidth;
                text5.Height = controlHeight;
                text5.TextChanged += UpdateQuery;
                text5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += spacing + (int)(10 * ScaleFactor);

                queryTextBox = new TextBox();
                queryTextBox.Font = new Font("Segoe UI", 9);
                queryTextBox.Location = new Point(xCombo, yPos);
                queryTextBox.Width = comboWidth + textWidth + (int)(5 * ScaleFactor);
                queryTextBox.Height = controlHeight;
                queryTextBox.ReadOnly = true;
                queryTextBox.BackColor = SystemColors.Control;
                queryTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += spacing;

                // Custom toggle and label
                customLabel = new Label();
                customLabel.Text = sCustom;
                customLabel.Font = new Font("Segoe UI", 9);
                customLabel.Location = new Point(xCombo, yPos + (int)(2 * ScaleFactor));
                customLabel.AutoSize = true;

                // Measure the label width to position toggle and buttons dynamically
                using (Graphics g = this.CreateGraphics())
                {
                    SizeF labelSize = g.MeasureString(sCustom, customLabel.Font);
                    int toggleX = xCombo + (int)labelSize.Width + (int)(10 * ScaleFactor);

                    customToggle = new ToggleSwitch();
                    customToggle.Location = new Point(toggleX, yPos);
                    customToggle.CheckedChanged += CustomToggle_CheckedChanged;

                    // Copy button (beside the toggle, initially hidden)
                    copyButton = new Button();
                    copyButton.Text = sCopy;
                    copyButton.Font = new Font("Segoe UI", 9);
                    copyButton.Width = (int)(60 * ScaleFactor);
                    copyButton.Height = (int)(24 * ScaleFactor);
                    copyButton.Location = new Point(toggleX + (int)(60 * ScaleFactor), yPos - (int)(2 * ScaleFactor));
                    copyButton.Visible = false;
                    copyButton.Click += CopyButton_Click;

                    // More button (beside the copy button, initially hidden)
                    moreButton = new Button();
                    moreButton.Text = sMore;
                    moreButton.Font = new Font("Segoe UI", 9);
                    moreButton.Width = (int)(60 * ScaleFactor);
                    moreButton.Height = (int)(24 * ScaleFactor);
                    moreButton.Location = new Point(toggleX + (int)(125 * ScaleFactor), yPos - (int)(2 * ScaleFactor));
                    moreButton.Visible = false;
                    moreButton.Click += (s, ev) =>
                    {
                        PropertySelectorDialog.Show(customTextBox);
                    };
                }

                yPos += spacing;

                // Custom text field (initially hidden)
                customTextBox = new TextBox();
                customTextBox.Font = new Font("Segoe UI", 9);
                customTextBox.Location = new Point(xCombo, yPos);
                customTextBox.Width = comboWidth + textWidth + (int)(5 * ScaleFactor);
                customTextBox.Height = controlHeight;
                customTextBox.Visible = false;
                customTextBox.TextChanged += CustomTextBox_TextChanged;
                customTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                yPos += (int)(5 * ScaleFactor);

                // History panel
                int historyPanelHeight = (int)(80 * ScaleFactor);
                int historyLineHeight = (int)(20 * ScaleFactor);

                historyPanel = new Panel();
                historyPanel.Location = new Point(xCombo, yPos);
                historyPanel.Width = comboWidth + textWidth + (int)(5 * ScaleFactor);
                historyPanel.Height = historyPanelHeight;
                historyPanel.BorderStyle = BorderStyle.FixedSingle;
                historyPanel.BackColor = SystemColors.Window;
                historyPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                // Enable double buffering to prevent flicker
                historyPanel.GetType().GetProperty("DoubleBuffered", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .SetValue(historyPanel, true, null);

                // Add tooltip for history panel
                ToolTip historyToolTip = new ToolTip();
                historyToolTip.InitialDelay = 100;
                historyToolTip.AutoPopDelay = 1000;
                historyToolTip.SetToolTip(historyPanel, "Ctrl-click to delete");

                // Edit button overlaid at upper right of history panel
                editHistoryButton = new Button();
                editHistoryButton.Text = sEdit;
                editHistoryButton.Font = new Font("Segoe UI", 9);
                editHistoryButton.Width = (int)(50 * ScaleFactor);
                editHistoryButton.Height = (int)(22 * ScaleFactor);
                editHistoryButton.Location = new Point(
                    historyPanel.Width - editHistoryButton.Width - 4,
                    2);
                editHistoryButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                editHistoryButton.FlatStyle = FlatStyle.Flat;
                editHistoryButton.FlatAppearance.BorderSize = 1;
                editHistoryButton.FlatAppearance.BorderColor = Color.FromArgb(173, 173, 173);
                editHistoryButton.Click += (s, ev) =>
                {
                    try
                    {
                        Process.Start(EditorExe, historyFilePath);
                    }
                    catch { }
                };

                // Add Edit button to history panel so it appears on top
                historyPanel.Controls.Add(editHistoryButton);

                // Custom paint for history items
                historyPanel.Paint += (s, pe) =>
                {
                    Panel panel = s as Panel;
                    if (panel != null)
                    {
                        pe.Graphics.Clear(panel.BackColor);

                        for (int i = 0; i < searchHistory.Count; i++)
                        {
                            int yLine = i * historyLineHeight - historyScrollOffset;

                            // Only draw lines that are visible
                            if (yLine + historyLineHeight >= 0 && yLine < panel.Height)
                            {
                                Rectangle lineRect = new Rectangle(0, yLine, panel.Width, historyLineHeight);

                                // Highlight on hover
                                if (i == hoveredLineIndex)
                                {
                                    Color hoverColor = Dark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(229, 241, 251);
                                    pe.Graphics.FillRectangle(new SolidBrush(hoverColor), lineRect);
                                }

                                // Draw text
                                Rectangle textRect = new Rectangle(5, yLine, panel.Width - 10, historyLineHeight);
                                TextRenderer.DrawText(pe.Graphics, searchHistory[i], panel.Font, textRect, panel.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                            }
                        }
                    }
                };

                // Mouse wheel for scrolling
                historyPanel.MouseWheel += (s, ev) =>
                {
                    int totalContentHeight = searchHistory.Count * historyLineHeight;
                    int maxScroll = Math.Max(0, totalContentHeight - historyPanel.Height);

                    historyScrollOffset -= ev.Delta / 3;
                    historyScrollOffset = Math.Max(0, Math.Min(maxScroll, historyScrollOffset));

                    historyPanel.Invalidate();
                };

                // Mouse move for hover effect
                historyPanel.MouseMove += (s, ev) =>
                {
                    int index = (ev.Y + historyScrollOffset) / historyLineHeight;

                    if (index >= 0 && index < searchHistory.Count)
                    {
                        if (hoveredLineIndex != index)
                        {
                            hoveredLineIndex = index;
                            historyPanel.Invalidate();
                        }
                    }
                    else
                    {
                        if (hoveredLineIndex != -1)
                        {
                            hoveredLineIndex = -1;
                            historyPanel.Invalidate();
                        }
                    }
                };

                // Mouse leave for hover effect
                historyPanel.MouseLeave += (s, ev) =>
                {
                    hoveredLineIndex = -1;
                    historyPanel.Invalidate();
                };

                // Click handler for history items
                historyPanel.MouseClick += (s, ev) =>
                {
                    int index = (ev.Y + historyScrollOffset) / historyLineHeight;

                    if (index >= 0 && index < searchHistory.Count)
                    {
                        // Check if Ctrl is held down for deletion
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            // Delete the line from history
                            searchHistory.RemoveAt(index);

                            // Update the history file
                            try
                            {
                                string directory = System.IO.Path.GetDirectoryName(historyFilePath);
                                if (!System.IO.Directory.Exists(directory))
                                {
                                    System.IO.Directory.CreateDirectory(directory);
                                }

                                System.IO.File.WriteAllLines(historyFilePath, searchHistory);
                            }
                            catch { }

                            // Clear hover state if we deleted the hovered item
                            if (hoveredLineIndex == index)
                            {
                                hoveredLineIndex = -1;
                            }
                            else if (hoveredLineIndex > index)
                            {
                                hoveredLineIndex--;
                            }

                            historyPanel.Invalidate();
                        }
                        else
                        {
                            // Normal click behavior
                            string selectedQuery = searchHistory[index];

                            if (customToggle.Checked)
                            {
                                // If Custom is on, place in Custom field
                                customTextBox.TextChanged -= CustomTextBox_TextChanged;
                                customTextBox.Text = selectedQuery;
                                customTextBox.TextChanged += CustomTextBox_TextChanged;
                                customFieldManuallyEdited = true;
                            }
                            else
                            {
                                // If Custom is off, execute the search
                                // Windows 7 doesn't support search-ms: protocol
                                if (buildNumber < 9200) // Windows 7 and earlier (Windows 8 is build 9200+)
                                {
                                    // Copy query to clipboard and open Explorer to the location
                                    try
                                    {
                                        Clipboard.SetText(selectedQuery);
                                        Process.Start("explorer.exe", StartDirectory);
                                    }
                                    catch { }
                                }
                                else
                                {
                                    // Windows 8 and later support search-ms: protocol
                                    string searchMsQuery = $"search-ms:query={selectedQuery}&crumb=location:{StartDirectory}";
                                    Process.Start(searchMsQuery);
                                }

                                // Close dialog if AutoClose is enabled
                                if (autoClose)
                                {
                                    this.Close();
                                }
                            }
                        }
                    }
                };

                yPos += historyPanelHeight + (int)(10 * ScaleFactor);

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                buttonOK.Click += ButtonOK_Click;
                buttonOK.Anchor = AnchorStyles.Bottom;

                // Recenter buttonOK on resize
                this.Resize += (s, ev) =>
                {
                    buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;

                    // Adjust history panel height to fill available space
                    int availableHeight = ClientSize.Height - historyPanel.Top - buttonOK.Height - (int)(20 * ScaleFactor);
                    int minHistoryHeight = (int)(80 * ScaleFactor);
                    historyPanel.Height = Math.Max(minHistoryHeight, availableHeight);
                };

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    copyButton.FlatStyle = FlatStyle.Flat;
                    copyButton.FlatAppearance.BorderColor = SystemColors.Highlight;
                    copyButton.FlatAppearance.BorderSize = 1;
                    copyButton.BackColor = Color.FromArgb(60, 60, 60);
                    copyButton.FlatAppearance.MouseOverBackColor = Color.Black;
                    moreButton.FlatStyle = FlatStyle.Flat;
                    moreButton.FlatAppearance.BorderColor = SystemColors.Highlight;
                    moreButton.FlatAppearance.BorderSize = 1;
                    moreButton.BackColor = Color.FromArgb(60, 60, 60);
                    moreButton.FlatAppearance.MouseOverBackColor = Color.Black;
                    editHistoryButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    editHistoryButton.BackColor = Color.FromArgb(60, 60, 60);
                    editHistoryButton.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;

                    tipLabel.BackColor = Color.FromArgb(55, 55, 45);
                    tipLabel.ForeColor = Color.FromArgb(200, 200, 200);

                    text1.BackColor = Color.FromArgb(45, 45, 45);
                    text1.ForeColor = Color.White;
                    text1.BorderStyle = BorderStyle.FixedSingle;
                    text2.BackColor = Color.FromArgb(45, 45, 45);
                    text2.ForeColor = Color.White;
                    text2.BorderStyle = BorderStyle.FixedSingle;
                    text3.BackColor = Color.FromArgb(45, 45, 45);
                    text3.ForeColor = Color.White;
                    text3.BorderStyle = BorderStyle.FixedSingle;
                    text4.BackColor = Color.FromArgb(45, 45, 45);
                    text4.ForeColor = Color.White;
                    text4.BorderStyle = BorderStyle.FixedSingle;
                    text5.BackColor = Color.FromArgb(45, 45, 45);
                    text5.ForeColor = Color.White;
                    text5.BorderStyle = BorderStyle.FixedSingle;

                    pickDateButton.FlatStyle = FlatStyle.Flat;
                    pickDateButton.FlatAppearance.BorderColor = SystemColors.Highlight;
                    pickDateButton.FlatAppearance.BorderSize = 1;
                    pickDateButton.BackColor = Color.FromArgb(60, 60, 60);
                    pickDateButton.FlatAppearance.MouseOverBackColor = Color.Black;

                    dateRangeButton.FlatStyle = FlatStyle.Flat;
                    dateRangeButton.FlatAppearance.BorderColor = SystemColors.Highlight;
                    dateRangeButton.FlatAppearance.BorderSize = 1;
                    dateRangeButton.BackColor = Color.FromArgb(60, 60, 60);
                    dateRangeButton.FlatAppearance.MouseOverBackColor = Color.Black;

                    folderPathLabel.BackColor = Color.FromArgb(45, 45, 45);
                    folderPathLabel.ForeColor = Color.White;
                    folderPathScrollBar.Theme = UITheme.VS2019DarkBlue;
                    queryTextBox.BackColor = Color.FromArgb(45, 45, 45);
                    queryTextBox.ForeColor = Color.White;
                    queryTextBox.BorderStyle = BorderStyle.FixedSingle;
                    customTextBox.BackColor = Color.FromArgb(45, 45, 45);
                    customTextBox.ForeColor = Color.White;
                    customTextBox.BorderStyle = BorderStyle.FixedSingle;
                    historyPanel.BackColor = Color.FromArgb(45, 45, 45);
                    historyPanel.ForeColor = Color.White;
                }

                Controls.Add(tipLabel);
                Controls.Add(combo1);
                Controls.Add(text1);
                Controls.Add(kindPresetsCombo);
                Controls.Add(combo2);
                Controls.Add(text2);
                Controls.Add(sizePresetsCombo);
                Controls.Add(combo3);
                Controls.Add(text3);
                Controls.Add(combo4);
                Controls.Add(text4);
                Controls.Add(combo5);
                Controls.Add(text5);
                Controls.Add(datePresetsCombo);
                Controls.Add(pickDateButton);
                Controls.Add(dateRangeButton);
                Controls.Add(folderPathLabel);
                Controls.Add(folderPathScrollBar);
                Controls.Add(queryTextBox);
                Controls.Add(customLabel);
                Controls.Add(customToggle);
                Controls.Add(customTextBox);
                Controls.Add(copyButton);
                Controls.Add(moreButton);
                Controls.Add(historyPanel);
                Controls.Add(messageLabel);
                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(buttonOK);

                folderPathLabel.Text = StartDirectory;

                // Restore custom toggle state after all controls are created
                // Note: If toggle was on when saved, the height already includes the custom field
                // so we just need to show the controls and adjust panel position
                if (customToggleWasOn)
                {
                    // Temporarily unhook the event to prevent height adjustment
                    customToggle.CheckedChanged -= CustomToggle_CheckedChanged;
                    customToggle.Checked = true;
                    customToggle.CheckedChanged += CustomToggle_CheckedChanged;

                    // Manually show the controls and move history panel down
                    customTextBox.Visible = true;
                    copyButton.Visible = true;
                    moreButton.Visible = true;
                    historyPanel.Top += spacing;

                    // Update minimum size to account for custom field
                    MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + spacing);
                    MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height + spacing);
                }

                // After custom toggle restoration, set up Load handler to adjust history panel height
                Load += (s, e) =>
                {
                    int availableHeight = ClientSize.Height - historyPanel.Top - buttonOK.Height - (int)(20 * ScaleFactor);
                    int minHistoryHeight = (int)(80 * ScaleFactor);
                    historyPanel.Height = Math.Max(minHistoryHeight, availableHeight);
                };

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void UpdateQuery(object sender, EventArgs e)
            {
                string query = "";

                if (!string.IsNullOrWhiteSpace(text1.Text))
                    query += $"{combo1.SelectedItem}{text1.Text} ";

                if (!string.IsNullOrWhiteSpace(text2.Text))
                    query += $"{combo2.SelectedItem}{text2.Text} ";

                if (!string.IsNullOrWhiteSpace(text3.Text))
                    query += $"{combo3.SelectedItem}{text3.Text} ";

                if (!string.IsNullOrWhiteSpace(text4.Text))
                    query += $"{combo4.SelectedItem}{text4.Text} ";

                if (!string.IsNullOrWhiteSpace(text5.Text))
                    query += $"{combo5.SelectedItem}{text5.Text} ";

                queryTextBox.Text = query.Trim();

                // Auto-update custom field if custom is on and not manually edited
                if (customToggle != null && customTextBox != null && customToggle.Checked && !customFieldManuallyEdited)
                {
                    customTextBox.TextChanged -= CustomTextBox_TextChanged;
                    customTextBox.Text = queryTextBox.Text;
                    customTextBox.TextChanged += CustomTextBox_TextChanged;
                }
            }

            private void FolderPathTextBox_MouseWheel(object sender, MouseEventArgs e)
            {
                // Reserved for future use with search history vertical scrolling
            }

            private void KindPresetsCombo_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (kindPresetsCombo.SelectedIndex >= 0)
                {
                    string selectedValue = kindPresetsCombo.SelectedItem.ToString();

                    // Check if Shift key is held down and there's existing text
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift && !string.IsNullOrWhiteSpace(text1.Text))
                    {
                        text1.Text += " OR " + selectedValue;
                    }
                    else
                    {
                        text1.Text = selectedValue;
                    }

                    kindPresetsCombo.SelectedIndex = -1; // Reset to show placeholder
                }
            }

            private void SizePresetsCombo_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (sizePresetsCombo.SelectedIndex >= 0)
                {
                    string selectedItem = sizePresetsCombo.SelectedItem.ToString();
                    // Extract keyword (text before the space/parenthesis)
                    int spaceIndex = selectedItem.IndexOf(' ');
                    string keyword = spaceIndex > 0 ? selectedItem.Substring(0, spaceIndex) : selectedItem;
                    text2.Text = keyword;
                    sizePresetsCombo.SelectedIndex = -1; // Reset to show placeholder
                }
            }

            private void DatePresetsCombo_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (datePresetsCombo.SelectedIndex >= 0)
                {
                    text3.Text = datePresetsCombo.SelectedItem.ToString();
                    datePresetsCombo.SelectedIndex = -1; // Reset to show placeholder
                }
            }

            private void PickDateButton_Click(object sender, EventArgs e)
            {
                using (Form dateForm = new Form())
                {
                    dateForm.Text = sPickADate;
                    dateForm.StartPosition = FormStartPosition.CenterParent;
                    dateForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dateForm.MaximizeBox = false;
                    dateForm.MinimizeBox = false;
                    dateForm.AutoSize = true;
                    dateForm.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                    FlowLayoutPanel panel = new FlowLayoutPanel();
                    panel.FlowDirection = FlowDirection.TopDown;
                    panel.AutoSize = true;
                    panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    panel.Padding = new Padding((int)(10 * ScaleFactor));

                    MonthCalendar calendar = new MonthCalendar();
                    calendar.MaxSelectionCount = 1;

                    Panel buttonPanel = new Panel();
                    buttonPanel.Height = (int)(36 * ScaleFactor);

                    Button okButton = new Button();
                    okButton.Text = sOK;
                    okButton.DialogResult = DialogResult.OK;
                    okButton.Width = (int)(75 * ScaleFactor);
                    okButton.Height = (int)(26 * ScaleFactor);
                    okButton.Top = (int)(5 * ScaleFactor);

                    if (Dark)
                    {
                        dateForm.BackColor = Color.FromArgb(50, 50, 50);
                        dateForm.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(50, 50, 50);
                        buttonPanel.BackColor = Color.FromArgb(50, 50, 50);
                        okButton.FlatStyle = FlatStyle.Flat;
                        okButton.FlatAppearance.BorderColor = SystemColors.Highlight;
                        okButton.FlatAppearance.BorderSize = 1;
                        okButton.BackColor = Color.FromArgb(60, 60, 60);
                        okButton.FlatAppearance.MouseOverBackColor = Color.Black;
                        DarkTitleBar(dateForm.Handle);
                    }

                    buttonPanel.Controls.Add(okButton);
                    panel.Controls.Add(calendar);
                    panel.Controls.Add(buttonPanel);
                    dateForm.Controls.Add(panel);

                    // After layout, set buttonPanel width and center the button
                    dateForm.Load += (s, ev) =>
                    {
                        buttonPanel.Width = calendar.Width;
                        okButton.Left = (buttonPanel.Width - okButton.Width) / 2;
                    };

                    if (dateForm.ShowDialog() == DialogResult.OK)
                    {
                        DateTime selectedDate = calendar.SelectionStart;
                        text3.Text = selectedDate.ToString("yyyy-MM-dd");
                    }
                }
            }

            private void DateRangeButton_Click(object sender, EventArgs e)
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                // First date picker
                using (Form dateForm1 = new Form())
                {
                    dateForm1.Text = sPickStartDate;
                    dateForm1.StartPosition = FormStartPosition.CenterParent;
                    dateForm1.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dateForm1.MaximizeBox = false;
                    dateForm1.MinimizeBox = false;
                    dateForm1.AutoSize = true;
                    dateForm1.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                    FlowLayoutPanel panel = new FlowLayoutPanel();
                    panel.FlowDirection = FlowDirection.TopDown;
                    panel.AutoSize = true;
                    panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    panel.Padding = new Padding((int)(10 * ScaleFactor));

                    MonthCalendar calendar1 = new MonthCalendar();
                    calendar1.MaxSelectionCount = 1;

                    Panel buttonPanel = new Panel();
                    buttonPanel.Height = (int)(36 * ScaleFactor);

                    Button okButton1 = new Button();
                    okButton1.Text = sOK;
                    okButton1.DialogResult = DialogResult.OK;
                    okButton1.Width = (int)(75 * ScaleFactor);
                    okButton1.Height = (int)(26 * ScaleFactor);
                    okButton1.Top = (int)(5 * ScaleFactor);

                    if (Dark)
                    {
                        dateForm1.BackColor = Color.FromArgb(45, 55, 45);
                        dateForm1.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(45, 55, 45);
                        buttonPanel.BackColor = Color.FromArgb(45, 55, 45);
                        okButton1.FlatStyle = FlatStyle.Flat;
                        okButton1.FlatAppearance.BorderColor = SystemColors.Highlight;
                        okButton1.FlatAppearance.BorderSize = 1;
                        okButton1.BackColor = Color.FromArgb(60, 60, 60);
                        okButton1.FlatAppearance.MouseOverBackColor = Color.Black;
                        DarkTitleBar(dateForm1.Handle);
                    }
                    else
                    {
                        dateForm1.BackColor = Color.FromArgb(235, 242, 235);
                        panel.BackColor = Color.FromArgb(235, 242, 235);
                        buttonPanel.BackColor = Color.FromArgb(235, 242, 235);
                        okButton1.BackColor = SystemColors.Control;
                    }

                    buttonPanel.Controls.Add(okButton1);
                    panel.Controls.Add(calendar1);
                    panel.Controls.Add(buttonPanel);
                    dateForm1.Controls.Add(panel);

                    // After layout, set buttonPanel width and center the button
                    dateForm1.Load += (s, ev) =>
                    {
                        buttonPanel.Width = calendar1.Width;
                        okButton1.Left = (buttonPanel.Width - okButton1.Width) / 2;
                    };

                    if (dateForm1.ShowDialog() == DialogResult.OK)
                    {
                        startDate = calendar1.SelectionStart;
                    }
                    else
                    {
                        return; // User cancelled
                    }
                }

                // Second date picker
                using (Form dateForm2 = new Form())
                {
                    dateForm2.Text = sPickEndDate;
                    dateForm2.StartPosition = FormStartPosition.CenterParent;
                    dateForm2.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dateForm2.MaximizeBox = false;
                    dateForm2.MinimizeBox = false;
                    dateForm2.AutoSize = true;
                    dateForm2.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                    FlowLayoutPanel panel = new FlowLayoutPanel();
                    panel.FlowDirection = FlowDirection.TopDown;
                    panel.AutoSize = true;
                    panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    panel.Padding = new Padding((int)(10 * ScaleFactor));

                    MonthCalendar calendar2 = new MonthCalendar();
                    calendar2.MaxSelectionCount = 1;
                    calendar2.SelectionStart = startDate.Value;

                    Panel buttonPanel = new Panel();
                    buttonPanel.Height = (int)(36 * ScaleFactor);

                    Button okButton2 = new Button();
                    okButton2.Text = sOK;
                    okButton2.DialogResult = DialogResult.OK;
                    okButton2.Width = (int)(75 * ScaleFactor);
                    okButton2.Height = (int)(26 * ScaleFactor);
                    okButton2.Top = (int)(5 * ScaleFactor);

                    if (Dark)
                    {
                        dateForm2.BackColor = Color.FromArgb(55, 45, 45);
                        dateForm2.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(55, 45, 45);
                        buttonPanel.BackColor = Color.FromArgb(55, 45, 45);
                        okButton2.FlatStyle = FlatStyle.Flat;
                        okButton2.FlatAppearance.BorderColor = SystemColors.Highlight;
                        okButton2.FlatAppearance.BorderSize = 1;
                        okButton2.BackColor = Color.FromArgb(60, 60, 60);
                        okButton2.FlatAppearance.MouseOverBackColor = Color.Black;
                        DarkTitleBar(dateForm2.Handle);
                    }
                    else
                    {
                        dateForm2.BackColor = Color.FromArgb(242, 235, 235);
                        panel.BackColor = Color.FromArgb(242, 235, 235);
                        buttonPanel.BackColor = Color.FromArgb(242, 235, 235);
                        okButton2.BackColor = SystemColors.Control;
                    }

                    buttonPanel.Controls.Add(okButton2);
                    panel.Controls.Add(calendar2);
                    panel.Controls.Add(buttonPanel);
                    dateForm2.Controls.Add(panel);

                    // After layout, set buttonPanel width and center the button
                    dateForm2.Load += (s, ev) =>
                    {
                        buttonPanel.Width = calendar2.Width;
                        okButton2.Left = (buttonPanel.Width - okButton2.Width) / 2;
                    };

                    if (dateForm2.ShowDialog() == DialogResult.OK)
                    {
                        endDate = calendar2.SelectionStart;
                    }
                    else
                    {
                        return; // User cancelled
                    }
                }

                // Format as AQS date range: startdate..enddate
                if (startDate.HasValue && endDate.HasValue)
                {
                    text3.Text = $"{startDate.Value:yyyy-MM-dd}..{endDate.Value:yyyy-MM-dd}";
                }
            }

            private void CustomToggle_CheckedChanged(object sender, EventArgs e)
            {
                customTextBox.Visible = customToggle.Checked;
                copyButton.Visible = customToggle.Checked;
                moreButton.Visible = customToggle.Checked;

                int spacing = (int)(30 * ScaleFactor);

                if (customToggle.Checked)
                {
                    // Auto-copy when first turned on
                    customFieldManuallyEdited = false;
                    customTextBox.Text = queryTextBox.Text;

                    // Move history panel down to make room for custom text box
                    historyPanel.Top += spacing;

                    // Grow the dialog to accommodate the custom text box
                    MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height + spacing);
                    Height += spacing;
                    MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + spacing);

                    // Adjust history panel height
                    int availableHeight = ClientSize.Height - historyPanel.Top - buttonOK.Height - (int)(20 * ScaleFactor);
                    int minHistoryHeight = (int)(80 * ScaleFactor);
                    historyPanel.Height = Math.Max(minHistoryHeight, availableHeight);
                }
                else
                {
                    // Move history panel up to fill space of custom text box
                    historyPanel.Top -= spacing;

                    // Shrink the dialog back
                    MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height - spacing);
                    Height -= spacing;
                    MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height - spacing);

                    // Adjust history panel height
                    int availableHeight = ClientSize.Height - historyPanel.Top - buttonOK.Height - (int)(20 * ScaleFactor);
                    int minHistoryHeight = (int)(80 * ScaleFactor);
                    historyPanel.Height = Math.Max(minHistoryHeight, availableHeight);
                }

                // Reposition OK button
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
            }

            private void CustomTextBox_TextChanged(object sender, EventArgs e)
            {
                // Mark as manually edited if user changes the text
                customFieldManuallyEdited = true;
            }

            private void CopyButton_Click(object sender, EventArgs e)
            {
                // Do nothing if query text box is empty
                if (string.IsNullOrWhiteSpace(queryTextBox.Text))
                    return;

                // Do nothing if query text box already matches custom text box
                if (queryTextBox.Text == customTextBox.Text)
                    return;

                // Warn if custom field has been manually edited
                if (customFieldManuallyEdited && !string.IsNullOrWhiteSpace(customTextBox.Text))
                {
                    DialogResult result = CustomMessageBox.Show(
                        "This will overwrite your custom query.",
                        "Warning");

                    if (result != DialogResult.OK)
                        return;
                }

                customFieldManuallyEdited = false;
                customTextBox.TextChanged -= CustomTextBox_TextChanged;
                customTextBox.Text = queryTextBox.Text;
                customTextBox.TextChanged += CustomTextBox_TextChanged;
                customFieldManuallyEdited = false;
            }

            private void ButtonOK_Click(object sender, EventArgs e)
            {
                string queryToUse = customToggle.Checked && !string.IsNullOrWhiteSpace(customTextBox.Text)
                    ? customTextBox.Text
                    : queryTextBox.Text;

                if (!string.IsNullOrWhiteSpace(queryToUse))
                {
                    // Save query to history file
                    try
                    {
                        string directory = System.IO.Path.GetDirectoryName(historyFilePath);
                        if (!System.IO.Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }

                        // Append to file if it doesn't already exist in history
                        if (!searchHistory.Contains(queryToUse))
                        {
                            System.IO.File.AppendAllText(historyFilePath, queryToUse + Environment.NewLine);
                            searchHistory.Add(queryToUse);

                            // Auto-scroll to show the newest item at the bottom
                            int historyLineHeight = (int)(20 * ScaleFactor);
                            int totalContentHeight = searchHistory.Count * historyLineHeight;
                            historyScrollOffset = Math.Max(0, totalContentHeight - historyPanel.Height);

                            historyPanel.Invalidate();
                        }
                    }
                    catch { }

                    // Windows 7 doesn't support search-ms: protocol
                    if (buildNumber < 9200) // Windows 7 and earlier (Windows 8 is build 9200+)
                    {
                        // Copy query to clipboard and open Explorer to the location
                        try
                        {
                            Clipboard.SetText(queryToUse);
                            Process.Start("explorer.exe", StartDirectory);
                        }
                        catch { }
                    }
                    else
                    {
                        // Windows 8 and later support search-ms: protocol
                        string searchMsQuery = $"search-ms:query={queryToUse}&crumb=location:{StartDirectory}";
                        Process.Start(searchMsQuery);
                    }

                    // Close dialog if AutoClose is enabled
                    if (autoClose)
                    {
                        this.Close();
                    }
                }
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    if (newFolder.Length > 260)
                    {
                        newFolder = GetShortPath(newFolder);
                    }

                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);
                    folderPathLabel.Text = StartDirectory;
                    folderPathScrollBar.Value = 0;

                    bool wasScrollbarVisible = folderPathScrollBar.Visible;

                    // Update scrollbar range
                    using (Graphics g = CreateGraphics())
                    {
                        SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                        int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                        folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                        folderPathScrollBar.Visible = maxScroll > 0;
                    }

                    // Adjust dialog height if scrollbar visibility changed
                    if (folderPathScrollBar.Visible && !wasScrollbarVisible)
                    {
                        int adjustment = SystemInformation.HorizontalScrollBarHeight;
                        MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height + adjustment);
                        Height += adjustment;
                        MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + adjustment);
                    }
                    else if (!folderPathScrollBar.Visible && wasScrollbarVisible)
                    {
                        int adjustment = SystemInformation.HorizontalScrollBarHeight;
                        MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height - adjustment);
                        Height -= adjustment;
                        MaximumSize = new Size(MaximumSize.Width, MaximumSize.Height - adjustment);
                    }

                    int controlHeight = (int)(24 * ScaleFactor);
                    int folderPathHeight = controlHeight + (folderPathScrollBar.Visible ? SystemInformation.HorizontalScrollBarHeight : 0);

                    // Adjust positions of all controls below
                    int yPos = (int)(40 * ScaleFactor) + folderPathHeight + (int)(10 * ScaleFactor);
                    int spacing = (int)(30 * ScaleFactor);

                    tipLabel.Top = yPos;
                    yPos += (int)(42 * ScaleFactor);

                    combo1.Top = yPos;
                    text1.Top = yPos;
                    yPos += spacing;

                    kindPresetsCombo.Top = yPos;
                    yPos += spacing;

                    combo2.Top = yPos;
                    text2.Top = yPos;
                    yPos += spacing;

                    sizePresetsCombo.Top = yPos;
                    yPos += spacing;

                    combo3.Top = yPos;
                    text3.Top = yPos;
                    yPos += spacing;

                    datePresetsCombo.Top = yPos;
                    int buttonOffset = (datePresetsCombo.Height - pickDateButton.Height) / 2;
                    pickDateButton.Top = yPos + buttonOffset;
                    dateRangeButton.Top = yPos + buttonOffset;
                    yPos += spacing;

                    combo4.Top = yPos;
                    text4.Top = yPos;
                    yPos += spacing;

                    combo5.Top = yPos;
                    text5.Top = yPos;
                    yPos += spacing + (int)(10 * ScaleFactor);

                    queryTextBox.Top = yPos;
                    yPos += spacing;

                    customLabel.Top = yPos + (int)(2 * ScaleFactor);
                    customToggle.Top = yPos;
                    copyButton.Top = yPos - (int)(2 * ScaleFactor);
                    moreButton.Top = yPos - (int)(2 * ScaleFactor);
                    yPos += spacing;

                    customTextBox.Top = yPos;
                    yPos += (customTextBox.Visible ? spacing : 0) + (int)(5 * ScaleFactor);

                    historyPanel.Top = yPos;

                    // Update scrollbar position
                    folderPathScrollBar.Top = folderPathLabel.Bottom - 1;

                    // Adjust history panel height to fill available space
                    int availableHeight = ClientSize.Height - historyPanel.Top - buttonOK.Height - (int)(20 * ScaleFactor);
                    int minHistoryHeight = (int)(80 * ScaleFactor);
                    historyPanel.Height = Math.Max(minHistoryHeight, availableHeight);

                    // Reposition OK button
                    buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                }
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var searchHelperDialog = new SearchHelperDialog(message, caption))
                {
                    return searchHelperDialog.ShowDialog();
                }
            }

        }

        // Dialog for Add-Del Path
        public class AddDelPathDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;
            private string messagePrefix;

            public AddDelPathDialog(string message, string caption)
            {
                messagePrefix = "\n\n\n\n\n";
                message = $"{messagePrefix}{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(400 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;
                helpPage = "add-or-remove-folder-in-path-variable";

                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                messageLabel.Padding = new Padding((int)(26 * ScaleFactor), 0, (int)(26 * ScaleFactor), 0);

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);

                userPathCheckbox = new CustomCheckBox();
                userPathCheckbox.Font = new Font("Segoe UI", 10);
                userPathCheckbox.Text = sUserPath;
                userPathCheckbox.Checked = InUserPath;
                userPathCheckbox.AutoSize = true;
                userPathCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(38 * ScaleFactor));

                systemPathCheckbox = new CustomCheckBox();
                systemPathCheckbox.Font = new Font("Segoe UI", 10);
                systemPathCheckbox.Text = sSystemPath;
                systemPathCheckbox.Checked = InSystemPath;
                systemPathCheckbox.AutoSize = true;
                systemPathCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(62 * ScaleFactor));

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(userPathCheckbox);
                Controls.Add(systemPathCheckbox);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);

                    string path = StartDirectory;
                    if (path.EndsWith(":")) path += "\\";

                    string updatedMessage = $"{messagePrefix}{path}";
                    messageLabel.Text = updatedMessage;

                    // Recalculate dialog height based on new message size
                    using (Graphics g = CreateGraphics())
                    {
                        SizeF size = g.MeasureString(updatedMessage, new Font("Segoe UI", 10), Width);
                        Height = Math.Max((int)(150 * ScaleFactor), (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                    }

                    // Reposition OK button
                    buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);

                    // Update checkbox states for the new path
                    InUserPath = IsPathInEnvironmentVariable(path, UserPath);
                    InSystemPath = IsPathInEnvironmentVariable(path, SystemPath);
                    userPathCheckbox.Checked = InUserPath;
                    systemPathCheckbox.Checked = InSystemPath;
                }
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var AddDelPathDialog = new AddDelPathDialog(message, caption))
                {
                    return AddDelPathDialog.ShowDialog();
                }
            }
        }

        // Dialog for Folder Color Picker
        public class FolderColorPickerDialog : Form
        {
            private Color? selectedColor = null;
            private int selectedColorIndex = -1;
            private int hoveredColorIndex = -1;

            // Define 16 colors in 2 rows
            private Color[] colors = new Color[]
            {
                // First row (vibrant colors)
                Color.FromArgb(247, 207, 56),   // Default yellow
                Color.FromArgb(218, 63, 44),    // Red
                Color.FromArgb(226, 114, 18),   // Orange
                Color.FromArgb(79, 160, 71),    // Green
                Color.FromArgb(66, 147, 142),   // Teal
                Color.FromArgb(67, 130, 209),   // Blue
                Color.FromArgb(152, 94, 200),   // Purple
                Color.FromArgb(195, 81, 181),   // Pink
                // Second row (light/pastel colors)
                Color.FromArgb(177, 183, 186),  // Gray
                Color.FromArgb(247, 188, 178),  // Light red
                Color.FromArgb(247, 192, 132),  // Light orange
                Color.FromArgb(150, 211, 143),  // Light green
                Color.FromArgb(137, 209, 205),  // Light teal
                Color.FromArgb(148, 199, 247),  // Light blue
                Color.FromArgb(210, 174, 247),  // Light purple
                Color.FromArgb(240, 169, 232)   // Light pink
            };

            public FolderColorPickerDialog()
            {
                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = sPickAColor;
                MaximizeBox = false;
                MinimizeBox = false;

                int buttonSize = (int)(20 * ScaleFactor); // ~1/4" at standard DPI
                int spacing = (int)(8 * ScaleFactor);
                int margin = (int)(25 * ScaleFactor);
                int rows = 2;
                int cols = 8;

                // Calculate dialog size
                int panelWidth = (cols * buttonSize) + ((cols - 1) * spacing) + (margin * 2);
                int panelHeight = (rows * buttonSize) + ((rows - 1) * spacing) + (margin * 2);
                int okButtonHeight = (int)(26 * ScaleFactor);
                int okButtonSpacing = (int)(16 * ScaleFactor);

                Width = panelWidth + (int)(16 * ScaleFactor); // Extra for borders
                Height = panelHeight + okButtonHeight + okButtonSpacing + (int)(50 * ScaleFactor); // Extra for title bar

                // Color panel with owner-drawn buttons
                Panel colorPanel = new Panel();
                colorPanel.Location = new Point((int)(4 * ScaleFactor), (int)(4 * ScaleFactor));
                colorPanel.Width = panelWidth;
                colorPanel.Height = panelHeight;
                colorPanel.BackColor = Dark ? Color.FromArgb(43, 43, 43) : SystemColors.Control;

                // Enable double buffering to prevent flicker
                colorPanel.GetType().GetProperty("DoubleBuffered", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .SetValue(colorPanel, true, null);

                // Custom paint for color buttons
                colorPanel.Paint += (s, pe) =>
                {
                    Panel panel = s as Panel;
                    if (panel != null)
                    {
                        pe.Graphics.Clear(panel.BackColor);
                        pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        for (int i = 0; i < colors.Length; i++)
                        {
                            int row = i / cols;
                            int col = i % cols;

                            int x = margin + col * (buttonSize + spacing);
                            int y = margin + row * (buttonSize + spacing);

                            Rectangle buttonRect = new Rectangle(x, y, buttonSize, buttonSize);

                            // Draw shadow if hovered
                            if (i == hoveredColorIndex)
                            {
                                Rectangle shadowRect = new Rectangle(x - 2, y - 2, buttonSize + 4, buttonSize + 4);
                                pe.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(60, 0, 0, 0)), shadowRect);
                            }

                            // Draw color circle
                            using (SolidBrush colorBrush = new SolidBrush(colors[i]))
                            {
                                pe.Graphics.FillEllipse(colorBrush, buttonRect);
                            }

                            // Draw border
                            Color borderColor = Dark ? Color.FromArgb(80, 80, 80) : Color.FromArgb(160, 160, 160);
                            using (Pen borderPen = new Pen(borderColor, 1.5f))
                            {
                                pe.Graphics.DrawEllipse(borderPen, buttonRect);
                            }

                            // Draw highlight ring on hover
                            if (i == hoveredColorIndex)
                            {
                                Rectangle highlightRect = new Rectangle(x - 2, y - 2, buttonSize + 4, buttonSize + 4);
                                using (Pen highlightPen = new Pen(Dark ? Color.White : Color.Black, 2))
                                {
                                    pe.Graphics.DrawEllipse(highlightPen, highlightRect);
                                }
                            }
                        }
                    }
                };

                // Mouse move for hover effect
                colorPanel.MouseMove += (s, ev) =>
                {
                    // Adjust for margin offset inside panel
                    int adjustedX = ev.X - margin;
                    int adjustedY = ev.Y - margin;

                    int col = adjustedX / (buttonSize + spacing);
                    int row = adjustedY / (buttonSize + spacing);

                    // Check if mouse is within a button's circular area
                    int index = row * cols + col;
                    if (index >= 0 && index < colors.Length)
                    {
                        int x = margin + col * (buttonSize + spacing);
                        int y = margin + row * (buttonSize + spacing);

                        int centerX = x + buttonSize / 2;
                        int centerY = y + buttonSize / 2;

                        int dx = ev.X - centerX;
                        int dy = ev.Y - centerY;
                        int distanceSquared = dx * dx + dy * dy;
                        int radiusSquared = (buttonSize / 2) * (buttonSize / 2);

                        if (distanceSquared <= radiusSquared)
                        {
                            if (hoveredColorIndex != index)
                            {
                                hoveredColorIndex = index;
                                colorPanel.Invalidate();
                            }
                        }
                        else
                        {
                            if (hoveredColorIndex != -1)
                            {
                                hoveredColorIndex = -1;
                                colorPanel.Invalidate();
                            }
                        }
                    }
                    else
                    {
                        if (hoveredColorIndex != -1)
                        {
                            hoveredColorIndex = -1;
                            colorPanel.Invalidate();
                        }
                    }
                };

                // Mouse leave for hover effect
                colorPanel.MouseLeave += (s, ev) =>
                {
                    hoveredColorIndex = -1;
                    colorPanel.Invalidate();
                };

                // Click handler to select color
                colorPanel.MouseClick += (s, ev) =>
                {
                    // Adjust for margin offset inside panel
                    int adjustedX = ev.X - margin;
                    int adjustedY = ev.Y - margin;

                    int col = adjustedX / (buttonSize + spacing);
                    int row = adjustedY / (buttonSize + spacing);

                    int index = row * cols + col;
                    if (index >= 0 && index < colors.Length)
                    {
                        int x = margin + col * (buttonSize + spacing);
                        int y = margin + row * (buttonSize + spacing);

                        int centerX = x + buttonSize / 2;
                        int centerY = y + buttonSize / 2;

                        int dx = ev.X - centerX;
                        int dy = ev.Y - centerY;
                        int distanceSquared = dx * dx + dy * dy;
                        int radiusSquared = (buttonSize / 2) * (buttonSize / 2);

                        if (distanceSquared <= radiusSquared)
                        {
                            selectedColor = colors[index];
                            selectedColorIndex = index;
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    }
                };

                // OK button
                Button buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = okButtonHeight;
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = colorPanel.Bottom + okButtonSpacing;

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                Controls.Add(colorPanel);
                Controls.Add(buttonOK);
            }

            public static new int? Show()
            {
                using (var dialog = new FolderColorPickerDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        return dialog.selectedColorIndex;
                    }
                    return null;
                }
            }
        }

        // Dialog for Folder Options
        public class FolderOptionsDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;
            private Label folderPathLabel;
            private FlatScrollBar folderPathScrollBar;
            private Label fileSystemLabel;
            private Label driveTypeLabel;
            private CustomGroupBox globalSettingsGroupBox;
            private CustomCheckBox aftdCheckbox;
            private CustomCheckBox alwaysShowIconsCheckbox;
            private CustomCheckBox disableFolderThumbnailsCheckbox;
            private CustomCheckBox applyToSubfoldersCheckbox;
            private CustomCheckBox IcoRestoreDefaultsCheckbox;
            private CustomCheckBox deleteDesktopIniCheckbox;
            private CustomComboBox folderTypeCombo;
            private CustomCheckBox FTRestoreDefaultsCheckbox;
            private Label folderTypeSubtitle;
            private Label folderTypeRequirementsLabel;
            private CustomGroupBox folderTypeGroupBox;
            private CustomComboBox iconSourceCombo;
            private CustomComboBox iconModeComboFull;      // All 6 modes for folder-based options
            private CustomComboBox iconModeComboSimple;   // 3 modes for single-image selection
            private CustomGroupBox folderIconGroupBox;
            private CustomCheckBox resetIconCacheCheckbox;
            private Label selectedFilePathLabel;
            private FlatScrollBar selectedFilePathScrollBar;
            private PictureBox iconPreviewBox;
            private List<string> selectedImagePaths;
            private string selectedIconPath;
            private Color? selectedColor;
            private int selectedColorIndex = -1;
            private bool DriveOK;
            private Dictionary<string, string> tempIconPaths;

            public FolderOptionsDialog(string message, string caption)
            {
                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(490 * ScaleFactor);
                Height = (int)(665 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                // Initialize selected path storage
                selectedImagePaths = new List<string>();
                selectedIconPath = null;
                selectedColor = null;
                tempIconPaths = new Dictionary<string, string>();

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                // Message label for title (centered between folder and help icons)
                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.MiddleCenter;
                messageLabel.AutoSize = false;
                messageLabel.Location = new Point((int)(35 * ScaleFactor), (int)(5 * ScaleFactor));
                messageLabel.Width = ClientSize.Width - (int)(70 * ScaleFactor);
                messageLabel.Height = (int)(20 * ScaleFactor);

                // Folder path display
                int controlHeight = (int)(24 * ScaleFactor);
                int xMargin = (int)(10 * ScaleFactor);

                folderPathLabel = new Label();
                folderPathLabel.Font = new Font("Segoe UI", 9);
                folderPathLabel.Location = new Point(xMargin, (int)(40 * ScaleFactor));
                folderPathLabel.Width = ClientSize.Width - (xMargin * 2);
                folderPathLabel.Height = controlHeight;
                folderPathLabel.AutoSize = false;
                folderPathLabel.BorderStyle = BorderStyle.None;
                folderPathLabel.Padding = new Padding(2, 2, 2, 2);
                folderPathLabel.BackColor = SystemColors.Control;

                // Custom paint to ensure text is always vertically centered
                folderPathLabel.Paint += (s, pe) =>
                {
                    Label lbl = s as Label;
                    if (lbl != null)
                    {
                        pe.Graphics.Clear(lbl.BackColor);

                        if (!string.IsNullOrEmpty(lbl.Text))
                        {
                            Rectangle textRect = new Rectangle(
                                lbl.Padding.Left, 
                                0, 
                                lbl.Width - lbl.Padding.Left - lbl.Padding.Right, 
                                lbl.Height);
                            TextRenderer.DrawText(
                                pe.Graphics, 
                                lbl.Text, 
                                lbl.Font, 
                                textRect, 
                                lbl.ForeColor, 
                                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                        }

                        // Always draw thin border
                        Color borderColor = Dark ? Color.FromArgb(100, 100, 100) : Color.FromArgb(171, 173, 179);
                        using (Pen borderPen = new Pen(borderColor, 1))
                        {
                            pe.Graphics.DrawRectangle(borderPen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                        }
                    }
                };

                // Add horizontal scrollbar for folder path
                folderPathScrollBar = new FlatScrollBar();
                folderPathScrollBar.Orientation = ScrollBarOrientation.Horizontal;
                folderPathScrollBar.Location = new Point(xMargin, (int)(40 * ScaleFactor) + controlHeight - 1);
                folderPathScrollBar.Width = ClientSize.Width - (xMargin * 2);
                folderPathScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
                folderPathScrollBar.Minimum = 0;
                folderPathScrollBar.SmallChange = 5;
                folderPathScrollBar.LargeChange = 20;
                folderPathScrollBar.Scroll += (s, ev) =>
                {
                    int offset = folderPathScrollBar.Value;
                    folderPathLabel.Text = StartDirectory.Length > offset ? StartDirectory.Substring(offset) : "";
                };

                // Calculate scrollbar range based on text length
                using (Graphics g = CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                    int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                    folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                    folderPathScrollBar.Visible = maxScroll > 0;
                }

                // Global Settings GroupBox - always reserve space for scrollbar
                int globalSettingsY = folderPathLabel.Bottom + SystemInformation.HorizontalScrollBarHeight + (int)(5 * ScaleFactor);
                globalSettingsGroupBox = new CustomGroupBox();
                globalSettingsGroupBox.Text = sGlobalSettings;
                globalSettingsGroupBox.Font = new Font("Segoe UI", 9);
                globalSettingsGroupBox.Location = new Point(xMargin, globalSettingsY);
                globalSettingsGroupBox.Width = ClientSize.Width - (xMargin * 2);

                // Information labels (inside global settings GroupBox)
                fileSystemLabel = new Label();
                fileSystemLabel.Font = new Font("Segoe UI", 9);
                fileSystemLabel.Location = new Point((int)(10 * ScaleFactor), (int)(20 * ScaleFactor));
                fileSystemLabel.AutoSize = true;
                fileSystemLabel.Text = sFileSystemPrefix;

                driveTypeLabel = new Label();
                driveTypeLabel.Font = new Font("Segoe UI", 9);
                driveTypeLabel.Location = new Point((int)(150 * ScaleFactor), (int)(20 * ScaleFactor));
                driveTypeLabel.AutoSize = true;
                driveTypeLabel.Text = sTypePrefix;

                // AFTD CheckBox (Automatic Folder Type Discovery) (inside global settings GroupBox)
                aftdCheckbox = new CustomCheckBox();
                aftdCheckbox.Font = new Font("Segoe UI", 9);
                aftdCheckbox.Text = sAFTD;
                aftdCheckbox.Location = new Point((int)(10 * ScaleFactor), (int)(48 * ScaleFactor));
                aftdCheckbox.AutoSize = true;
                aftdCheckbox.CheckedChanged += (s, e) => UpdateFolderTypeComboVisibility();

                // Add subtitle label below AFTD checkbox (inside global settings GroupBox)
                Label aftdSubtitle = new Label();
                aftdSubtitle.Font = new Font("Segoe UI", 8, FontStyle.Italic);
                aftdSubtitle.ForeColor = SystemColors.GrayText;
                aftdSubtitle.Text = sAFTDSubtitle;
                aftdSubtitle.Location = new Point((int)(30 * ScaleFactor), (int)(68 * ScaleFactor));
                aftdSubtitle.AutoSize = true;

                // Always Show Icons CheckBox (inside global settings GroupBox)
                alwaysShowIconsCheckbox = new CustomCheckBox();
                alwaysShowIconsCheckbox.Font = new Font("Segoe UI", 9);
                alwaysShowIconsCheckbox.Text = sAlwaysShowIcons;
                alwaysShowIconsCheckbox.Location = new Point((int)(10 * ScaleFactor), (int)(92 * ScaleFactor));
                alwaysShowIconsCheckbox.AutoSize = true;
                alwaysShowIconsCheckbox.Checked = IsAlwaysShowIconsEnabled();

                // Disable Folder Thumbnails CheckBox (inside global settings GroupBox)
                disableFolderThumbnailsCheckbox = new CustomCheckBox();
                disableFolderThumbnailsCheckbox.Font = new Font("Segoe UI", 9);
                disableFolderThumbnailsCheckbox.Text = sDisableFolderThumbnails;
                disableFolderThumbnailsCheckbox.Location = new Point((int)(10 * ScaleFactor), (int)(116 * ScaleFactor));
                disableFolderThumbnailsCheckbox.AutoSize = true;
                disableFolderThumbnailsCheckbox.Checked = IsFolderThumbnailsDisabled();

                // Add controls to global settings GroupBox
                globalSettingsGroupBox.Controls.Add(fileSystemLabel);
                globalSettingsGroupBox.Controls.Add(driveTypeLabel);
                globalSettingsGroupBox.Controls.Add(aftdCheckbox);
                globalSettingsGroupBox.Controls.Add(aftdSubtitle);
                globalSettingsGroupBox.Controls.Add(alwaysShowIconsCheckbox);
                globalSettingsGroupBox.Controls.Add(disableFolderThumbnailsCheckbox);

                // Group Box for Folder Type (only visible when DriveOK is true)
                folderTypeGroupBox = new CustomGroupBox();
                folderTypeGroupBox.Text = sForceFolderType;
                folderTypeGroupBox.Font = new Font("Segoe UI", 9);
                folderTypeGroupBox.Location = new Point(xMargin, globalSettingsGroupBox.Bottom + (int)(20 * ScaleFactor));
                folderTypeGroupBox.Width = ClientSize.Width - (xMargin * 2);
                folderTypeGroupBox.Visible = false;

                // Folder Type ComboBox (inside the GroupBox)
                folderTypeCombo = new CustomComboBox();
                folderTypeCombo.Items.AddRange(new string[] { sNoChange, sGeneralItems, sDocuments, sPictures, sMusic, sVideos });
                folderTypeCombo.Font = new Font("Segoe UI", 9);
                folderTypeCombo.Location = new Point((int)(10 * ScaleFactor), (int)(25 * ScaleFactor));
                folderTypeCombo.Width = (int)(150 * ScaleFactor);
                folderTypeCombo.Height = controlHeight;
                folderTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
                folderTypeCombo.SelectedIndex = 0; // Default to "No change"

                // Remove FolderType checkbox (below the ComboBox)
                FTRestoreDefaultsCheckbox = new CustomCheckBox();
                FTRestoreDefaultsCheckbox.Font = new Font("Segoe UI", 9);
                FTRestoreDefaultsCheckbox.Text = sRestoreDefaults;
                FTRestoreDefaultsCheckbox.Location = new Point((int)(10 * ScaleFactor), (int)(60 * ScaleFactor));
                FTRestoreDefaultsCheckbox.AutoSize = true;
                FTRestoreDefaultsCheckbox.Checked = false;
                FTRestoreDefaultsCheckbox.CheckedChanged += (s, e) => UpdateFolderTypeComboVisibility();

                // Subtitle label that shows when AFTD is unchecked (in place of hidden combobox)
                folderTypeSubtitle = new Label();
                folderTypeSubtitle.Font = new Font("Segoe UI", 8, FontStyle.Italic);
                folderTypeSubtitle.ForeColor = SystemColors.GrayText;
                folderTypeSubtitle.Text = sRequiresAFTD;
                folderTypeSubtitle.Location = new Point((int)(10 * ScaleFactor), (int)(25 * ScaleFactor));
                folderTypeSubtitle.AutoSize = true;
                folderTypeSubtitle.Visible = false; // Initially hidden

                // Drive requirements label that shows when DriveOK is false
                folderTypeRequirementsLabel = new Label();
                folderTypeRequirementsLabel.Font = new Font("Segoe UI", 8, FontStyle.Italic);
                folderTypeRequirementsLabel.ForeColor = SystemColors.GrayText;
                folderTypeRequirementsLabel.Text = sRequiresNTFS;
                folderTypeRequirementsLabel.Location = new Point((int)(10 * ScaleFactor), (int)(25 * ScaleFactor));
                folderTypeRequirementsLabel.AutoSize = true;
                folderTypeRequirementsLabel.Visible = false; // Initially hidden

                folderTypeGroupBox.Controls.Add(folderTypeCombo);
                folderTypeGroupBox.Controls.Add(folderTypeSubtitle);
                folderTypeGroupBox.Controls.Add(folderTypeRequirementsLabel);
                folderTypeGroupBox.Controls.Add(FTRestoreDefaultsCheckbox);

                // Group Box for Folder Icon
                folderIconGroupBox = new CustomGroupBox();
                folderIconGroupBox.Text = sSetFolderIcon;
                folderIconGroupBox.Font = new Font("Segoe UI", 9);
                folderIconGroupBox.Location = new Point(xMargin, globalSettingsGroupBox.Bottom + (int)(20 * ScaleFactor));
                folderIconGroupBox.Width = ClientSize.Width - (xMargin * 2);
                folderIconGroupBox.Visible = true;

                // Icon Source ComboBox (inside the GroupBox)
                iconSourceCombo = new CustomComboBox();
                iconSourceCombo.Items.AddRange(new string[] { 
                    sNoChange,
                    sSelectedColor,
                    sSelectedIcon,
                    sSelectedImage, 
                    sMostRecentImages, 
                    sFirstAlphabeticalImages
                });
                iconSourceCombo.Font = new Font("Segoe UI", 9);
                iconSourceCombo.Location = new Point((int)(10 * ScaleFactor), (int)(25 * ScaleFactor));
                iconSourceCombo.Width = (int)(250 * ScaleFactor);
                iconSourceCombo.Height = controlHeight;
                iconSourceCombo.DropDownStyle = ComboBoxStyle.DropDownList;
                iconSourceCombo.SelectedIndex = 0; // Default to "No change"
                iconSourceCombo.SelectedIndexChanged += IconSourceCombo_SelectedIndexChanged;

                // Icon Mode ComboBox - Full version (for folder-based options)
                iconModeComboFull = new CustomComboBox();
                iconModeComboFull.Items.AddRange(new string[] { 
                    sFitTransparent, 
                    sFitSolid, 
                    sFillSingle, 
                    sFill2Landscape, 
                    sFill2Portrait, 
                    sFill4Images 
                });
                iconModeComboFull.Font = new Font("Segoe UI", 9);
                iconModeComboFull.Location = new Point((int)(10 * ScaleFactor), (int)(55 * ScaleFactor));
                iconModeComboFull.Width = (int)(250 * ScaleFactor);
                iconModeComboFull.Height = controlHeight;
                iconModeComboFull.DropDownStyle = ComboBoxStyle.DropDownList;
                iconModeComboFull.SelectedIndex = 0;
                iconModeComboFull.Visible = false;
                iconModeComboFull.SelectedIndexChanged += IconModeCombo_SelectedIndexChanged;

                // Icon Mode ComboBox - Simple version (for single-image selection)
                iconModeComboSimple = new CustomComboBox();
                iconModeComboSimple.Items.AddRange(new string[] { 
                    sFitTransparent, 
                    sFitSolid, 
                    sFillSingle 
                });
                iconModeComboSimple.Font = new Font("Segoe UI", 9);
                iconModeComboSimple.Location = new Point((int)(10 * ScaleFactor), (int)(55 * ScaleFactor));
                iconModeComboSimple.Width = (int)(250 * ScaleFactor);
                iconModeComboSimple.Height = controlHeight;
                iconModeComboSimple.DropDownStyle = ComboBoxStyle.DropDownList;
                iconModeComboSimple.SelectedIndex = 0;
                iconModeComboSimple.Visible = false;
                iconModeComboSimple.SelectedIndexChanged += IconModeCombo_SelectedIndexChanged;

                // Icon preview box (positioned to the right of the comboboxes)
                iconPreviewBox = new PictureBox();
                iconPreviewBox.Location = new Point((int)(10 * ScaleFactor) + (int)(250 * ScaleFactor) + (int)(10 * ScaleFactor), (int)(25 * ScaleFactor));
                iconPreviewBox.Size = new Size((int)(64 * ScaleFactor), (int)(64 * ScaleFactor));
                iconPreviewBox.SizeMode = PictureBoxSizeMode.Normal;
                iconPreviewBox.BackColor = SystemColors.Control;
                iconPreviewBox.Visible = false;

                // Custom paint handler for high-quality icon rendering
                iconPreviewBox.Paint += (s, pe) =>
                {
                    if (iconPreviewBox.Image != null)
                    {
                        pe.Graphics.Clear(iconPreviewBox.BackColor);
                        pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        pe.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                        // Calculate aspect-ratio-preserving dimensions
                        float imgAspect = (float)iconPreviewBox.Image.Width / iconPreviewBox.Image.Height;
                        float boxAspect = (float)iconPreviewBox.Width / iconPreviewBox.Height;

                        int drawWidth, drawHeight, drawX, drawY;

                        if (imgAspect > boxAspect)
                        {
                            // Image is wider - fit to width
                            drawWidth = iconPreviewBox.Width;
                            drawHeight = (int)(iconPreviewBox.Width / imgAspect);
                            drawX = 0;
                            drawY = (iconPreviewBox.Height - drawHeight) / 2;
                        }
                        else
                        {
                            // Image is taller or square - fit to height
                            drawHeight = iconPreviewBox.Height;
                            drawWidth = (int)(iconPreviewBox.Height * imgAspect);
                            drawX = (iconPreviewBox.Width - drawWidth) / 2;
                            drawY = 0;
                        }

                        pe.Graphics.DrawImage(iconPreviewBox.Image, drawX, drawY, drawWidth, drawHeight);
                    }
                };

                // Selected file path display (initially hidden)
                selectedFilePathLabel = new Label();
                selectedFilePathLabel.Font = new Font("Segoe UI", 9);
                selectedFilePathLabel.Location = new Point((int)(10 * ScaleFactor), (int)(85 * ScaleFactor));
                selectedFilePathLabel.Width = folderIconGroupBox.Width - (int)(20 * ScaleFactor);
                selectedFilePathLabel.Height = controlHeight;
                selectedFilePathLabel.AutoSize = false;
                selectedFilePathLabel.BorderStyle = BorderStyle.None;
                selectedFilePathLabel.Padding = new Padding(2, 2, 2, 2);
                selectedFilePathLabel.BackColor = SystemColors.Control;
                selectedFilePathLabel.Visible = false;

                // Custom paint to ensure text is always vertically centered
                selectedFilePathLabel.Paint += (s, pe) =>
                {
                    Label lbl = s as Label;
                    if (lbl != null)
                    {
                        pe.Graphics.Clear(lbl.BackColor);

                        if (!string.IsNullOrEmpty(lbl.Text))
                        {
                            Rectangle textRect = new Rectangle(
                                lbl.Padding.Left, 
                                0, 
                                lbl.Width - lbl.Padding.Left - lbl.Padding.Right, 
                                lbl.Height);
                            TextRenderer.DrawText(
                                pe.Graphics, 
                                lbl.Text, 
                                lbl.Font, 
                                textRect, 
                                lbl.ForeColor, 
                                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                        }

                        // Always draw thin border
                        Color borderColor = Dark ? Color.FromArgb(100, 100, 100) : Color.FromArgb(171, 173, 179);
                        using (Pen borderPen = new Pen(borderColor, 1))
                        {
                            pe.Graphics.DrawRectangle(borderPen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                        }
                    }
                };

                // Add horizontal scrollbar for selected file path
                selectedFilePathScrollBar = new FlatScrollBar();
                selectedFilePathScrollBar.Orientation = ScrollBarOrientation.Horizontal;
                selectedFilePathScrollBar.Location = new Point((int)(10 * ScaleFactor), (int)(85 * ScaleFactor) + controlHeight);
                selectedFilePathScrollBar.Width = folderIconGroupBox.Width - (int)(20 * ScaleFactor);
                selectedFilePathScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
                selectedFilePathScrollBar.Minimum = 0;
                selectedFilePathScrollBar.SmallChange = 5;
                selectedFilePathScrollBar.LargeChange = 20;
                selectedFilePathScrollBar.Visible = false;
                selectedFilePathScrollBar.Scroll += (s, ev) =>
                {
                    int offset = selectedFilePathScrollBar.Value;
                    string fullText = GetSelectedFilePathDisplayText();
                    selectedFilePathLabel.Text = fullText.Length > offset ? fullText.Substring(offset) : "";
                };

                // Reset icon cache checkbox
                resetIconCacheCheckbox = new CustomCheckBox();
                resetIconCacheCheckbox.Font = new Font("Segoe UI", 9);
                resetIconCacheCheckbox.Text = sResetIconCacheRestart;
                resetIconCacheCheckbox.Location = new Point((int)(10 * ScaleFactor), (int)(25 * ScaleFactor));
                resetIconCacheCheckbox.AutoSize = true;
                resetIconCacheCheckbox.Checked = false;

                // Restore defaults checkbox
                IcoRestoreDefaultsCheckbox = new CustomCheckBox();
                IcoRestoreDefaultsCheckbox.Font = new Font("Segoe UI", 9);
                IcoRestoreDefaultsCheckbox.Text = sRestoreDefaults;
                IcoRestoreDefaultsCheckbox.Checked = false;
                IcoRestoreDefaultsCheckbox.AutoSize = true;
                IcoRestoreDefaultsCheckbox.CheckedChanged += (s, ev) =>
                {
                    if (IcoRestoreDefaultsCheckbox.Checked)
                    {
                        // Hide the icon source combo and preview when restoring defaults
                        iconSourceCombo.Visible = false;
                        iconModeComboFull.Visible = false;
                        iconModeComboSimple.Visible = false;
                        iconPreviewBox.Visible = false;
                        iconPreviewBox.Image = null;
                        selectedImagePaths.Clear();
                        selectedIconPath = null;
                        selectedColor = null;
                        selectedColorIndex = -1;
                        tempIconPaths.Clear();
                        UpdateDialogLayout();

                        this.ActiveControl = null; // Unfocus to prevent white background
                    }
                    else
                    {
                        // Restore icon source combo visibility
                        iconSourceCombo.Visible = true;
                        UpdateDialogLayout();
                    }
                };

                // Reset icon cache checkbox (positioned outside the GroupBox, before Delete desktop.ini)
                resetIconCacheCheckbox = new CustomCheckBox();
                resetIconCacheCheckbox.Font = new Font("Segoe UI", 9);
                resetIconCacheCheckbox.Text = sResetIconCacheRestart;
                resetIconCacheCheckbox.AutoSize = true;
                resetIconCacheCheckbox.Checked = false;

                folderIconGroupBox.Controls.Add(iconSourceCombo);
                folderIconGroupBox.Controls.Add(iconModeComboFull);
                folderIconGroupBox.Controls.Add(iconModeComboSimple);
                folderIconGroupBox.Controls.Add(iconPreviewBox);
                folderIconGroupBox.Controls.Add(selectedFilePathLabel);
                folderIconGroupBox.Controls.Add(selectedFilePathScrollBar);
                folderIconGroupBox.Controls.Add(IcoRestoreDefaultsCheckbox);

                // Delete desktop.ini checkbox
                deleteDesktopIniCheckbox = new CustomCheckBox();
                deleteDesktopIniCheckbox.Font = new Font("Segoe UI", 9);
                deleteDesktopIniCheckbox.Text = sDeleteDesktopIni;
                deleteDesktopIniCheckbox.Checked = false;
                deleteDesktopIniCheckbox.AutoSize = true;
                deleteDesktopIniCheckbox.CheckedChanged += (s, ev) =>
                {
                    if (deleteDesktopIniCheckbox.Checked)
                    {
                        // Hide folder type and icon group boxes since desktop.ini is being deleted
                        folderTypeGroupBox.Visible = false;
                        folderIconGroupBox.Visible = false;

                        // Hide IcoRestoreDefaultsCheckbox since both files will be deleted
                        IcoRestoreDefaultsCheckbox.Visible = false;

                        UpdateDialogLayout();
                        this.ActiveControl = null; // Unfocus to prevent white background
                    }
                    else
                    {
                        // Restore visibility based on DriveOK
                        folderTypeGroupBox.Visible = DriveOK;
                        folderIconGroupBox.Visible = true;

                        // Show IcoRestoreDefaultsCheckbox again
                        IcoRestoreDefaultsCheckbox.Visible = true;

                        UpdateDialogLayout();
                    }
                };

                // Apply to Subfolders checkbox (positioned before OK button)
                applyToSubfoldersCheckbox = new CustomCheckBox();
                applyToSubfoldersCheckbox.Font = new Font("Segoe UI", 9);
                applyToSubfoldersCheckbox.Text = sAlsoApplyToSubfolders;
                applyToSubfoldersCheckbox.Checked = false;
                applyToSubfoldersCheckbox.AutoSize = true;
                applyToSubfoldersCheckbox.CheckedChanged += (s, ev) =>
                {
                    // Update preview when subfolder checkbox changes (affects fallback preview visibility)
                    UpdateSelectedFilePathDisplay();
                };

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                buttonOK.Click += ButtonOK_Click;

                // Position checkbox just above OK button
                applyToSubfoldersCheckbox.Location = new Point(
                    (ClientSize.Width - applyToSubfoldersCheckbox.PreferredSize.Width) / 2,
                    buttonOK.Top - applyToSubfoldersCheckbox.PreferredSize.Height - (int)(7 * ScaleFactor)
                );

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;

                    folderPathLabel.BackColor = Color.FromArgb(45, 45, 45);
                    folderPathLabel.ForeColor = Color.White;
                    folderPathScrollBar.Theme = UITheme.VS2019DarkBlue;
                    selectedFilePathLabel.BackColor = Color.FromArgb(45, 45, 45);
                    selectedFilePathLabel.ForeColor = Color.White;
                    selectedFilePathScrollBar.Theme = UITheme.VS2019DarkBlue;
                    iconPreviewBox.BackColor = Color.FromArgb(43, 43, 43);

                    // CustomGroupBox border and title colors
                    globalSettingsGroupBox.BorderColor = Color.FromArgb(100, 100, 100);
                    globalSettingsGroupBox.TitleColor = Color.FromArgb(100, 100, 100);
                    folderTypeGroupBox.BorderColor = Color.FromArgb(100, 100, 100);
                    folderTypeGroupBox.TitleColor = Color.FromArgb(100, 100, 100);
                    folderIconGroupBox.BorderColor = Color.FromArgb(100, 100, 100);
                    folderIconGroupBox.TitleColor = Color.FromArgb(100, 100, 100);

                    // Child control colors remain white
                    fileSystemLabel.ForeColor = Color.White;
                    driveTypeLabel.ForeColor = Color.White;
                    aftdSubtitle.ForeColor = Color.FromArgb(160, 160, 160);
                    folderTypeSubtitle.ForeColor = Color.FromArgb(160, 160, 160);
                    folderTypeRequirementsLabel.ForeColor = Color.FromArgb(160, 160, 160);
                }

                Controls.Add(messageLabel);
                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(folderPathLabel);
                Controls.Add(folderPathScrollBar);
                Controls.Add(globalSettingsGroupBox);
                Controls.Add(folderTypeGroupBox);
                Controls.Add(folderIconGroupBox);
                Controls.Add(resetIconCacheCheckbox);
                Controls.Add(deleteDesktopIniCheckbox);
                Controls.Add(applyToSubfoldersCheckbox);
                Controls.Add(buttonOK);

                folderPathLabel.Text = StartDirectory;

                // Suspend layout while we update positions and visibility
                SuspendLayout();

                // Update information labels with current folder data (after all controls are created)
                UpdateFolderInfo();

                // Update folderTypeCombo visibility based on checkbox states
                UpdateFolderTypeComboVisibility();

                // Add handler to folderTypeCombo to uncheck deleteDesktopIniCheckbox when non-"No change" selected
                folderTypeCombo.SelectedIndexChanged += (s, ev) =>
                {
                    if (folderTypeCombo.SelectedIndex != 0 && deleteDesktopIniCheckbox != null) // Not "No change"
                    {
                        deleteDesktopIniCheckbox.Checked = false;
                    }
                };

                // Resume layout and force complete layout update
                ResumeLayout(true);
                PerformLayout();

                // Force one final layout update after resume to ensure all calculations are applied
                UpdateDialogLayout();

                // Force the form to update its bounds after layout changes
                Update();

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                // Final layout update right before the form is displayed
                // This ensures all size calculations are applied correctly
                UpdateDialogLayout();
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    if (newFolder.Length > 260)
                    {
                        newFolder = GetShortPath(newFolder);
                    }

                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);

                    folderPathLabel.Text = StartDirectory;
                    folderPathScrollBar.Value = 0;

                    // Update scrollbar range
                    using (Graphics g = CreateGraphics())
                    {
                        SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                        int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                        folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                        folderPathScrollBar.Visible = maxScroll > 0;
                    }

                    // Update folder information
                    UpdateFolderInfo();
                }
            }

            private void UpdateFolderInfo()
            {
                string driveLetter = System.IO.Path.GetPathRoot(StartDirectory)?.TrimEnd('\\');

                if (!string.IsNullOrEmpty(driveLetter))
                {
                    string fileSystem = Program.GetFileSystem(driveLetter);
                    string driveType = Program.GetDriveTypeLabel(driveLetter);
                    bool aftdEnabled = Program.IsAFTDEnabled();

                    fileSystemLabel.Text = sFileSystemPrefix + " " + fileSystem;
                    driveTypeLabel.Text = sTypePrefix + " " + driveType;

                    // Set AFTD CheckBox state
                    aftdCheckbox.Checked = aftdEnabled;

                    // Calculate DriveOK: NTFS + Local Disk (AFTD state doesn't affect GroupBox visibility)
                    DriveOK = (fileSystem == "NTFS") && (driveType == "Local Disk");
                }
                else
                {
                    fileSystemLabel.Text = $"{sFileSystemPrefix} {sNA}";
                    driveTypeLabel.Text = $"{sTypePrefix} {sNA}";
                    aftdCheckbox.Checked = false;
                    DriveOK = false;
                }

                // Folder Type GroupBox is always visible, but controls visibility depends on DriveOK
                folderTypeGroupBox.Visible = true;

                // Control visibility within folder type group based on DriveOK
                folderTypeCombo.Visible = DriveOK;
                FTRestoreDefaultsCheckbox.Visible = DriveOK;
                folderTypeSubtitle.Visible = false; // This is controlled by UpdateFolderTypeComboVisibility
                folderTypeRequirementsLabel.Visible = !DriveOK;

                // If DriveOK is true, update combo visibility based on AFTD state
                if (DriveOK)
                {
                    UpdateFolderTypeComboVisibility();
                }

                // Icon GroupBox is always visible regardless of drive prerequisites
                folderIconGroupBox.Visible = true;

                UpdateDialogLayout();
            }

            private void UpdateFolderTypeComboVisibility()
            {
                // Only manage visibility if DriveOK is true
                if (DriveOK)
                {
                    // Show combobox only when AFTD is enabled (checked) and removeFolderType is not checked
                    bool showCombo = aftdCheckbox.Checked && !FTRestoreDefaultsCheckbox.Checked;
                    folderTypeCombo.Visible = showCombo;

                    // Show subtitle when AFTD is disabled and removeFolderType is not checked
                    folderTypeSubtitle.Visible = !aftdCheckbox.Checked && !FTRestoreDefaultsCheckbox.Checked;

                    // Requirements label should never show when DriveOK is true
                    folderTypeRequirementsLabel.Visible = false;
                }

                UpdateDialogLayout();
            }



            private void UpdateDialogLayout()
            {
                int xMargin = (int)(10 * ScaleFactor);

                // Start positioning with static calculation - always reserve space for scrollbar
                int currentY = folderPathLabel.Bottom + SystemInformation.HorizontalScrollBarHeight + (int)(5 * ScaleFactor);

                // Global Settings GroupBox - fixed height to accommodate all states
                globalSettingsGroupBox.Location = new Point(xMargin, currentY);
                globalSettingsGroupBox.Height = (int)(146 * ScaleFactor);
                currentY = globalSettingsGroupBox.Bottom + (int)(20 * ScaleFactor);

                // Folder Type GroupBox - position if visible
                if (folderTypeGroupBox.Visible)
                {
                    folderTypeGroupBox.Location = new Point(xMargin, currentY);
                    folderTypeGroupBox.Height = (int)(90 * ScaleFactor);
                }
                // Always advance currentY to reserve space for folder type group box, even if hidden
                currentY += (int)(90 * ScaleFactor) + (int)(20 * ScaleFactor);

                // Folder Icon GroupBox - position if visible
                if (folderIconGroupBox.Visible)
                {
                    folderIconGroupBox.Location = new Point(xMargin, currentY);
                    folderIconGroupBox.Height = (int)(115 * ScaleFactor);
                }
                // Always advance currentY to reserve space for folder icon group box, even if hidden
                currentY += (int)(112 * ScaleFactor) + (int)(20 * ScaleFactor);

                // Position Restore defaults (IcoRestoreDefaultsCheckbox) checkbox inside icon group
                int iconGroupInternalY = (int)(25 * ScaleFactor); // Start after title
                iconGroupInternalY += (int)(30 * ScaleFactor); // Space for icon source combo
                iconGroupInternalY += (int)(30 * ScaleFactor); // Space for icon mode combo
                IcoRestoreDefaultsCheckbox.Location = new Point((int)(10 * ScaleFactor), iconGroupInternalY);

                // Fixed baseline for controls after group boxes - space is reserved even when group boxes hidden
                int fixedBaseY = currentY;

                // Position Reset icon cache checkbox (left justified, outside group box)
                resetIconCacheCheckbox.Location = new Point(xMargin, fixedBaseY);
                int nextY = resetIconCacheCheckbox.Bottom + (int)(7 * ScaleFactor);

                // Position Delete desktop.ini checkbox (left justified)
                deleteDesktopIniCheckbox.Location = new Point(xMargin, nextY);
                nextY = deleteDesktopIniCheckbox.Bottom + (int)(10 * ScaleFactor);

                // Position Apply to Subfolders checkbox
                applyToSubfoldersCheckbox.Location = new Point(
                    (ClientSize.Width - applyToSubfoldersCheckbox.PreferredSize.Width) / 2,
                    nextY
                );
                nextY = applyToSubfoldersCheckbox.Bottom + (int)(10 * ScaleFactor);

                // Position OK button
                buttonOK.Location = new Point(
                    (ClientSize.Width - buttonOK.Width) / 2,
                    nextY
                );
            }

            private void ButtonOK_Click(object sender, EventArgs e)
            {
                // Apply AFTD setting
                bool currentAftdState = Program.IsAFTDEnabled();
                if (aftdCheckbox.Checked != currentAftdState)
                {
                    if (aftdCheckbox.Checked)
                    {
                        SetAFTDEnabled();
                    }
                    else
                    {
                        SetAFTDDisabled();
                    }
                }

                // Apply Always Show Icons setting
                bool currentAlwaysShowIconsState = Program.IsAlwaysShowIconsEnabled();
                if (alwaysShowIconsCheckbox.Checked != currentAlwaysShowIconsState)
                {
                    Program.SetAlwaysShowIcons(alwaysShowIconsCheckbox.Checked);
                }

                // Apply Disable Folder Thumbnails setting
                bool currentFolderThumbnailsState = Program.IsFolderThumbnailsDisabled();
                if (disableFolderThumbnailsCheckbox.Checked != currentFolderThumbnailsState)
                {
                    Program.SetFolderThumbnailsDisabled(disableFolderThumbnailsCheckbox.Checked);
                }

                // Handle removal of FolderType if checkbox is checked
                if (DriveOK && FTRestoreDefaultsCheckbox.Checked)
                {
                    try
                    {
                        if (applyToSubfoldersCheckbox.Checked)
                        {
                            // Remove recursively from all subfolders
                            ApplyFolderTypeRecursive("None", StartDirectory);
                        }
                        else
                        {
                            // Remove only from the current folder
                            Program.ApplyFolderType("None", StartDirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error removing folder type: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // Apply folder type if one is selected in ComboBox and DriveOK is true (and remove checkbox is not checked)
                else if (DriveOK && folderTypeCombo.SelectedItem != null && !FTRestoreDefaultsCheckbox.Checked)
                {
                    string selectedType = folderTypeCombo.SelectedItem.ToString();

                    // Skip if "No change" is selected
                    if (selectedType != sNoChange)
                    {
                        string folderType = null;

                        // Map display name to internal folder type
                        if (selectedType == sGeneralItems)
                        {
                            folderType = "Generic";
                        }
                        else if (selectedType == sDocuments)
                        {
                            folderType = "Documents";
                        }
                        else if (selectedType == sPictures)
                        {
                            folderType = "Pictures";
                        }
                        else if (selectedType == sMusic)
                        {
                            folderType = "Music";
                        }
                        else if (selectedType == sVideos)
                        {
                            folderType = "Videos";
                        }

                        if (!string.IsNullOrEmpty(folderType))
                        {
                            try
                            {
                                if (applyToSubfoldersCheckbox.Checked)
                                {
                                    // Apply recursively to all subfolders
                                    ApplyFolderTypeRecursive(folderType, StartDirectory);
                                }
                                else
                                {
                                    // Apply only to the current folder
                                    Program.ApplyFolderType(folderType, StartDirectory);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error applying folder type: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                // Apply folder icon if one is selected (no DriveOK requirement)
                if (iconSourceCombo.SelectedItem != null)
                {
                    string iconSource = iconSourceCombo.SelectedItem.ToString();

                    // Skip if "No change" is selected
                    if (iconSource != sNoChange)
                    {
                        try
                        {
                            if (applyToSubfoldersCheckbox.Checked)
                            {
                                ApplyFolderIconRecursive(StartDirectory);
                            }
                            else
                            {
                                ApplyFolderIcon(StartDirectory);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error applying folder icon: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                // Set flag for post-dialog reset (will be checked in Program.cs after dialog returns)
                // This should be set regardless of whether an icon was applied
                Program.needsIconCacheReset = resetIconCacheCheckbox.Checked;

                // Handle folder icon restore defaults (delete desktop.ico and remove icon entries from desktop.ini)
                if (IcoRestoreDefaultsCheckbox.Checked)
                {
                    try
                    {
                        if (applyToSubfoldersCheckbox.Checked)
                        {
                            DeleteDesktopIcoRecursive(StartDirectory);
                            RemoveIconFromDesktopIniRecursive(StartDirectory);
                        }
                        else
                        {
                            DeleteDesktopIco(StartDirectory);
                            RemoveIconFromDesktopIni(StartDirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error restoring folder icon defaults: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Handle desktop.ini deletion (also deletes desktop.ico files)
                if (deleteDesktopIniCheckbox.Checked)
                {
                    try
                    {
                        if (applyToSubfoldersCheckbox.Checked)
                        {
                            DeleteDesktopIniRecursive(StartDirectory);
                            DeleteDesktopIcoRecursive(StartDirectory);
                        }
                        else
                        {
                            DeleteDesktopIni(StartDirectory);
                            DeleteDesktopIco(StartDirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting desktop.ini: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Delete desktop.ico files when using Selected icon or Selected color
                // (these reference existing icons but don't create desktop.ico, so clean up)
                if (iconSourceCombo.SelectedItem != null)
                {
                    string iconSource = iconSourceCombo.SelectedItem.ToString();
                    if (iconSource == sSelectedIcon || iconSource == sSelectedColor)
                    {
                        try
                        {
                            if (applyToSubfoldersCheckbox.Checked)
                            {
                                DeleteDesktopIcoRecursive(StartDirectory);
                            }
                            else
                            {
                                DeleteDesktopIco(StartDirectory);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting desktop.ico: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }

            private void ApplyFolderTypeRecursive(string folderType, string directory)
            {
                // Apply to the selected folder
                try
                {
                    Program.ApplyFolderType(folderType, directory);
                }
                catch { }

                // Recursively apply to subdirectories
                try
                {
                    string[] subDirectories = System.IO.Directory.GetDirectories(directory);
                    foreach (string subDirectory in subDirectories)
                    {
                        ApplyFolderTypeRecursive(folderType, subDirectory);
                    }
                }
                catch { }
            }

            private void DeleteDesktopIco(string directory)
            {
                string filePath = System.IO.Path.Combine(directory, "desktop.ico");
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.SetAttributes(filePath, System.IO.FileAttributes.Normal);
                        System.IO.File.Delete(filePath);
                    }
                    catch { }
                }
            }

            private void DeleteDesktopIcoRecursive(string directory)
            {
                DeleteDesktopIco(directory);
                try
                {
                    string[] subDirs = System.IO.Directory.GetDirectories(directory);
                    foreach (string subDir in subDirs)
                    {
                        DeleteDesktopIcoRecursive(subDir);
                    }
                }
                catch { }
            }

            private void RemoveIconFromDesktopIni(string directory)
            {
                try
                {
                    // Use SHGetSetFolderCustomSettings API to clear the icon (same as Explorer does)
                    const uint FCS_FORCEWRITE = 0x00000002;
                    const uint FCSM_ICONFILE = 0x00000010;

                    Program.SHFOLDERCUSTOMSETTINGS fcs = new Program.SHFOLDERCUSTOMSETTINGS();
                    fcs.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Program.SHFOLDERCUSTOMSETTINGS));
                    fcs.dwMask = FCSM_ICONFILE;
                    fcs.pszIconFile = null; // Clear the icon
                    fcs.iIconIndex = 0;

                    // Clear the icon setting - API handles desktop.ini updates
                    SHGetSetFolderCustomSettings(ref fcs, directory, FCS_FORCEWRITE);

                    // Remove ReadOnly attribute if no other customizations exist
                    string desktopIniPath = System.IO.Path.Combine(directory, "desktop.ini");
                    if (System.IO.File.Exists(desktopIniPath) && !Program.HasOtherEntries(desktopIniPath))
                    {
                        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(directory);
                        dirInfo.Attributes &= ~System.IO.FileAttributes.ReadOnly;
                    }
                }
                catch { }
            }

            private void RemoveIconFromDesktopIniRecursive(string directory)
            {
                RemoveIconFromDesktopIni(directory);
                try
                {
                    string[] subDirs = System.IO.Directory.GetDirectories(directory);
                    foreach (string subDir in subDirs)
                    {
                        RemoveIconFromDesktopIniRecursive(subDir);
                    }
                }
                catch { }
            }

            private void DeleteDesktopIni(string directory)
            {
                string filePath = System.IO.Path.Combine(directory, "desktop.ini");
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.SetAttributes(filePath, System.IO.FileAttributes.Normal);
                        System.IO.File.Delete(filePath);
                    }
                    catch { }
                }
            }

            private void DeleteDesktopIniRecursive(string directory)
            {
                DeleteDesktopIni(directory);
                try
                {
                    string[] subDirs = System.IO.Directory.GetDirectories(directory);
                    foreach (string subDir in subDirs)
                    {
                        DeleteDesktopIniRecursive(subDir);
                    }
                }
                catch { }
            }

            private void SetAFTDEnabled()
            {
                try
                {
                    string keyPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            // Remove the FolderType value to enable AFTD
                            key.DeleteValue("FolderType", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error enabling AFTD: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void SetAFTDDisabled()
            {
                try
                {
                    string keyPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell";
                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
                    {
                        if (key != null)
                        {
                            // Set FolderType to "Generic" to disable AFTD
                            key.SetValue("FolderType", "Generic", RegistryValueKind.String);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error disabling AFTD: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void IconSourceCombo_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (iconSourceCombo.SelectedIndex < 0) return;

                string iconSource = iconSourceCombo.SelectedItem.ToString();

                // Determine which icon mode combo to show based on selection
                // Index 0: "No change" - hide both
                // Index 1: "Selected color" - hide both
                // Index 2: "Selected icon" - hide both
                // Index 3: "Selected image" - show simple combo
                // Index 4-5: Folder-based options - show full combo

                bool showFullCombo = iconSourceCombo.SelectedIndex >= 4 && iconSourceCombo.SelectedIndex <= 5;
                bool showSimpleCombo = iconSourceCombo.SelectedIndex == 3;

                iconModeComboFull.Visible = showFullCombo;
                iconModeComboSimple.Visible = showSimpleCombo;

                UpdateDialogLayout();

                // Handle immediate file selection for "Selected image" and "Selected icon"
                if (iconSource == sSelectedImage)
                {
                    // Get required count based on current icon mode
                    int requiredCount = GetRequiredImageCount();

                    OpenFileDialog openDialog = new OpenFileDialog();
                    openDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff)|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff|All Files (*.*)|*.*";
                    openDialog.Title = $"Select {requiredCount} Image File{(requiredCount > 1 ? "s" : "")}";
                    openDialog.Multiselect = requiredCount > 1;

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedImagePaths = new List<string>(openDialog.FileNames);

                        if (selectedImagePaths.Count != requiredCount)
                        {
                            MessageBox.Show($"Please select exactly {requiredCount} image{(requiredCount > 1 ? "s" : "")}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            selectedImagePaths.Clear();
                            UpdateSelectedFilePathDisplay();
                            return;
                        }

                        selectedIconPath = null;
                        selectedColor = null;
                        selectedColorIndex = -1;

                        // Generate temp icon previews for all variants
                        GenerateTempIconPreviews(selectedImagePaths);

                        UpdateSelectedFilePathDisplay();
                    }
                    else
                    {
                        // User cancelled - reset to "No change"
                        iconSourceCombo.SelectedIndex = 0; // "No change"
                        selectedImagePaths.Clear();
                        selectedIconPath = null;
                        selectedColor = null;
                        UpdateSelectedFilePathDisplay();
                        this.ActiveControl = null;
                        return;
                    }
                }
                else if (iconSource == sSelectedIcon)
                {
                    OpenFileDialog openDialog = new OpenFileDialog();
                    openDialog.Filter = "Icon Files (*.ico)|*.ico|All Files (*.*)|*.*";
                    openDialog.InitialDirectory = System.IO.Path.Combine(appParts, "FolderIcons");
                    openDialog.Title = sSelectIconFile;

                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedIconPath = openDialog.FileName;
                        selectedImagePaths.Clear();
                        selectedColor = null;
                        selectedColorIndex = -1;
                        UpdateSelectedFilePathDisplay();
                    }
                    else
                    {
                        // User cancelled - reset to "No change"
                        iconSourceCombo.SelectedIndex = 0; // "No change"
                        selectedIconPath = null;
                        selectedImagePaths.Clear();
                        selectedColor = null;
                        UpdateSelectedFilePathDisplay();
                        this.ActiveControl = null;
                        return;
                    }
                }
                else if (iconSource == sSelectedColor)
                {
                    int? pickedColorIndex = FolderColorPickerDialog.Show();

                    if (pickedColorIndex.HasValue)
                    {
                        selectedColorIndex = pickedColorIndex.Value;
                        // Derive color from the index (same colors array as in picker)
                        Color[] colors = new Color[]
                        {
                            Color.FromArgb(247, 207, 56),   // 0: Yellow
                            Color.FromArgb(218, 63, 44),    // 1: Red
                            Color.FromArgb(226, 114, 18),   // 2: Orange
                            Color.FromArgb(79, 160, 71),    // 3: Green
                            Color.FromArgb(66, 147, 142),   // 4: Teal
                            Color.FromArgb(67, 130, 209),   // 5: Blue
                            Color.FromArgb(152, 94, 200),   // 6: Purple
                            Color.FromArgb(195, 81, 181),   // 7: Pink
                            Color.FromArgb(177, 183, 186),  // 8: Gray
                            Color.FromArgb(247, 188, 178),  // 9: Light Red
                            Color.FromArgb(247, 192, 132),  // 10: Light Orange
                            Color.FromArgb(150, 211, 143),  // 11: Light Green
                            Color.FromArgb(137, 209, 205),  // 12: Light Teal
                            Color.FromArgb(148, 199, 247),  // 13: Light Blue
                            Color.FromArgb(210, 174, 247),  // 14: Light Purple
                            Color.FromArgb(240, 169, 232)   // 15: Light Pink
                        };
                        selectedColor = colors[selectedColorIndex];
                        selectedIconPath = null;
                        selectedImagePaths.Clear();
                        UpdateSelectedFilePathDisplay();
                    }
                    else
                    {
                        // User cancelled - reset to "No change"
                        iconSourceCombo.SelectedIndex = 0; // "No change"
                        selectedColor = null;
                        selectedColorIndex = -1;
                        selectedIconPath = null;
                        selectedImagePaths.Clear();
                        UpdateSelectedFilePathDisplay();
                        this.ActiveControl = null;
                        return;
                    }
                }
                else if (iconSource == sMostRecentImages || iconSource == sFirstAlphabeticalImages)
                {
                    // Always clear previous selections when switching to folder-based options
                    selectedImagePaths.Clear();
                    selectedIconPath = null;
                    selectedColor = null;
                    selectedColorIndex = -1;
                    tempIconPaths.Clear();

                    // Generate temp icon previews for folder-based options
                    // Fetch up to 4 images to generate all possible variants
                    List<string> folderImages = GetImagePathsForPreview(StartDirectory, iconSource);
                    if (folderImages.Count > 0)
                    {
                        GenerateTempIconPreviews(folderImages);
                    }

                    // Always update display to trigger fallback logic for empty folders
                    UpdateSelectedFilePathDisplay();
                }
                else
                {
                    // For other options, clear any selected paths
                    selectedImagePaths.Clear();
                    selectedIconPath = null;
                    selectedColor = null;
                    selectedColorIndex = -1;
                    tempIconPaths.Clear();
                    UpdateSelectedFilePathDisplay();
                }
            }

            private void IconModeCombo_SelectedIndexChanged(object sender, EventArgs e)
            {
                // If "Selected image" is active and mode changed, prompt for new images
                if (iconSourceCombo.SelectedItem?.ToString() == sSelectedImage)
                {
                    int requiredCount = GetRequiredImageCount();

                    // Only re-prompt if the required count changed
                    if (selectedImagePaths.Count != requiredCount)
                    {
                        OpenFileDialog openDialog = new OpenFileDialog();
                        openDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff)|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff|All Files (*.*)|*.*";
                        openDialog.Title = $"Select {requiredCount} Image File{(requiredCount > 1 ? "s" : "")}";
                        openDialog.Multiselect = requiredCount > 1;

                        if (openDialog.ShowDialog() == DialogResult.OK)
                        {
                            selectedImagePaths = new List<string>(openDialog.FileNames);

                            if (selectedImagePaths.Count != requiredCount)
                            {
                                MessageBox.Show($"Please select exactly {requiredCount} image{(requiredCount > 1 ? "s" : "")}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                selectedImagePaths.Clear();
                            }
                            else
                            {
                                // Regenerate temp icon previews with new images
                                GenerateTempIconPreviews(selectedImagePaths);
                            }

                            UpdateSelectedFilePathDisplay();
                        }
                    }
                    else
                    {
                        // Image count hasn't changed, just update preview for new mode
                        UpdateSelectedFilePathDisplay();
                    }
                }
                else if (iconSourceCombo.SelectedItem?.ToString() == sMostRecentImages || 
                         iconSourceCombo.SelectedItem?.ToString() == sFirstAlphabeticalImages)
                {
                    // Update preview for folder-based options when mode changes
                    UpdateSelectedFilePathDisplay();
                }
            }

            private string GetColorIconFilename(int colorIndex)
            {
                // Map color index to icon filename
                string[] colorNames = new string[]
                {
                    "Yellow", "Red", "Orange", "Green", "Teal", "Blue", "Purple", "Pink",
                    "Gray", "Red", "Orange", "Green", "Teal", "Blue", "Purple", "Pink"
                };

                int fileNumber = colorIndex + 1; // 0-based to 1-based (01-16)
                return $"{fileNumber:D2}{colorNames[colorIndex]}.ico";
            }

            private string GetSelectedFilePathDisplayText()
            {
                if (!string.IsNullOrEmpty(selectedIconPath))
                {
                    return selectedIconPath;
                }
                else if (selectedImagePaths.Count > 0)
                {
                    return string.Join("; ", selectedImagePaths);
                }
                else if (selectedColorIndex >= 0)
                {
                    // Return empty string - we'll show icon preview instead
                    return "";
                }
                else
                {
                    return "";
                }
            }

            private void UpdateSelectedFilePathDisplay()
            {
                // Handle colored folder icon preview in iconPreviewBox
                if (selectedColorIndex >= 0)
                {
                    string iconFolder = GetIconFolderForVersion();
                    string iconFilename = GetColorIconFilename(selectedColorIndex);
                    string iconPath = System.IO.Path.Combine(appParts, "Icons", iconFolder, iconFilename);

                    if (System.IO.File.Exists(iconPath))
                    {
                        try
                        {
                            // Extract 256x256 bitmap from icon
                            Bitmap bitmap = ExtractLargestBitmapFromIcon(iconPath);
                            iconPreviewBox.Image = bitmap;
                            iconPreviewBox.Visible = true;
                        }
                        catch
                        {
                            iconPreviewBox.Image = null;
                            iconPreviewBox.Visible = false;
                        }
                    }
                    else
                    {
                        iconPreviewBox.Image = null;
                        iconPreviewBox.Visible = false;
                    }

                    selectedFilePathLabel.Visible = false;
                    selectedFilePathScrollBar.Visible = false;
                }
                // Handle selected icon preview in iconPreviewBox
                else if (!string.IsNullOrEmpty(selectedIconPath))
                {
                    if (System.IO.File.Exists(selectedIconPath))
                    {
                        try
                        {
                            // Extract 256x256 bitmap from icon
                            Bitmap bitmap = ExtractLargestBitmapFromIcon(selectedIconPath);
                            iconPreviewBox.Image = bitmap;
                            iconPreviewBox.Visible = true;
                        }
                        catch
                        {
                            iconPreviewBox.Image = null;
                            iconPreviewBox.Visible = false;
                        }
                    }
                    else
                    {
                        iconPreviewBox.Image = null;
                        iconPreviewBox.Visible = false;
                    }

                    selectedFilePathLabel.Visible = false;
                    selectedFilePathScrollBar.Visible = false;
                }
                // Handle selected image(s) preview or folder-based options in iconPreviewBox
                else if (tempIconPaths.Count > 0 || 
                         iconSourceCombo.SelectedItem?.ToString() == "Most recent image(s) in folder" ||
                         iconSourceCombo.SelectedItem?.ToString() == "First image(s) alphabetically in folder")
                {
                    try
                    {
                        // Load and display the appropriate temp icon variant based on current mode
                        string currentMode = GetCurrentIconModeKey();
                        string iconPathToDisplay = null;

                        // First try to use the temp icon if it exists
                        if (tempIconPaths.ContainsKey(currentMode))
                        {
                            string tempIconPath = tempIconPaths[currentMode];
                            if (System.IO.File.Exists(tempIconPath))
                            {
                                iconPathToDisplay = tempIconPath;
                            }
                        }

                        // Fall back to pre-made preview icons only if "Also apply to subfolders" is checked
                        // (If not checked and current folder has no images, nothing will be set, so don't show misleading preview)
                        if (iconPathToDisplay == null && applyToSubfoldersCheckbox.Checked)
                        {
                            string filename = GetIconFilenameFromMode(currentMode);
                            string fallbackPath = System.IO.Path.Combine(appParts, "Icons", "Preview", filename);
                            if (System.IO.File.Exists(fallbackPath))
                            {
                                iconPathToDisplay = fallbackPath;
                            }
                        }

                        if (iconPathToDisplay != null)
                        {
                            // Extract 256x256 bitmap from icon
                            Bitmap bitmap = ExtractLargestBitmapFromIcon(iconPathToDisplay);
                            iconPreviewBox.Image = bitmap;
                            iconPreviewBox.Visible = true;
                        }
                        else
                        {
                            iconPreviewBox.Image = null;
                            iconPreviewBox.Visible = false;
                        }
                    }
                    catch
                    {
                        iconPreviewBox.Image = null;
                        iconPreviewBox.Visible = false;
                    }

                    selectedFilePathLabel.Visible = false;
                    selectedFilePathScrollBar.Visible = false;
                }
                else
                {
                    iconPreviewBox.Image = null;
                    iconPreviewBox.Visible = false;
                    selectedFilePathLabel.Visible = false;
                    selectedFilePathScrollBar.Visible = false;
                }

                UpdateDialogLayout();
            }

            private void ApplyFolderIcon(string directory)
            {
                if (iconSourceCombo.SelectedIndex < 0) return;

                string iconSource = iconSourceCombo.SelectedItem.ToString();

                if (iconSource == "Selected icon")
                {
                    if (!string.IsNullOrEmpty(selectedIconPath))
                    {
                        SetFolderIcon(directory, selectedIconPath);
                    }
                }
                else if (iconSource == sSelectedImage)
                {
                    if (selectedImagePaths.Count > 0)
                    {
                        CreateAndSetFolderIcon(directory, selectedImagePaths);
                    }
                }
                else if (iconSource == sSelectedColor)
                {
                    if (selectedColorIndex >= 0)
                    {
                        string iconFolder = GetIconFolderForVersion();
                        string iconFilename = GetColorIconFilename(selectedColorIndex);
                        string iconPath = System.IO.Path.Combine(appParts, "Icons", iconFolder, iconFilename);

                        if (System.IO.File.Exists(iconPath))
                        {
                            SetFolderIcon(directory, iconPath);
                        }
                    }
                }
                else
                {
                    // Image-based icon from folder
                    List<string> imagePaths = GetImagePaths(directory, iconSource);
                    if (imagePaths.Count > 0)
                    {
                        CreateAndSetFolderIcon(directory, imagePaths);
                    }
                }
            }

            private void ApplyFolderIconRecursive(string directory)
            {
                // Apply to the selected folder
                try
                {
                    string iconSource = iconSourceCombo.SelectedItem.ToString();

                    if (iconSource == sSelectedImage)
                    {
                        // Use stored image paths for all folders
                        if (selectedImagePaths.Count > 0)
                        {
                            CreateAndSetFolderIconRecursive(directory, selectedImagePaths);
                        }
                    }
                    else if (iconSource == sSelectedIcon)
                    {
                        // Use stored icon path for all folders
                        if (!string.IsNullOrEmpty(selectedIconPath))
                        {
                            SetFolderIconRecursive(directory, selectedIconPath);
                        }
                    }
                    else if (iconSource == sSelectedColor)
                    {
                        // Use selected color icon for all folders
                        if (selectedColorIndex >= 0)
                        {
                            string iconFolder = GetIconFolderForVersion();
                            string iconFilename = GetColorIconFilename(selectedColorIndex);
                            string iconPath = System.IO.Path.Combine(appParts, "Icons", iconFolder, iconFilename);

                            if (System.IO.File.Exists(iconPath))
                            {
                                SetFolderIconRecursive(directory, iconPath);
                            }
                        }
                    }
                    else
                    {
                        ApplyFolderIcon(directory);

                        // Recursively apply to subdirectories
                        string[] subDirectories = System.IO.Directory.GetDirectories(directory);
                        foreach (string subDirectory in subDirectories)
                        {
                            ApplyFolderIconRecursive(subDirectory);
                        }
                    }
                }
                catch { }
            }

            private void CreateAndSetFolderIconRecursive(string directory, List<string> selectedImages)
            {
                try
                {
                    CreateAndSetFolderIcon(directory, selectedImages);

                    string[] subDirectories = System.IO.Directory.GetDirectories(directory);
                    foreach (string subDirectory in subDirectories)
                    {
                        CreateAndSetFolderIconRecursive(subDirectory, selectedImages);
                    }
                }
                catch { }
            }

            private void SetFolderIconRecursive(string directory, string iconPath)
            {
                try
                {
                    SetFolderIcon(directory, iconPath);

                    string[] subDirectories = System.IO.Directory.GetDirectories(directory);
                    foreach (string subDirectory in subDirectories)
                    {
                        SetFolderIconRecursive(subDirectory, iconPath);
                    }
                }
                catch { }
            }

            private List<string> GetImagePaths(string directory, string source)
            {
                List<string> imagePaths = new List<string>();
                int requiredCount = GetRequiredImageCount();

                if (source == "Most recent image(s) in folder")
                {
                    if (requiredCount == 1)
                    {
                        string newest = Program.GetNewestImageFileInDirectory(directory);
                        if (newest != null) imagePaths.Add(newest);
                    }
                    else if (requiredCount == 2)
                    {
                        imagePaths = Program.GetTwoNewestImageFilesInDirectory(directory);
                    }
                    else if (requiredCount == 4)
                    {
                        imagePaths = Program.GetFourNewestImageFilesInDirectory(directory);
                    }
                }
                else if (source == "First image(s) alphabetically in folder")
                {
                    if (requiredCount == 1)
                    {
                        string first = Program.GetFirstImageFileInDirectory(directory);
                        if (first != null) imagePaths.Add(first);
                    }
                    else if (requiredCount == 2)
                    {
                        imagePaths = Program.GetFirstTwoImageFilesInDirectory(directory);
                    }
                    else if (requiredCount == 4)
                    {
                        imagePaths = Program.GetFirstFourImageFilesInDirectory(directory);
                    }
                }

                return imagePaths;
            }

            private int GetRequiredImageCount()
            {
                // Check which combo box is visible and get the mode from it
                CustomComboBox activeCombo = iconModeComboFull.Visible ? iconModeComboFull : iconModeComboSimple;

                if (activeCombo.SelectedIndex < 0) return 1;

                string mode = activeCombo.SelectedItem.ToString();
                if (mode.StartsWith("Fill (2")) return 2;
                if (mode.StartsWith("Fill (4")) return 4;
                return 1;
            }

            private void CreateAndSetFolderIcon(string directory, List<string> imagePaths)
            {
                if (imagePaths.Count == 0) return;

                string iconPath = System.IO.Path.Combine(directory, "desktop.ico");

                // Get the selected mode from whichever combo is visible
                CustomComboBox activeCombo = iconModeComboFull.Visible ? iconModeComboFull : iconModeComboSimple;
                string selectedMode = activeCombo.SelectedItem.ToString();
                string actualMode = selectedMode;
                List<string> actualImagePaths = imagePaths;

                // Apply fallback logic based on available image count
                if (selectedMode.StartsWith("Fill (4"))
                {
                    // 4 images mode
                    if (imagePaths.Count < 4 && imagePaths.Count >= 2)
                    {
                        // Fall back to 2 landscape
                        actualMode = sFill2Landscape;
                        actualImagePaths = new List<string> { imagePaths[0], imagePaths[1] };
                    }
                    else if (imagePaths.Count == 1)
                    {
                        // Fall back to single image
                        actualMode = sFillSingle;
                        actualImagePaths = new List<string> { imagePaths[0] };
                    }
                    else if (imagePaths.Count == 0)
                    {
                        return; // No images, don't create icon
                    }
                }
                else if (selectedMode.StartsWith("Fill (2"))
                {
                    // 2 landscape or 2 portrait mode
                    if (imagePaths.Count == 1)
                    {
                        // Fall back to single image
                        actualMode = sFillSingle;
                        actualImagePaths = new List<string> { imagePaths[0] };
                    }
                    else if (imagePaths.Count == 0)
                    {
                        return; // No images, don't create icon
                    }
                }

                // Create icon from image(s)
                Program.CreateIconFromImages(actualImagePaths, iconPath, actualMode);

                // Set icon in desktop.ini
                SetFolderIcon(directory, iconPath);
            }

            private void SetFolderIcon(string directory, string iconPath)
            {
                try
                {
                    // Set desktop.ico as hidden and system (only if it exists in the target directory)
                    string desktopIcoPath = System.IO.Path.Combine(directory, "desktop.ico");
                    if (System.IO.File.Exists(desktopIcoPath))
                    {
                        System.IO.File.SetAttributes(desktopIcoPath, System.IO.File.GetAttributes(desktopIcoPath) | System.IO.FileAttributes.System | System.IO.FileAttributes.Hidden);
                    }

                    // Use SHGetSetFolderCustomSettings API to set the icon (same as Explorer does)
                    const uint FCS_FORCEWRITE = 0x00000002;
                    const uint FCSM_ICONFILE = 0x00000010;

                    Program.SHFOLDERCUSTOMSETTINGS fcs = new Program.SHFOLDERCUSTOMSETTINGS();
                    fcs.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Program.SHFOLDERCUSTOMSETTINGS));
                    fcs.dwMask = FCSM_ICONFILE;
                    fcs.pszIconFile = iconPath;
                    fcs.cchIconFile = (uint)iconPath.Length;
                    fcs.iIconIndex = 0;

                    // Write the icon setting - this should trigger immediate AFTD
                    int result = SHGetSetFolderCustomSettings(ref fcs, directory, FCS_FORCEWRITE);

                    // Set folder attribute to ReadOnly (required for custom icons)
                    System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(directory);
                    dirInfo.Attributes |= System.IO.FileAttributes.ReadOnly;
                }
                catch { }
            }

            private void GenerateTempIconPreviews(List<string> imagePaths)
            {
                if (imagePaths.Count == 0) return;

                try
                {
                    string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RightClickTools");

                    // Ensure temp directory exists
                    if (!System.IO.Directory.Exists(tempDir))
                    {
                        System.IO.Directory.CreateDirectory(tempDir);
                    }

                    tempIconPaths.Clear();

                    // Generate all possible variants based on available images
                    // Always generate single-image variants
                    GenerateTempIconVariant(imagePaths, tempDir, "Fit (transparent background)", 1);
                    GenerateTempIconVariant(imagePaths, tempDir, "Fit (solid background)", 1);
                    GenerateTempIconVariant(imagePaths, tempDir, "Fill (single image)", 1);

                    // Generate 2-image variants if we have at least 2 images
                    if (imagePaths.Count >= 2)
                    {
                        GenerateTempIconVariant(imagePaths, tempDir, "Fill (2 landscape images)", 2);
                        GenerateTempIconVariant(imagePaths, tempDir, "Fill (2 portrait images)", 2);
                    }

                    // Generate 4-image variant if we have at least 4 images
                    if (imagePaths.Count >= 4)
                    {
                        GenerateTempIconVariant(imagePaths, tempDir, "Fill (4 images)", 4);
                    }
                }
                catch { }
            }

            private void GenerateTempIconVariant(List<string> imagePaths, string tempDir, string mode, int imageCount)
            {
                try
                {
                    string key = mode;
                    string filename = GetIconFilenameFromMode(mode);
                    string tempIconPath = System.IO.Path.Combine(tempDir, filename);

                    // Take only the required number of images
                    List<string> imagesToUse = imagePaths.Take(imageCount).ToList();

                    // Generate the icon
                    Program.CreateIconFromImages(imagesToUse, tempIconPath, mode);

                    // Store the path
                    tempIconPaths[key] = tempIconPath;
                }
                catch { }
            }

            private string GetCurrentIconModeKey()
            {
                CustomComboBox activeCombo = iconModeComboFull.Visible ? iconModeComboFull : iconModeComboSimple;
                if (activeCombo.SelectedIndex >= 0)
                {
                    return activeCombo.SelectedItem.ToString();
                }
                return "Fit (transparent background)"; // Default
            }

            private string GetIconFilenameFromMode(string mode)
            {
                switch (mode)
                {
                    case "Fit (transparent background)":
                        return "Fit_trans.ico";
                    case "Fit (solid background)":
                        return "Fit_solid.ico";
                    case "Fill (single image)":
                        return "Fill_1.ico";
                    case "Fill (2 landscape images)":
                        return "Fill_2L.ico";
                    case "Fill (2 portrait images)":
                        return "Fill_2P.ico";
                    case "Fill (4 images)":
                        return "Fill_4.ico";
                    default:
                        return "Fill_1.ico";
                }
            }

            private Bitmap ExtractLargestBitmapFromIcon(string iconPath)
            {
                using (Icon icon = new Icon(iconPath))
                {
                    // Try to find and extract the 256x256 image from the icon
                    using (System.IO.FileStream fs = new System.IO.FileStream(iconPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
                        {
                            // Read icon header
                            br.ReadUInt16(); // Reserved (always 0)
                            br.ReadUInt16(); // Image type (1 for icon)
                            ushort imageCount = br.ReadUInt16();

                            int largestSize = 0;
                            int largestOffset = 0;
                            int largestByteCount = 0;

                            // Read directory entries to find the largest icon
                            for (int i = 0; i < imageCount; i++)
                            {
                                byte width = br.ReadByte();
                                byte height = br.ReadByte();
                                br.ReadByte(); // Color count
                                br.ReadByte(); // Reserved
                                br.ReadUInt16(); // Color planes
                                br.ReadUInt16(); // Bits per pixel
                                int byteCount = br.ReadInt32();
                                int offset = br.ReadInt32();

                                // 0 means 256
                                int actualWidth = width == 0 ? 256 : width;
                                int actualHeight = height == 0 ? 256 : height;
                                int size = Math.Min(actualWidth, actualHeight);

                                if (size > largestSize)
                                {
                                    largestSize = size;
                                    largestOffset = offset;
                                    largestByteCount = byteCount;
                                }
                            }

                            // Read the largest icon image
                            if (largestByteCount > 0)
                            {
                                fs.Seek(largestOffset, System.IO.SeekOrigin.Begin);
                                byte[] iconData = br.ReadBytes(largestByteCount);

                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(iconData))
                                {
                                    return new Bitmap(ms);
                                }
                            }
                        }
                    }

                    // Fallback if extraction fails
                    return icon.ToBitmap();
                }
            }

            private List<string> GetImagePathsForPreview(string directory, string source)
            {
                // For preview generation, always fetch up to 4 images to generate all variants
                List<string> imagePaths = new List<string>();

                if (source == "Most recent image(s) in folder")
                {
                    imagePaths = Program.GetFourNewestImageFilesInDirectory(directory);
                }
                else if (source == "First image(s) alphabetically in folder")
                {
                    imagePaths = Program.GetFirstFourImageFilesInDirectory(directory);
                }

                return imagePaths;
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var folderOptionsDialog = new FolderOptionsDialog(message, caption))
                {
                    return folderOptionsDialog.ShowDialog();
                }
            }
        }

        // Dialog for Shortcut Tool
        public class ShortcutToolDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;
            private Label folderPathLabel;
            private FlatScrollBar folderPathScrollBar;
            private CustomCheckBox convertUrlToLnkCheckbox;
            private CustomCheckBox moveUrlToRecycleBinCheckbox;
            private CustomCheckBox applyToSubfoldersCheckbox;
            private TextBox searchTextBox;
            private TextBox replaceTextBox;
            private CustomCheckBox searchTargetCheckbox;
            private CustomCheckBox searchStartInCheckbox;
            private CustomCheckBox searchIconCheckbox;
            private CustomGroupBox convertGroupBox;
            private CustomGroupBox searchReplaceGroupBox;

            // Public properties to access checkbox states
            public bool ConvertUrlToLnk => convertUrlToLnkCheckbox.Checked;
            public bool MoveUrlToRecycleBin => moveUrlToRecycleBinCheckbox.Checked;
            public bool ApplyToSubfolders => applyToSubfoldersCheckbox.Checked;
            public string SearchText => searchTextBox.Text;
            public string ReplaceText => replaceTextBox.Text;
            public bool SearchTarget => searchTargetCheckbox.Checked;
            public bool SearchStartIn => searchStartInCheckbox.Checked;
            public bool SearchIcon => searchIconCheckbox.Checked;

            public ShortcutToolDialog(string message, string caption)
            {
                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(490 * ScaleFactor);
                Height = (int)(450 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                // Help button (top right)
                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                // Folder picker button (top left)
                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                // Message label for title (centered between folder and help icons)
                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.MiddleCenter;
                messageLabel.AutoSize = false;
                messageLabel.Location = new Point((int)(35 * ScaleFactor), (int)(5 * ScaleFactor));
                messageLabel.Width = ClientSize.Width - (int)(70 * ScaleFactor);
                messageLabel.Height = (int)(20 * ScaleFactor);

                // Folder path display
                int controlHeight = (int)(24 * ScaleFactor);
                int xMargin = (int)(10 * ScaleFactor);

                folderPathLabel = new Label();
                folderPathLabel.Font = new Font("Segoe UI", 9);
                folderPathLabel.Location = new Point(xMargin, (int)(40 * ScaleFactor));
                folderPathLabel.Width = ClientSize.Width - (xMargin * 2);
                folderPathLabel.Height = controlHeight;
                folderPathLabel.AutoSize = false;
                folderPathLabel.BorderStyle = BorderStyle.None;
                folderPathLabel.Padding = new Padding(2, 2, 2, 2);
                folderPathLabel.BackColor = SystemColors.Control;

                // Custom paint to ensure text is always vertically centered
                folderPathLabel.Paint += (s, pe) =>
                {
                    Label lbl = s as Label;
                    if (lbl != null)
                    {
                        pe.Graphics.Clear(lbl.BackColor);

                        if (!string.IsNullOrEmpty(lbl.Text))
                        {
                            Rectangle textRect = new Rectangle(
                                lbl.Padding.Left, 
                                0, 
                                lbl.Width - lbl.Padding.Left - lbl.Padding.Right, 
                                lbl.Height);
                            TextRenderer.DrawText(
                                pe.Graphics, 
                                lbl.Text, 
                                lbl.Font, 
                                textRect, 
                                lbl.ForeColor, 
                                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                        }

                        // Always draw thin border
                        Color borderColor = Dark ? Color.FromArgb(100, 100, 100) : Color.FromArgb(171, 173, 179);
                        using (Pen borderPen = new Pen(borderColor, 1))
                        {
                            pe.Graphics.DrawRectangle(borderPen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                        }
                    }
                };

                // Add horizontal scrollbar for folder path
                folderPathScrollBar = new FlatScrollBar();
                folderPathScrollBar.Orientation = ScrollBarOrientation.Horizontal;
                folderPathScrollBar.Location = new Point(xMargin, (int)(40 * ScaleFactor) + controlHeight - 1);
                folderPathScrollBar.Width = ClientSize.Width - (xMargin * 2);
                folderPathScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
                folderPathScrollBar.Minimum = 0;
                folderPathScrollBar.SmallChange = 5;
                folderPathScrollBar.LargeChange = 20;
                folderPathScrollBar.Scroll += (s, ev) =>
                {
                    int offset = folderPathScrollBar.Value;
                    folderPathLabel.Text = StartDirectory.Length > offset ? StartDirectory.Substring(offset) : "";
                };

                // Calculate scrollbar range based on text length
                using (Graphics g = CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                    int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                    folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                    folderPathScrollBar.Visible = maxScroll > 0;
                }

                // Convert GroupBox
                convertGroupBox = new CustomGroupBox();
                convertGroupBox.Font = new Font("Segoe UI", 9);
                convertGroupBox.Text = sConvert;
                convertGroupBox.Location = new Point(xMargin, (int)(87 * ScaleFactor));
                convertGroupBox.Width = ClientSize.Width - (xMargin * 2);
                convertGroupBox.Height = (int)(75 * ScaleFactor);
                convertGroupBox.Padding = new Padding((int)(10 * ScaleFactor), (int)(5 * ScaleFactor), (int)(10 * ScaleFactor), (int)(15 * ScaleFactor));

                // Convert URL to LNK checkbox
                convertUrlToLnkCheckbox = new CustomCheckBox();
                convertUrlToLnkCheckbox.Font = new Font("Segoe UI", 9);
                convertUrlToLnkCheckbox.Text = sConvertUrlToLnk;
                convertUrlToLnkCheckbox.Checked = false;
                convertUrlToLnkCheckbox.AutoSize = true;
                convertUrlToLnkCheckbox.Location = new Point((int)(10 * ScaleFactor), (int)(22 * ScaleFactor));
                convertUrlToLnkCheckbox.CheckedChanged += (s, e) =>
                {
                    moveUrlToRecycleBinCheckbox.Enabled = convertUrlToLnkCheckbox.Checked;
                    if (!convertUrlToLnkCheckbox.Checked) moveUrlToRecycleBinCheckbox.Checked = false;
                };

                // Move URL to Recycle Bin sub-checkbox (indented)
                moveUrlToRecycleBinCheckbox = new CustomCheckBox();
                moveUrlToRecycleBinCheckbox.Font = new Font("Segoe UI", 9);
                moveUrlToRecycleBinCheckbox.Text = sMoveUrlToRecycleBin;
                moveUrlToRecycleBinCheckbox.Checked = false;
                moveUrlToRecycleBinCheckbox.Enabled = false;
                moveUrlToRecycleBinCheckbox.AutoSize = true;
                moveUrlToRecycleBinCheckbox.Location = new Point((int)(30 * ScaleFactor), (int)(44 * ScaleFactor));

                convertGroupBox.Controls.Add(convertUrlToLnkCheckbox);
                convertGroupBox.Controls.Add(moveUrlToRecycleBinCheckbox);

                // Search and Replace GroupBox
                searchReplaceGroupBox = new CustomGroupBox();
                searchReplaceGroupBox.Font = new Font("Segoe UI", 9);
                searchReplaceGroupBox.Text = sSearchAndReplace;
                searchReplaceGroupBox.Location = new Point(xMargin, (int)(184 * ScaleFactor));
                searchReplaceGroupBox.Width = ClientSize.Width - (xMargin * 2);
                searchReplaceGroupBox.Height = (int)(135 * ScaleFactor);
                searchReplaceGroupBox.Padding = new Padding((int)(10 * ScaleFactor), (int)(5 * ScaleFactor), (int)(10 * ScaleFactor), (int)(15 * ScaleFactor));

                // Search label and textbox
                Label searchLabel = new Label();
                searchLabel.Font = new Font("Segoe UI", 9);
                searchLabel.Text = sSearchFor;
                searchLabel.AutoSize = true;
                searchLabel.Location = new Point((int)(10 * ScaleFactor), (int)(24 * ScaleFactor));

                searchTextBox = new TextBox();
                searchTextBox.Font = new Font("Segoe UI", 9);
                searchTextBox.Location = new Point((int)(110 * ScaleFactor), (int)(22 * ScaleFactor));
                searchTextBox.Width = searchReplaceGroupBox.Width - (int)(125 * ScaleFactor);
                searchTextBox.TextChanged += (s, e) =>
                {
                    bool hasSearchText = !string.IsNullOrEmpty(searchTextBox.Text);
                    searchTargetCheckbox.Enabled = hasSearchText;
                    searchStartInCheckbox.Enabled = hasSearchText;
                    searchIconCheckbox.Enabled = hasSearchText;
                    if (!hasSearchText)
                    {
                        searchTargetCheckbox.Checked = false;
                        searchStartInCheckbox.Checked = false;
                        searchIconCheckbox.Checked = false;
                    }
                };

                // Replace label and textbox
                Label replaceLabel = new Label();
                replaceLabel.Font = new Font("Segoe UI", 9);
                replaceLabel.Text = sReplaceWith;
                replaceLabel.AutoSize = true;
                replaceLabel.Location = new Point((int)(10 * ScaleFactor), (int)(54 * ScaleFactor));

                replaceTextBox = new TextBox();
                replaceTextBox.Font = new Font("Segoe UI", 9);
                replaceTextBox.Location = new Point((int)(110 * ScaleFactor), (int)(52 * ScaleFactor));
                replaceTextBox.Width = searchReplaceGroupBox.Width - (int)(125 * ScaleFactor);

                // Search field checkboxes
                Label searchFieldsLabel = new Label();
                searchFieldsLabel.Font = new Font("Segoe UI", 9);
                searchFieldsLabel.Text = sSearchIn;
                searchFieldsLabel.AutoSize = true;
                searchFieldsLabel.Location = new Point((int)(10 * ScaleFactor), (int)(84 * ScaleFactor));

                searchTargetCheckbox = new CustomCheckBox();
                searchTargetCheckbox.Font = new Font("Segoe UI", 9);
                searchTargetCheckbox.Text = sTarget;
                searchTargetCheckbox.Checked = false;
                searchTargetCheckbox.Enabled = false;
                searchTargetCheckbox.AutoSize = true;
                searchTargetCheckbox.Location = new Point((int)(30 * ScaleFactor), (int)(104 * ScaleFactor));

                searchStartInCheckbox = new CustomCheckBox();
                searchStartInCheckbox.Font = new Font("Segoe UI", 9);
                searchStartInCheckbox.Text = sStartIn;
                searchStartInCheckbox.Checked = false;
                searchStartInCheckbox.Enabled = false;
                searchStartInCheckbox.AutoSize = true;
                searchStartInCheckbox.Location = new Point((int)(120 * ScaleFactor), (int)(104 * ScaleFactor));

                searchIconCheckbox = new CustomCheckBox();
                searchIconCheckbox.Font = new Font("Segoe UI", 9);
                searchIconCheckbox.Text = sIcon;
                searchIconCheckbox.Checked = false;
                searchIconCheckbox.Enabled = false;
                searchIconCheckbox.AutoSize = true;
                searchIconCheckbox.Location = new Point((int)(210 * ScaleFactor), (int)(104 * ScaleFactor));

                searchReplaceGroupBox.Controls.Add(searchLabel);
                searchReplaceGroupBox.Controls.Add(searchTextBox);
                searchReplaceGroupBox.Controls.Add(replaceLabel);
                searchReplaceGroupBox.Controls.Add(replaceTextBox);
                searchReplaceGroupBox.Controls.Add(searchFieldsLabel);
                searchReplaceGroupBox.Controls.Add(searchTargetCheckbox);
                searchReplaceGroupBox.Controls.Add(searchStartInCheckbox);
                searchReplaceGroupBox.Controls.Add(searchIconCheckbox);

                // Apply to Subfolders checkbox (positioned before OK button)
                applyToSubfoldersCheckbox = new CustomCheckBox();
                applyToSubfoldersCheckbox.Font = new Font("Segoe UI", 9);
                applyToSubfoldersCheckbox.Text = sAlsoApplyToSubfolders;
                applyToSubfoldersCheckbox.Checked = false;
                applyToSubfoldersCheckbox.AutoSize = true;

                // OK button
                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                buttonOK.Click += ButtonOK_Click;

                // Position checkbox just above OK button
                applyToSubfoldersCheckbox.Location = new Point(
                    (ClientSize.Width - applyToSubfoldersCheckbox.PreferredSize.Width) / 2,
                    buttonOK.Top - applyToSubfoldersCheckbox.PreferredSize.Height - (int)(7 * ScaleFactor)
                );

                // Dark mode support
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;

                    folderPathLabel.BackColor = Color.FromArgb(45, 45, 45);
                    folderPathLabel.ForeColor = Color.White;
                    folderPathScrollBar.Theme = UITheme.VS2019DarkBlue;

                    // CustomGroupBox border and title colors
                    convertGroupBox.BorderColor = Color.FromArgb(100, 100, 100);
                    convertGroupBox.TitleColor = Color.FromArgb(100, 100, 100);
                    searchReplaceGroupBox.BorderColor = Color.FromArgb(100, 100, 100);
                    searchReplaceGroupBox.TitleColor = Color.FromArgb(100, 100, 100);

                    searchTextBox.BackColor = Color.FromArgb(45, 45, 45);
                    searchTextBox.ForeColor = Color.White;
                    searchTextBox.BorderStyle = BorderStyle.FixedSingle;
                    replaceTextBox.BackColor = Color.FromArgb(45, 45, 45);
                    replaceTextBox.ForeColor = Color.White;
                    replaceTextBox.BorderStyle = BorderStyle.FixedSingle;
                }

                Controls.Add(messageLabel);
                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(folderPathLabel);
                Controls.Add(folderPathScrollBar);
                Controls.Add(convertGroupBox);
                Controls.Add(searchReplaceGroupBox);
                Controls.Add(applyToSubfoldersCheckbox);
                Controls.Add(buttonOK);

                folderPathLabel.Text = StartDirectory;
                folderPathScrollBar.Value = 0;

                // Position dialog at cursor
                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    if (newFolder.Length > 260)
                    {newFolder = GetShortPath(newFolder);
                    }
                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);

                    folderPathLabel.Text = StartDirectory;
                    folderPathScrollBar.Value = 0;

                    // Update scrollbar range
                    using (Graphics g = CreateGraphics())
                    {
                        SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                        int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                        folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                        folderPathScrollBar.Visible = maxScroll > 0;
                    }
                }
            }

            private void ButtonOK_Click(object sender, EventArgs e)
            {
                DialogResult = DialogResult.OK;
                Close();
            }

            public static ShortcutToolDialog Show(string message, string caption)
            {
                var shortcutToolDialog = new ShortcutToolDialog(message, caption);
                shortcutToolDialog.ShowDialog();
                return shortcutToolDialog;
            }
        }

        public class DateTimeToolDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Label buttonFolderPicker;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;
            private Image folderImageNormal;
            private Image folderImageHover;
            private Label folderPathLabel;
            private FlatScrollBar folderPathScrollBar;
            private CustomRadioButton radioSetDateModified;
            private CustomRadioButton radioSetDateCreated;
            private CustomRadioButton radioCopyModifiedToCreated;
            private CustomRadioButton radioCopyCreatedToModified;
            private CustomRadioButton radioCopyTakenToCreated;
            private CustomRadioButton radioCopyTakenToCreatedAndModified;
            private CustomCheckBox onlyIfOlderCheckbox;
            private CustomCheckBox applyToSubfoldersCheckbox;
            private DateTimePicker datePicker;
            private TimeSpinnerControl timeSpinner;
            private Panel dateTimePanel;

            public enum DateTimeAction { SetDateModified, SetDateCreated, CopyModifiedToCreated, CopyCreatedToModified, CopyTakenToCreated, CopyTakenToCreatedAndModified }

            public DateTimeAction SelectedAction
            {
                get
                {
                    if (radioSetDateModified.Checked) return DateTimeAction.SetDateModified;
                    if (radioSetDateCreated.Checked) return DateTimeAction.SetDateCreated;
                    if (radioCopyModifiedToCreated.Checked) return DateTimeAction.CopyModifiedToCreated;
                    if (radioCopyCreatedToModified.Checked) return DateTimeAction.CopyCreatedToModified;
                    if (radioCopyTakenToCreated.Checked) return DateTimeAction.CopyTakenToCreated;
                    return DateTimeAction.CopyTakenToCreatedAndModified;
                }
            }

            public DateTime SelectedDateTime
            {
                get
                {
                    DateTime d = datePicker.Value.Date;
                    TimeSpan t = timeSpinner.Time;
                    return new DateTime(d.Year, d.Month, d.Day, t.Hours, t.Minutes, t.Seconds);
                }
            }
            public bool OnlyIfDateModifiedIsOlder => onlyIfOlderCheckbox.Checked;
            public bool ApplyToSubfolders => applyToSubfoldersCheckbox.Checked;

            // Keep for backward compat
            public bool CopyDateTakenToDateCreated => radioCopyTakenToCreated.Checked;

            public DateTimeToolDialog(string message, string caption)
            {
                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(490 * ScaleFactor);
                Height = (int)(464 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                // Help button (top right)
                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                // Folder picker button (top left)
                buttonFolderPicker = new Label();
                Image folderImage = Image.FromFile($@"{appParts}\Icons\Folder.png");
                Bitmap scaledFolderImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledFolderImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(folderImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                folderImageNormal = scaledFolderImage;
                folderImageHover = CreateTransparentImage(scaledFolderImage, 0.5f);
                buttonFolderPicker.BackgroundImage = folderImageNormal;
                buttonFolderPicker.BackgroundImageLayout = ImageLayout.Stretch;
                buttonFolderPicker.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonFolderPicker.FlatStyle = FlatStyle.Flat;
                buttonFolderPicker.Left = (int)(4 * ScaleFactor);
                buttonFolderPicker.Top = (int)(4 * ScaleFactor);
                buttonFolderPicker.Click += ButtonFolderPicker_Click;
                buttonFolderPicker.MouseEnter += (s, e) => buttonFolderPicker.BackgroundImage = folderImageHover;
                buttonFolderPicker.MouseLeave += (s, e) => buttonFolderPicker.BackgroundImage = folderImageNormal;

                // Message label (centered between folder and help icons)
                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.MiddleCenter;
                messageLabel.AutoSize = false;
                messageLabel.Location = new Point((int)(35 * ScaleFactor), (int)(5 * ScaleFactor));
                messageLabel.Width = ClientSize.Width - (int)(70 * ScaleFactor);
                messageLabel.Height = (int)(20 * ScaleFactor);

                // Folder path display
                int controlHeight = (int)(24 * ScaleFactor);
                int xMargin = (int)(10 * ScaleFactor);

                folderPathLabel = new Label();
                folderPathLabel.Font = new Font("Segoe UI", 9);
                folderPathLabel.Location = new Point(xMargin, (int)(40 * ScaleFactor));
                folderPathLabel.Width = ClientSize.Width - (xMargin * 2);
                folderPathLabel.Height = controlHeight;
                folderPathLabel.AutoSize = false;
                folderPathLabel.BorderStyle = BorderStyle.None;
                folderPathLabel.Padding = new Padding(2, 2, 2, 2);
                folderPathLabel.BackColor = SystemColors.Control;

                folderPathLabel.Paint += (s, pe) =>
                {
                    Label lbl = s as Label;
                    if (lbl != null)
                    {
                        pe.Graphics.Clear(lbl.BackColor);
                        if (!string.IsNullOrEmpty(lbl.Text))
                        {
                            Rectangle textRect = new Rectangle(
                                lbl.Padding.Left, 0,
                                lbl.Width - lbl.Padding.Left - lbl.Padding.Right,
                                lbl.Height);
                            TextRenderer.DrawText(pe.Graphics, lbl.Text, lbl.Font, textRect, lbl.ForeColor,
                                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                        }
                        Color borderColor = Dark ? Color.FromArgb(100, 100, 100) : Color.FromArgb(171, 173, 179);
                        using (Pen borderPen = new Pen(borderColor, 1))
                        {
                            pe.Graphics.DrawRectangle(borderPen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                        }
                    }
                };

                folderPathScrollBar = new FlatScrollBar();
                folderPathScrollBar.Orientation = ScrollBarOrientation.Horizontal;
                folderPathScrollBar.Location = new Point(xMargin, (int)(40 * ScaleFactor) + controlHeight - 1);
                folderPathScrollBar.Width = ClientSize.Width - (xMargin * 2);
                folderPathScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
                folderPathScrollBar.Minimum = 0;
                folderPathScrollBar.SmallChange = 5;
                folderPathScrollBar.LargeChange = 20;
                folderPathScrollBar.Scroll += (s, ev) =>
                {
                    int offset = folderPathScrollBar.Value;
                    folderPathLabel.Text = StartDirectory.Length > offset ? StartDirectory.Substring(offset) : "";
                };

                using (Graphics g = CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                    int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                    folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                    folderPathScrollBar.Visible = maxScroll > 0;
                }

                // Radio buttons and sub-controls
                int radioX = (int)(20 * ScaleFactor);
                int radioTop = (int)(80 * ScaleFactor);
                int radioSpacing = (int)(34 * ScaleFactor);
                Font radioFont = new Font("Segoe UI", 9);

                radioSetDateModified = new CustomRadioButton();
                radioSetDateModified.Text = sSetDateModified;
                radioSetDateModified.Font = radioFont;
                radioSetDateModified.AutoSize = true;
                radioSetDateModified.Location = new Point(radioX, radioTop);
                radioSetDateModified.CheckedChanged += RadioOption_CheckedChanged;

                radioSetDateCreated = new CustomRadioButton();
                radioSetDateCreated.Text = sSetDateCreated;
                radioSetDateCreated.Font = radioFont;
                radioSetDateCreated.AutoSize = true;
                radioSetDateCreated.Location = new Point(radioX, radioTop + radioSpacing);
                radioSetDateCreated.CheckedChanged += RadioOption_CheckedChanged;

                radioCopyTakenToCreated = new CustomRadioButton();
                radioCopyTakenToCreated.Text = sCopyDateTakenToDateCreated;
                radioCopyTakenToCreated.Font = radioFont;
                radioCopyTakenToCreated.AutoSize = true;
                radioCopyTakenToCreated.Location = new Point(radioX, radioTop + radioSpacing * 2);
                radioCopyTakenToCreated.CheckedChanged += RadioOption_CheckedChanged;

                radioCopyTakenToCreatedAndModified = new CustomRadioButton();
                radioCopyTakenToCreatedAndModified.Text = sCopyDateTakenToDateCreatedAndModified;
                radioCopyTakenToCreatedAndModified.Font = radioFont;
                radioCopyTakenToCreatedAndModified.AutoSize = true;
                radioCopyTakenToCreatedAndModified.Location = new Point(radioX, radioTop + radioSpacing * 3);
                radioCopyTakenToCreatedAndModified.CheckedChanged += RadioOption_CheckedChanged;

                radioCopyCreatedToModified = new CustomRadioButton();
                radioCopyCreatedToModified.Text = sCopyDateCreatedToDateModified;
                radioCopyCreatedToModified.Font = radioFont;
                radioCopyCreatedToModified.AutoSize = true;
                radioCopyCreatedToModified.Location = new Point(radioX, radioTop + radioSpacing * 4);
                radioCopyCreatedToModified.CheckedChanged += RadioOption_CheckedChanged;

                radioCopyModifiedToCreated = new CustomRadioButton();
                radioCopyModifiedToCreated.Text = sCopyDateModifiedToDateCreated;
                radioCopyModifiedToCreated.Font = radioFont;
                radioCopyModifiedToCreated.AutoSize = true;
                radioCopyModifiedToCreated.Location = new Point(radioX, radioTop + radioSpacing * 5);
                radioCopyModifiedToCreated.CheckedChanged += RadioOption_CheckedChanged;

                // Sub-option: Only if Date modified is older (indented under radioCopyModifiedToCreated)
                onlyIfOlderCheckbox = new CustomCheckBox();
                onlyIfOlderCheckbox.Text = sOnlyIfDateModifiedIsOlder;
                onlyIfOlderCheckbox.Font = radioFont;
                onlyIfOlderCheckbox.AutoSize = true;
                onlyIfOlderCheckbox.Checked = true;
                onlyIfOlderCheckbox.Enabled = false;
                onlyIfOlderCheckbox.Location = new Point(radioX + (int)(20 * ScaleFactor), radioTop + radioSpacing * 6);

                // Date + Time panel (shown only for first two radio options)
                dateTimePanel = new Panel();
                dateTimePanel.Location = new Point(xMargin, radioTop + radioSpacing * 7 + (int)(4 * ScaleFactor));
                dateTimePanel.Width = ClientSize.Width - (xMargin * 2);
                dateTimePanel.Height = (int)(32 * ScaleFactor);  // tall enough for date/time controls
                dateTimePanel.Visible = false;

                datePicker = new DateTimePicker();
                datePicker.Format = DateTimePickerFormat.Short;
                datePicker.Font = radioFont;
                datePicker.Value = DateTime.Now;
                datePicker.Width = (int)(130 * ScaleFactor);

                timeSpinner = new TimeSpinnerControl();
                timeSpinner.Font = radioFont;
                // Match the date picker's natural height so both controls align
                timeSpinner.Height = datePicker.Height;
                timeSpinner.Width = (int)(148 * ScaleFactor);

                int spinLeft = datePicker.Width + (int)(8 * ScaleFactor);
                int panelH = dateTimePanel.Height;
                datePicker.Location = new Point(0, (panelH - datePicker.Height) / 2);
                timeSpinner.Location = new Point(spinLeft, (panelH - timeSpinner.Height) / 2);

                dateTimePanel.Controls.Add(datePicker);
                dateTimePanel.Controls.Add(timeSpinner);

                // OK button
                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.Font = radioFont;
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                buttonOK.Click += ButtonOK_Click;

                // Apply to Subfolders checkbox (above OK button)
                applyToSubfoldersCheckbox = new CustomCheckBox();
                applyToSubfoldersCheckbox.Font = radioFont;
                applyToSubfoldersCheckbox.Text = sAlsoApplyToSubfolders;
                applyToSubfoldersCheckbox.Checked = false;
                applyToSubfoldersCheckbox.AutoSize = true;
                applyToSubfoldersCheckbox.Location = new Point(
                    (ClientSize.Width - applyToSubfoldersCheckbox.PreferredSize.Width) / 2,
                    buttonOK.Top - applyToSubfoldersCheckbox.PreferredSize.Height - (int)(7 * ScaleFactor));

                // Select first radio by default
                radioSetDateModified.Checked = true;

                // Dark mode
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                    folderPathLabel.BackColor = Color.FromArgb(45, 45, 45);
                    folderPathLabel.ForeColor = Color.White;
                    folderPathScrollBar.Theme = UITheme.VS2019DarkBlue;
                    foreach (CustomRadioButton rb in new[] { radioSetDateModified, radioSetDateCreated, radioCopyModifiedToCreated, radioCopyCreatedToModified, radioCopyTakenToCreated, radioCopyTakenToCreatedAndModified })
                    {
                        rb.ForeColor = Color.White;
                        rb.BackColor = Color.FromArgb(43, 43, 43);
                    }
                    dateTimePanel.BackColor = Color.FromArgb(43, 43, 43);
                }

                Controls.Add(messageLabel);
                Controls.Add(buttonFolderPicker);
                Controls.Add(buttonHelp);
                Controls.Add(folderPathLabel);
                Controls.Add(folderPathScrollBar);
                Controls.Add(radioSetDateModified);
                Controls.Add(radioSetDateCreated);
                Controls.Add(radioCopyTakenToCreated);
                Controls.Add(radioCopyTakenToCreatedAndModified);
                Controls.Add(radioCopyCreatedToModified);
                Controls.Add(radioCopyModifiedToCreated);
                Controls.Add(onlyIfOlderCheckbox);
                Controls.Add(dateTimePanel);
                Controls.Add(applyToSubfoldersCheckbox);
                Controls.Add(buttonOK);

                folderPathLabel.Text = StartDirectory;
                folderPathScrollBar.Value = 0;

                // Position dialog at cursor
                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void RadioOption_CheckedChanged(object sender, EventArgs e)
            {
                bool specificDate = radioSetDateModified.Checked || radioSetDateCreated.Checked;
                dateTimePanel.Visible = specificDate;
                onlyIfOlderCheckbox.Enabled = radioCopyModifiedToCreated.Checked;
                if (radioCopyModifiedToCreated.Checked)
                    onlyIfOlderCheckbox.Checked = true;
                else
                    onlyIfOlderCheckbox.Checked = false;
            }

            private void ButtonFolderPicker_Click(object sender, EventArgs e)
            {
                string newFolder = SelectFolder(StartDirectory);
                if (newFolder != StartDirectory && !string.IsNullOrEmpty(newFolder))
                {
                    if (newFolder.Length > 260)
                        newFolder = GetShortPath(newFolder);
                    StartDirectory = newFolder;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\RightClickTools", "StartDirectory", newFolder, RegistryValueKind.String);

                    folderPathLabel.Text = StartDirectory;
                    folderPathScrollBar.Value = 0;

                    using (Graphics g = CreateGraphics())
                    {
                        SizeF textSize = g.MeasureString(StartDirectory, folderPathLabel.Font);
                        int maxScroll = Math.Max(0, (int)textSize.Width - (folderPathLabel.Width - 4));
                        folderPathScrollBar.Maximum = maxScroll > 0 ? (int)(StartDirectory.Length * 0.9) : 0;
                        folderPathScrollBar.Visible = maxScroll > 0;
                    }
                }
            }

            private void ButtonOK_Click(object sender, EventArgs e)
            {
                string warning = applyToSubfoldersCheckbox.Checked ? sWarnChangeDatesSubfolders : sWarnChangeDates;
                if (CustomMessageBox.Show(warning, sMain) != DialogResult.OK)
                    return;
                DialogResult = DialogResult.OK;
                Close();
            }

            public static DateTimeToolDialog Show(string message, string caption)
            {
                var dialog = new DateTimeToolDialog(message, caption);
                dialog.ShowDialog();
                return dialog;
            }
        }

        // Dialog for Property Selector
        public class PropertySelectorDialog : Form
        {
            private class PropertyItem
            {
                public string DisplayName { get; set; }
                public string CanonicalName { get; set; }
                public bool IsChecked { get; set; }
            }

            private System.Collections.Generic.List<PropertyItem> properties = new System.Collections.Generic.List<PropertyItem>();
            private Panel propertyPanel;
            private FlatScrollBar propertyScrollBar;
            private int scrollOffset = 0;
            private int hoveredIndex = -1;
            private int itemHeight;
            private TextBox targetTextBox;
            private string typeToFindBuffer = "";
            private System.Windows.Forms.Timer typeToFindTimer;

            public PropertySelectorDialog(TextBox targetTextBox)
            {
                this.targetTextBox = targetTextBox;
                itemHeight = (int)(22 * ScaleFactor);

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.Sizable;
                Text = sSelectProperties;
                Width = (int)(325 * ScaleFactor);
                Height = (int)(500 * ScaleFactor);
                MinimumSize = new Size((int)(275 * ScaleFactor), (int)(350 * ScaleFactor));
                MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                MaximizeBox = false;
                MinimizeBox = false;
                KeyPreview = true;

                // Initialize type-to-find timer
                typeToFindTimer = new System.Windows.Forms.Timer();
                typeToFindTimer.Interval = 1000; // Reset after 1 second of no typing
                typeToFindTimer.Tick += (s, ev) =>
                {
                    typeToFindBuffer = "";
                    typeToFindTimer.Stop();
                };

                // Load properties
                LoadProperties();

                // Property panel
                propertyPanel = new Panel();
                propertyPanel.Location = new Point((int)(10 * ScaleFactor), (int)(10 * ScaleFactor));
                propertyPanel.Width = ClientSize.Width - (int)(40 * ScaleFactor);
                propertyPanel.Height = ClientSize.Height - (int)(60 * ScaleFactor);
                propertyPanel.BorderStyle = BorderStyle.FixedSingle;
                propertyPanel.BackColor = SystemColors.Window;
                propertyPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                // Enable double buffering to prevent flicker
                propertyPanel.GetType().GetProperty("DoubleBuffered", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .SetValue(propertyPanel, true, null);

                // Scrollbar
                propertyScrollBar = new FlatScrollBar();
                propertyScrollBar.Orientation = ScrollBarOrientation.Vertical;
                propertyScrollBar.Location = new Point(propertyPanel.Right, propertyPanel.Top);
                propertyScrollBar.Width = (int)(18 * ScaleFactor);
                propertyScrollBar.Height = propertyPanel.Height;
                propertyScrollBar.Minimum = 0;
                propertyScrollBar.SmallChange = itemHeight;
                propertyScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
                int totalHeight = properties.Count * itemHeight;
                propertyScrollBar.Maximum = Math.Max(0, totalHeight - propertyPanel.Height);
                // Set LargeChange to the visible area to make thumb size proportional
                propertyScrollBar.LargeChange = Math.Max(itemHeight, propertyPanel.Height);
                propertyScrollBar.Scroll += (s, ev) =>
                {
                    scrollOffset = propertyScrollBar.Value;
                    propertyPanel.Invalidate();
                };

                // Custom paint for property list
                propertyPanel.Paint += (s, pe) =>
                {
                    Panel panel = s as Panel;
                    if (panel != null)
                    {
                        pe.Graphics.Clear(panel.BackColor);

                        int checkBoxSize = (int)(16 * ScaleFactor);
                        int checkBoxMargin = (int)(5 * ScaleFactor);

                        for (int i = 0; i < properties.Count; i++)
                        {
                            int yLine = i * itemHeight - scrollOffset;

                            // Only draw lines that are visible
                            if (yLine + itemHeight >= 0 && yLine < panel.Height)
                            {
                                Rectangle lineRect = new Rectangle(0, yLine, panel.Width - 2, itemHeight);

                                // Highlight on hover
                                if (i == hoveredIndex)
                                {
                                    Color hoverColor = Dark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(229, 241, 251);
                                    pe.Graphics.FillRectangle(new SolidBrush(hoverColor), lineRect);
                                }

                                // Draw checkbox
                                int checkBoxY = yLine + (itemHeight - checkBoxSize) / 2;
                                Rectangle checkBoxRect = new Rectangle(checkBoxMargin, checkBoxY, checkBoxSize, checkBoxSize);

                                // Draw checkbox background and border with rounded corners
                                Color fillColor = properties[i].IsChecked
                                    ? (Dark ? Color.FromArgb(85, 196, 255) : Color.FromArgb(0, 103, 192))
                                    : (Dark ? Color.FromArgb(43, 43, 43) : Color.FromArgb(240, 240, 240));
                                Color borderColor = Dark ? Color.FromArgb(120, 120, 120) : Color.FromArgb(140, 140, 140);

                                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                int cornerRadius = (int)(4 * ScaleFactor);

                                using (GraphicsPath checkBoxPath = GetRoundedRectanglePath(checkBoxRect, cornerRadius))
                                {
                                    using (SolidBrush fillBrush = new SolidBrush(fillColor))
                                    {
                                        pe.Graphics.FillPath(fillBrush, checkBoxPath);
                                    }
                                    using (Pen borderPen = new Pen(borderColor, 1.6f))
                                    {
                                        pe.Graphics.DrawPath(borderPen, checkBoxPath);
                                    }
                                }

                                // Draw checkmark if checked
                                if (properties[i].IsChecked)
                                {
                                    using (Pen checkPen = new Pen(Dark ? Color.Black : Color.White, 2))
                                    {
                                        Point[] checkPoints = new Point[]
                                        {
                                            new Point(checkBoxRect.X + (int)(3 * ScaleFactor), checkBoxRect.Y + (int)(7 * ScaleFactor)),
                                            new Point(checkBoxRect.X + (int)(6 * ScaleFactor), checkBoxRect.Y + (int)(11 * ScaleFactor)),
                                            new Point(checkBoxRect.X + (int)(13 * ScaleFactor), checkBoxRect.Y + (int)(4 * ScaleFactor))
                                        };
                                        pe.Graphics.DrawLines(checkPen, checkPoints);
                                    }
                                }

                                // Draw text
                                Rectangle textRect = new Rectangle(
                                    checkBoxSize + checkBoxMargin * 2 + (int)(5 * ScaleFactor),
                                    yLine,
                                    panel.Width - checkBoxSize - checkBoxMargin * 2 - (int)(10 * ScaleFactor),
                                    itemHeight);
                                TextRenderer.DrawText(pe.Graphics, properties[i].DisplayName, panel.Font, textRect,
                                    panel.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                            }
                        }
                    }
                };

                // Mouse wheel for scrolling
                propertyPanel.MouseWheel += (s, ev) =>
                {
                    int totalContentHeight = properties.Count * itemHeight;
                    int maxScroll = Math.Max(0, totalContentHeight - propertyPanel.Height);

                    scrollOffset -= ev.Delta / 3;
                    scrollOffset = Math.Max(0, Math.Min(maxScroll, scrollOffset));
                    propertyScrollBar.Value = scrollOffset;

                    propertyPanel.Invalidate();
                };

                // Mouse move for hover effect
                propertyPanel.MouseMove += (s, ev) =>
                {
                    int index = (ev.Y + scrollOffset) / itemHeight;

                    if (index >= 0 && index < properties.Count)
                    {
                        if (hoveredIndex != index)
                        {
                            hoveredIndex = index;
                            propertyPanel.Invalidate();
                        }
                    }
                    else
                    {
                        if (hoveredIndex != -1)
                        {
                            hoveredIndex = -1;
                            propertyPanel.Invalidate();
                        }
                    }
                };

                // Mouse leave for hover effect
                propertyPanel.MouseLeave += (s, ev) =>
                {
                    hoveredIndex = -1;
                    propertyPanel.Invalidate();
                };

                // Click handler for checkbox toggle
                propertyPanel.MouseClick += (s, ev) =>
                {
                    int index = (ev.Y + scrollOffset) / itemHeight;

                    if (index >= 0 && index < properties.Count)
                    {
                        properties[index].IsChecked = !properties[index].IsChecked;
                        propertyPanel.Invalidate();
                    }
                };

                // Type-to-find functionality
                this.KeyPress += (s, ev) =>
                {
                    char typedChar = ev.KeyChar;

                    // Ignore control characters except backspace
                    if (char.IsControl(typedChar) && typedChar != '\b')
                        return;

                    if (typedChar == '\b' && typeToFindBuffer.Length > 0)
                    {
                        // Backspace - remove last character
                        typeToFindBuffer = typeToFindBuffer.Substring(0, typeToFindBuffer.Length - 1);
                    }
                    else if (!char.IsControl(typedChar))
                    {
                        // Add typed character to buffer
                        typeToFindBuffer += typedChar.ToString();
                    }

                    // Reset timer
                    typeToFindTimer.Stop();
                    typeToFindTimer.Start();

                    // Find first matching item
                    if (!string.IsNullOrEmpty(typeToFindBuffer))
                    {
                        int matchIndex = properties.FindIndex(p => 
                            p.DisplayName.StartsWith(typeToFindBuffer, StringComparison.OrdinalIgnoreCase));

                        if (matchIndex >= 0)
                        {
                            // Scroll to the matched item
                            int targetScrollOffset = matchIndex * itemHeight;
                            int maxScroll = Math.Max(0, properties.Count * itemHeight - propertyPanel.Height);

                            // Center the item if possible
                            targetScrollOffset = Math.Max(0, targetScrollOffset - propertyPanel.Height / 2);
                            scrollOffset = Math.Min(maxScroll, targetScrollOffset);
                            propertyScrollBar.Value = scrollOffset;

                            // Highlight the matched item
                            hoveredIndex = matchIndex;
                            propertyPanel.Invalidate();
                        }
                    }

                    ev.Handled = true;
                };

                // OK button
                Button buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                buttonOK.Anchor = AnchorStyles.Bottom;
                buttonOK.Click += (s, ev) =>
                {
                    // Append selected properties to target textbox
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (!string.IsNullOrWhiteSpace(targetTextBox.Text))
                    {
                        sb.Append(targetTextBox.Text);
                        if (!targetTextBox.Text.EndsWith(" "))
                        {
                            sb.Append(" ");
                        }
                    }

                    bool first = string.IsNullOrWhiteSpace(targetTextBox.Text);
                    foreach (var prop in properties)
                    {
                        if (prop.IsChecked)
                        {
                            if (!first)
                            {
                                sb.Append(" ");
                            }
                            // Remove spaces from display name for AQS compatibility
                            string propertyName = prop.DisplayName.Replace(" ", "");
                            sb.Append(propertyName);
                            sb.Append(":");
                            first = false;
                        }
                    }

                    targetTextBox.Text = sb.ToString();
                    Close();
                };

                // Recenter buttonOK on resize
                this.Resize += (s, ev) =>
                {
                    buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                };

                // Cleanup timer on form closing
                this.FormClosing += (s, ev) =>
                {
                    if (typeToFindTimer != null)
                    {
                        typeToFindTimer.Stop();
                        typeToFindTimer.Dispose();
                    }
                };

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    propertyScrollBar.Theme = UITheme.VS2019DarkBlue;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                    propertyPanel.BackColor = Color.FromArgb(45, 45, 45);
                    propertyPanel.ForeColor = Color.White;
                }

                Controls.Add(propertyPanel);
                Controls.Add(propertyScrollBar);
                Controls.Add(buttonOK);
            }

            private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
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

            private void LoadProperties()
            {
                try
                {
                    PSEnumeratePropertyDescriptions(PROPDESC_ENUMFILTER.PDEF_ALL, typeof(IPropertyDescriptionList).GUID, out var list);
                    for (var i = 0; i < list.GetCount(); i++)
                    {
                        var pd = list.GetAt(i, typeof(IPropertyDescription).GUID);

                        pd.GetDisplayName(out var p);
                        if (p != IntPtr.Zero)
                        {
                            var viewable = pd.GetTypeFlags(PROPDESC_TYPE_FLAGS.PDTF_ISVIEWABLE) == PROPDESC_TYPE_FLAGS.PDTF_ISVIEWABLE;

                            if (viewable)
                            {
                                string dname = Marshal.PtrToStringUni(p);
                                Marshal.FreeCoTaskMem(p);

                                pd.GetCanonicalName(out p);
                                string cname = Marshal.PtrToStringUni(p);
                                Marshal.FreeCoTaskMem(p);

                                properties.Add(new PropertyItem
                                {
                                    DisplayName = dname,
                                    CanonicalName = cname,
                                    IsChecked = false
                                });
                            }
                        }
                    }

                    // Sort properties alphabetically by display name
                    properties.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    // If property enumeration fails, add a message
                    properties.Add(new PropertyItem
                    {
                        DisplayName = "Failed to load properties",
                        CanonicalName = "",
                        IsChecked = false
                    });
                }
            }

            public static void Show(TextBox targetTextBox)
            {
                using (var dialog = new PropertySelectorDialog(targetTextBox))
                {
                    dialog.ShowDialog();
                }
            }
        }

        // Dialog for Shell Refresh
        public class ShellRefreshDialog : Form
        {
            private Label messageLabel;
            private Label buttonHelp;
            private Button buttonOK;
            private Image helpImageNormal;
            private Image helpImageHover;

            public ShellRefreshDialog(string message, string caption)
            {
                message = $"\n\n\n\n{message}";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(350 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(100 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;
                helpPage = "refresh-shell";

                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);

                ShellRefreshCheckbox = new CustomCheckBox();
                ShellRefreshCheckbox.Font = new Font("Segoe UI", 10);
                ShellRefreshCheckbox.Text = sShellRefresh;
                ShellRefreshCheckbox.AutoSize = true;
                ShellRefreshCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(16 * ScaleFactor));
                ShellRefreshCheckbox.Checked = true;
                ShellRefreshCheckbox.CheckedChanged += new EventHandler(CB1);

                iconCacheCheckbox = new CustomCheckBox();
                iconCacheCheckbox.Font = new Font("Segoe UI", 10);
                iconCacheCheckbox.Text = sResetIcons;
                iconCacheCheckbox.AutoSize = true;
                iconCacheCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(40 * ScaleFactor));
                iconCacheCheckbox.CheckedChanged += new EventHandler(CB2);

                thumbCacheCheckbox = new CustomCheckBox();
                thumbCacheCheckbox.Font = new Font("Segoe UI", 10);
                thumbCacheCheckbox.Text = sResetThumbs;
                thumbCacheCheckbox.AutoSize = true;
                thumbCacheCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(64 * ScaleFactor));
                thumbCacheCheckbox.CheckedChanged += new EventHandler(CB2);

                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                Controls.Add(buttonHelp);
                Controls.Add(ShellRefreshCheckbox);
                Controls.Add(iconCacheCheckbox);
                Controls.Add(thumbCacheCheckbox);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            private void CB1(object sender, EventArgs e)
            {
                if (ShellRefreshCheckbox.Checked)
                {
                    iconCacheCheckbox.Checked = false;
                    thumbCacheCheckbox.Checked = false;
                }
                if (!iconCacheCheckbox.Checked && !thumbCacheCheckbox.Checked)
                {
                    ShellRefreshCheckbox.Checked = true;
                }

            }
            private void CB2(object sender, EventArgs e)
            {
                if (iconCacheCheckbox.Checked || thumbCacheCheckbox.Checked)
                {
                    ShellRefreshCheckbox.Checked = false;
                }
                else
                {
                    ShellRefreshCheckbox.Checked = true;
                }
            }


            public static DialogResult Show(string message, string caption)
            {
                using (var ShellRefreshDialog = new ShellRefreshDialog(message, caption))
                {
                    return ShellRefreshDialog.ShowDialog();
                }
            }
        }

        // Dialog for Clear History
        public class ClearHistoryDialog : Form
        {
            private Label messageLabel;
            private Button buttonOK;
            private Label buttonHelp;
            private Image helpImageNormal;
            private Image helpImageHover;

            public ClearHistoryDialog(string message, string caption)
            {
                message = $"\n\n\n\n\n\n\n\n\n\n{message}?";

                Icon = new Icon(myIcon);
                StartPosition = FormStartPosition.Manual;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = caption;
                Width = (int)(350 * ScaleFactor);
                Height = (int)(150 * ScaleFactor);
                MaximizeBox = false;
                MinimizeBox = false;

                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Font = new Font("Segoe UI", 10);
                messageLabel.TextAlign = ContentAlignment.TopCenter;
                messageLabel.Dock = DockStyle.Fill;

                using (Graphics g = CreateGraphics())
                {
                    SizeF size = g.MeasureString(message, new Font("Segoe UI", 10), Width);
                    Height = Math.Max(Height, (int)(size.Height * 1.1 + (int)(80 * ScaleFactor)));
                }

                buttonHelp = new Label();
                Image image = Image.FromFile($@"{appParts}\Icons\Question.png");
                Bitmap scaledImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                }
                helpImageNormal = scaledImage;
                helpImageHover = CreateTransparentImage(scaledImage, 0.5f);
                buttonHelp.BackgroundImage = helpImageNormal;
                buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                buttonHelp.FlatStyle = FlatStyle.Flat;
                buttonHelp.Left = ClientSize.Width - (int)(30 * ScaleFactor);
                buttonHelp.Top = (int)(4 * ScaleFactor);
                buttonHelp.Click += ButtonHelp_Click;
                buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;
                helpPage = "clear-history";
                
                buttonOK = new Button();
                buttonOK.Text = sOK;
                buttonOK.DialogResult = DialogResult.OK;
                buttonOK.Font = new Font("Segoe UI", 9);
                buttonOK.Width = (int)(75 * ScaleFactor);
                buttonOK.Height = (int)(26 * ScaleFactor);
                buttonOK.Left = (ClientSize.Width - buttonOK.Width) / 2;
                buttonOK.Top = ClientSize.Height - buttonOK.Height - (int)(10 * ScaleFactor);
                if (Dark)
                {
                    buttonOK.FlatStyle = FlatStyle.Flat;
                    buttonOK.FlatAppearance.BorderColor = SystemColors.Highlight;
                    buttonOK.FlatAppearance.BorderSize = 1;
                    buttonOK.BackColor = Color.FromArgb(60, 60, 60);
                    buttonOK.FlatAppearance.MouseOverBackColor = Color.Black;
                    DarkTitleBar(Handle);
                    BackColor = Color.FromArgb(43, 43, 43);
                    ForeColor = Color.White;
                }

                RecentItemsCheckbox = new CustomCheckBox();
                RecentItemsCheckbox.Font = new Font("Segoe UI", 10);
                RecentItemsCheckbox.Text = sRecent;
                RecentItemsCheckbox.Checked = false;
                RecentItemsCheckbox.AutoSize = true;
                RecentItemsCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(16 * ScaleFactor));

                AutoSuggestCheckbox = new CustomCheckBox();
                AutoSuggestCheckbox.Font = new Font("Segoe UI", 10);
                AutoSuggestCheckbox.Text = sAutoSuggest;
                AutoSuggestCheckbox.Checked = false;
                AutoSuggestCheckbox.AutoSize = true;
                AutoSuggestCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(40 * ScaleFactor));

                TempFilesCheckbox = new CustomCheckBox();
                TempFilesCheckbox.Font = new Font("Segoe UI", 10);
                TempFilesCheckbox.Text = sTemp;
                TempFilesCheckbox.Checked = false;
                TempFilesCheckbox.AutoSize = true;
                TempFilesCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(64 * ScaleFactor));

                RecycleBinCheckbox = new CustomCheckBox();
                RecycleBinCheckbox.Font = new Font("Segoe UI", 10);
                RecycleBinCheckbox.Text = sRecycleBin;
                RecycleBinCheckbox.Checked = false;
                RecycleBinCheckbox.AutoSize = true;
                RecycleBinCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(88 * ScaleFactor));

                DefenderCheckbox = new CustomCheckBox();
                DefenderCheckbox.Font = new Font("Segoe UI", 10);
                DefenderCheckbox.Text = sDefender;
                DefenderCheckbox.Checked = false;
                DefenderCheckbox.AutoSize = true;
                DefenderCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(112 * ScaleFactor));

                SpecifiedFoldersCheckbox = new CustomCheckBox();
                SpecifiedFoldersCheckbox.Font = new Font("Segoe UI", 10);
                SpecifiedFoldersCheckbox.Text = sSpecifiedFolders;
                SpecifiedFoldersCheckbox.Checked = false;
                SpecifiedFoldersCheckbox.AutoSize = true;
                SpecifiedFoldersCheckbox.Location = new Point((int)(8 * ScaleFactor), (int)(136 * ScaleFactor));
                SpecifiedFoldersCheckbox.CheckedChanged += (s, e) =>
                {
                    if (!SpecifiedFoldersCheckbox.Checked) return;
                    string cleanupFile = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "RightClickTools", "Cleanup.txt");
                    try
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(cleanupFile));
                        if (!System.IO.File.Exists(cleanupFile))
                            System.IO.File.WriteAllText(cleanupFile, "");
                        Process.Start(EditorExe, cleanupFile);
                    }
                    catch { }
                };

                Controls.Add(buttonHelp);
                Controls.Add(RecentItemsCheckbox);
                Controls.Add(AutoSuggestCheckbox);
                Controls.Add(TempFilesCheckbox);
                Controls.Add(RecycleBinCheckbox);
                Controls.Add(DefenderCheckbox);
                Controls.Add(SpecifiedFoldersCheckbox);
                Controls.Add(buttonOK);
                Controls.Add(messageLabel);

                Location = GetDialogPosition(this, -(int)(50 * ScaleFactor));
            }

            public static DialogResult Show(string message, string caption)
            {
                using (var ClearHistoryDialog = new ClearHistoryDialog(message, caption))
                {
                    return ClearHistoryDialog.ShowDialog();
                }
            }
        }
        private static Image CreateTransparentImage(Image original, float opacity)
        {
            Bitmap transparentBitmap = new Bitmap(original.Width, original.Height);
            using (Graphics g = Graphics.FromImage(transparentBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = opacity;
                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            return transparentBitmap;
        }
        // Custom toggle switch control similar to Windows 10 Settings
        public class ToggleSwitch : Control
        {
            private bool _checked = false;
            private float _animationProgress = 0;
            private System.Windows.Forms.Timer _animationTimer;
            private const int AnimationSteps = 10;
            private bool _isHovered = false;

            public bool Checked
            {
                get { return _checked; }
                set
                {
                    if (_checked != value)
                    {
                        _checked = value;
                        StartAnimation();
                        CheckedChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            public event EventHandler CheckedChanged;

            public ToggleSwitch()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
                this.Size = new Size((int)(40 * ScaleFactor), (int)(18 * ScaleFactor));

                _animationTimer = new System.Windows.Forms.Timer();
                _animationTimer.Interval = 15;
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            private void StartAnimation()
            {
                _animationTimer.Start();
            }

            private void AnimationTimer_Tick(object sender, EventArgs e)
            {
                if (_checked)
                {
                    _animationProgress += 1.0f / AnimationSteps;
                    if (_animationProgress >= 1.0f)
                    {
                        _animationProgress = 1.0f;
                        _animationTimer.Stop();
                    }
                }
                else
                {
                    _animationProgress -= 1.0f / AnimationSteps;
                    if (_animationProgress <= 0.0f)
                    {
                        _animationProgress = 0.0f;
                        _animationTimer.Stop();
                    }
                }
                this.Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                bool isDark = Dark;

                // Different hover colors for light and dark mode
                Color hoverColor = isDark ? Color.Black : Color.FromArgb(77, 161, 227);

                // Windows 10 style colors
                Color trackColorOff = _isHovered
                    ? hoverColor
                    : (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(200, 200, 200));

                Color trackColorOn = _isHovered
                    ? hoverColor
                    : Color.FromArgb(0, 120, 215); // Windows 10 blue

                // Different thumb colors for light and dark mode
                Color thumbColor = Color.White;
                Color borderColor = isDark ? Color.FromArgb(80, 80, 80) : Color.FromArgb(180, 180, 180);

                // Interpolate colors based on animation progress
                int r = (int)(trackColorOff.R + (trackColorOn.R - trackColorOff.R) * _animationProgress);
                int g = (int)(trackColorOff.G + (trackColorOn.G - trackColorOff.G) * _animationProgress);
                int b = (int)(trackColorOff.B + (trackColorOn.B - trackColorOff.B) * _animationProgress);
                Color currentTrackColor = Color.FromArgb(r, g, b);

                // Draw track (rounded rectangle) - inset by 1 pixel to account for border
                int trackHeight = this.Height - 2;  // Preserve space for border
                int trackWidth = this.Width - 2;
                Rectangle trackRect = new Rectangle(1, 1, trackWidth, trackHeight);

                using (GraphicsPath trackPath = GetRoundedRectangle(trackRect, trackHeight / 2))
                {
                    using (SolidBrush trackBrush = new SolidBrush(currentTrackColor))
                    {
                        e.Graphics.FillPath(trackBrush, trackPath);
                    }

                    // Draw border
                    using (Pen borderPen = new Pen(borderColor, 1))
                    {
                        e.Graphics.DrawPath(borderPen, trackPath);
                    }
                }

                // Calculate thumb position - smaller thumb for Windows 10 style with equal padding on both sides
                int thumbSize = trackHeight - 8; // Smaller by 4 pixels for more visible background
                int thumbMaxX = trackWidth - thumbSize - 8; // 4 pixels padding on each side
                int thumbX = (int)(4 + thumbMaxX * _animationProgress);
                int thumbY = 5;

                // Draw thumb (circle with shadow)
                Rectangle thumbRect = new Rectangle(thumbX, thumbY, thumbSize, thumbSize);

                // Shadow
                Rectangle shadowRect = new Rectangle(thumbX + 1, thumbY + 1, thumbSize, thumbSize);
                using (GraphicsPath shadowPath = new GraphicsPath())
                {
                    shadowPath.AddEllipse(shadowRect);
                    using (PathGradientBrush shadowBrush =
                           new PathGradientBrush(shadowPath))
                    {
                        shadowBrush.CenterColor = Color.FromArgb(30, 0, 0, 0);
                        shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        e.Graphics.FillEllipse(shadowBrush, shadowRect);
                    }
                }

                // Thumb
                using (SolidBrush thumbBrush = new SolidBrush(thumbColor))
                {
                    e.Graphics.FillEllipse(thumbBrush, thumbRect);
                }
            }

            private GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
            {
                int diameter = radius * 2;
                Size size = new Size(diameter, diameter);
                Rectangle arc = new Rectangle(bounds.Location, size);
                GraphicsPath path = new GraphicsPath();

                if (radius == 0)
                {
                    path.AddRectangle(bounds);
                    return path;
                }

                // Top left arc
                path.AddArc(arc, 180, 90);

                // Top right arc
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);

                // Bottom right arc
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // Bottom left arc
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);

                path.CloseFigure();
                return path;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                Checked = !Checked;
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                _isHovered = true;
                this.Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _isHovered = false;
                this.Invalidate();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _animationTimer?.Stop();
                    _animationTimer?.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        // Settings dialog with quick access to common Windows settings
        class SettingsDialog
        {
            public static void Show()
            {
                helpPage = "settings";

                // Read AutoClose setting from [Settings] section in RightClickTools.ini
                bool autoClose = ReadString(myIniFile, "Settings", "AutoClose", "0") == "1";

                using (var settingsDialog = new Form())
                {
                    settingsDialog.Icon = new Icon(myIcon);
                    settingsDialog.StartPosition = FormStartPosition.Manual;
                    settingsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    settingsDialog.Text = sMain;
                    settingsDialog.Width = (int)(350 * ScaleFactor);
                    settingsDialog.Height = (int)(466 * ScaleFactor);
                    settingsDialog.MaximizeBox = false;
                    settingsDialog.MinimizeBox = false;

                    // Help button
                    Label buttonHelp = new Label();
                    Image helpImage = Image.FromFile($@"{appParts}\Icons\Question.png");
                    Bitmap scaledHelpImage = new Bitmap((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                    using (Graphics g = Graphics.FromImage(scaledHelpImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(helpImage, 0, 0, (int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                    }
                    Image helpImageNormal = scaledHelpImage;
                    Image helpImageHover = Program.CreateTransparentImage(scaledHelpImage, 0.5f);
                    buttonHelp.BackgroundImage = helpImageNormal;
                    buttonHelp.BackgroundImageLayout = ImageLayout.Stretch;
                    buttonHelp.Size = new Size((int)(26 * ScaleFactor), (int)(26 * ScaleFactor));
                    buttonHelp.FlatStyle = FlatStyle.Flat;
                    buttonHelp.Left = settingsDialog.ClientSize.Width - (int)(30 * ScaleFactor);
                    buttonHelp.Top = (int)(4 * ScaleFactor);
                    buttonHelp.Click += ButtonHelp_Click;
                    buttonHelp.MouseEnter += (s, e) => buttonHelp.BackgroundImage = helpImageHover;
                    buttonHelp.MouseLeave += (s, e) => buttonHelp.BackgroundImage = helpImageNormal;

                    // Title label
                    Label titleLabel = new Label();
                    titleLabel.Text = sSettings;
                    titleLabel.Font = new Font("Segoe UI", 10);
                    titleLabel.TextAlign = ContentAlignment.MiddleCenter;
                    titleLabel.AutoSize = false;
                    titleLabel.Location = new Point((int)(35 * ScaleFactor), (int)(5 * ScaleFactor));
                    titleLabel.Width = settingsDialog.ClientSize.Width - (int)(70 * ScaleFactor);
                    titleLabel.Height = (int)(20 * ScaleFactor);

                    int yPos = (int)(40 * ScaleFactor);
                    int buttonWidth = (int)(280 * ScaleFactor);
                    int buttonHeight = (int)(26 * ScaleFactor);
                    int spacing = (int)(8 * ScaleFactor);
                    int xButton = (settingsDialog.ClientSize.Width - buttonWidth) / 2;

                    // Button 1: Right-Click Tools Settings
                    Button btnRCTSettings = new Button();
                    btnRCTSettings.Text = sRCTSettings;
                    btnRCTSettings.Font = new Font("Segoe UI", 9);
                    btnRCTSettings.Width = buttonWidth;
                    btnRCTSettings.Height = buttonHeight;
                    btnRCTSettings.Left = xButton;
                    btnRCTSettings.Top = yPos;
                    btnRCTSettings.Click += (s, e) =>
                    {
                        try
                        {
                            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                            Process.Start("explorer.exe", $"{localAppData}\\RightClickTools");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 2: Windows Settings
                    Button btnWinSettings = new Button();
                    btnWinSettings.Text = sWinSettings;
                    btnWinSettings.Font = new Font("Segoe UI", 9);
                    btnWinSettings.Width = buttonWidth;
                    btnWinSettings.Height = buttonHeight;
                    btnWinSettings.Left = xButton;
                    btnWinSettings.Top = yPos;
                    btnWinSettings.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("ms-settings:");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 3: Apps & features
                    Button btnAppsFeatures = new Button();
                    btnAppsFeatures.Text = sAppsFeatures;
                    btnAppsFeatures.Font = new Font("Segoe UI", 9);
                    btnAppsFeatures.Width = buttonWidth;
                    btnAppsFeatures.Height = buttonHeight;
                    btnAppsFeatures.Left = xButton;
                    btnAppsFeatures.Top = yPos;
                    btnAppsFeatures.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("ms-settings:appsfeatures");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 4: Control Panel
                    Button btnControlPanel = new Button();
                    btnControlPanel.Text = sControlPanel;
                    btnControlPanel.Font = new Font("Segoe UI", 9);
                    btnControlPanel.Width = buttonWidth;
                    btnControlPanel.Height = buttonHeight;
                    btnControlPanel.Left = xButton;
                    btnControlPanel.Top = yPos;
                    btnControlPanel.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("Control.exe");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 5: Programs and Features
                    Button btnProgramsFeatures = new Button();
                    btnProgramsFeatures.Text = sProgramsFeatures;
                    btnProgramsFeatures.Font = new Font("Segoe UI", 9);
                    btnProgramsFeatures.Width = buttonWidth;
                    btnProgramsFeatures.Height = buttonHeight;
                    btnProgramsFeatures.Left = xButton;
                    btnProgramsFeatures.Top = yPos;
                    btnProgramsFeatures.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("explorer", "shell:ChangeRemoveProgramsFolder");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 6: Optional Features
                    Button btnOptFeatures = new Button();
                    btnOptFeatures.Text = sOptFeatures;
                    btnOptFeatures.Font = new Font("Segoe UI", 9);
                    btnOptFeatures.Width = buttonWidth;
                    btnOptFeatures.Height = buttonHeight;
                    btnOptFeatures.Left = xButton;
                    btnOptFeatures.Top = yPos;
                    btnOptFeatures.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("OptionalFeatures.exe");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 7: Classic settings flat list
                    Button btnAllClassic = new Button();
                    btnAllClassic.Text = sClassicSettings;
                    btnAllClassic.Font = new Font("Segoe UI", 9);
                    btnAllClassic.Width = buttonWidth;
                    btnAllClassic.Height = buttonHeight;
                    btnAllClassic.Left = xButton;
                    btnAllClassic.Top = yPos;
                    btnAllClassic.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("explorer.exe", "shell:::{ED7BA470-8E54-465E-825C-99712043E01C}");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 8: System Properties
                    Button btnSysProps = new Button();
                    btnSysProps.Text = sSysProps;
                    btnSysProps.Font = new Font("Segoe UI", 9);
                    btnSysProps.Width = buttonWidth;
                    btnSysProps.Height = buttonHeight;
                    btnSysProps.Left = xButton;
                    btnSysProps.Top = yPos;
                    btnSysProps.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("SystemPropertiesComputerName.exe");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 9: Environment Variables
                    Button btnEnvVars = new Button();
                    btnEnvVars.Text = sEnvVars;
                    btnEnvVars.Font = new Font("Segoe UI", 9);
                    btnEnvVars.Width = buttonWidth;
                    btnEnvVars.Height = buttonHeight;
                    btnEnvVars.Left = xButton;
                    btnEnvVars.Top = yPos;
                    btnEnvVars.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("rundll32.exe", "sysdm.cpl,EditEnvironmentVariables");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Button 10: Performance Options
                    Button btnPerfOptions = new Button();
                    btnPerfOptions.Text = sPerfOptions;
                    btnPerfOptions.Font = new Font("Segoe UI", 9);
                    btnPerfOptions.Width = buttonWidth;
                    btnPerfOptions.Height = buttonHeight;
                    btnPerfOptions.Left = xButton;
                    btnPerfOptions.Top = yPos;
                    btnPerfOptions.Click += (s, e) =>
                    {
                        try
                        {
                            Process.Start("SystemPropertiesPerformance.exe");
                            if (autoClose) settingsDialog.Close();
                        }
                        catch { }
                    };
                    yPos += buttonHeight + spacing;

                    // Item 11: Scale (left half) + Theme/Dark-Light (right half)
                    // Scale feature requires Windows 10+ (build 10240+) for setdpi.exe and DPI APIs
                    // Theme feature works on all Windows versions (changes RightClickTools appearance)
                    bool supportsScale = buildNumber >= 10240;

                    int comboGap   = (int)(4 * ScaleFactor);
                    int halfWidth  = (buttonWidth - comboGap) / 2;
                    int rightWidth = buttonWidth - halfWidth - comboGap;

                    CustomComboBox cboScale = null;
                    if (supportsScale)
                    {
                        cboScale = new CustomComboBox();
                        cboScale.Font = new Font("Segoe UI", 9);
                        cboScale.Width = halfWidth;
                        cboScale.Height = buttonHeight;
                        cboScale.Left = xButton;
                        cboScale.Top = yPos;
                        cboScale.HeaderIndex = 0;

                        // Detect current DPI of the monitor under the cursor and pre-select it
                        int monitorIndex = 1;
                        int currentScalePct = 100;
                        uint rawDpiX = 96;
                        try
                        {
                            System.Drawing.Point curPt = Cursor.Position;
                            Screen[] screens = Screen.AllScreens;
                            Screen curScreen = Screen.FromPoint(curPt);
                            int idx = Array.IndexOf(screens, curScreen);
                            monitorIndex = (idx >= 0) ? idx + 1 : 1;
                            IntPtr hMon = MonitorFromPoint(curPt, 2 /*MONITOR_DEFAULTTONEAREST*/);
                            uint dpiX, dpiY, rawDpiY;
                            // Temporarily switch the thread to per-monitor DPI awareness so
                            // GetDpiForMonitor returns the true per-monitor value instead of
                            // the virtualized system DPI that a system-DPI-aware process sees.
                            IntPtr oldCtx = SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                            try
                            {
                                if (GetDpiForMonitor(hMon, 0 /*MDT_EFFECTIVE_DPI*/, out dpiX, out dpiY) == 0 && dpiX > 0)
                                    currentScalePct = (int)Math.Round(dpiX * 100.0 / 96.0);
                                uint rx;
                                if (GetDpiForMonitor(hMon, 2 /*MDT_RAW_DPI*/, out rx, out rawDpiY) == 0 && rx > 0)
                                    rawDpiX = rx;
                            }
                            finally
                            {
                                SetThreadDpiAwarenessContext(oldCtx);
                            }
                        }
                        catch { }

                        // Build the scale list in 25% increments from 100% up to MaxScale.
                        // Default MaxScale is 300, but can be overridden in [Settings] section of RightClickTools.ini.
                        int maxScale = 300;  // Default value
                        string maxScaleStr = ReadString(myIniFile, "Settings", "MaxScale", "");
                        if (!string.IsNullOrEmpty(maxScaleStr) && int.TryParse(maxScaleStr, out int userMaxScale))
                        {
                            // Enforce minimum of 125, maximum of 500
                            maxScale = Math.Max(125, Math.Min(500, userMaxScale));
                            // Round to nearest multiple of 25
                            maxScale = (int)Math.Round(maxScale / 25.0) * 25;
                        }
                        var scaleList = new System.Collections.Generic.List<int>();
                        for (int v = 100; v <= maxScale; v += 25)
                            scaleList.Add(v);
                        int[] scaleValues = scaleList.ToArray();

                        cboScale.Items.Add(sScale);  // index 0 — header, not selectable
                        foreach (int v in scaleValues)
                            cboScale.Items.Add(v + "%");

                        // Pre-select the closest entry to the current scale
                        int bestIdx = 0;
                        int bestDiff = int.MaxValue;
                        for (int i = 0; i < scaleValues.Length; i++)
                        {
                            int diff = Math.Abs(scaleValues[i] - currentScalePct);
                            if (diff < bestDiff) { bestDiff = diff; bestIdx = i; }
                        }
                        cboScale.SelectedIndex = bestIdx + 1;  // +1 to skip the header at index 0

                        int lastValidIdx = bestIdx + 1;
                        bool suppressScaleChange = false;
                        cboScale.SelectedIndexChanged += (s, e) =>
                        {
                            if (suppressScaleChange) return;
                            if (cboScale.SelectedIndex <= 0)
                            {
                                // Header selected — bounce back silently with no side effects
                                suppressScaleChange = true;
                                cboScale.SelectedIndex = lastValidIdx;
                                suppressScaleChange = false;
                                return;
                            }
                            lastValidIdx = cboScale.SelectedIndex;
                            int pct = scaleValues[cboScale.SelectedIndex - 1];

                            // Determine which monitor to apply the scale change to based on current mouse position
                            int targetMonitorIndex = 1;
                            try
                            {
                                System.Drawing.Point curPt = Cursor.Position;
                                Screen[] screens = Screen.AllScreens;
                                Screen curScreen = Screen.FromPoint(curPt);
                                int idx = Array.IndexOf(screens, curScreen);
                                targetMonitorIndex = (idx >= 0) ? idx + 1 : 1;
                            }
                            catch { }

                            try
                            {
                                string setdpiExe = System.IO.Path.Combine(appParts, "setdpi.exe");
                                Process.Start(setdpiExe, $"{pct} {targetMonitorIndex}");
                            }
                            catch { }
                        };
                    }

                    // Theme combobox: Light / Dark
                    // Works on all Windows versions - switches RightClickTools to dark/light mode
                    var cboTheme = new CustomComboBox();
                    cboTheme.Font = new Font("Segoe UI", 9);
                    // If scale is shown, use right half; otherwise use full width
                    cboTheme.Width = supportsScale ? rightWidth : buttonWidth;
                    cboTheme.Height = buttonHeight;
                    cboTheme.Left = supportsScale ? (xButton + halfWidth + comboGap) : xButton;
                    cboTheme.Top = yPos;

                    cboTheme.Items.Add("Light");
                    cboTheme.Items.Add("Dark");

                    // Pre-select based on current Windows apps color mode (or default to Light on Win7)
                    try
                    {
                        using (var rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                        {
                            int appsLight = (rk != null) ? (int)(rk.GetValue("AppsUseLightTheme", 1)) : 1;
                            cboTheme.SelectedIndex = (appsLight == 0) ? 1 : 0;  // 0=Light, 1=Dark
                        }
                    }
                    catch { cboTheme.SelectedIndex = 0; }

                    cboTheme.SelectedIndexChanged += (s, e) =>
                    {
                        bool chooseDark = (cboTheme.SelectedIndex == 1);
                        int val = chooseDark ? 0 : 1;
                        try
                        {
                            using (var rk = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                            {
                                rk.SetValue("AppsUseLightTheme",   val, Microsoft.Win32.RegistryValueKind.DWord);
                                rk.SetValue("SystemUsesLightTheme", val, Microsoft.Win32.RegistryValueKind.DWord);
                            }
                            BroadcastThemeChange();
                        }
                        catch { }
                    };

                    if (Dark)
                    {
                        btnRCTSettings.FlatStyle = FlatStyle.Flat;
                        btnRCTSettings.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnRCTSettings.FlatAppearance.BorderSize = 1;
                        btnRCTSettings.BackColor = Color.FromArgb(60, 60, 60);
                        btnRCTSettings.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnWinSettings.FlatStyle = FlatStyle.Flat;
                        btnWinSettings.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnWinSettings.FlatAppearance.BorderSize = 1;
                        btnWinSettings.BackColor = Color.FromArgb(60, 60, 60);
                        btnWinSettings.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnAppsFeatures.FlatStyle = FlatStyle.Flat;
                        btnAppsFeatures.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnAppsFeatures.FlatAppearance.BorderSize = 1;
                        btnAppsFeatures.BackColor = Color.FromArgb(60, 60, 60);
                        btnAppsFeatures.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnControlPanel.FlatStyle = FlatStyle.Flat;
                        btnControlPanel.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnControlPanel.FlatAppearance.BorderSize = 1;
                        btnControlPanel.BackColor = Color.FromArgb(60, 60, 60);
                        btnControlPanel.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnProgramsFeatures.FlatStyle = FlatStyle.Flat;
                        btnProgramsFeatures.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnProgramsFeatures.FlatAppearance.BorderSize = 1;
                        btnProgramsFeatures.BackColor = Color.FromArgb(60, 60, 60);
                        btnProgramsFeatures.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnOptFeatures.FlatStyle = FlatStyle.Flat;
                        btnOptFeatures.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnOptFeatures.FlatAppearance.BorderSize = 1;
                        btnOptFeatures.BackColor = Color.FromArgb(60, 60, 60);
                        btnOptFeatures.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnAllClassic.FlatStyle = FlatStyle.Flat;
                        btnAllClassic.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnAllClassic.FlatAppearance.BorderSize = 1;
                        btnAllClassic.BackColor = Color.FromArgb(60, 60, 60);
                        btnAllClassic.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnSysProps.FlatStyle = FlatStyle.Flat;
                        btnSysProps.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnSysProps.FlatAppearance.BorderSize = 1;
                        btnSysProps.BackColor = Color.FromArgb(60, 60, 60);
                        btnSysProps.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnEnvVars.FlatStyle = FlatStyle.Flat;
                        btnEnvVars.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnEnvVars.FlatAppearance.BorderSize = 1;
                        btnEnvVars.BackColor = Color.FromArgb(60, 60, 60);
                        btnEnvVars.FlatAppearance.MouseOverBackColor = Color.Black;

                        btnPerfOptions.FlatStyle = FlatStyle.Flat;
                        btnPerfOptions.FlatAppearance.BorderColor = SystemColors.Highlight;
                        btnPerfOptions.FlatAppearance.BorderSize = 1;
                        btnPerfOptions.BackColor = Color.FromArgb(60, 60, 60);
                        btnPerfOptions.FlatAppearance.MouseOverBackColor = Color.Black;

                        DarkTitleBar(settingsDialog.Handle);
                        settingsDialog.BackColor = Color.FromArgb(43, 43, 43);
                        settingsDialog.ForeColor = Color.White;
                    }

                    settingsDialog.Controls.Add(titleLabel);
                    settingsDialog.Controls.Add(buttonHelp);
                    settingsDialog.Controls.Add(btnRCTSettings);
                    settingsDialog.Controls.Add(btnWinSettings);
                    settingsDialog.Controls.Add(btnAppsFeatures);
                    settingsDialog.Controls.Add(btnControlPanel);
                    settingsDialog.Controls.Add(btnProgramsFeatures);
                    settingsDialog.Controls.Add(btnOptFeatures);
                    settingsDialog.Controls.Add(btnAllClassic);
                    settingsDialog.Controls.Add(btnSysProps);
                    settingsDialog.Controls.Add(btnEnvVars);
                    settingsDialog.Controls.Add(btnPerfOptions);

                    // Only add Scale control on Windows 10+ (requires modern DPI APIs)
                    if (cboScale != null) settingsDialog.Controls.Add(cboScale);

                    // Always add Theme control (works on all Windows versions)
                    settingsDialog.Controls.Add(cboTheme);

                    settingsDialog.Location = GetDialogPosition(settingsDialog, -(int)(50 * ScaleFactor));

                    settingsDialog.ShowDialog();
                }
            }
        }
    }
}
