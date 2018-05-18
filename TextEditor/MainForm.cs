using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TextEditor.FileHandlers;
using System.Text.RegularExpressions;

namespace TextEditor
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Stores the open File Handler instance, null if no file open.
        /// </summary>
        public FileHandler fileHandler;

        //A property to access textEditorBox.Lines publically.
        public string[] TextBoxLines { get { return this.textEditorBox.Lines; } set {TextBoxLines = this.textEditorBox.Lines; } }

        public MainForm()
        {
            InitializeComponent();
            UpdateOptions();
        }

        private void UpdateOptions()
        {
            if (lineNumbersToolStripMenuItem.Checked)
                lineNumbersBox.Show();
            else
                lineNumbersBox.Hide();

            if (statusBarToolStripMenuItem.Checked)
                textStatusStrip.Show();
            else
                textStatusStrip.Hide();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            //Create a new file handler with the path.
            fileHandler = new FileHandler(openFileDialog.FileName);
            textEditorBox.Lines = fileHandler.FileLines;


            //This section is used to control which features are active, and is present in many events.
            //The user can't save if no file is open, so the save button needs to be enabled/disabled appropriately.
            //This is common with many controls.

            textEditorBox.Enabled = true;
            saveToolStripMenu.Enabled = true;
            saveAsToolStripMenu.Enabled = true;
            closeToolStripMenu.Enabled = true;
            findAndReplaceToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            filePathLabel.Text = openFileDialog.FileName;
            TextEditorBox_TextChanged(null, EventArgs.Empty);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler.SaveFileToCurrentPath(textEditorBox.Lines);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If a file is closed, empty the file handler and text box, also disable edit tools.

            fileHandler = null;
            textEditorBox.Lines = null;
            textEditorBox.Enabled = false;
            fontToolStripMenuItem.Enabled = false;
            closeToolStripMenu.Enabled = false;
            saveToolStripMenu.Enabled = false;
            saveAsToolStripMenu.Enabled = false;
            findAndReplaceToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            paragraphCountLabel.Text = "0 Paragraphs";
            wordCountLabel.Text = "0 Words";
            characterCountLabel.Text = "0 Characters";
            linesCountLabel.Text = "0 Lines";
            filePathLabel.Text = "No File";
        }

        private void SaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            fileHandler = null;
            fileHandler = new FileHandler(saveFileDialog.FileName);
            fileHandler.SaveFileToCurrentPath(textEditorBox.Lines);
            filePathLabel.Text = saveFileDialog.FileName;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create a new file handler with no path.
            fileHandler = new FileHandler();
            textEditorBox.Text = "";
            textEditorBox.Enabled = true;
            fontToolStripMenuItem.Enabled = true;
            saveAsToolStripMenu.Enabled = true;
            closeToolStripMenu.Enabled = true;
            findAndReplaceToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileHandler != null || textEditorBox.Lines != null)
            {
                DialogResult areYouSure = MessageBox.Show("You have unsaved text, are you sure you would like to exit?", "Text Editor - Exit", MessageBoxButtons.OKCancel);
                if (areYouSure == DialogResult.Cancel)
                {
                    return;
                }
            }
            Environment.Exit(0);
        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog.ShowDialog();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textEditorBox.Font;
            fontDialog.ShowDialog();
            textEditorBox.Font = fontDialog.Font;
        }

        private void LineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        private void TextEditorBox_TextChanged(object sender, EventArgs e)
        {
            List<string> lineNumbers = new List<string>();
            for (int i = 1; i < textEditorBox.Lines.Length + 1; i++)
            {
                lineNumbers.Add(i.ToString());
            }
            lineNumbersBox.Lines = lineNumbers.ToArray();

            this.linesCountLabel.Text = this.textEditorBox.Lines.Count().ToString() + " Lines";
            this.characterCountLabel.Text = this.textEditorBox.Text.Length.ToString() + " Characters";
            int wordCount = this.textEditorBox.Text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Count();
            this.wordCountLabel.Text = wordCount.ToString() + " Words";
            string[] paragraphs = this.textEditorBox.Text.Split(new string[] { "\r\r", "\n\n", "\r\n\r\n", "\n\r\n\r" }, StringSplitOptions.RemoveEmptyEntries);
            this.paragraphCountLabel.Text = paragraphs.Count().ToString() + " Paragraphs";
        }

        private void FindAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindAndReplaceForm findAndReplaceForm = new FindAndReplaceForm();
            findAndReplaceForm.ShowDialog();

            FindAndReplaceAll(findAndReplaceForm.Find, findAndReplaceForm.Replace);
            TextEditorBox_TextChanged(null, EventArgs.Empty);
        }

        public void FindAndReplaceAll(string find, string replace)
        {
            if (String.IsNullOrWhiteSpace(find))
                return;

            if (replace == null)
                replace = "";

            string[] lines = textEditorBox.Lines;

            for (int i = 0; i < lines.Count(); i++)
            {
                if(lines[i].Contains(find))
                {
                    lines[i] = lines[i].Replace(find, replace);
                }
            }

            textEditorBox.Lines = lines;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateOptions();
        }

        #region Edit Control Events

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Cut();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Copy();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Paste();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Undo();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Redo();
        }

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            #if DEBUG
            foreach (Control control in this.Controls)
            {
                control.Visible = true;
            }
            #endif
        }

        //This event opens the Help document when the user clicks Help.
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("help.html");
        }

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr handle, int nBar, int nPos, bool bRedraw);
        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr handle, uint msg, int wParam, int lParam);
        [DllImport("User32.dll")]
        private extern static int GetScrollPos(IntPtr handle, int nBar);

        private void TextEditorBox_VScroll(object sender, EventArgs e)
        {
            int verticalPosition = GetScrollPos(textEditorBox.Handle, 1);
            SetScrollPos(lineNumbersBox.Handle, 0x1, verticalPosition, true);
            PostMessage(lineNumbersBox.Handle, 0x115, 4 + 0x10000 * verticalPosition, 0);
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }
    }
}
