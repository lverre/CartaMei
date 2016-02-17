﻿using CartaMei.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartaMei.GSHHG
{
    public class Utils
    {
        #region Singleton

        private static readonly Utils _instance = new Utils();
        public static Utils Instance { get { return _instance; } }

        private Utils()
        {
            this.StatusText = new TextStatusItem();
            this.StatusProgress = new ProgressBarStatusItem();
        }

        #endregion

        #region Status

        private bool _isStatusBlocked = false;

        internal TextStatusItem StatusText { get; private set; }

        internal ProgressBarStatusItem StatusProgress { get; private set; }

        internal bool SetStatus(string text, bool withStatus = false, bool isStatusMarquee = true, int percentage = 0, bool block = false)
        {
            if (_isStatusBlocked) return false;

            _isStatusBlocked = block;

            this.StatusText.Text = text;
            this.StatusText.IsVisible = true;
            this.StatusProgress.IsVisible = withStatus;
            this.StatusProgress.IsMarquee = isStatusMarquee;
            this.StatusProgress.Percentage = percentage;

            return true;
        }

        internal void HideStatus()
        {
            this.StatusProgress.IsVisible = false;
            this.StatusText.IsVisible = false;
            _isStatusBlocked = false;
        }

        #endregion
    }
}
