namespace DataInterface
{
    public class SelectionChangedEventArgs
    {
        #region Constructor
        public SelectionChangedEventArgs(bool _IsSelected)
        {
            IsSelected = _IsSelected;
        }
        #endregion

        #region Properties
        public bool IsSelected { get; }
        #endregion
    }
    public class CheckedChangedEventArgs
    {
        #region Constructor
        public CheckedChangedEventArgs(bool _IsChecked)
        {
            IsChecked = _IsChecked;
        }
        #endregion

        #region Properties
        public bool IsChecked { get; }
        #endregion
    }
    public class RankChangedEventArgs
    {
        #region Constructor
        public RankChangedEventArgs()
        {

        }
        #endregion

        #region Properties
        public HDynamicObject RankObj
        {
            get; set;

        }
        #endregion
    }
    

}
