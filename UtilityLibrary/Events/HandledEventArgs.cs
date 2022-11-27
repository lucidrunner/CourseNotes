//Björn Rundquist 12/10-2020

using System;

namespace UtilityLibrary.Events
{
    /// <summary>
    /// Simple EventArgs that allows an a arbitrary source & a general Handled-status to be set
    /// </summary>
    public class HandledEventArgs: EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Create a new HandledEventArgs for an event
        /// </summary>
        /// <param name="aSource">The source object of the event</param>
        public HandledEventArgs(object aSource)
        {
            Handled = false;
            Source = aSource;
        }

        #endregion Public Constructors

        #region Public Properties

        public object Context { get; private set; }

        /// <summary>
        /// The status of the HandledEventArgs, should be set to true by the first to handle the event
        /// </summary>
        public bool Handled { get; private set; }

        /// <summary>
        /// The original source of the event
        /// </summary>
        public object Source { get; }

        #endregion Public Properties

        #region Public Methods

        ///<summary>
        ///Sets the Context object for the event. This can be used to track the raiser of the event when the source & raiser aren't the same.
        /// </summary>
        public void SetContext(object aContext)
        {
            Context = aContext;
        }

        /// <summary>
        /// Set the event as Handled
        /// </summary>
        public void SetHandled()
        {
            Handled = true;
        }

        #endregion Public Methods
    }
}