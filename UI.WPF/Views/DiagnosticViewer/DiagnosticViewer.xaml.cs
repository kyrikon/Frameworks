using DevExpress.Spreadsheet;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PLEXOSCommon.Controls;
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

namespace PLEXOS.UI.Views.DiagnosticViewer
{
    /// <summary>
    /// Interaction logic for DataGridTest.xaml
    /// </summary>
    public partial class DiagnosticViewer : UserControl
    {
        public DiagnosticViewer()
        {
            InitializeComponent();

            DataContext = new DiagnosticViewerVM();

        }

        private void ObjView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(sender.GetType() == typeof(TableView))
            {
                TableView TView = (TableView)sender;
                if(TView.Grid.GetType() == typeof(ExtendedDXGridControl))
                {
                    // if inside the GridSearch then ignore
                    var isVP = PLEXOSCommon.Helpers.UIHelpers.FindVisualParent<GridSearchControl>((UIElement)e.OriginalSource);
                    if(isVP != null)
                        return;
                    ExtendedDXGridControl TGrid = (ExtendedDXGridControl)TView.Grid;
                    switch(e.Key)
                    {
                        case Key.Delete:
                        case Key.Back:
                            {
                                if(!TView.IsEditing && TView.ActiveEditor == null)
                                {
                                    DXTableViewDeleteCells(TView, false);
                                    e.Handled = true;
                                }

                                break;
                            }

                        case Key.Left:
                        case Key.Right:
                            {
                                if(TView.IsEditing && TView.ActiveEditor != null && Keyboard.Modifiers == 0)
                                {
                                    if(TView.ActiveEditor is TextEdit)
                                    {
                                        TextEdit getCurrTextEdit = (TextEdit)TView.ActiveEditor;
                                        if(e.OriginalSource is TextBox)
                                        {
                                            TextBox getTb = (TextBox)e.OriginalSource;
                                            if(getTb.SelectionLength == getTb.Text.Length)
                                                getTb.CaretIndex = getTb.Text.Length;
                                            if(e.Key == Key.Left)
                                            {
                                                if(getTb.CaretIndex > 0)
                                                    getTb.CaretIndex = getTb.CaretIndex - 1;
                                                else
                                                    getTb.CaretIndex = 0;
                                            }
                                            else if(e.Key == Key.Right)
                                            {
                                                if(getTb.CaretIndex < getTb.Text.Length - 1)
                                                    getTb.CaretIndex = getTb.CaretIndex + 1;
                                                else
                                                    getTb.CaretIndex = getTb.Text.Length;
                                            }
                                            e.Handled = true;
                                        }
                                    }
                                }
                                break;
                            }

                        case Key.Home:
                        case Key.End:
                            {
                                if(TView.IsEditing && TView.ActiveEditor != null && Keyboard.Modifiers == 0)
                                {
                                    if(TView.ActiveEditor is TextEdit)
                                    {
                                        TextEdit getCurrTextEdit = (TextEdit)TView.ActiveEditor;
                                        if(e.OriginalSource is TextBox)
                                        {
                                            TextBox getTb = (TextBox)e.OriginalSource;
                                            getTb.SelectionLength = 0;
                                            if(e.Key == Key.Home)
                                                getTb.CaretIndex = 0;
                                            else if(e.Key == Key.End)
                                                getTb.CaretIndex = getTb.Text.Length;
                                            e.Handled = true;
                                        }
                                    }
                                }

                                break;
                            }

                        case Key.Return:
                            {
                                //    if(TView.AllowEditing && TView.ActiveEditor != null)
                                //    {
                                //        GridColumn GetEditColumn = (GridColumn)TGrid.CurrentColumn;
                                //        Dispatcher.BeginInvoke(() =>
                                //        {
                                //            if(TView.FocusedRowHandle < 0)
                                //            {
                                //                TView.CommitEditing();
                                //                TView.Grid.CurrentColumn = GetEditColumn;
                                //                TView.FocusedRowHandle = TGrid.VisibleRowCount - 2;
                                //                TView.SelectCell(TView.FocusedRowHandle, (GridColumn)TView.Grid.CurrentColumn);
                                //                e.Handled = true;
                                //                return;
                                //            }
                                //            TView.CommitEditing();
                                //            TView.UnselectCell(TView.FocusedRowHandle, (GridColumn)TView.Grid.CurrentColumn);
                                //            TView.MoveNextRow();
                                //            if(TView.FocusedRowHandle > 0)
                                //                TView.SelectCell(TView.FocusedRowHandle, (GridColumn)TView.Grid.CurrentColumn);
                                //            e.Handled = true;
                                //        },new { });
                                //    }
                                //    else
                                //    {
                                //        TView.UnselectCell(TView.FocusedRowHandle, (GridColumn)TView.Grid.CurrentColumn);
                                //        TView.MoveNextRow();
                                //        TView.SelectCell(TView.FocusedRowHandle, (GridColumn)TView.Grid.CurrentColumn);
                                //        e.Handled = true;
                                //    }

                                break;
                            }

                        case Key.Up:
                        case Key.Down:
                            {
                                if(TView.AllowEditing && TView.ActiveEditor != null)
                                {
                                    if(TView.ActiveEditor is ComboBoxEdit)
                                    {
                                        ComboBoxEdit CBE = (ComboBoxEdit)TView.ActiveEditor;
                                        if(e.Key == Key.Down)
                                            CBE.SpinDown();
                                        else
                                            CBE.SpinUp();
                                        e.Handled = true;
                                    }
                                    else if(TView.ActiveEditor is DateEdit)
                                    {
                                        DateEdit DTE = (DateEdit)TView.ActiveEditor;
                                        if(e.Key == Key.Down)
                                            DTE.SpinDown();
                                        else
                                            DTE.SpinUp();
                                        e.Handled = true;
                                    }
                                    else if(TView.ActiveEditor is TextEdit)
                                    {
                                        if(TView.ActiveEditor.EditCore is ComboBox)
                                        {
                                            ComboBox WCBE = (ComboBox)TView.ActiveEditor.EditCore;
                                            if(e.Key == Key.Down)
                                            {
                                                if(WCBE.SelectedIndex < WCBE.Items.Count)
                                                    WCBE.SelectedIndex = WCBE.SelectedIndex + 1;
                                            }
                                            else if(WCBE.SelectedIndex > -1)
                                                WCBE.SelectedIndex = WCBE.SelectedIndex - 1;
                                        }
                                        e.Handled = true;
                                    }
                                }

                                break;
                            }

                        case Key.Escape:
                            {
                                if(TView.ActiveEditor != null)
                                {
                                    if(!TView.HasValidationError)
                                    {
                                        //Dispatcher.BeginInvoke(() =>
                                        //{
                                        //    if(TView.FocusedRowHandle < 0)
                                        //        TView.CancelRowEdit();
                                        //});
                                    }
                                    if(TView.ActiveEditor is DateEdit)
                                        TView.CancelRowEdit();
                                }

                                break;
                            }

                        case Key.OemQuotes:
                            {
                                if(Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && TGrid != null && !TView.IsEditing)
                                    CopyAboveCell(TGrid);
                                break;
                            }

                    }

                    if(!TView.IsEditing)
                    {
                        if(TGrid.CurrentColumn != null && TGrid.CurrentColumn.EditSettings != null && TGrid.CurrentColumn.EditSettings.Tag != null && TGrid.CurrentColumn.EditSettings.Tag.ToString() == "Date")
                        {
                            if(((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
                                TView.ShowEditor();
                            else
                                switch(e.Key)
                                {
                                    case Key.Left:
                                    case Key.Right:
                                    case Key.Home:
                                    case Key.End:
                                    case Key.Enter:
                                    case Key.Escape:
                                    case Key.Delete:
                                    case Key.Back:
                                    case Key.Up:
                                    case Key.Down:
                                        {
                                            break;
                                        }

                                    default:
                                        {
                                            e.Handled = true;
                                            break;
                                        }
                                }
                        }
                    }
                }
            }
        }

        private void CopyAboveCell(ExtendedDXGridControl grid)
        {
            throw new NotImplementedException();
        }

        private void DXTableViewDeleteCells(TableView view, bool v)
        {
            throw new NotImplementedException();
        }
    }
}

