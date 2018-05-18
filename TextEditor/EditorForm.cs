using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class MainForm : Form
    {
        public FileHandler fileHandler;

        public string[] textBoxLines { get { return this.textEditorBox.Lines; } set {textBoxLines = this.textEditorBox.Lines; } }

        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            fileHandler = new FileHandler(openFileDialog.FileName);
            textEditorBox.Lines = fileHandler.fileLines;

            textEditorBox.Enabled = true;
            saveToolStripMenu.Enabled = true;
            saveAsToolStripMenu.Enabled = true;
            closeToolStripMenu.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler.SaveFileToCurrentPath(textEditorBox.Lines);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler = null;
            textEditorBox.Lines = null;
            textEditorBox.Enabled = false;
            fontToolStripMenuItem.Enabled = false;
            closeToolStripMenu.Enabled = false;
            saveToolStripMenu.Enabled = false;
            saveAsToolStripMenu.Enabled = false;
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            fileHandler = null;
            fileHandler = new FileHandler(saveFileDialog.FileName);
            fileHandler.SaveFileToCurrentPath(textEditorBox.Lines);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler = new FileHandler();
            textEditorBox.Enabled = true;
            fontToolStripMenuItem.Enabled = true;
            saveAsToolStripMenu.Enabled = true;
            closeToolStripMenu.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textEditorBox.Font;
            fontDialog.ShowDialog();
            textEditorBox.Font = fontDialog.Font;
        }

        private void lineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lineNumbersToolStripMenuItem.Checked == true)
            {
                textBoxLineNumbers.Show();
            }
            else
            {
                textBoxLineNumbers.Hide();
            }

        }

        private void textEditorBox_TextChanged(object sender, EventArgs e)
        {
            List<string> lineNumbers = new List<string>();
            for(int i = 0; i < textEditorBox.Lines.Length; i++)
            {
                lineNumbers.Add(i.ToString());
            }
            textBoxLineNumbers.Lines = lineNumbers.ToArray();

            //for(int i = 0; i < textBox1.Lines.Length; i++)
            //{
            //    textBox2.Lines[textBox2.Lines.Length] = "\n" + i;
            //}
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string find;
            string replace;
            FindAndReplaceForm findAndReplaceForm = new FindAndReplaceForm();
            findAndReplaceForm.ShowDialog();

            FindAndReplaceAll(findAndReplaceForm.find, findAndReplaceForm.replace);
        }


        public void FindAndReplaceAll(string find, string replace)
        {
            for (int i = 0; i < textEditorBox.Lines.Count(); i++)
            {
                if(textEditorBox.Lines[i].Contains(find))
                {
                    textEditorBox.Lines[i].Replace(find, replace);
                }
            }
        }
    }
}
