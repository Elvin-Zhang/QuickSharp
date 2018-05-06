using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamplePlugin
{
    public class Constants
    {
        /*
         * Make all the keys used in the application public
         * so that they can be accessed by other plugins.
         */

        public const string PLUGIN_NAME = "SamplePlugin";
        public const string DOCKED_FORM_NAME = "SampleDockedForm";
        public const string DOCKED_FORM_KEY = "D1EE86D3-7974-4A46-95C3-C5BEBB1A4F50";
        public const string UI_DOCKED_FORM_MENU_ITEM = "UI_DOCKED_FORM_MENU_ITEM";
        public const string UI_TOOLS_MENU_SINGLETON = "UI_TOOLS_MENU_SINGLETON";
        public const string UI_IMAGE_MENU = "UI_IMAGE_MENU";
        public const string UI_IMAGE_MENU_STRETCH = "UI_IMAGE_MENU_STRETCH";
        public const string UI_TOOLBAR_STRETCH = "UI_TOOLBAR_STRETCH";
        public const string UI_OPTIONS_PAGE1 = "UI_OPTIONS_PAGE1";
        public const string UI_OPTIONS_PAGE2 = "UI_OPTIONS_PAGE2";
        public const string UI_OPTIONS_PAGE3 = "UI_OPTIONS_PAGE3";
        public const string UI_TOOLBAR = "UI_TOOLBAR";
        public const string UI_TOOLBAR_BUTTON1 = "UI_TOOLBAR_BUTTON1";
        public const string UI_TOOLBAR_BUTTON2 = "UI_TOOLBAR_BUTTON2";
        public const string UI_TOOLBAR_BUTTON3 = "UI_TOOLBAR_BUTTON3";
        public const string KEY_OPTION_TEXT_MESSAGE = "OptionTextMessage";
        public const string SINGLETON_DOCUMENT_TYPE = ".__SINGLETON__";
        public const string SINGLETON_DOCUMENT_NAME = "__SINGLETON__.__SINGLETON__";
    }
}
