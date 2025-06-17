using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace RustPluginObfuscator
{
    [System.ComponentModel.DesignerCategory("Code")]
    internal sealed class MainForm : Form
    {
        private TextBox txtFilePath;
        private Button btnBrowse;
        private CheckBox chkStringObf;
        private CheckBox chkVarObf;
        private CheckBox chkCompress;
        private CheckBox chkNoise;
        private Button btnStart;
        private TextBox txtLog;
        private OpenFileDialog openDialog;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Rust Plugin Obfuscator";
            this.Width = 660;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // --- Theme colors ---
            Color background = Color.FromArgb(32, 30, 45); // тёмно-серый с фиолетовым оттенком
            Color foreground = Color.FromArgb(220, 200, 255); // мягкий светло-фиолетовый
            this.BackColor = background;
            this.ForeColor = foreground;

            // --- File path row ---
            txtFilePath = new TextBox
            {
                Left = 20,
                Top = 20,
                Width = 480,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AllowDrop = true
            };
            txtFilePath.DragEnter += TxtFilePath_DragEnter;
            txtFilePath.DragDrop += TxtFilePath_DragDrop;

            btnBrowse = new Button
            {
                Text = "Обзор…",
                Left = txtFilePath.Right + 10,
                Top = 18,
                Width = 100
            };
            btnBrowse.Click += BtnBrowse_Click;

            // --- Options ---
            chkStringObf = new CheckBox { Text = "Обфускация строк", Left = 20, Top = 60, Width = 200, ForeColor = foreground, BackColor = background };
            chkVarObf = new CheckBox { Text = "Обфускация переменных", Left = 20, Top = 85, Width = 200, ForeColor = foreground, BackColor = background };
            chkCompress = new CheckBox { Text = "Сжатие кода", Left = 20, Top = 110, Width = 200, ForeColor = foreground, BackColor = background };
            chkNoise = new CheckBox { Text = "Шумовые методы/комм. (β)", Left = 20, Top = 135, Width = 220, ForeColor = foreground, BackColor = background };

            chkStringObf.Checked = false;
            chkVarObf.Checked = false;
            chkCompress.Checked = false;
            chkNoise.Checked = false;

            // --- Start button ---
            btnStart = new Button
            {
                Text = "Обфусцировать",
                Left = 20,
                Top = 175,
                Width = 120,
                Height = 30,
                ForeColor = foreground,
                BackColor = Color.FromArgb(60, 50, 80),
                FlatStyle = FlatStyle.Popup
            };
            btnStart.Click += BtnStart_ClickAsync;

            // --- Log ---
            txtLog = new TextBox
            {
                Left = 20,
                Top = 220,
                Width = 600,
                Height = 200,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.FromArgb(45, 40, 60),
                ForeColor = foreground
            };

            // --- File dialog ---
            openDialog = new OpenFileDialog
            {
                Filter = "C# files (*.cs)|*.cs|All files (*.*)|*.*",
                Title = "Выберите C# плагин"
            };

            // --- Add controls ---
            this.Controls.Add(txtFilePath);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(chkStringObf);
            this.Controls.Add(chkVarObf);
            this.Controls.Add(chkCompress);
            this.Controls.Add(chkNoise);
            this.Controls.Add(btnStart);
            this.Controls.Add(txtLog);
        }

        private void TxtFilePath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TxtFilePath_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                txtFilePath.Text = files[0];
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openDialog.FileName;
            }
        }

        private async void BtnStart_ClickAsync(object sender, EventArgs e)
        {
            string path = txtFilePath.Text;
            if (!File.Exists(path))
            {
                MessageBox.Show("Укажите корректный путь к .cs файлу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Disable UI during processing
            ToggleUi(false);
            txtLog.Clear();
            AppendLog($"Начинаем обфускацию: {Path.GetFileName(path)}\r\n");

            try
            {
                string code = await File.ReadAllTextAsync(path);
                AppendLog("Чтение исходного кода…\r\n");

                bool noise = chkNoise.Checked;

                string obfCode = await Task.Run(() => ObfuscationEngine.ObfuscateCode(code, noise));
                AppendLog("Обфускация завершена.\r\n");

                string outputPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, "obfuscated_" + Path.GetFileName(path));
                await File.WriteAllTextAsync(outputPath, obfCode);
                AppendLog($"Файл сохранён: {outputPath}\r\n");
            }
            catch (Exception ex)
            {
                AppendLog($"ОШИБКА: {ex.Message}\r\n");
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleUi(true);
            }
        }

        private void ToggleUi(bool enable)
        {
            txtFilePath.Enabled = enable;
            btnBrowse.Enabled = enable;
            chkStringObf.Enabled = enable;
            chkVarObf.Enabled = enable;
            chkCompress.Enabled = enable;
            chkNoise.Enabled = enable;
            btnStart.Enabled = enable;
            Cursor = enable ? Cursors.Default : Cursors.WaitCursor;
        }

        private void AppendLog(string text)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => txtLog.AppendText(text)));
            }
            else
            {
                txtLog.AppendText(text);
            }
        }
    }
} 