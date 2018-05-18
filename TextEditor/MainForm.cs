using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace TextEditor
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Stores the open File Handler instance, null if no file open.
        /// </summary>
        public FileHandler fileHandler;

        //A property to access textEditorBox.Lines publically.
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
            //Create a new file handler with the path.
            fileHandler = new FileHandler(openFileDialog.FileName);
            textEditorBox.Lines = fileHandler.fileLines;


            //This section is used to control which features are active, and is present in many events.
            //The user can't save if no file is open, so the save button needs to be enabled/disabled appropriately.
            //This is common with many controls.

            textEditorBox.Enabled = true;
            saveToolStripMenu.Enabled = true;
            saveAsToolStripMenu.Enabled = true;
            closeToolStripMenu.Enabled = true;
            findAndReplaceToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            textEditorBox_TextChanged(null, EventArgs.Empty);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler.SaveFileToCurrentPath(textEditorBox.Lines);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
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
            characterCountLabel.Text = "0 Characters";
            linesCountLabel.Text = "0 Lines";
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            fileHandler = null;
            fileHandler = new FileHandler(saveFileDialog.FileName);
            fileHandler.SaveFileToCurrentPath(textEditorBox.Lines);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = textEditorBox.Font;
            fontDialog.ShowDialog();
            textEditorBox.Font = fontDialog.Font;
        }

        private void lineNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If the line numbers button is checked, show line numbers. Else, don't.
            if (lineNumbersToolStripMenuItem.Checked == true)
            {
                textBox2.Show();
            }
            else
            {
                textBox2.Hide();
            }

        }

        private void textEditorBox_TextChanged(object sender, EventArgs e)
        {
            //Commented due to performance issues.
            //This feature needs to be in a different class, and probably needs a different thread.

            //List<string> lineNumbers = new List<string>();
            //for(int i = 0; i < textBox1.Lines.Length; i++)
            //{
            //    lineNumbers.Add(i.ToString());
            //}
            //textBox2.Lines = lineNumbers.ToArray();

            this.linesCountLabel.Text = this.textEditorBox.Lines.Count().ToString() + " Lines";
            this.characterCountLabel.Text = this.textEditorBox.Text.Length.ToString() + " Characters";
            string[] words = this.textEditorBox.Text.Split(new string[] { "[^ ] [^ ]" }, StringSplitOptions.None);
            this.wordCountLabel.Text = words.Count().ToString();

            //for(int i = 0; i < textBox1.Lines.Length; i++)
            //{
            //    textBox2.Lines[textBox2.Lines.Length] = "\n" + i;
            //}
        }

        #region Not Yet Implemented

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Commented due to: Not Yet Implemented.

            //string find;
            //string replace;
            //FindAndReplaceForm findAndReplaceForm = new FindAndReplaceForm();
            //findAndReplaceForm.ShowDialog();

            //FindAndReplaceAll(findAndReplaceForm.find, findAndReplaceForm.replace);
        }

        /// <summary>
        /// This is WIP method for the find and replace tool, which was not ready for the release build.
        /// The feature has been hidden from the form.
        /// </summary>
        /// <param name="find"></param>
        /// <param name="replace"></param>
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

        #endregion

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(statusBarToolStripMenuItem.Checked)
            {
                textStatusStrip.Show();
            }
            else
            {
                textStatusStrip.Hide();
            }
        }

        #region Edit Control Events

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Paste();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textEditorBox.Redo();
        }

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {

            //The plan here is to show features that aren't yet finished, if the program is in debug.
            //For some reason this isn't working. The loop is fine but the controls remain invisable.
            //Commented due to unknown effects.
            #if DEBUG
            foreach (Control control in this.Controls)
            {
                //control.Visible = true;
            }
            #endif
        }

        //This event opens the Help document when the user clicks Help.
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("help.html");
        }

    }
}
