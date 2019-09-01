using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pyramids
{
    /// <summary>
    /// Logika interakcji dla klasy PagePyramidGenerator.xaml
    /// </summary>
    public partial class PagePyramidGenerator : Page
    {
        MainWindow parent;
        int size;
        Control[,] listOfControls;
        PyramidForGenerate pyramid;
        bool showBoard = true;
        public PagePyramidGenerator(MainWindow parent)
        {          
            InitializeComponent();            
            size = 0;
            ButtonIncrease_Click(null, null);
            this.parent = parent;
        }


        private void ButtonDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (size > 1)
            {
                size--;
                RedrawPanel();
                UpdateLabel();
                pyramid = new PyramidForGenerate(size);
                UpdateBoard();
            }
        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            size++;
            RedrawPanel();
            UpdateLabel();
            pyramid = new PyramidForGenerate(size);
            UpdateBoard();

        }

        private void UpdateLabel()
        {
            labelSize.Content = size.ToString() + " x " + size.ToString();
        }

        private void RedrawPanel()
        {
            gridMainPanel.Children.Clear();
            gridMainPanel.ColumnDefinitions.Clear();
            gridMainPanel.RowDefinitions.Clear();
            listOfControls = new Control[size + 2, size + 2];

            for (int i = 0; i <= size + 1; i++)
            {
                gridMainPanel.RowDefinitions.Add(new RowDefinition());
                gridMainPanel.ColumnDefinitions.Add(new ColumnDefinition());
                if (i == 0 || i == size + 1)
                {
                    for (int j = 1; j <= size; j++)
                    {
                        Button b = new Button();
                        b.Click += RemoveRestoreNumber;
                        Grid.SetColumn(b, j);
                        Grid.SetRow(b, i);
                        gridMainPanel.Children.Add(b);
                        listOfControls[i, j] = b;
                    }
                }
                else
                {
                    for (int j = 0; j <= size + 1; j++)
                    {
                        if (j == 0 || j == size + 1)
                        {
                            Button b = new Button();
                            b.Click += RemoveRestoreNumber;
                            Grid.SetColumn(b, j);
                            Grid.SetRow(b, i);
                            gridMainPanel.Children.Add(b);
                            listOfControls[i, j] = b;
                        }
                        else
                        {
                            Label l = new Label();
                            Grid.SetColumn(l, j);
                            Grid.SetRow(l, i);
                            l.BorderBrush = Brushes.Black;
                            l.BorderThickness = new Thickness(0, 0, 1, 1);
                            l.HorizontalContentAlignment = HorizontalAlignment.Center;
                            l.VerticalContentAlignment = VerticalAlignment.Center;
                            l.Content = " ";
                            gridMainPanel.Children.Add(l);
                            listOfControls[i, j] = l;
                        }
                    }
                }
            }
        }


        private void UpdateBoard()
        {

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        ((Label)listOfControls[i + 1, j + 1]).Content = showBoard==true? pyramid.GetField(i, j).ToString() : null;
                    }
                }


                for (int i = 0; i < size; i++)
                {
                    ((Button)listOfControls[0, i + 1]).Content = pyramid.GetNumber(i, 0) == 0 ? null : pyramid.GetNumber(i, 0).ToString();
                    ((Button)listOfControls[i + 1, size + 1]).Content = pyramid.GetNumber(i, 1) == 0 ? null : pyramid.GetNumber(i, 1).ToString();
                    ((Button)listOfControls[size + 1, i + 1]).Content = pyramid.GetNumber(i, 2) == 0 ? null : pyramid.GetNumber(i, 2).ToString();
                    ((Button)listOfControls[i + 1, 0]).Content = pyramid.GetNumber(i, 3) == 0 ? null : pyramid.GetNumber(i, 3).ToString();

                }
            
        }



        private void ButtonIncreaseNumbersSize_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control c in listOfControls)
            {
                if (c != null)
                {
                    c.FontSize += 2;
                }
            }
        }

        private void ButtonDecreasNumbersSize_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control c in listOfControls)
            {
                if (c != null)
                {
                    if (c.FontSize > 3)
                        c.FontSize -= 2;
                }
            }
           
        }

        private void ButtonWhatArePyramids_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.wydawnictwologi.pl/piramida");
        }

        private void ButtonChangeToSolver_Click(object sender, RoutedEventArgs e)
        {
            parent.Change();
        }

        private void ButtonBackPermutate_Click(object sender, RoutedEventArgs e)
        {
            pyramid.BackLastPermutation();
            UpdateBoard();
        }

        private void ButtonBackNumber_Click(object sender, RoutedEventArgs e)
        {
            pyramid.RestorDeletedNumber();
            UpdateBoard();
        }

        private void ButtonPermutateCol_Click(object sender, RoutedEventArgs e)
        {
            pyramid.Permutate(true);
            UpdateBoard();
        }

        private void ButtonPermutateRow_Click(object sender, RoutedEventArgs e)
        {
            pyramid.Permutate(false);
            UpdateBoard();
        }

        private void ButtonDeleteNumber_Click(object sender, RoutedEventArgs e)
        {
            pyramid.DeleteRandomNumber();
            UpdateBoard();
        }

        private void RemoveRestoreNumber(object sender, RoutedEventArgs e)
        {
           
            int y = Grid.GetColumn((Button)sender);
            int x = Grid.GetRow((Button)sender);
            if (x == 0 || x == size + 1)
            {
                if (((Button)sender).Content != null)
                {
                    pyramid.RemoveNumber(y - 1, x == 0 ? 0 : 2);
                }
                else
                {
                    pyramid.RestoreNumber(y - 1, x == 0 ? 0 : 2);
                }     
                UpdateBoard();
            }
            else
            {
                if (((Button)sender).Content != null)
                {
                    pyramid.RemoveNumber(x - 1, y == 0 ? 3 : 1);
                }
                else
                {
                    pyramid.RestoreNumber(x - 1, y == 0 ? 3 : 1);
                }
                UpdateBoard();
            }

        }

        private void ButtonClearRememberedPermutations_Click(object sender, RoutedEventArgs e)
        {
            pyramid.ClearListOfOperations();
        }



        private void CheckboxShowBoard_Click_1(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                showBoard = true;
            }
            else
            {
                showBoard = false;
            }
            
            UpdateBoard();
        }

        private void ButtonChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            parent.ChangeStyle();
        }
    }
}
