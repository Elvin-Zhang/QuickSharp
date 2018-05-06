/*
 * QuickSharp Copyright (C) 2008-2011 Steve Walker.
 *
 * This file is part of QuickSharp.
 *
 * QuickSharp is free software: you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * QuickSharp is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with QuickSharp. If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides global access to various application properties and
    /// functionality. Uses a singleton; use the GetInstance() method
    /// to access.
    /// </summary>
    public class ApplicationManager
    {
        #region Singleton

        private static ApplicationManager _singleton;

        /// <summary>
        /// Provides access to the ApplicationManager singleton.
        /// </summary>
        /// <returns>A reference to the ApplicationManager.</returns>
        public static ApplicationManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new ApplicationManager();

            return _singleton;
        }

        #endregion

        private string _quickSharpHome;
        private string _quickSharpUserHome;
        private ClientProfile _clientProfile;
        private MainForm _mainForm;
        private List<String> _commandLineSwitches;
        private Dictionary<String, String> _commandLineSwitchValues;
        private DocumentType _newDocumentType;
        private DocumentType _unknownDocumentType;
        private NewDocumentHandler _newDocumentHandler;
        private List<DocumentFilterHandler> _documentFilterHandlers;
        private Dictionary<String, OpenDocumentHandler> _openDocumentHandlers;
        private List<DocumentPreLoadHandler> _documentPreLoadHandlers;
        private Dictionary<String, BaseDockedForm> _dockedFormsRegistry;
        private Dictionary<String, PersistenceManagerFactoryHandler> _persistenceProviders;
        private List<OptionsPageFactoryHandler> _optionsPageFactoryHandlers;
        private Dictionary<String, IQuickSharpTheme> _themeProviders;
        private string _selectedThemeProviderId;

        /// <summary>
        /// Raised when a change has been made to the filesystem. Plugins with an interest
        /// in the file system can register handlers to update any file views they present
        /// each time a filesystem change takes place.
        /// </summary>
        public event MessageHandler FileSystemChange;

        private ApplicationManager()
        {
            System.Reflection.Assembly a = 
                System.Reflection.Assembly.GetEntryAssembly();

            _quickSharpHome = Path.GetDirectoryName(a.Location);
            _quickSharpUserHome = _quickSharpHome;

            _commandLineSwitches = new List<String>();
            _commandLineSwitchValues = new Dictionary<String, String>();
            _documentFilterHandlers = new List<DocumentFilterHandler>();
            _openDocumentHandlers = new Dictionary<String, OpenDocumentHandler>();
            _documentPreLoadHandlers = new List<DocumentPreLoadHandler>();
            _dockedFormsRegistry = new Dictionary<String, BaseDockedForm>();
            _persistenceProviders = new Dictionary<String, PersistenceManagerFactoryHandler>();
            _themeProviders = new Dictionary<String, IQuickSharpTheme>();
            _optionsPageFactoryHandlers = new List<OptionsPageFactoryHandler>();
        }

        #region Application properties

        /// <summary>
        /// The path to the QuickSharp installation directory.
        /// </summary>
        public string QuickSharpHome
        {
            get { return _quickSharpHome; }
        }

        /// <summary>
        /// The path to the user's private QuickSharp data directory.
        /// </summary>
        public string QuickSharpUserHome
        {
            get
            {
                if (_clientProfile == null)
                    return _quickSharpHome;

                if (_clientProfile.DisableUserHome)
                    return _quickSharpHome;
                else
                {
                    /*
                     * It appears Win7 will no longer automatically create
                     * folders when files are created so we make sure that
                     * whenever the user folder is requested that it exists.
                     */

                    /*
                     * Let this go to the global exception handler - it's a
                     * pretty serious error if the user's directory can't be
                     * created.
                     */

                    if (!Directory.Exists(_quickSharpUserHome))
                        Directory.CreateDirectory(_quickSharpUserHome);

                    return _quickSharpUserHome;
                }
            }
        }

        /// <summary>
        /// The path of the docking window configuration file. This contains the
        /// layout of the docked windows saved on exit from QuickSharp and is saved
        /// to the user's private QuickSharp data directory on shutdown.
        /// </summary>
        public string DockPanelConfigFile
        {
            get
            {
                return Path.Combine(
                    QuickSharpUserHome, Constants.DOCK_PANEL_CONFIG);
            }
        }

        /// <summary>
        /// Determine if there is a saved docking window configuration file.
        /// </summary>
        public bool HaveDockPanelConfig
        {
            get { return File.Exists(DockPanelConfigFile); }
        }

        /// <summary>
        /// Stores the document preload event handlers registered with the application.
        /// </summary>
        public List<DocumentPreLoadHandler> DocumentPreLoadHandlers
        {
            get { return _documentPreLoadHandlers; }
        }

        /// <summary>
        /// Stores the document filter handlers registered with the application.
        /// </summary>
        public List<DocumentFilterHandler> DocumentFilterHandlers
        {
            get { return _documentFilterHandlers; }
        }

        /// <summary>
        /// Returns a set of filename filters for file Open and Save dialogs
        /// by combining all of the registered filer handlers.
        /// </summary>
        public string DocumentFilter
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (DocumentFilterHandler d in DocumentFilterHandlers)
                    sb.Append(d());

                sb.Append(AllDocumentsFilter);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Provides the default 'All Files' filename filter for file Open and Save 
        /// dialogs.
        /// </summary>
        public string AllDocumentsFilter
        {
            get { return Resources.AllDocumentsFilter; }
        }

        /// <summary>
        /// Stores the ClientProfile data for the application.
        /// The ClientProfile contains properties required to customize
        /// a QuickSharp-based application.
        /// </summary>
        public ClientProfile ClientProfile
        {
            get
            { 
                return _clientProfile;
            }
            set
            {
                _clientProfile = value;

                _quickSharpUserHome = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    _clientProfile.ClientName);
            }
        }

        /// <summary>
        /// The application main form.
        /// </summary>
        public MainForm MainForm
        {
            get { return _mainForm; }
            set { _mainForm = value; }
        }

        #endregion

        #region Command line switches

        /// <summary>
        /// Process a string argument as a command line switch and
        /// add it to the list of switches. (Internal use only).
        /// </summary>
        /// <param name="arg">The argument to be processed.</param>
        public void AddCommandLineSwitch(string arg)
        {
            if (arg.IndexOf(":") == -1)
            {
                // Simple flag
                arg = arg.ToLower();
                if (!_commandLineSwitches.Contains(arg))
                    _commandLineSwitches.Add(arg);
            }
            else
            {
                // Flag with value
                string[] split = arg.Split(':');
                _commandLineSwitchValues[split[0].ToLower()] = split[1];
            }
        }

        /// <summary>
        /// Determine if an argument has been passed at startup. This is
        /// used for command line switches consisting of a single text value.
        /// The argument is case-insensitive and can be supplied using a '-' or
        /// '/' prefix.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <returns>True if the switch was provided at startup.</returns>
        public bool HaveCommandLineSwitch(string name)
        {
            return _commandLineSwitches.Contains(name.ToLower());
        }

        /// <summary>
        /// Get the value from a command line switch provided at startup.
        /// Command line values are passed as a name/value pair separated by
        /// ':'. Requesting the name returns the value (or null if not found).
        /// Values containing spaces should be surrounded with double-quotes.
        /// </summary>
        /// <param name="name">The name of the switch.</param>
        /// <returns>The value of the switch or null if not found.</returns>
        public string GetCommandLineSwitchValue(string name)
        {
            name = name.ToLower();

            if (_commandLineSwitchValues.ContainsKey(name))
                return _commandLineSwitchValues[name];
            else
                return null;
        }

        #endregion

        #region Persistence manager

        /// <summary>
        /// Register a persistence provider. Associates the provider name
        /// with a persistence manager factory method.
        /// </summary>
        /// <param name="provider">The name of the provider.</param>
        /// <param name="handler">A factory method for creating instances of the provider
        /// persistence manager.</param>
        public void RegisterPersistenceProvider(
            string provider, PersistenceManagerFactoryHandler handler)
        {
            // Overwrite if provider already exists.
            _persistenceProviders[provider] = handler;
        }

        /// <summary>
        /// Gets a persistence manager for the storage key specified.
        /// </summary>
        /// <param name="key">The storage key.</param>
        /// <returns>A persistence manager object.</returns>
        public IPersistenceManager GetPersistenceManager(string key)
        {
            string provider = ClientProfile.PersistenceProvider;

            if (!_persistenceProviders.ContainsKey(provider))
                throw new Exception(String.Format(
                    "Unknown persistence provider type: {0}", provider));

            PersistenceManagerFactoryHandler handler = 
                _persistenceProviders[provider];

            return handler(key);
        }

        #endregion

        #region Theme manager

        /// <summary>
        /// Registers a theme provider. The provider ID is the registration
        /// key: if a provider with the same key has already been registered
        /// it will be replaced with the new provider.
        /// </summary>
        /// <param name="provider">A theme provider (implements IThemeProvider).</param>
        public void RegisterThemeProvider(IQuickSharpTheme provider)
        {
            string id = provider.GetID();

            if (String.IsNullOrEmpty(id))
                throw new Exception("Theme provider has invalid ID: "
                    + provider.GetName());
            
            // Overwrite existing provider if key exists.
            _themeProviders[id] = provider;
        }

        /// <summary>
        /// Get the theme provider with the specified ID.
        /// </summary>
        /// <param name="providerId">The provider ID.</param>
        /// <returns>A reference to the provider or null if the ID
        /// was not found.
        /// </returns>
        public IQuickSharpTheme GetThemeProvider(string providerId)
        {
            if (String.IsNullOrEmpty(providerId))
                return null;

            if (!_themeProviders.ContainsKey(providerId))
                return null;

            return _themeProviders[providerId];
        }

        /// <summary>
        /// Get the currently selected theme provider.
        /// </summary>
        /// <returns>A reference to the selected provider or the
        /// default provider if the selected provider has not been
        /// registered.
        /// </returns>
        public IQuickSharpTheme GetSelectedThemeProvider()
        {
            return GetThemeProvider(_selectedThemeProviderId);
        }

        /// <summary>
        /// Gets the key of the currently selected theme provider.
        /// </summary>
        /// <returns>The key of the selected provider or the
        /// default provider if the selected provider has not been
        /// registered.
        /// </returns>
        public string GetSelectedThemeProviderKey()
        {
            return GetSelectedThemeProvider().GetKey();
        }

        /// <summary>
        /// Gets the currently registered theme providers
        /// as a list of StringMaps sorted by Name.
        /// </summary>
        public List<StringMap> ThemeProviderMap
        {
            get
            {
                List<StringMap> list = new List<StringMap>();

                foreach (string id in _themeProviders.Keys)
                {
                    list.Add(new StringMap()
                    {
                        Name = _themeProviders[id].GetName(),
                        Value = id
                    });
                }

                list.Sort(new StringMapNameComparer());

                return list;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the currently selected theme provider.
        /// Assigning an unregistered ID will cause the default theme to be
        /// selected.
        /// </summary>
        public string SelectedTheme
        {
            get
            {
                return _selectedThemeProviderId;
            }
            set
            {
                if (_themeProviders.ContainsKey(value))
                    _selectedThemeProviderId = value;
                else
                    _selectedThemeProviderId = Constants.DEFAULT_THEME_ID;
            }
        }

        #endregion

        #region Document management

        /**********************************************************************
         * DOCUMENT MANAGEMENT
         * 
         * The mainform manages the creation of documents and provides New and
         * Open menu/toolbar options for this. Document types are defined by
         * filename extensions (e.g. '.txt') and are used to determine the
         * actions associated with a particular document type.
         * 
         * The 'new document type' determines the document type created when
         * New is selected and the 'new document handler' is called to create
         * the document as required.
         * 
         * To manage exisiting files document types are associated with an
         * 'open document' handler which is called whenever a file of its type
         * is opened. The handler is responsible for performing the appropriate
         * actions to create the document.
         * 
         * If no document handler has been defined for a document type the
         * document is (optionally) passed to the shell for opening. If
         * this fails it is then passed to the unknown document handler.
         * If an unkbown document type has been defined, the unknown document
         * is treated as though it has this type and if a handler for the
         * type has been registered it is used to open the document. If no
         * handler is available the load fails and an (optional) error
         * message is displayed.
         * 
         **********************************************************************/

        /// <summary>
        /// Stores the document type registered as the type for new documents.
        /// </summary>
        public DocumentType NewDocumentType
        {
            get { return _newDocumentType; }
            set { _newDocumentType = value; }
        }

        /// <summary>
        /// Gets the open document handler registered for new documents.
        /// </summary>
        public NewDocumentHandler NewDocumentHandler
        {
            get { return _newDocumentHandler; }
            set { _newDocumentHandler = value; }
        }

        /// <summary>
        /// Stores the document type registered as the type to use when a document
        /// of an unknown (i.e. unregistered) type is requested.
        /// </summary>
        public DocumentType UnknownDocumentType
        {
            get { return _unknownDocumentType; }
            set { _unknownDocumentType = value; }
        }

        /// <summary>
        /// Gets the open document handler for unknown document types.
        /// </summary>
        public OpenDocumentHandler UnknownDocumentHandler
        {
            get
            {
                return GetOpenDocumentHandler(_unknownDocumentType);
            }
        }

        #endregion

        #region Open document handling

        /*
         * Registration.
         */

        /// <summary>
        /// Registers an open document handler for a document type provided as a DocumentType object.
        /// A handler will be rejected if a handler has already been registered for the document type.
        /// </summary>
        /// <param name="documentType">The document type to be opened by the handler.</param>
        /// <param name="handler">The handler used to open documents of the registered type.</param>
        /// <returns>True if the handler was registered successfully.</returns>
        public bool RegisterOpenDocumentHandler(
            DocumentType documentType, OpenDocumentHandler handler)
        {
            return RegisterOpenDocumentHandler(
                documentType.ToString(), handler);
        }

        /// <summary>
        /// Registers an open document handler for a document type provided as a string.
        /// A handler will be rejected if a handler has already been registered for the document type.
        /// </summary>
        /// <param name="documentType">The document type to be opened by the handler.</param>
        /// <param name="handler">The handler used to open documents of the registered type.</param>
        /// <returns>True if the handler was registered successfully.</returns>
        public bool RegisterOpenDocumentHandler(
            string documentType, OpenDocumentHandler handler)
        {
            if (_openDocumentHandlers.ContainsKey(documentType))
                return false;

            _openDocumentHandlers[documentType] = handler;

            return true;
        }

        /*
         * Retrieval.
         */

        /// <summary>
        /// Provides access to the registered open document handlers.
        /// </summary>
        public Dictionary<String, OpenDocumentHandler> OpenDocumentHandlers
        {
            get { return _openDocumentHandlers; }
        }

        /// <summary>
        /// Gets the open document handler registered for a document type specified
        /// as a DocumentType object.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>The open document handler registered for the document type.</returns>
        public OpenDocumentHandler GetOpenDocumentHandler(
            DocumentType documentType)
        {
            if (documentType == null)
                return null;

            return GetOpenDocumentHandler(documentType.ToString());
        }

        /// <summary>
        /// Gets the open document handler registered for a document type specified
        /// as a string.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>The open document handler registered for the document type.</returns>
        public OpenDocumentHandler GetOpenDocumentHandler(
            String documentType)
        {
            if (String.IsNullOrEmpty(documentType))
                return null;

            if (!_openDocumentHandlers.ContainsKey(documentType))
                return null;

            return _openDocumentHandlers[documentType];
        }

        /*
         * Removal.
         */

        /// <summary>
        /// Unregisters the handler registered for a document type specified as a
        /// DocumentType object.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>The open document handler or null if the document type is invalid or
        /// no handler exists for the type.</returns>
        public OpenDocumentHandler RemoveOpenDocumentHandler(
            DocumentType documentType)
        {
            return RemoveOpenDocumentHandler(documentType.ToString());
        }

        /// <summary>
        /// Unregisters the handler registered for a document type specified as a
        /// string.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>The open document handler or null if the document type is invalid or
        /// no handler exists for the type.</returns>
        public OpenDocumentHandler RemoveOpenDocumentHandler(
            string documentType)
        {
            if (String.IsNullOrEmpty(documentType))
                return null;

            if (!_openDocumentHandlers.ContainsKey(documentType))
                return null;

            OpenDocumentHandler handler = 
                _openDocumentHandlers[documentType]; 

            _openDocumentHandlers.Remove(documentType);

            return handler;
        }

        #endregion

        #region Options form

        /**********************************************************************
         * OPTIONS FORM
         * 
         * The Core provides a shared Options configuration form for use by
         * plugins with user configurable parameters. Each page in the form
         * is provided by a plugin and is created on demand when the main
         * form calls the plugin's option page factory method. Each plugin
         * registers its page factory with the main form. Each plugin
         * tab is responsible for saving it's own data when the main form
         * calls it's Save() method.
         **********************************************************************/

        /// <summary>
        /// Register an OptionsPage factory handler.
        /// </summary>
        /// <param name="handler">The factory handler.</param>
        public void RegisterOptionsPageFactory(
            OptionsPageFactoryHandler handler)
        {
            _optionsPageFactoryHandlers.Add(handler);
        }

        /// <summary>
        /// Stores the registered OptionsPage factory handlers.
        /// </summary>
        public List<OptionsPageFactoryHandler>
            OptionsPageFactoryHandlers
        {
            get { return _optionsPageFactoryHandlers; }
        }

        #endregion

        #region File system change notification

        /**********************************************************************
         * FILE SYSTEM CHANGE NOTIFICATION
         * 
         * Plugins which show the file system state (e.g. the Workspace)
         * can opt to be notified if another plugin performs an action
         * which may change the file system (e.g. creating a file).
         **********************************************************************/

        /// <summary>
        /// Send a file system change notification to all registered handlers.
        /// </summary>
        public void NotifyFileSystemChange()
        {
            if (FileSystemChange != null)
                FileSystemChange();
        }

        #endregion

        #region Docked Forms

        /// <summary>
        /// Register a docked form. The form is registered using the key
        /// provided by its FormKey property.
        /// </summary>
        /// <param name="form">The form.</param>
        public void RegisterDockedForm(BaseDockedForm form)
        {
            RegisterDockedForm(form, form.FormKey);
        }

        /// <summary>
        /// Register a docked form using the form key provided.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="key">The key.</param>
        public void RegisterDockedForm(BaseDockedForm form, string key)
        {
            _dockedFormsRegistry[key] = form;
        }

        /// <summary>
        /// Get a registered docked form.
        /// </summary>
        /// <param name="key">The key under which the form was registered.</param>
        /// <returns>The form or null if the key is not registered.</returns>
        public BaseDockedForm GetDockedForm(string key)
        {
            if (_dockedFormsRegistry.ContainsKey(key))
                return _dockedFormsRegistry[key];
            else
                return null;
        }

        #endregion
    }
}
