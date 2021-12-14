using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBelign
{
    /// <summary>
    /// A class for source fragments
    /// </summary>
    public class SourceFragment : BaseFragment
    {
        #region Private members

        /// <summary>
        /// Indicates if this source fragment has a target
        /// </summary>
        private bool mHasTarget = true;

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if this source fragment has a target
        /// </summary>
        public bool HasTarget
        {
            get => mHasTarget;
            set
            {
                mHasTarget = value;

                RaisePropertyChanged(nameof(HasTarget));
            }
        }

        #endregion
    }
}
