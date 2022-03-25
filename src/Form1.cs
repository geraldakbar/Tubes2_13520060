using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Folder_Crawling
{
    public partial class Form1 : Form
    {
        private bool isBFS = false;
        private bool isDFS = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void chooseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                printStarting.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void printStarting_TextChanged(object sender, EventArgs e)
        {
            string directory = printStarting.Text;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "explorer.exe";
                startInfo.Arguments = directory;
                //Process.Start(startInfo);
            }
            catch
            {
                MessageBox.Show(string.Format("Error While opening directory {0}", directory));
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.isBFS = true;
            }
            else
            {
                this.isBFS = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                this.isDFS = true;
            }
            else
            {
                this.isDFS = false;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            searchButton.Enabled = false;
            bool isNumeric = int.TryParse(StepDelayTextbox.Text, out _);
            if (printStarting.Text == "")
            {
                MessageBox.Show("Please input Starting Directory");
                searchButton.Enabled = true;
                return;
            }
            if (inputName.Text == "")
            {
                MessageBox.Show("File name must be filled out!");
                searchButton.Enabled = true;
                return;
            }
            if (StepDelayTextbox.Text == "")
            {
                MessageBox.Show("Please input step delay");
                searchButton.Enabled = true;
                return;
            }
            if (!isNumeric)
            {
                MessageBox.Show("Step delay must be integer!");
                searchButton.Enabled = true;
                return;
            }
            if (!this.isBFS && !this.isDFS)
            {
                MessageBox.Show("Please choose searching method");
                searchButton.Enabled = true;
                return;
            }
            else if (this.isBFS && this.isDFS)
            {
                MessageBox.Show("Please choose only one searching method");
                searchButton.Enabled = true;
                return;
            }
            else
            {
                string startingDirectory = printStarting.Text;
                string filename = inputName.Text;
                bool findAllOccurance = findAllCheck.Checked;
                int.TryParse(StepDelayTextbox.Text, out int stepDelay);

                flowLayoutPanel1.Controls.Clear();
                gViewer.Graph = null;
                
                if (this.isDFS)
                {
                    LabelSearching("DFS");
                } 
                else if (this.isBFS)
                {
                    LabelSearching("BFS");
                }

                // Start TimeSearching
                Stopwatch stopwatch = Stopwatch.StartNew();

                DFS dfs = null;
                BFS bfs = null;
                if (this.isDFS)
                {
                    // Lakukan pencarian dengan metode DFS
                    dfs = new DFS(startingDirectory, gViewer);
                    dfs.searchFilePathDFS(startingDirectory, filename, null, stepDelay, findAllOccurance);
                    stopwatch.Stop();
                }
                else if (this.isBFS)
                {
                    // Lakukan pencarian dengan metode BFS
                    bfs = new BFS(startingDirectory, gViewer);
                    bfs.searchFilePathBFS(startingDirectory, filename, stepDelay, findAllOccurance);
                    stopwatch.Stop();
                }

                // Tambahin garis
                PathSearching();

                // Add Time Searching
                TimerSearching(stopwatch);

                // Tambahin garis
                PathSearching();

                // Tambahin tulisan path
                ShowPathSearching();

                if (this.isDFS)
                {
                    if (dfs.getSolutionPath().Count > 0)
                    {
                        foreach (String dir in dfs.getSolutionPath())
                        {
                            Console.WriteLine(dir);
                            addDirectoryLinkLabel(dir, flowLayoutPanel1);
                        }
                    }
                    else
                    {
                        NotFoundLabel(flowLayoutPanel1);
                    }
                }
                else if (this.isBFS)
                {
                    if (bfs.getSolutionPath().Count > 0)
                    {
                        foreach (String dir in bfs.getSolutionPath())
                        {
                            Console.WriteLine(dir);
                            addDirectoryLinkLabel(dir, flowLayoutPanel1);
                        }
                    }
                    else
                    {
                        NotFoundLabel(flowLayoutPanel1);
                    }
                }
                searchButton.Enabled = true;
                return;
            }
        }

        public void LabelSearching(string searchMethod){
            // Label metode pencarian yang digunakan
            Label methodLabel = new Label();
            methodLabel.Text = string.Format("{0} method is used in searching process.", searchMethod);
            methodLabel.AutoSize = true;
            methodLabel.ForeColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            flowLayoutPanel1.Controls.Add(methodLabel);
        }

        public void TimerSearching(Stopwatch stopwatch){
            // Label waktu pencarian
            Label searchTimeLabel = new Label();
            searchTimeLabel.Text = String.Format("Execution time: {0} s", (float)stopwatch.ElapsedMilliseconds / 1000);
            searchTimeLabel.AutoSize = true;
            searchTimeLabel.ForeColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            flowLayoutPanel1.Controls.Add(searchTimeLabel);
        }

        public void PathSearching(){
            Label spacing = new Label();
            spacing.Text = "---------------------------------------------------";
            spacing.AutoSize = true;
            spacing.ForeColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            flowLayoutPanel1.Controls.Add(spacing);
        }

        public void ShowPathSearching(){
            Label pathsLabel = new Label();
            pathsLabel.Text = "File Path(s): ";
            pathsLabel.AutoSize = true;
            pathsLabel.ForeColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            flowLayoutPanel1.Controls.Add(pathsLabel);
        }

        public void PathLinkLabel_Clicked(object sender, EventArgs e)
        {
            // Event Handler untuk path link label yang dihasilkan dari pencarian
            LinkLabel linkLabel = sender as LinkLabel;
            string directory = System.IO.Directory.GetParent(linkLabel.Text).FullName;

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "explorer.exe";
                startInfo.Arguments = directory;
                Process.Start(startInfo);
            }
            catch
            {
                MessageBox.Show(string.Format("Error in opening directory {0}", directory));
            }
        }

         public void addDirectoryLinkLabel(String directory, FlowLayoutPanel flp)
        {
            LinkLabel pathLinkLabel = new LinkLabel();
            pathLinkLabel.Text = directory;
            pathLinkLabel.AutoSize = true;
            pathLinkLabel.LinkColor = Color.Cyan;
            pathLinkLabel.Click += new EventHandler(PathLinkLabel_Clicked);
            flp.Controls.Add(pathLinkLabel);
        }

        public void NotFoundLabel(FlowLayoutPanel flp)
        {
            Label notFoundLabel = new Label();
            notFoundLabel.Text = "No matching file found";
            notFoundLabel.AutoSize = true;
            notFoundLabel.ForeColor = Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            flp.Controls.Add(notFoundLabel);
        }

        private void inputName_TextChanged(object sender, EventArgs e)
        {

        }

        private void findAllCheck_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void StepDelayTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void gViewer_Load(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
