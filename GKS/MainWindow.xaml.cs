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
                    if (listOfUnequeOperations.FindIndex(x=>x == s) == -1)
                    {
                        listOfUnequeOperations.Add(s);
                    }
                }
                iter++;
            }



            #region OutInTextBlock
            tbOut.Text = listOfUnequeOperations.Count().ToString()+"\n";
            for (int i = 0; i < cntFields; i++)
            {
                foreach(int value in details[i])
                {
                    tbOut.Text += value + " ";
                }
                tbOut.Text += "\n";
            }
            #endregion

        }
    }
}
