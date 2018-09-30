using System;
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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void DrawNewFields(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Test");
            try
            {
                spValue.Children.Clear();
                int cntFields = Convert.ToInt32(tbCntFields.Text);

                this.Height = 140 + cntFields * 30;
                for (int i = 0; i < cntFields; i++)
                {
                    TextBox tb = new TextBox();
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    tb.Margin = new Thickness(5);
                    tb.MinWidth = 200;
                    spValue.Children.Add(tb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        public void Calc(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Test");
            int cntFields = Convert.ToInt32(tbCntFields.Text);

            List<int>[] details = new List<int>[cntFields];
            for (int i = 0; i < cntFields; i++)
                details[i] = new List<int>();
            List<string> listOfOperaions = new List<string>();
            foreach (Object o in spValue.Children)
            {
                TextBox tb = (TextBox)o;
                listOfOperaions.Add(tb.Text);
            }

            List<string> listOfUnequeOperations = new List<string>();

            int iter = 0;
            foreach (string strOfOperation in listOfOperaions)
            {
                string[] words = strOfOperation.Split(new char[] { ' ' });
                foreach (string s in words)
                {
                    switch (s)
                    {
                        case "T1":
                            details[iter].Add(11);
                            break;
                        case "T2":
                            details[iter].Add(12);
                            break;
                        case "T3":
                            details[iter].Add(13);
                            break;
                        case "T4":
                            details[iter].Add(14);
                            break;

                        case "C1":
                            details[iter].Add(21);
                            break;
                        case "C2":
                            details[iter].Add(22);
                            break;
                        case "C3":
                            details[iter].Add(23);
                            break;
                        case "C4":
                            details[iter].Add(24);
                            break;

                        case "P1":
                            details[iter].Add(31);
                            break;
                        case "P2":
                            details[iter].Add(32);
                            break;
                        case "P3":
                            details[iter].Add(33);
                            break;
                        case "P4":
                            details[iter].Add(34);
                            break;

                        case "F1":
                            details[iter].Add(41);
                            break;
                        case "F2":
                            details[iter].Add(42);
                            break;
                        case "F3":
                            details[iter].Add(43);
                            break;
                        case "F4":
                            details[iter].Add(44);
                            break;
                    }
                    if (listOfUnequeOperations.FindIndex(x => x == s) == -1)
                    {
                        listOfUnequeOperations.Add(s);
                    }
                }
                iter++;
            }

            #region OutInTextBlock


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
            tbOut.Text += "Count of unique elements: " + listOfUnequeOperations.Count().ToString() + "\n";
            #endregion

            #region CreateHelpMatrix
            int[,] helpMatrix = new int[cntFields, cntFields];

            for (int i = 1; i < cntFields; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    int cntDifElements = 0;
                    foreach (int item in details[j])
                    {
                        if (details[i].FindIndex(x => x == item) == -1)
                        {
                            cntDifElements++;
                        }
                    }
                    foreach (int item in details[i])
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
                                             {3,5,4,5,0 } };*/

            #region OutputHelpMatrix

            tbOut.Text += "Help matrix:\n";
            for (int i = 0; i < cntFields; i++)
            {
                for (int j = 0; j < cntFields; j++)
                {
                    tbOut.Text += helpMatrix[i, j] + " ";
                }
                tbOut.Text += "\n";
            }
            
            #endregion
            

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
                            (maxJ.FindIndex(x => x == j) != -1)
                            ))
                        {
                            maxI.Add(i);
                            maxJ.Add(j);
                        }
                    }
                }

                //output help matrix
                tbOut.Text += "Help matrix:\n";
                for (int i = 0; i < cntFields; i++)
                {
                    for (int j = 0; j < cntFields; j++)
                    {
                        tbOut.Text += helpMatrix[i, j] + " ";
                    }
                    tbOut.Text += "\n";
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

            #endregion

            #region OutputGroups
            tbOut.Text += "\n Groups:\n";
            for (int i = 0; i < groups.Length - 1; i++)
            {
                tbOut.Text += "\n " + i + " - { ";
                foreach (var item in groups[i])
                {
                    tbOut.Text += (item + 1) + " ";
                }
                tbOut.Text += "}";
            }
            #endregion

            #region OutInTextBlock


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
            tbOut.Text += "Count of unique elements: " + listOfUnequeOperations.Count().ToString() + "\n";
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
    }
}
