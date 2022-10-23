using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MVVM_Walkthrough
{
    interface IDemoModel
    {
        string StringValue { get; set; }
        int IntValue { get; set; }
        int RsltValue { get; set; }
        
        bool BoolValue { get; set; }
        bool BoolValue2 { get; set; }
        void calculateFib();
    }
    internal sealed class DemoModel : NotifyPropertyChanged, IDemoModel
    {
        #region CTor
        public DemoModel()
        {
            StringValue = "A String";
            IntValue = 0;
            BoolValue = false;
        }
        #endregion
        #region Properties
        public string StringValue
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
        public int IntValue
        {
            get
            {
                return GetPropertyValue<int>();
            }
            set
            {
                SetPropertyValue<int>(value);
            }
        }
        public int RsltValue
        {
            get
            {
                return GetPropertyValue<int>();
            }
            set
            {
                SetPropertyValue<int>(value);
            }
        }
        public bool BoolValue
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue<bool>(value);
                base.OnPropertyChanged(nameof(BGColour));
            }
        }
        public bool BoolValue2
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue<bool>(value);
                base.OnPropertyChanged(nameof(BGColour));
            }
        }
        public SolidColorBrush BGColour
        {
            get
            {
               if(BoolValue && BoolValue2)
                {
                    return Brushes.Yellow;
                }
                return Brushes.Green;
            }
          
        }
        public void calculateFib()
        {

            //row 1 -1
            //row 2 - 2 -3
            //row 3 - 3 - 6
            //row 
            int numvalues = 0;
            int numRows = IntValue;
            for (int i = 0; i <= numRows;i++)
            {
                numvalues += i;
            }
            RsltValue = 0;

            int number = numvalues - 1; //Need to decrement by 1 since we are starting from 0  
            int[] Fib = new int[number + 1];
            Fib[0] = 0;
            Fib[1] = 1;
            for (int i = 2; i <= number; i++)
            {
                Fib[i] = Fib[i - 2] + Fib[i - 1];
            }
            for(int i = Fib.Length - numRows;i < Fib.Length; i++)
            {
                 RsltValue += Fib[i];
            }           
            
        }
        #endregion
    }
}
