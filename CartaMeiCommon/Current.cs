using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.Common
{
    public class Current
    {
        #region Owner

        private static Current _instance;
        private Current() { }

        public static Current Create()
        {
            if (_instance == null)
            {
                _instance = new Current();
                return _instance;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Map

        private IMap _map;

        public void SetMap(IMap value)
        {
            if (value != _map)
            {
                var oldValue = _map;
                _map = value;
                Current.MapChanged?.Invoke(new CurrentPropertyChangedEventArgs<IMap>(oldValue, _map));
            }
        }

        public static IMap Map { get { return _instance._map; } }

        public static event CurrentPropertyChangedEventHandler<IMap> MapChanged;

        #endregion

        #region Display Type

        private DisplayType _displayType;

        public void SetDisplayType(DisplayType value)
        {
            if (value != _displayType)
            {
                var oldValue = _displayType;
                _displayType = value;
                Current.DisplayTypeChanged?.Invoke(new CurrentPropertyChangedEventArgs<DisplayType>(oldValue, _displayType));
            }
        }

        public static DisplayType DisplayType { get { return _instance._displayType; } }

        public static event CurrentPropertyChangedEventHandler<DisplayType> DisplayTypeChanged;

        #endregion

        #region Animation Step

        private long _animationStep;

        public void SetAnimationStep(long value)
        {
            if (value != _animationStep)
            {
                var oldValue = _animationStep;
                _animationStep = value;
                Current.AnimationStepChanged?.Invoke(new CurrentPropertyChangedEventArgs<long>(oldValue, _animationStep));
            }
        }

        public static long AnimationStep { get { return _instance._animationStep; } }

        public static event CurrentPropertyChangedEventHandler<long> AnimationStepChanged;

        #endregion
    }

    public class CurrentPropertyChangedEventArgs<T>
    {
        public CurrentPropertyChangedEventArgs(T oldValue, T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
    }

    public delegate void CurrentPropertyChangedEventHandler<T>(CurrentPropertyChangedEventArgs<T> e);
}
