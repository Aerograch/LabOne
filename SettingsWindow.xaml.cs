using Microsoft.Win32;
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
using System.Windows.Shapes;
using LabOne;

namespace LabOne
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SaveFileDialog save = new SaveFileDialog();
        public SettingsWindow()
        {
            InitializeComponent();
            save.Filter = "Json files(*.json)|*.json|Xml files(*.xml)|*.xml|Csv files(*.csv)|*.csv|Excel tabels(*.xlsx)|*.xlsx";
            baseFormatComboBox.SelectedIndex = (int)MainWindow.settings.FirstConverter - 1;
            newFormatComboBox.SelectedIndex = (int)MainWindow.settings.SecondConverter - 1;
            boolComboBox.SelectedIndex = Convert.ToInt32(MainWindow.settings.PropertyBool);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void baseFormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = baseFormatComboBox.SelectedIndex;
            switch (id)
            {
                case 0:
                    MainWindow.settings.FirstConverter = ConverterType.Json;
                    break;
                case 1:
                    MainWindow.settings.FirstConverter = ConverterType.Xml;
                    break;
                case 2:
                    MainWindow.settings.FirstConverter = ConverterType.Csv;
                    break;
                case 3:
                    MainWindow.settings.FirstConverter = ConverterType.Xlsx;
                    break;
            }
        }

        private void newFormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = newFormatComboBox.SelectedIndex;
            switch (id)
            {
                case 0:
                    MainWindow.settings.SecondConverter = ConverterType.Json;
                    specificPropLabel.Content = "Красивый вывод?";
                    break;
                case 1:
                    MainWindow.settings.SecondConverter = ConverterType.Xml;
                    specificPropLabel.Content = "Добавить букву a?";
                    break;
                case 2:
                    MainWindow.settings.SecondConverter = ConverterType.Csv;
                    specificPropLabel.Content = "Добавить букву b?";
                    break;
                case 3:
                    MainWindow.settings.SecondConverter = ConverterType.Xlsx;
                    specificPropLabel.Content = "Добавить букву c?";
                    break;
            }
        }

        private void boolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = boolComboBox.SelectedIndex;
            switch (id)
            {
                case 0:
                    MainWindow.settings.PropertyBool = false;
                    break;
                case 1:
                    MainWindow.settings.PropertyBool = true;
                    break;
            }
        }

        private void choseSavePathButton_Click(object sender, RoutedEventArgs e)
        {
            if (save.ShowDialog() != true)
            {
                return;
            }
            MainWindow.settings.TargetFilePath = save.FileName;
        }

        /*private void fileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.TargetFileName = fileNameTextBox.Text;
        }*/
    }
}
