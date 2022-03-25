// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Folder_Crawling 
{
    public class BFS {

        private Graph GraphFile;
        private List<string> solutionPath;
        private Microsoft.Msagl.GraphViewerGdi.GViewer viewer;

        public BFS(string rootDirectory, Microsoft.Msagl.GraphViewerGdi.GViewer viewer)
        {
            this.GraphFile = new Graph(rootDirectory);
            this.solutionPath = new List<string>();
            this.viewer = viewer;
        }

        // Setter
        public void setSolution(string Path) {
            this.solutionPath.Add(Path);
        }

        // Getter

        public List<string> getSolutionPath() {
            return this.solutionPath;
        }

        // Checker

        // Fungsi untuk basis, mengecek apakah filenya ada di directory atau tidak
        public bool isFileFound(string rootDir, string filename) {
            return Directory.GetFiles(rootDir, filename).Length > 0;
        }

        // Mengecek apakah dalam suatu directory ada file atau tidak
        public bool isExistFileInDirectory(string Path) {
            return Directory.GetFiles(Path, "*").Length > 0;
        }

        // Mengecek apakah directory bisa ditelusuri (valid atau tidak)
        // Valid apabila directorynya tidak kosong (ada directory lain/file di dalamnya)
        public bool isDirectoryValid(string directory) {
            string[] directories = Directory.GetDirectories(directory, "*");
            return !(directories.Length == 0) | (Directory.GetFiles(directory, "*")).Length > 0;
        }

        // Parameter: path ke rootnya dan nama file yang dicari
        public void searchFilePathBFS(string Root, string SearchFile, int Delay, bool FindAll) 
        {
            string directoryR = Root;
            List<string> res = new List<string>();
            Queue<string> q = new Queue<string>();
            Queue<Microsoft.Msagl.Drawing.Node> nodeQueue = new Queue<Microsoft.Msagl.Drawing.Node>();

            // Add node di awal untuk root
            q.Enqueue(Root);
            nodeQueue.Enqueue(GraphFile.R);

            Microsoft.Msagl.Drawing.Node currentParentNode = GraphFile.R;
            // Ubah warna node jadi merah
            currentParentNode.Attr.Color = Microsoft.Msagl.Drawing.Color.Red;

            // Implementasi
            while ((FindAll || this.solutionPath.Count == 0) && (q.Count > 0))
            {
                directoryR = q.Dequeue();

                currentParentNode = nodeQueue.Dequeue();
                string[] directories = Directory.GetDirectories(directoryR, "*");

                // Tambahkan node semua folder directory
                Array.Reverse(directories);

                foreach (string directory in directories)
                {
                    Microsoft.Msagl.Drawing.Node N = GraphFile.AddEdgeBlack(currentParentNode, Path.GetFileName(directory));
                }

                Array.Reverse(directories);

                // Tambahkan node semua file directory
                string[] files = Directory.GetFiles(directoryR);

                Array.Reverse(files);

                foreach (string file in files)
                {
                    Microsoft.Msagl.Drawing.Node N = GraphFile.AddEdgeBlack(currentParentNode, Path.GetFileName(file));
                }

                Array.Reverse(files);

                // Tampilkan pohon
                GraphFile.VisualizeGraph(this.viewer, Delay);

                // Search file
                foreach (string file in files)
                {
                    if (Path.GetFileName(file).Equals(SearchFile))
                    {
                        this.setSolution(file);
                        GraphFile.TurnBlue(GraphFile.dirToList(file));
                        GraphFile.VisualizeGraph(this.viewer, Delay);
                        // Kalo tidak ketemu
                        if (!FindAll)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Microsoft.Msagl.Drawing.Node N = GraphFile.ColorEdgeRed(currentParentNode, Path.GetFileName(file));
                        Graph.ColorNodeRed(N);
                        GraphFile.VisualizeGraph(this.viewer, Delay);
                    }
                }

                // Search directory
                if (FindAll || this.solutionPath.Count == 0)
                {
                    foreach (string directory in directories)
                    {
                        Microsoft.Msagl.Drawing.Node N = GraphFile.ColorEdgeRed(currentParentNode, Path.GetFileName(directory));
                        Graph.ColorNodeRed(N);
                        string dirName = Path.GetFileName(directory);
                        if (isDirectoryValid(directory))
                        {
                            q.Enqueue(directory);
                            nodeQueue.Enqueue(N);
                        }

                        GraphFile.VisualizeGraph(this.viewer, Delay);
                    }
                }
            }
        }
    }
}