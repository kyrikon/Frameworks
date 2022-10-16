using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Walkthrough
{
    interface IDemoModel
    {
        string StringValue { get; set; }
        int IntValue { get; set; }
        bool BoolValue { get; set; }
    }
    public class DemoModel : IDemoModel
    {
        public string StringValue { get; set; } = "A string";
        public int IntValue { get; set; } = 10;
        public bool BoolValue { get; set; } = false;
    }
}
