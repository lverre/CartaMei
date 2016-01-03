using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.MainPlugin
{
    internal static class Buttons
    {
        #region Main Menus

        internal static readonly ButtonModel File = new ButtonModel()
        {
            Name = "_File",
            Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>()
            {
                NewMap,
                OpenMap,
                SaveMap
            }
        };

        internal static readonly ButtonModel Edit = new ButtonModel()
        {
            Name = "_Edit"
        };

        internal static readonly ButtonModel View = new ButtonModel()
        {
            Name = "_View"
        };

        internal static readonly ButtonModel Tools = new ButtonModel()
        {
            Name = "_Tools",
            Children = new System.Collections.ObjectModel.ObservableCollection<IButtonModel>()
            {
                Options
            }
        };

        internal static readonly ButtonModel Help = new ButtonModel()
        {
            Name = "_Help"
        };

        #endregion

        #region Tools

        internal static readonly ButtonModel NewMap = new ButtonModel()
        {
            Name = "_New Map"
        };

        internal static readonly ButtonModel OpenMap = new ButtonModel()
        {
            Name = "_Open Map"
        };

        internal static readonly ButtonModel SaveMap = new ButtonModel()
        {
            Name = "_Save Map"
        };

        internal static readonly ButtonModel Options = new ButtonModel()
        {
            Name = "_Options"
        };

        #endregion
    }
}
