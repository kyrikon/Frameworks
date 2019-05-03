using DataInterface;
using DataInterface.Extensions;
using DevExpress.Xpf.Editors;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI.WPF.ResourceDictionaries
{
    public partial class EditorDataTemplates : ResourceDictionary
    {
        public EditorDataTemplates()
        {
            InitializeComponent();
        }

        private void TextEdit_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {

        }        
    }
}
