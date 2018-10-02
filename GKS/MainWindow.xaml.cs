using System;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GKS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void InsertValuesFromFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                        doc.Load(fs, DataFormats.Text);
                }
            }
        }

        public void Calc(object sender, RoutedEventArgs e)
        {

            string rtbText = new TextRange(rtb.Document.ContentStart,
                                           rtb.Document.ContentEnd).Text;

            List<string> listOfOperaions = rtbText.Split(new[] { Environment.NewLine },
                                                         StringSplitOptions.RemoveEmptyEntries).ToList();

            int cntFields = listOfOperaions.Count();

            if (cntFields > 5)
                this.Width = this.Width + (cntFields - 5) * 75;
            else
                this.Width = 520;

            List<string>[] details = new List<string>[cntFields];
            for (int i = 0; i < cntFields; i++)
                details[i] = new List<string>();

            List<string> listOfUnequeOperations = new List<string>();

            int iter = 0;
            foreach (string strOfOperation in listOfOperaions)
            {
                details[iter] = strOfOperation.Split(new char[] { ' ' }).ToList();
                foreach (string s in details[iter])
                    if (listOfUnequeOperations.FindIndex(x => x == s) == -1)
                        listOfUnequeOperations.Add(s);
                iter++;
            }

            #region OutInTextBlock

            tbOut.Text = "Count of unique elements: " + listOfUnequeOperations.Count().ToString() + "\n";
            //tbOut.Text += "Max element of help matrix: " + details.Max() + "\n";
            /*
            for (int i = 0; i < cntFields; i++)
            {
                foreach (int value in details[i])
                {
                    tbOut.Text += value + " ";
                }
                tbOut.Text += "\n";
            }
            */

            #endregion

            #region CreateHelpMatrix

            int[,] helpMatrix = new int[cntFields, cntFields];

            for (int i = 1; i < cntFields; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    int cntDifElements = 0;
                    foreach (string item in details[j])
                    {
                        if (details[i].FindIndex(x => x == item) == -1)
                        {
                            cntDifElements++;
                        }
                    }
                    foreach (string item in details[i])
                    {
                        if (details[j].FindIndex(x => x == item) == -1)
                        {
                            cntDifElements++;
                        }
                    }
                    helpMatrix[i, j] = listOfUnequeOperations.Count() - cntDifElements;
                }
            }

            #endregion

            /*
            int[,] helpMatrix = new int[,] { {0,0,0,0,0 },
                                             {3,0,0,0,0 },
                                             {2,5,0,0,0 },
                                             {4,5,3,0,0 },
                                             {3,4,4,5,0 } };
            */
            PrintHelpMatrixInTB(helpMatrix, cntFields);

            #region CreateGroup
            List<int>[] groups = new List<int>[1];

            //find group

            int iterator = 0;
            while (SumOfAllElements(helpMatrix, cntFields) != 0)
            {
                groups[iterator] = new List<int>();

                int maxValue = 0;
                List<int> maxI = new List<int>();
                List<int> maxJ = new List<int>();
                //fing max values
                for (int i = 1; i < cntFields; i++)
                {
                    for (int j = 0; j < i; j++)
                    {

                        if (helpMatrix[i, j] > maxValue)
                        {
                            maxI.Clear();
                            maxJ.Clear();
                            maxValue = helpMatrix[i, j];
                            maxI.Add(i);
                            maxJ.Add(j);
                        }
                        if ((helpMatrix[i, j] == maxValue) &&
                            (
                            (maxI.FindIndex(x => x == i) != -1) ||
                            (maxJ.FindIndex(x => x == j) != -1) ||
                            (maxI.FindIndex(x => x == j) != -1) ||
                            (maxJ.FindIndex(x => x == i) != -1)
                            ))
                        {
                            maxI.Add(i);
                            maxJ.Add(j);
                        }
                    }
                }

                //clear help matrix
                for (int i = 1; i < cntFields; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (
                            (maxI.FindIndex(x => x == i) != -1) ||
                            (maxJ.FindIndex(x => x == j) != -1)
                            )
                        {
                            helpMatrix[i, j] = 0;
                        }
                    }
                }


                foreach (var item in maxI)
                    if (groups[iterator].FindIndex(x => x == item) == -1
                        && !FindElement(groups, iterator, item)
                        )
                        groups[iterator].Add(item);
                foreach (var item in maxJ)
                    if (groups[iterator].FindIndex(x => x == item) == -1
                        && !FindElement(groups, iterator, item)
                        )
                        groups[iterator].Add(item);

                Array.Resize(ref groups, groups.Length + 1);
                iterator++;

            }
            Array.Resize(ref groups, groups.Length - 1);

            #endregion

            tbOut.Text += "\n Groups:";
            PrintGroups(groups);

            #region ClaryfingTheContentOfGroups

            List<int> blockDetails = new List<int>();

            for (int j = 0; j < groups.Length - 1; j++)
            {

                #region Sort Group

                int maxLenght = 0;
                int maxRow = 0;
                int cntMaxGroup = 0;
                List<int> indexMaxGroup = new List<int>();
                for (int i = j; i < groups.Length; i++)
                {
                    List<string> temp = new List<string>();
                    temp = FindUniqueOperationInGroup(groups[i], details);
                    if (maxLenght < temp.Count())
                    {
                        maxLenght = temp.Count();
                        maxRow = i;
                        cntMaxGroup = 0;
                        indexMaxGroup.Clear();
                    }
                    else if (maxLenght == temp.Count())
                    {
                        cntMaxGroup++;
                        indexMaxGroup.Add(i);
                    }

                }

                #region sorting groups with the same number of elements

                if (cntMaxGroup > 1)
                {
                    int maxDetail = 0;
                    for (int i = j; i < groups.Length; i++)
                    {
                        if (indexMaxGroup.FindIndex(x => x == i) != -1)
                        {
                            int cntDetail = 0;
                            for (int q = 0; q < details.Length; q++)
                            {
                                if (groups[i].FindIndex(x => x == q) == -1
                                    &&
                                    blockDetails.FindIndex(x => x == q) == -1)
                                {
                                    int cntSameOperations = 0;
                                    foreach (var operation in details[i])
                                        if (FindUniqueOperationInGroup(groups[i], details).FindIndex(x => x == operation) != -1)
                                            cntSameOperations++;

                                    if (details[i].Count == cntSameOperations)
                                        cntDetail++;

                                }
                            }
                            if (maxDetail < cntDetail)
                            {
                                maxDetail = cntDetail;
                                maxRow = i;
                            }

                        }
                    }
                }

                #endregion

                //sort groups;
                List<int> tmp = groups[maxRow];
                groups[maxRow] = groups[j];
                groups[j] = tmp;

                #endregion

                for (int i = 0; i < details.Length; i++)
                {
                    if (groups[j].FindIndex(x => x == i) == -1
                        &&
                        blockDetails.FindIndex(x => x == i) == -1)
                    {
                        int cntSameOperations = 0;
                        foreach (var operation in details[i])
                            if (FindUniqueOperationInGroup(groups[j], details).FindIndex(x => x == operation) != -1)
                                cntSameOperations++;

                        if (details[i].Count == cntSameOperations)
                        {
                            foreach (var item in groups)
                                if (item.FindIndex(x => x == i) != -1)
                                    item.Remove(i);

                            groups[j].Add(i);
                        }
                    }
                }

                foreach (var item in groups[j])
                    blockDetails.Add(item);
            }
            #endregion

            #region Create Update Group

            List<int>[] updateGroups = new List<int>[1];
            int indexNewGroup = 0;
            foreach (var group in groups)
            {
                if (group.Count() != 0)
                {
                    updateGroups[indexNewGroup] = group;
                    Array.Resize(ref updateGroups, updateGroups.Length + 1);
                    indexNewGroup++;
                }
            }
            Array.Resize(ref updateGroups, updateGroups.Length - 1);

            #endregion

            tbOut.Text += "\n\nNewGroup:";
            PrintGroups(updateGroups);

            #region Creating Graph/Matrix

            List<int[,]> listOfAdjacencyMatrix = new List<int[,]>();

            tbOut.Text += "\n\nMatrix for groups:\n\n";

            foreach (var group in updateGroups)
            {
                List<string> uniqueOperationForGroup = FindUniqueOperationInGroup(group, details);
                int cntUniqueOperations = uniqueOperationForGroup.Count;
                int[,] adjacencyMatrix = new int[cntUniqueOperations, cntUniqueOperations];
                foreach (var detailId in group)
                {
                    string[] operation = details[detailId].ToArray();
                    int cntOperationInDetail = operation.Length;
                    for (int i = 1; i < cntOperationInDetail; i++)
                    {
                        int trackIndex = uniqueOperationForGroup.FindIndex(x => x == operation[i]);
                        int prevIndex = uniqueOperationForGroup.FindIndex(x => x == operation[i - 1]);
                        adjacencyMatrix[prevIndex, trackIndex] = 1;
                    }
                }

                listOfAdjacencyMatrix.Add(adjacencyMatrix);

                #region Print Matrix

                tbOut.Text += "\n\t";
                for (int i = 0; i < cntUniqueOperations; i++)
                {
                    string[] operation = uniqueOperationForGroup.ToArray();
                    tbOut.Text += operation[i] + "\t";
                }
                tbOut.Text += "\n";

                for (int i = 0; i < cntUniqueOperations; i++)
                {
                    string[] operation = uniqueOperationForGroup.ToArray();
                    tbOut.Text += operation[i] + "\t";
                    for (int j = 0; j < cntUniqueOperations; j++)
                        tbOut.Text += adjacencyMatrix[i, j] + "\t";
                    tbOut.Text += "\n";
                }

                #endregion

            }

            #endregion


        }

        public int SumOfAllElements(int[,] array, int size)
        {
            int sum = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    sum += array[i, j];
            return sum;
        }

        public bool FindElement(List<int>[] array, int size, int findValue)
        {
            for (int i = 0; i < size; i++)
                if (array[i].FindIndex(x => x == findValue) != -1)
                    return true;
            return false;
        }

        public void PrintHelpMatrixInTB(int[,] helpMatrix, int cntFields)
        {
            tbOut.Text += "Help matrix:\n";
            for (int i = 0; i < cntFields; i++)
            {
                for (int j = 0; j < cntFields; j++)
                    tbOut.Text += helpMatrix[i, j] + "\t";
                tbOut.Text += "\n";
            }
        }

        public void PrintGroups(List<int>[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                tbOut.Text += "\n " + (i + 1) + " - { ";
                foreach (var item in groups[i])
                    tbOut.Text += (item + 1) + " ";
                tbOut.Text += "}";
            }
        }

        public List<string> FindUniqueOperationInGroup(List<int> detailsInGroup, List<string>[] details)
        {
            List<string> uniqueOperationList = new List<string>();
            foreach (var detailId in detailsInGroup)
                foreach (var operation in details[detailId])
                    if (uniqueOperationList.FindIndex(x => x == operation) == -1)
                        uniqueOperationList.Add(operation);

            return uniqueOperationList;
        }

    }
}