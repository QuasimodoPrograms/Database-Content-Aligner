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
using System.Windows.Shapes;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for Window_AlignFragments.xaml
    /// </summary>
    public partial class Window_AlignFragments : Window
    {
        public Window_AlignFragments()
        {
            InitializeComponent();
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
            // TODO: close the caller and show this window
        }
    }
}
