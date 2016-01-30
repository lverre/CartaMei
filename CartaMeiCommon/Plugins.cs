using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;

namespace CartaMei.Common
{
    /// <summary>
    /// Describes what a plugin shall provide.
    /// </summary>
    public interface IPlugin
    {
        #region Properties

        /// <summary>
        /// Gets the menu buttons this plugin provides.
        /// </summary>
        PluginMenu Menu { get; }

        /// <summary>
        /// Gets the toolbar buttons this plugin provides.
        /// </summary>
        IEnumerable<IButtonModel> Toolbar { get; }

        /// <summary>
        /// Gets the map projections this plugin provides.
        /// </summary>
        IEnumerable<PluginItemProvider<IProjection>> ProjectionProviders { get; }

        /// <summary>
        /// Gets the datums this plugin provides.
        /// </summary>
        IEnumerable<Datum> Datums { get; }

        /// <summary>
        /// Gets an object that shows and can change the plugin's settings.
        /// </summary>
        object Settings { get; }
        
        #endregion
    }

    /// <summary>
    /// Describes what a plugin that has graphical capabilities shall provide.
    /// </summary>
    public interface IGraphicalPlugin : IPlugin
    {
        #region Properties

        /// <summary>
        /// Gets the type of layers this plugin provides.
        /// </summary>
        IEnumerable<PluginItemProvider<ILayer>> LayerProviders { get; }

        /// <summary>
        /// Gets a list of anchorable tools.
        /// </summary>
        IEnumerable<Tuple<IAnchorableTool, DataTemplate>> AnchorableTools { get; }

        #endregion
    }

    public abstract class PluginBase : IPlugin
    {
        #region IPlugin

        /// <summary>
        /// Gets the menu buttons this plugin provides.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual PluginMenu Menu { get { return null; } }

        /// <summary>
        /// Gets the toolbar buttons this plugin provides.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual IEnumerable<IButtonModel> Toolbar { get { return null; } }

        /// <summary>
        /// Gets the map projections this plugin provides.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual IEnumerable<PluginItemProvider<IProjection>> ProjectionProviders { get { return null; } }

        /// <summary>
        /// Gets the datums this plugin provides.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual IEnumerable<Datum> Datums { get { return null; } }
        
        /// <summary>
        /// Gets an object that shows and can change the plugin's settings.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual object Settings { get { return null; } }

        #endregion
    }

    public abstract class GraphicalPluginBase : PluginBase, IGraphicalPlugin
    {
        #region IGraphicalPlugin

        /// <summary>
        /// Gets the type of layers this plugin provides.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual IEnumerable<PluginItemProvider<ILayer>> LayerProviders { get { return null; } }

        /// <summary>
        /// Gets a list of anchorable tools.
        /// </summary>
        /// <remarks>Returns always <c>null</c>.</remarks>
        public virtual IEnumerable<Tuple<IAnchorableTool, DataTemplate>> AnchorableTools { get { return null; } }

        #endregion
    }

    public class PluginItemProvider<T>
    {
        #region Properties

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual Func<IMap, T> Create { get; set; }

        #endregion

        #region Object

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }

    public class PluginMenu
    {
        #region Enums

        public enum PluginMenuLocation
        {
            TopLevel,
            ToolsMenu,
            ToolsSubmenu
        }

        #endregion

        #region Constructor

        public PluginMenu()
        {
            this.ItemsLocation = PluginMenuLocation.ToolsSubmenu;
        }

        #endregion

        #region Properties

        public PluginMenuLocation ItemsLocation { get; set; }

        public IEnumerable<IButtonModel> Items { get; set; }

        #endregion
    }

    public interface IPluginMetadata
    {
        #region Properties

        [DefaultValue(null)]
        string Name { get; }

        [DefaultValue(null)]
        string Description { get; }

        [DefaultValue(null)]
        string Version { get; }

        [DefaultValue(null)]
        string Author { get; }

        [DefaultValue(null)]
        string Copyright { get; }

        [DefaultValue(null)]
        string Trademark { get; }

        [DefaultValue(null)]
        string License { get; }

        [DefaultValue(null)]
        string Link { get; }

        #endregion
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginAttribute : ExportAttribute, IPluginMetadata
    {
        #region Constructor

        public PluginAttribute() : base(typeof(IPlugin)) { }

        #endregion

        #region IPluginMetadata

        public string License { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        public string Trademark { get; set; }

        public string Version { get; set; }

        public string Link { get; set; }

        #endregion
    }
}
