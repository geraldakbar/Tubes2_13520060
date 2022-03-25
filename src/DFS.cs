using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Folder_Crawling
{
    public class DFS
    {
        private Graph fileGraph;

        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        private List<string> solutionPath = new List<String>();

        private Microsoft.Msagl.GraphViewerGdi.GViewer viewer;

        public DFS(string rootDirectory, Microsoft.Msagl.GraphViewerGdi.GViewer viewer)
        {
            this.fileGraph = new Graph(rootDirectory);
            this.solutionPath = new List<string>();
            this.viewer = viewer;
        }

        // Getter
        public List<string> getSolutionPath()
        {
            return this.solutionPath;
        }

        // Adder
        public void addSolution(string Path)
        {
            this.solutionPath.Add(Path);
        }

        public void searchFilePathDFS(string Root, string SearchFile, Microsoft.Msagl.Drawing.Node StartNode, int Delay, bool findAll)
        {
            // Mengecek apakah file sudah ketemu atau belum
            if (!findAll && solutionPath.Count > 0)
            {
                return;
            }
            else{
                // Inisialisasi
                System.IO.FileInfo[] files = null;
                System.IO.DirectoryInfo[] SubDirecs = null;
                System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(Root);

                if (StartNode == null)
                {
                    StartNode = fileGraph.R;
                }

                try
                {
                    files = root.GetFiles("*.*");
                }

                // Handler dir yang tidak bisa diakses
                catch (UnauthorizedAccessException e)
                {
                    log.Add(e.Message);
                }

                // Handler dir yang tidak ditemukan
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                
                // Kalo file tidak null
                if (files != null)
                {
                    // Ubah node menjadi warna merah jika warna awalnya bukan merah

                    if (StartNode.Attr.Color != Microsoft.Msagl.Drawing.Color.Blue)
                    {
                        StartNode.Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    }

                    // Tambahkan node semua folder start directory 
                    SubDirecs = root.GetDirectories();

                    // Reverse
                    Array.Reverse(SubDirecs);

                    foreach (System.IO.DirectoryInfo dirInfo in SubDirecs)
                    {
                        Microsoft.Msagl.Drawing.Node subdirectory = fileGraph.AddEdgeBlack(StartNode, dirInfo.Name);
                    }

                    Array.Reverse(SubDirecs);

                    // Tambahkan node semua file start directory
                    Array.Reverse(files);
                    foreach (System.IO.FileInfo file in files)
                    {
                        Microsoft.Msagl.Drawing.Node subdirectory = fileGraph.AddEdgeBlack(StartNode, file.Name);
                    }
                    Array.Reverse(files);

                    // Tampilkan pohon
                    fileGraph.VisualizeGraph(viewer, Delay);


                    foreach (System.IO.FileInfo fi in files)
                    {
                        // Cek apakah file merupakan file yang ingin dicari
                        if (SearchFile.Equals(fi.Name))
                        {
                            this.addSolution(fi.FullName);
                            fileGraph.TurnBlue(fileGraph.dirToList(fi.FullName));

                            // Show pohon
                            fileGraph.VisualizeGraph(viewer, Delay);

                            if (!findAll)
                            {
                                break;
                            }  
                        }
                        else
                        {
                            Microsoft.Msagl.Drawing.Node N = fileGraph.ColorEdgeRed(StartNode, fi.Name);
                            Graph.ColorNodeRed(N);

                            // Show pohon
                            fileGraph.VisualizeGraph(viewer, Delay);
                        }
                    }

                    // Continue DFS
                    if (this.solutionPath.Count == 0 || findAll)
                    {
                        foreach (System.IO.DirectoryInfo DirectoryInfo in SubDirecs)
                        {
                            Microsoft.Msagl.Drawing.Node NextNode = fileGraph.ColorEdgeRed(StartNode, DirectoryInfo.Name);
                            searchFilePathDFS(DirectoryInfo.FullName, SearchFile, NextNode, Delay, findAll);
                        }
                    }
                }
            }
        }
    }
}