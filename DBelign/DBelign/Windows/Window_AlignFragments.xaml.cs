﻿using System;
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
using System.Windows.Shapes;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for Window_AlignFragments.xaml
    /// </summary>
    public partial class Window_AlignFragments : Window, IDisposable
    {
        #region Private members

        /// <summary>
        /// Source fragments obtained from the source text
        /// </summary>
        private readonly string[] mSourceFragments;

        /// <summary>
        /// Target fragments obtained from the target text
        /// </summary>
        private readonly string[] mTargetFragments;

        #endregion

        void IDisposable.Dispose() { }

        #region Constructor

        /// <summary>
        /// Constructor that can be called either directly or using the <see cref="Window_AlignFragmentsManager"/> wrapper
        /// </summary>
        /// <param name="sourceFragments">Source fragments obtained from the source text</param>
        /// <param name="targetFragments">Target fragments obtained from the target text</param>
        public Window_AlignFragments(string[] sourceFragments, string[] targetFragments)
        {
            InitializeComponent();

            mSourceFragments = sourceFragments;
            mTargetFragments = targetFragments;
        }



        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            listView1.ItemsSource = mSourceFragments;

            listView2.ItemsSource = mTargetFragments;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }
    }

    /// <summary>
    /// A wrapper for <see cref="Window_AlignFragments"/>
    /// </summary>
    public static class Window_AlignFragmentsManager
    {
        /// <summary>
        /// Closes the calling window and shows <see cref="Window_AlignFragments"/>
        /// </summary>
        /// <param name="sourceFragments">Source fragments obtained from the source text</param>
        /// <param name="targetFragments">Target fragments obtained from the target text</param>
        /// <param name="windowToClose">The calling window which needs to be closed</param>
        public static void ClosePreviousAndShow(string[] sourceFragments, string[] targetFragments, Window windowToClose)
        {
            // Utilize the "using" constuct to ensure that the resources are freed when the window is closed
            using (var window = new Window_AlignFragments(sourceFragments, targetFragments))
            {
                // Close the caller
                windowToClose.Close();

                // Show this window
                window.ShowDialog();
            }
        }
    }
}
