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

namespace QuickSharp.CodeAssist.JScript
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "F3A2DA86-4D4B-4485-AE75-7A55B4EA82DF";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist JScript.NET Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides JScript.NET support for the Code Assist plugin.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist, "QuickSharp.CodeAssist", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist_DotNet, "QuickSharp.CodeAssist.DotNet", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            ActivatePlugin();
        }

        #endregion

        private void ActivatePlugin()
        {
            CodeAssistManager.GetInstance().RegisterProvider(
                new JScriptCodeAssistProvider());
        }
    }
}
