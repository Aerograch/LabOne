using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

namespace LabOne
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Settings settings;
        IConverterOptions options;
        OpenFileDialog open = new OpenFileDialog();
        List<IConverter> converters = new List<IConverter>();
        public MainWindow()
        {
            InitializeComponent();
            open.Filter = "Json files(*.json)|*.json|Xml files(*.xml)|*.xml|Csv files(*.csv)|*.csv|Excel tabels(*.xlsx)|*.xlsx";
            converters.Add(new JSONConverter());
            converters.Add(new XMLConverter());
            converters.Add(new CSVConverter());
            converters.Add(new XLSXConverter());
            settings = new Settings();
        }

        private void ChoseFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (open.ShowDialog() == false)
            {
                return;
            }
            settings.OriginFilePath = open.FileName;
            fileNameLabel.Content = open.SafeFileName;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new SettingsWindow();
            window.ShowDialog();
        }

        private void ConvertFileButton_Click(object sender, RoutedEventArgs e)
        {
            switch (settings.SecondConverter)
            {
                case ConverterType.Json:
                    options = new JSONConverterOptions(
                        new JsonSerializerOptions() 
                        {
                            WriteIndented = !settings.PropertyBool
                        });
                    break;
                case ConverterType.Xml:
                    options = new XMLConverterOptions(settings.PropertyBool);
                    break;
                case ConverterType.Csv:
                    options = new CSVConverterOptions(settings.PropertyBool);
                    break;
                case ConverterType.Xlsx:
                    options = new XLSXConverterOptions(settings.PropertyBool);
                    break;
            }
            List<Tool> tools;
            try
            {
                tools = converters.Find(x => x.Type == settings.FirstConverter).ReadFromFile(settings.OriginFilePath);
                converters.Find(x => x.Type == settings.SecondConverter).WriteToFile(settings.TargetFilePath, tools, options);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Выбранный файл не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось загрузить данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Файл конвертировался!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
