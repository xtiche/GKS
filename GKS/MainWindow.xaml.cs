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
using QuickGraph;
using GraphSharp.Controls;

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
            /*
            if (cntFields > 5)
                this.Width = this.Width + (cntFields - 5) * 75;
            else
                this.Width = 520;

            */
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
            PrintHelpMatrix(helpMatrix, cntFields);

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
                for (int i = 0; i < cntFields; i++)
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
                            (maxJ.FindIndex(x => x == j) != -1)

                            || (maxI.FindIndex(x => x == j) != -1) ||
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

                if (groups.Last() != null && groups[iterator].Count != 0)
                {
                    Array.Resize(ref groups, groups.Length + 1);
                    iterator++;
                }

            }

            while (groups[iterator] == null || groups[iterator].Count() == 0)
            {
                Array.Resize(ref groups, groups.Length - 1);
                iterator--;
            }
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

            #region Creating Adjacency Matrix

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
                    tbOut.Text += operation[i] + "  ";
                }
                tbOut.Text += "\n";

                for (int i = 0; i < cntUniqueOperations; i++)
                {
                    string[] operation = uniqueOperationForGroup.ToArray();
                    tbOut.Text += operation[i] + "\t";
                    for (int j = 0; j < cntUniqueOperations; j++)
                        tbOut.Text += adjacencyMatrix[i, j] + "    ";
                    tbOut.Text += "\n";
                }

                #endregion

            }

            #endregion


            #region Create Graph

            tabControl.Items.Clear();
            for (int i = 0; i < updateGroups.Count(); i++)
            {
                var g = new BidirectionalGraph<object, IEdge<object>>();

                List<string> uniqueOperationForGroup = FindUniqueOperationInGroup(updateGroups[i], details);
                int cntUniqueOperations = uniqueOperationForGroup.Count;
                foreach (var operation in uniqueOperationForGroup)
                    g.AddVertex(operation);

                foreach (var detailId in updateGroups[i])
                {
                    string[] operations = details[detailId].ToArray();
                    int cntOperationInDetail = operations.Length;
                    for (int j = 1; j < cntOperationInDetail; j++)
                        g.AddEdge(new Edge<object>(operations[j - 1], operations[j]));

                }
                GraphLayout gl = new GraphLayout();
                gl.LayoutAlgorithmType = "KK";
                gl.OverlapRemovalAlgorithmType = "FSA";
                gl.Graph = g;

                TabItem ti = new TabItem();
                ti.Header = "Group" + (i + 1);
                ti.Content = gl;

                tabControl.Items.Add(ti);

            }


            #endregion

            #region Create Models

            List<List<string>>[] arrayOfGroupModels = new List<List<string>>[updateGroups.Length];

            for (int i = 0; i < updateGroups.Length; i++)
            {
                List<string> uniqueOpeationInGroup = FindUniqueOperationInGroup(updateGroups[i], details);
                List<List<string>> listOfModels = new List<List<string>>();
                for (int j = 0; j < uniqueOpeationInGroup.Count; j++)
                {
                    List<string> model = new List<string>();
                    model.Add(uniqueOpeationInGroup[j]);
                    listOfModels.Add(model);
                }
                listOfModels.RemoveAll(x => x.Count == 0);
                arrayOfGroupModels[i] = listOfModels;
            }

            PrintArrayOfModels(arrayOfGroupModels);
            #endregion

            #region group merger

            //PrintArrayOfModels(arrayOfGroupModels);

            #region circuit check

            bool checkCircuit = false;

            for (int i = 0; i < updateGroups.Length; i++)
            {
                int[,] currentAndjancencyMatrix = CreateAdjacencyMatrixToModel(arrayOfGroupModels[i], listOfAdjacencyMatrix[i], FindUniqueOperationInGroup(updateGroups[i], details));

                for (int r = 0; r < currentAndjancencyMatrix.GetLength(0) && !checkCircuit; r++)
                    for (int c = 0; c < currentAndjancencyMatrix.GetLength(0) && !checkCircuit; c++)
                    {
                        if (currentAndjancencyMatrix[r, c] == 1)
                        {
                            List<int> usedIndex = new List<int>();
                            usedIndex.Add(c);
                            checkCircuit = CheckCircuit(ref arrayOfGroupModels[i], currentAndjancencyMatrix, usedIndex, r);

                            if (checkCircuit)
                            {
                                if ((arrayOfGroupModels[i][r].Count + arrayOfGroupModels[i][c].Count) <= 5)
                                    MergeModel(ref arrayOfGroupModels[i], r, c);
                                arrayOfGroupModels[i].RemoveAll(x => x.Count == 0);
                            }

                        }
                    }

                if (checkCircuit)
                {
                    checkCircuit = false;
                    i = -1;
                }
            }

            #endregion

            //PrintArrayOfModels(arrayOfGroupModels);

            #region check on closed cirle

            bool circleCheck = false;

            for (int i = 0; i < updateGroups.Length; i++)
            {
                int[,] currentAndjancencyMatrix = CreateAdjacencyMatrixToModel(arrayOfGroupModels[i], listOfAdjacencyMatrix[i], FindUniqueOperationInGroup(updateGroups[i], details));

                for (int r = 0; r < currentAndjancencyMatrix.GetLength(0) && !circleCheck; r++)
                    for (int c = 0; c < currentAndjancencyMatrix.GetLength(0) && !circleCheck; c++)
                    {
                        if (currentAndjancencyMatrix[r, c] == 1)
                        {
                            List<int> usedIndex = new List<int>();
                            usedIndex.Add(c);
                            circleCheck = CheckCirlce(ref arrayOfGroupModels[i], currentAndjancencyMatrix, usedIndex, r);

                            if (circleCheck)
                            {
                                if ((arrayOfGroupModels[i][r].Count + arrayOfGroupModels[i][c].Count) <= 5)
                                    MergeModel(ref arrayOfGroupModels[i], r, c);
                                arrayOfGroupModels[i].RemoveAll(x => x.Count == 0);
                            }

                        }
                    }

                if (circleCheck)
                {
                    circleCheck = false;
                    i = -1;
                }
            }

            #endregion

            PrintArrayOfModels(arrayOfGroupModels);

            #endregion


            #region Model Clarifying

            List<List<string>> listOfAllModel = new List<List<string>>();
            for (int i = 0; i < arrayOfGroupModels.Length; i++)
                foreach (var listOfModel in arrayOfGroupModels[i])
                    listOfAllModel.Add(listOfModel);

            #region sortList
            int max = 0;
            for (int i = 0; i < listOfAllModel.Count - 1; i++)
            {
                max = i;
                for (int j = i + 1; j < listOfAllModel.Count; j++)
                    if (listOfAllModel[j].Count > listOfAllModel[max].Count)
                        max = j;

                if (max != i)
                {
                    var tmp = listOfAllModel[i];
                    listOfAllModel[i] = listOfAllModel[max];
                    listOfAllModel[max] = tmp;
                }
            }
            #endregion

            tbOut.Text += "\nAll Models:\n";
            PrintModel(listOfAllModel);

            List<List<string>> listOfClarifyingModel = new List<List<string>>();
            for (int i = 0; i < listOfAllModel.Count; i++)
                if (!FindInListOfAllModel(listOfAllModel[i], listOfClarifyingModel))
                    listOfClarifyingModel.Add(listOfAllModel[i]);


            tbOut.Text += "\nClarifying Models 1 part:\n";
            PrintModel(listOfClarifyingModel);

            for (int i = 0; i < listOfClarifyingModel.Count; i++)
            {
                for (int j = 0; j < listOfClarifyingModel[i].Count; j++)
                {
                    int index = 0;
                    if ((index = FindOperationInListOfModel(listOfClarifyingModel[i][j], listOfClarifyingModel, i)) != -1)
                    {
                        if (listOfClarifyingModel[i].Count > listOfClarifyingModel[index].Count)
                            listOfClarifyingModel[i].Remove(listOfClarifyingModel[i][j]);
                        else
                            listOfClarifyingModel[index].Remove(listOfClarifyingModel[i][j]);

                        i = -1;
                        break;
                    }
                }
            }

            tbOut.Text += "\nClarifying Models 2 part:\n";
            PrintModel(listOfClarifyingModel);

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

        public int SumOfAllElementsInRow(int[,] ajdencecyMatrix, int row)
        {
            int sum = 0;
            for (int i = 0; i < ajdencecyMatrix.GetLength(1); i++)
                sum += ajdencecyMatrix[row, i];
            return sum;
        }

        public bool FindElement(List<int>[] array, int size, int findValue)
        {
            for (int i = 0; i < size; i++)
                if (array[i].FindIndex(x => x == findValue) != -1)
                    return true;
            return false;
        }

        public void PrintHelpMatrix(int[,] helpMatrix, int cntFields)
        {
            tbOut.Text += "Help matrix:\n";
            for (int i = 0; i < cntFields; i++)
            {
                for (int j = 0; j < cntFields; j++)
                    tbOut.Text += helpMatrix[i, j] + " ";
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

        public void PrintArrayOfModels(List<List<string>>[] arrayOfGroupModels)
        {
            for (int i = 0; i < arrayOfGroupModels.Length; i++)
            {
                tbOut.Text += "\nModels of " + (i + 1) + " group\n";
                PrintModel(arrayOfGroupModels[i]);
            }
        }

        public void PrintModel(List<List<string>> listOfModel)
        {
            for (int p = 0; p < listOfModel.Count; p++)
            {
                tbOut.Text += "Model " + (p + 1) + ": { ";
                foreach (var operation in listOfModel[p])
                    tbOut.Text += operation + " ";
                tbOut.Text += "}\n";
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

        public int[,] CreateAdjacencyMatrixToModel(List<List<string>> listOfModels, int[,] adjacencyMatrix, List<string> uniqueOperationInGroup)
        {
            int[,] adjacencyMatrixToModel = new int[listOfModels.Count, listOfModels.Count];
            //выбор модели
            for (int i = 0; i < listOfModels.Count; i++)
                //выбор операции
                for (int j = 0; j < listOfModels[i].Count; j++)
                {
                    int indexInAdjacencyMatrix = uniqueOperationInGroup.FindIndex(x => x == listOfModels[i][j]);
                    for (int iam = 0; iam < adjacencyMatrix.GetLength(0); iam++)
                    {
                        var currentUniqueValue = uniqueOperationInGroup[iam];
                        if (adjacencyMatrix[indexInAdjacencyMatrix, iam] == 1
                            && listOfModels[i].FindIndex(x => x == currentUniqueValue) == -1)
                        {
                            int indexModel = -1;
                            foreach (var model in listOfModels)
                                if (model.FindIndex(x => x == currentUniqueValue) != -1)
                                    indexModel = listOfModels.FindIndex(x => x == model);
                            adjacencyMatrixToModel[i, indexModel] = 1;
                        }
                    }

                }
            return adjacencyMatrixToModel;
        }

        public void MergeModel(ref List<List<string>> listOfModel, int indexToWrite, int indexToRemove)
        {
            while (listOfModel[indexToRemove].Count > 0)
            {
                listOfModel[indexToWrite].Add(listOfModel[indexToRemove].First());
                listOfModel[indexToRemove].Remove(listOfModel[indexToRemove].First());
            }
        }

        public bool CheckCircuit(ref List<List<string>> listOfModel, int[,] currentAndjancencyMatrix, List<int> usedIndexList, int mainIndex)
        {
            int indexOfParent = usedIndexList.Last();
            if (SumOfAllElementsInRow(currentAndjancencyMatrix, indexOfParent) > 1) return false;

            for (int i = 0; i < currentAndjancencyMatrix.GetLength(0); i++)
            {
                if (currentAndjancencyMatrix[indexOfParent, i] == 1)
                {
                    if (currentAndjancencyMatrix[mainIndex, i] == 1)
                    {
                        if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                        {
                            MergeModel(ref listOfModel, indexOfParent, i);
                            return true;
                        }
                    }
                    else if (usedIndexList.FindIndex(x => x == i) == -1)
                    {
                        usedIndexList.Add(i);
                        if (CheckCircuit(ref listOfModel, currentAndjancencyMatrix, usedIndexList, mainIndex))
                        {
                            if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                            {
                                MergeModel(ref listOfModel, indexOfParent, i);
                                return true;
                            }
                        }
                    }
                }

            }
            return false;
        }

        public bool CheckCirlce(ref List<List<string>> listOfModel, int[,] currentAndjancencyMatrix, List<int> usedIndexList, int mainIndex)
        {

            int indexOfParent = usedIndexList.Last();

            for (int i = 0; i < currentAndjancencyMatrix.GetLength(0); i++)
            {
                if (currentAndjancencyMatrix[indexOfParent, i] == 1)
                {
                    if (currentAndjancencyMatrix[i, mainIndex] == 1)
                    {
                        if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                        {
                            MergeModel(ref listOfModel, indexOfParent, i);
                            return true;
                        }
                    }
                    else if (usedIndexList.FindIndex(x => x == i) == -1)
                    {
                        usedIndexList.Add(i);
                        if (CheckCirlce(ref listOfModel, currentAndjancencyMatrix, usedIndexList, mainIndex))
                        {
                            if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                            {
                                MergeModel(ref listOfModel, indexOfParent, i);
                                return true;
                            }
                        }
                    }
                }

            }
            return false;
        }

        public bool FindInListOfAllModel(List<string> currentModel, List<List<string>> listOfAllModels)
        {
            foreach (var model in listOfAllModels)
            {
                int cntSameOperation = 0;
                foreach (var operation in currentModel)
                    if (model.FindIndex(x => x == operation) != -1)
                        cntSameOperation++;
                if (cntSameOperation == currentModel.Count)
                    return true;
            }
            return false;
        }

        public int FindOperationInListOfModel(string value, List<List<string>> listOfModels, int blockIndex)
        {
            for (int i = 0; i < listOfModels.Count; i++)
                if (listOfModels[i].FindIndex(x => x == value) != -1 && blockIndex != i)
                    return i;
            return -1;
        }
    }
}