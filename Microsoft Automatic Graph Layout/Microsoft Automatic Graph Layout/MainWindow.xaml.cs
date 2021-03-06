﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Prototype.Ranking;
using Microsoft.Win32;

namespace Microsoft_Automatic_Graph_Layout
{
    public partial class MainWindow
    {
         
        private string Filename;
        public List<InputTable> result = new List<InputTable>();

        public MainWindow()
        {
            InitializeComponent();    
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {           
            SetGrid();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWindow();
            addWindow.ShowDialog();
            if (addWindow.item.Time != -1)
            {
                result.Add(addWindow.item);
                SetGrid();
                CreateGraph();
            }
        }

        private void ButtonChange_OnClick(object sender, RoutedEventArgs e)
        {
            if  (grid.SelectedItem != null)
            {
                InputTable item = grid.SelectedItem as InputTable;
                var index = result.IndexOf(item);
                result.Remove(item);
                result.Insert(index, item);
                CreateGraph();
            }
            else
            {
                MessageBox.Show("Choose item from grid");
            }
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (grid.SelectedIndex >= 0)
            {
                InputTable customer = grid.SelectedItem as InputTable;
                result.Remove(customer);
                SetGrid();
                CreateGraph();
            }
            else
            {
                MessageBox.Show("Choose item from grid");
            }
        }

        private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
        {
            SetFilename();
            ReadTable();
            SetGrid();
            CreateGraph();
        }

        private void MenuSave_OnClick(object sender, RoutedEventArgs e)
        {
            WriteTable();
        }
        private void MenuExit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SetFilename()
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".netab",
                Filter = "Network Table (.netab)|*.netab"
            };
            if (ofd.ShowDialog() == true)
            {
                Filename = ofd.FileName;
            }
        }

        private void CreateGraph()
        {
            var graph = new Graph { LayoutAlgorithmSettings = new RankingLayoutSettings() };

            SetGrid();
           
            List<Edge> netGraph = new List<Edge>();

            var count = 0;
            var newCount = 0;
            var last = true;
            List<string> beforeOpline = new List<string>();
            foreach (var input in result.Where(input => input.BeforeOperations != "-"))
            {
                if (input.BeforeOperations.Contains(","))
                {
                    if (last) newCount++;
                    var beforeOperations = input.BeforeOperations.Split(',');
                    foreach (var operation in beforeOperations)
                    {
                        if (operation == "-" && !beforeOpline.Contains(input.BeforeOperations))
                        {
                            count = 0;
                            beforeOpline.Add(operation);
                        }
                        else
                        {
                            if (ContainBefore(netGraph, operation) != "not contain")
                            {
                                count = int.Parse(ContainBefore(netGraph, operation));
                            }
                            else
                            {
                                count++;
                            }
                        }

                        var time = GetOp(operation).Time;
                        netGraph.Add(new Edge(count.ToString(), operation + "/" + time.ToString(), newCount.ToString()));
                    }
                    if (!CheckBefore(input.Operation))
                    {
                        newCount++;
                        count++;
                        last = false;
                        netGraph.Add(new Edge(count.ToString(), input.Operation + "/" + input.Time.ToString(), newCount.ToString()));
                    }
                }
                else
                {
                    var operation = GetOp(input.BeforeOperations).BeforeOperations;
                    if (operation == "-" && !beforeOpline.Contains(input.BeforeOperations))
                    {
                        count = 0;
                        beforeOpline.Add(input.BeforeOperations);
                    }
                    else
                    {
                        if (ContainBefore(netGraph, operation) != "not contain")
                        {
                            count = int.Parse(ContainBefore(netGraph, operation));
                        }
                        else
                        {
                            count++;
                        }
                    }

                    if (last) newCount++;

                    var time = GetOp(input.BeforeOperations).Time;

                    if (!CheckBefore(input.Operation))
                    {
                        if (ContainBefore(netGraph, input.BeforeOperations) != "not contain")
                        {
                            count = int.Parse(ContainBefore(netGraph, input.BeforeOperations));
                        }
                        else
                        {
                            count++;
                        }
                        last = false;
                        netGraph.Add(new Edge(count.ToString(), input.Operation + "/" + input.Time.ToString(), newCount.ToString()));
                    }
                    else
                    {
                        netGraph.Add(new Edge(count.ToString(), input.BeforeOperations + "/" + GetOp(input.BeforeOperations).Time.ToString(), newCount.ToString()));
                    }
                }
            }

            if (netGraph.Any())
            {
                if (netGraph != null)
                    foreach (var edge in netGraph)
                    {
                        graph.AddEdge(edge.vertex1, edge.edgeLabel, edge.vertex2);
                    }
                gViewer.Graph = graph;
            }

        }

        private void ReadTable()
        {
            result = new List<InputTable>();
            try
            {
                var lines = File.ReadAllLines(Filename);
                result.AddRange(lines.Select(t => t.Split(' ')).Select(splitLine => new InputTable(splitLine[0], splitLine[1], int.Parse(splitLine[2]))));
            }
            catch (Exception)
            {
                MessageBox.Show("Could not read the file");
            }
        }
        private void WriteTable()
        {
            try
            {
                if (Filename != null)
                {
                    using (var file = new StreamWriter(Filename))
                    {
                        foreach (var item in result)
                        {
                            file.WriteLine(item);
                        }
                    }
                }
                else
                {
                    var saveFileDialog = new SaveFileDialog { Filter = "Network Table (*.netab)|*.netab" };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        using (var file = File.AppendText(saveFileDialog.FileName))
                        {
                            foreach (var item in result)
                            {
                                file.WriteLine(item);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not write the file");
            }
        }

        private void SetGrid()
        {
            grid.ItemsSource = result;
            grid.Items.Refresh();
        }

        private InputTable GetOp(string operation)
        {
            foreach (var input in result.Where(input => input.Operation == operation))
            {
                return input;
            }
            throw new Exception("Такой операции нет!");
        }
        private bool CheckBefore(string operation)
        {
            foreach (var input in result)
            {
                if (input.BeforeOperations.Contains(","))
                {
                    var beforeOps = input.BeforeOperations.Split(',');
                    if (beforeOps.Any(ope => operation == ope))
                    {
                        return true;
                    }
                }
                else
                {
                    if (operation == input.BeforeOperations) return true;
                } 
            }
            return false;
        }
        private string ContainBefore(List<Edge> graph, string str)
        {
            foreach (var edge in graph)
            {
                if (edge.edgeLabel.First().ToString() == str) return edge.vertex2;
            }
            return "not contain";
        }
    }
}
