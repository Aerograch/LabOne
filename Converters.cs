using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using ClosedXML.Excel;
using System.Windows.Shapes;
using System.Xml.Xsl;
using System.Xml;

namespace LabOne
{
    public interface IConverterOptions
    {
        ConverterType Type { get; }
        object? BaseOptions { get; }
    }

    public class UnspecifiedConverterOptions : IConverterOptions
    {
        public ConverterType Type { get; private set; }
        public object? BaseOptions { get; private set; }
        public UnspecifiedConverterOptions()
        {
            Type = ConverterType.None;
            BaseOptions = null;
        }
    }

    public interface IConverter
    {
        ConverterType Type { get; }
        public List<Tool> ReadFromFile(string filePath);
        public void WriteToFile(string filePath, List<Tool> tools, IConverterOptions options);
    }

    public enum ConverterType
    {
        None = 0,
        Json = 1,
        Xml = 2,
        Csv = 3,
        Xlsx = 4
    }

    public class JSONConverterOptions : IConverterOptions
    {
        public ConverterType Type { get; private set; }
        public object? BaseOptions { get; private set; }
        public JSONConverterOptions(object? baseOptions)
        {
            Type = ConverterType.Json;
            BaseOptions = baseOptions;
        }
    }

    public class JSONConverter : IConverter
    {
        public ConverterType Type => ConverterType.Json;

        public List<Tool> ReadFromFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string json = Encoding.UTF8.GetString(buffer);
                return JsonSerializer.Deserialize(json, typeof(IEnumerable<Tool>)) as List<Tool>;
            }
        }

        public void WriteToFile(string filePath, List<Tool> tools, IConverterOptions options)
        {
            JsonSerializerOptions serializerOptions = options.BaseOptions as JsonSerializerOptions;
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tools, serializerOptions));
                fs.Write(buffer, 0, buffer.Length);
            }
        }
    }

    public class XMLConverterOptions : IConverterOptions
    {
        public ConverterType Type { get; private set; }
        public object? BaseOptions { get; private set; }
        public bool AddA { get; private set; }

        public XMLConverterOptions(bool addA)
        {
            Type = ConverterType.Xml;
            BaseOptions = null;
            AddA = addA;
        }
    }

    public class XMLConverter : IConverter
    {
        public ConverterType Type => ConverterType.Xml;

        public List<Tool> ReadFromFile(string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Tool>));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return xmlSerializer.Deserialize(fs) as List<Tool>;
            }
        }

        public void WriteToFile(string filePath, List<Tool> tools, IConverterOptions options)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Tool>));
            List<string> fp = filePath.Split('.').ToList();
            if (!(options as XMLConverterOptions).AddA) filePath = fp[0] + "a." + fp[1];
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings() 
                {
                    Indent = true
                };
                using (XmlWriter writer = XmlWriter.Create(fs, writerSettings))
                xmlSerializer.Serialize(writer, tools);
            }
        }
    }

    public class CSVConverterOptions : IConverterOptions
    {
        public ConverterType Type { get; private set; }
        public object? BaseOptions { get; private set; }
        public bool AddB { get; private set; }

        public CSVConverterOptions(bool addB)
        {
            Type = ConverterType.Csv;
            BaseOptions = null;
            AddB = addB;
        }
    }

    public class CSVConverter : IConverter
    {
        public ConverterType Type => ConverterType.Csv;

        public List<Tool> ReadFromFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                List<Tool> output = new List<Tool>();
                foreach (string s in Encoding.Default.GetString(buffer).Split('\n'))
                {
                    output.Add(new Tool(s.Split(',').ToList()));
                }
                return output;
            }
        }

        public void WriteToFile(string filePath, List<Tool> tools, IConverterOptions options)
        {
            List<string> fp = filePath.Split('.').ToList();
            if (!(options as CSVConverterOptions).AddB) filePath = fp[0] + "b." + fp[1];
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                string stringBuffer = "";
                for (int i = 0; i < tools.Count; i++)
                {
                    Tool t = tools[i];
                    stringBuffer += $"{t.Name},{t.Description},{t.Type}";
                    if (i != tools.Count - 1)
                    {
                        stringBuffer += '\n';
                    }
                }
                byte[] buffer = Encoding.Default.GetBytes(stringBuffer);
                fs.Write(buffer, 0, buffer.Length);
            }
        }
    }

    public class XLSXConverterOptions : IConverterOptions
    {
        public ConverterType Type { get; private set; }
        public object? BaseOptions { get; private set; }
        public bool AddC { get; private set; }

        public XLSXConverterOptions(bool addC)
        {
            Type = ConverterType.Xlsx;
            BaseOptions = null;
            AddC = addC;
        }
    }

    public class XLSXConverter : IConverter
    {
        public ConverterType Type => ConverterType.Xlsx;

        public List<Tool> ReadFromFile(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                XLWorkbook wb = new XLWorkbook(fs);
                IXLWorksheet ws = wb.Worksheet("Sheet 1");
                List<Tool> output = new List<Tool>();
                for (int i = 1; i <= ws.LastRowUsed().RowNumber(); i++)
                {
                    Tool t = new Tool(
                        ws.Cell(i, 1).Value.GetText(),
                        ws.Cell(i, 2).Value.GetText(),
                        ws.Cell(i, 3).Value.GetText());
                    output.Add(t);
                }
                return output;
            }
        }

        public void WriteToFile(string filePath, List<Tool> tools, IConverterOptions options)
        {
            XLWorkbook wb = new XLWorkbook();
            IXLWorksheet ws = wb.AddWorksheet("Sheet 1");
            for (int i = 0; i < tools.Count; i++)
            {
                Tool t = tools[i];
                ws.Cell(i + 1, 1).Value = t.Name;
                ws.Cell(i + 1, 2).Value = t.Description;
                ws.Cell(i + 1, 3).Value = t.Type;
            }
            List<string> fp = filePath.Split('.').ToList();
            if (!(options as XLSXConverterOptions).AddC) filePath = fp[0] + "c." + fp[1];
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                wb.SaveAs(fs);
            }
        }
    }
}
