using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for Window_Save.xaml
    /// </summary>
    public partial class Window_Save : Window
    {
        #region Public members

        /// <summary>
        /// A collection of <see cref="SourceFragment"/>
        /// </summary>
        public List<SourceFragment> SourceFragments;

        /// <summary>
        /// A collection of target fragments
        /// </summary>
        public ItemCollection TargetFragments;

        #endregion

        #region Private members

        /// <summary>
        /// The default table name displayed on startup
        /// </summary>
        private string mDefaultTableName = "table1";

        #endregion

        public Window_Save()
        {
            InitializeComponent();
        }

        private void DragWithHeader(object sender, MouseButtonEventArgs e)
        {
            // If left mouse button is pressed...
            if (e.ChangedButton == MouseButton.Left)
                // Drag this window
                DragMove();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void hyperlink_OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the folder where a new file will be created
                Process.Start("explorer.exe", Path.GetDirectoryName(tb_location.Text));
            }
            catch (Exception ex)
            {
                // Show the error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_browseFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create a new save file dilog
            var saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Plain Text File (*.txt)|*.txt|Microsoft Access 2002-2003 Database (*.mdb)|*.mdb",
                FileName = "Untitled.txt",
            };

            // Create behavior for pressing the Save button
            saveFileDialog.FileOk += SaveFileDialog_FileOk;

            // Show the save file dialog
            saveFileDialog.ShowDialog();
        }

        private void SaveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cast the sender to save file dialog
            var saveFileDialog = (SaveFileDialog)sender;

            // Get the extension
            var extension = Path.GetExtension(saveFileDialog.FileName);

            // If neither .txt, nor .mdb is selected...
            if (extension != ".txt" && extension != ".mdb")
                // Show an error
                MessageBox.Show("Unsupported extension", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // If the extension is supported
            else
                // Put the path in the textbox
                tb_location.Text = saveFileDialog.FileName;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // If the directory for the future file exists...
            if (Directory.Exists(Path.GetDirectoryName(tb_location.Text)))
            {
                // Get the extension
                var extension = Path.GetExtension(tb_location.Text);

                // If plain text is needed...
                if (extension == ".txt")
                {
                    // Create a new txt file
                    using (var sw = File.CreateText(tb_location.Text))
                    {
                        // Write the table name to the file
                        sw.WriteLine($"Table name: { tb_tableName.Text }");

                        // Iterate through all source fragments
                        for (int i = 0; i < SourceFragments.Count; i++)
                        {
                            // Text that will be written if there are target fragments
                            var secondColumn = string.Empty;

                            // TODO: Replace with a target of source fragment
                            // If there are target fragments
                            if (i < TargetFragments.Count)
                                // Add tab and target fragment
                                secondColumn = $"\t{ TargetFragments[i] }";

                            // Write a line to the file
                            sw.WriteLine($"{ SourceFragments[i] }{ secondColumn }");
                        }
                    };

                    // Show a message
                    MessageBox.Show("File saved");

                    // Close the window
                    Close();
                }
                // If database is needed...
                else if (extension == ".mdb")
                {
                    File.Copy(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\template.mdb", tb_location.Text, true);

                    #region Connect to DB

                    var connect = new OleDbConnection
                    {
                        ConnectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source = {0};Persist Security Info=False;", tb_location.Text)
                    };
                    try
                    {
                        connect.Open();
                    }
                    catch (Exception)
                    {
                        connect.ConnectionString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source = {0};Persist Security Info=False;", tb_location.Text);

                        connect.Open();
                    }

                    var command = new OleDbCommand
                    {
                        Connection = connect
                    };

                    try
                    {
                        var tableName = tb_tableName.Text;

                        if (tableName != mDefaultTableName)
                        {
                            command.CommandText = string.Format(@"SELECT * INTO `{0}` FROM {1}", tableName, mDefaultTableName);
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(@"DROP TABLE {0}", mDefaultTableName);
                            command.ExecuteNonQuery();
                        }

                        // Iterate through all source fragments
                        for (int i = 0; i < SourceFragments.Count; i++)
                        {
                            var sourceFragment = SourceFragments[i].Text.Replace("'", "''");

                            // Text that will be written if there are target fragments
                            var secondColumn = string.Empty;

                            // TODO: Replace with a target of source fragment
                            // If there are target fragments
                            if (i < TargetFragments.Count)
                                // Add tab and target fragment
                                secondColumn = $"\t{ TargetFragments[i] }";

                            command.CommandText = $@"INSERT INTO `{ tableName }`(source,target) VALUES('{ sourceFragment }', '{ secondColumn }')";

                            command.ExecuteNonQuery();
                        }

                        connect.Close();

                        // Show a message
                        MessageBox.Show("Database saved.");

                        // Close the window
                        Close();
                    }
                    catch (Exception ex)
                    {
                        connect.Close();

                        MessageBox.Show(ex.Message + Environment.NewLine + "The database was filled up to the error", "Error");
                    }
                    #endregion
                }
                // If the extension is not supported...
                else
                    // Show an error
                    MessageBox.Show("Unsupported extension", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // If the directory for the future file does not exist...
            else
                // Show an error
                MessageBox.Show("Directory does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
