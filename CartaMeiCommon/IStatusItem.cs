using System.ComponentModel;

namespace CartaMei.Common
{
    public interface IStatusItem : INotifyPropertyChanged
    {
        bool IsVisible { get; }
    }
    
    public interface ITextStatusItem : IStatusItem
    {
        string Text { get; }
    }

    public interface IProgressBarStatusItem : IStatusItem
    {
        int Percentage { get; }
        
        bool IsMarquee { get; }
    }

    public abstract class AStatusItem : NotifyPropertyChangedBase, IStatusItem
    {
        #region IStatusItem

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }

    public class SeparatorStatusItem : AStatusItem { }

    public class TextStatusItem : AStatusItem, ITextStatusItem
    {
        #region ITextStatusItem

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }

    public class ProgressBarStatusItem : AStatusItem, IProgressBarStatusItem
    {
        #region IProgressBarStatusItem

        private int _percentage;
        public int Percentage
        {
            get { return _percentage; }
            set
            {
                if (value != _percentage)
                {
                    _percentage = value;
                    onPropetyChanged();
                }
            }
        }

        private bool _isMarquee;
        public bool IsMarquee
        {
            get { return _isMarquee; }
            set
            {
                if (value != _isMarquee)
                {
                    _isMarquee = value;
                    onPropetyChanged();
                }
            }
        }

        #endregion
    }
}
