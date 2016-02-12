using CartaMei.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CartaMei.GSHHG
{
    [Plugin(License = "MIT")]
    public class GSHHGPlugin : GraphicalPluginBase
    {
        #region IPlugin

        public override IEnumerable<PluginItemProvider<ILayer>> LayerProviders
        {
            get
            {
                yield return new PluginItemProvider<ILayer>()
                {
                    Name = ShorelineLayer.LayerName,
                    Description = ShorelineLayer.LayerDescription,
                    Create = delegate (IMap map)
                    {
                        checkMapsDirectory();
                        return new ShorelineLayer(map);
                    }
                };
                // TODO: provide other kind of layers provided by gshhg data
                yield break;
            }
        }

        public override IEnumerable<IStatusItem> StatusBar
        {
            get
            {
                yield return Utils.Instance.StatusText;
                yield return Utils.Instance.StatusProgress;
                yield break;
            }
        }

        public override object Settings { get { return PluginSettings.Instance; } }

        public override void Unload()
        {
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Tools

        private bool checkMapsDirectory()
        {
            if (!Directory.Exists(PluginSettings.Instance.MapsDirectory))
            {
                var programLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#if DEBUG
                PluginSettings.Instance.MapsDirectory = Path.Combine(programLocation, @"..\..\..\..\maps");
                //return true;
#endif

                var gshhgUrl = "https://www.ngdc.noaa.gov/mgg/shorelines/data/gshhg/latest/";
                var message =
@"You haven't set the directory for the GSHHG map files.
If you already have the files in your computer, click No.
If you want to download the files, click Yes. Your browser will open NOAA's GSHHG website (" + gshhgUrl + @"). From there, download the bin file and unzip it somewhere in your computer.";
                var offerDowloadResult = System.Windows.MessageBox.Show(message, "Download Maps", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (offerDowloadResult == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(gshhgUrl);
                }

                var dialog = new FolderBrowserDialog()
                {
                    Description = "Please choose the folder that contains the GSHHG map files.",
                    SelectedPath = programLocation
                };
                switch (dialog.ShowDialog())
                {
                    case DialogResult.OK:
                    case DialogResult.Yes:
                        PluginSettings.Instance.MapsDirectory = dialog.SelectedPath;
                        break;
                }
            }

            return Directory.Exists(PluginSettings.Instance.MapsDirectory);
        }

        #endregion
    }
}
