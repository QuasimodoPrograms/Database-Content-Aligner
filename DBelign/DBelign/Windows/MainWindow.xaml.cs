using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using QP = QP_Helpers.QP_Helpers;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string fullRegistryPath = @"HKEY_CURRENT_USER\Software\Quasimodo Programs\DBelign";
            string registryValueName = "KeY";

            string key = QP.GetRegister(fullRegistryPath, registryValueName);

            if (key != null && QP.IsLicensed(key, "DBelign.LicenseKeys.txt"))
                QP._isLicensed = true;
            else
            {
                QP._isLicensed = false;

                Window_Buy win = new Window_Buy();

                win.ShowDialog();
            }

            InitializeComponent();
        }

        private void btnOpenFile1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "All text files (*.docx;*.doc;*.xlsx;*.xls;*.txt;*.xml;*.cs)|*.docx;*.doc;*.xlsx;*.xls;*.txt;*.xml;*.cs"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string currentFile = openFileDialog.FileName;
                    string extension = Path.GetExtension(currentFile);

                    if (extension == ".docx" || extension == ".doc")
                        txtEditor1.Text = ReadDocxDoc(currentFile);
                    else if (extension == ".xlsx" || extension == ".xls")
                        txtEditor1.Text = ReadExcel(currentFile);
                    else txtEditor1.Text = File.ReadAllText(currentFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string ReadDocxDoc(string fileName)
        {
            Microsoft.Office.Interop.Word._Application wordObject = new Microsoft.Office.Interop.Word.Application();
            object file = fileName;
            object readOnly = true;
            object addToRecentFiles = false;

            Microsoft.Office.Interop.Word._Document doc = wordObject.Documents.Open(ref file, ReadOnly: ref readOnly, AddToRecentFiles: ref addToRecentFiles);

            string text = doc.Content.Text;

            object saveChanges = false;

            wordObject.Quit(SaveChanges: ref saveChanges);

            return text;
        }

        private string ReadExcel(string fileName)
        {
            Excel.Application xlApp = new Excel.Application();

            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(fileName, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range = xlWorkSheet.UsedRange;

            // Get whole document
            string str = string.Empty;

            for (int rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
            {
                for (int cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                    str += GetCellValue(xlWorkSheet, rCnt, cCnt) + " ";

                str += Environment.NewLine;
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            return str;
        }

        private string GetCellValue(Excel.Worksheet sheet, int row, int column) => sheet.Cells[row, column].Text.ToString();
    }
}
