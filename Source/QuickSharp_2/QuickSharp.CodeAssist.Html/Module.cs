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

using System.Collections.Generic;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.Html
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "B44BB094-4786-45ED-AEC7-D4215FB57435";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist Web Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides web language support for the Code Assist plugin.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist, "QuickSharp.CodeAssist", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            ActivatePlugin();
        }

        #endregion

        private void ActivatePlugin()
        {
            CodeAssistManager codeAssistManager =
                CodeAssistManager.GetInstance();

            codeAssistManager.RegisterProvider(new HtmlCodeAssistProvider());
            codeAssistManager.RegisterProvider(new HtmCodeAssistProvider());
            codeAssistManager.RegisterProvider(new PhpCodeAssistProvider());
            codeAssistManager.RegisterProvider(new ErbCodeAssistProvider());
            codeAssistManager.RegisterProvider(new RhtmlCodeAssistProvider());
            codeAssistManager.RegisterProvider(new CssCodeAssistProvider());
        }
    }
}
