using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using QP = QP_Helpers.QP_Helpers;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window_DataSources : Window
    {
        #region Private members

        /// <summary>
        /// If the program is not licensed, this is the max number of entries to work at a time
        /// </summary>
        private const int mLimitMaxEntryCount = 100;

        /// <summary>
        /// This window's opacity when another window is opened to make fading effect
        /// </summary>
        private const double mDarkOpacity = 0.5;

        #endregion

        public Window_DataSources()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // If the program is registered...
            if (QP._isLicensed)
                // Hide the register button
                btn_Register.Visibility = Visibility.Collapsed;
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            // Darken this window
            Opacity = mDarkOpacity;

            // Initialize Window_Buy
            Window_Buy win = new Window_Buy()
            {
                Owner = this,
                ShowInTaskbar = false
            };

            // Show Window_Buy
            win.ShowDialog();

            // Remove darkness from this window
            Opacity = 1;

            // Show this window in Taskbar
            ShowInTaskbar = true;
        }

        private void btnOpenFile1_Click(object sender, RoutedEventArgs e)
        {
            // Open a file and read its content into the source textbox
            OpenFile(txtEditor1);
        }

        private void btnOpenFile2_Click(object sender, RoutedEventArgs e)
        {
            // Open a file and read its content into the target textbox
            OpenFile(txtEditor2);
        }

        /// <summary>
        /// Open a file and read its content into the specified textbox
        /// </summary>
        /// <param name="textEditor">TextBox to read the text to</param>
        private void OpenFile(TextBox textEditor)
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
                        textEditor.Text = ReadDocxDoc(currentFile);
                    else if (extension == ".xlsx" || extension == ".xls")
                        textEditor.Text = ReadExcel(currentFile);
                    else textEditor.Text = File.ReadAllText(currentFile);
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



        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the source or target TextBox contains no text...
            if (QP.ClearBlankCharacters(txtEditor1.Text) == string.Empty || QP.ClearBlankCharacters(txtEditor2.Text) == string.Empty)
                // Disable the button to align texts
                btn_AlignTwoTexts.IsEnabled = false;
            // If source and target TextBoxes contain text...
            else
                // Enable the button to align texts
                btn_AlignTwoTexts.IsEnabled = true;

            // If the source TextBox does not contain text...
            if (QP.ClearBlankCharacters(txtEditor1.Text) == string.Empty)
                // Disable the source menu
                menu_source.IsEnabled = false;
            // If the source TextBox contains text...
            else
                // Enable the source menu
                menu_source.IsEnabled = true;

            // If the target TextBox does not contain text...
            if (QP.ClearBlankCharacters(txtEditor2.Text) == string.Empty)
                // Disable the target menu
                menu_target.IsEnabled = false;
            // If the target TextBox contains text...
            else
                // Enable the target menu
                menu_target.IsEnabled = true;
        }


        /// <summary>
        /// Launch the update program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Try to stasrt the update program
                Process.Start("update.exe", "/checknow");
            }
            // If there is an error...
            catch (Exception ex)
            {
                // Show the error message
                MessageBox.Show($"{ ex.Message }{ Environment.NewLine }The product's page will be opened.");

                // Open the product page
                Process.Start("https://www.youtube.com/c/QuasimodoPrograms");
            }
        }

        private void sourceItem_AlignAsTableSingleRow_Click(object sender, RoutedEventArgs e)
        {
            // Align text fragments of the text in the source textbox as a table which consists of 2 columns and 1 row
            AlignAsTableSingleRow(txtEditor1.Text);
        }

        /// <summary>
        /// Align text fragments of the text in the 1 textbox as a table which consists of 2 columns and 1 row
        /// </summary>
        /// <param name="text">Text in a TextBox</param>
        private void AlignAsTableSingleRow(string text)
        {
            // A list of source fragments
            List<string> sourceList = new List<string>();

            // A list of target fragments
            List<string> targetList = new List<string>();

            // Get tables in text
            string[] tables = text.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries);

            // Iterate through all tables
            foreach (string table in tables)
            {
                // If the table is empty...
                if (table == "\r")
                    // Skip
                    continue;

                // Get columns from the table
                string[] columns = table.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);

                // Get the source text from the source column
                string sourceText = columns[0].Replace("", string.Empty);

                // Get source fragments from the source text
                string[] sourceFragments = sourceText.Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                // Add source fragments to the list
                sourceList.AddRange(sourceFragments);

                // If the second column exists...
                if (columns.Length > 1)
                {
                    // Get target text from the second column
                    string targetText = columns[1].Replace("", string.Empty);

                    // Get target fragments from the target text
                    string[] targetFragments = targetText.Split(new string[] { Environment.NewLine, "\n", "\r" },
                        StringSplitOptions.RemoveEmptyEntries);

                    // Add target fragments to the list
                    targetList.AddRange(targetFragments);
                }
            }

            // If the program is licensed...
            if (QP._isLicensed)
                // Pass all fragments
                Window_AlignFragmentsManager.ClosePreviousAndShow(sourceList.ToArray(), targetList.ToArray(), this);
            // If the program is not licensed...
            else
                // Pass only first MAX fragments
                Window_AlignFragmentsManager.ClosePreviousAndShow(sourceList.Take(mLimitMaxEntryCount).ToArray(),
                    targetList.Take(mLimitMaxEntryCount).ToArray(), this);
        }

        /// <summary>
        /// Align text fragments of the text in the 1 textbox as a table which consists of 2 columns and multiple rows
        /// </summary>
        /// <param name="text">Text in a TextBox</param>
        private void AlignAsTableMultipleRows(string text)
        {
            // A list of source fragments
            List<string> sourceList = new List<string>();

            // A list of target fragments
            List<string> targetList = new List<string>();

            // Get rows in the table
            string[] rows = text.Split(new string[] { "\r\r" }, StringSplitOptions.RemoveEmptyEntries);

            // Iterate through all rows
            foreach (string row in rows)
            {
                // If the row is empty...
                if (row == "\r")
                    // Skip
                    continue;

                // Get cells from the row
                string[] cells = row.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);

                // Add the left cell to the source fragments list
                sourceList.Add(cells[0]);

                // If the second column exists...
                if (cells.Length > 1)
                    // Add the right cell to the target fragments list
                    targetList.Add(cells[1]);
            }

            // If the program is licensed...
            if (QP._isLicensed)
                // Pass all fragments
                Window_AlignFragmentsManager.ClosePreviousAndShow(sourceList.ToArray(), targetList.ToArray(), this);
            // If the program is not licensed...
            else
                // Pass only first MAX fragments
                Window_AlignFragmentsManager.ClosePreviousAndShow(sourceList.Take(mLimitMaxEntryCount).ToArray(),
                    targetList.Take(mLimitMaxEntryCount).ToArray(), this);
        }

        private void sourceItem_AlignAsTableMultipleRows_Click(object sender, RoutedEventArgs e)
        {
            // Align text fragments of the text in the source textbox as a table which consists of 2 columns and multiple rows
            AlignAsTableMultipleRows(txtEditor1.Text);
        }

        private void btn_ClearSource_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_AlignTwoTexts_Click(object sender, RoutedEventArgs e)
        {
            // Get the source text cleared from Word table markers
            string sourceText = txtEditor1.Text.Replace("", string.Empty);

            // Get the target text cleared from Word table markers
            string targetText = txtEditor2.Text.Replace("", string.Empty);

            // An array for source fragments
            string[] sourceFragments;

            // An array for target fragments
            string[] targetFragments;

            // If we align by paragraphs...
            if (comboBox_Separator.SelectedIndex == 0)
            {
                // Split the source text by new lines
                sourceFragments = sourceText.Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                // Split the target text by new lines
                targetFragments = targetText.Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            }
            // If we align by sentences...
            else if (comboBox_Separator.SelectedIndex == 1)
            {
                // Split the source text by new lines, periods, exclamation and question marks
                sourceFragments = sourceText.Split(new string[] { Environment.NewLine, "\n", "\r", ".", "!", "?" },
                    StringSplitOptions.RemoveEmptyEntries);

                // Split the target text by new lines, periods, exclamation and question marks
                targetFragments = targetText.Split(new string[] { Environment.NewLine, "\n", "\r", ".", "!", "?" },
                    StringSplitOptions.RemoveEmptyEntries);
            }
            // If we align by words...
            else
            {
                // Split the source text by new lines and whitespaces
                sourceFragments = sourceText.Split(new string[] { Environment.NewLine, "\n", "\r", " " },
                    StringSplitOptions.RemoveEmptyEntries);

                // Split the target text by new lines and whitespaces
                targetFragments = targetText.Split(new string[] { Environment.NewLine, "\n", "\r", " " },
                    StringSplitOptions.RemoveEmptyEntries);
            }

            // Remove source fragments that are empty or whitespace
            sourceFragments = sourceFragments.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            // Remove target fragments that are empty or whitespace
            targetFragments = targetFragments.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            // If the program is licensed...
            if (QP._isLicensed)
                // Pass all fragments
                Window_AlignFragmentsManager.ClosePreviousAndShow(sourceFragments, targetFragments, this);
            // If the program is not licensed...
            else
                // Pass only first MAX fragments
                Window_AlignFragmentsManager.ClosePreviousAndShow(sourceFragments.Take(mLimitMaxEntryCount).ToArray(),
                    targetFragments.Take(mLimitMaxEntryCount).ToArray(), this);
        }



        private void targetItem_AlignAsTableSingleRow_Click(object sender, RoutedEventArgs e)
        {
            // Align text fragments of the text in the target textbox as a table which consists of 2 columns and 1 row
            AlignAsTableSingleRow(txtEditor2.Text);
        }

        private void targetItem_AlignAsTableMultipleRows_Click(object sender, RoutedEventArgs e)
        {
            // Align text fragments of the text in the target textbox as a table which consists of 2 columns and multiple rows
            AlignAsTableMultipleRows(txtEditor2.Text);
        }

        private void btn_ClearTarget_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_About_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtEditor_PreviewDragOver(object sender, DragEventArgs e)
        {

        }

        private void txtEditor_Drop(object sender, DragEventArgs e)
        {

        }
    }
}
