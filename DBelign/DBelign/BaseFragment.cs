using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBelign
{
    /// <summary>
    /// A base class for fragments
    /// </summary>
    public class BaseFragment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Public Properties

        /// <summary>
        /// The numeric Id of a fragment
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The text of a fragment
        /// </summary>
        public string Text { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the <see cref="Text"/> property of a fragment
        /// </summary>
        /// <returns>The <see cref="Text"/> property of a fragment</returns>
        public override string ToString() => Text;

        /// <summary>
        /// Get 10 default <see cref="BaseFragment"/> class instances
        /// </summary>
        /// <returns>A list of 10 default <see cref="BaseFragment"/> class instances</returns>
        public static List<BaseFragment> GetDefaultItems()
        {
            // Create a list
            List<BaseFragment> items = new List<BaseFragment>();

            // Iterate 10 times
            for (int i = 0; i < 10; ++i)
            {
                // Add a new item to the list
                items.Add(new BaseFragment
                {
                    ID = i,
                    Text = $"Default text value{ i }",
                });
            }

            // Return the list
            return items;
        }

        #endregion
    }
}
