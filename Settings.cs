using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabOne
{
    public struct Settings
    {
        public ConverterType FirstConverter { get; set; }
        public ConverterType SecondConverter { get; set; }
        public bool PropertyBool { get; set; }
        public string OriginFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public Settings()
        {
            FirstConverter = ConverterType.Json;
            SecondConverter = ConverterType.Json;
            PropertyBool = true;
            OriginFilePath = "anExampe.json";
            TargetFilePath = "";
        }
    }
}
