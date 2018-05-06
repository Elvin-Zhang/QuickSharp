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

namespace QuickSharp.CodeAssist.AspNet
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "E61D1149-27D0-4369-8C9A-BBAE93FB4CA2";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist ASP.NET Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides ASP.NET support for the Code Assist plugin (C# only).";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist, "QuickSharp.CodeAssist", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist_DotNet, "QuickSharp.CodeAssist.DotNet", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist_CSharp, "QuickSharp.CodeAssist.CSharp", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist_Html, "QuickSharp.CodeAssist.Html", 1));
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

            codeAssistManager.RegisterProvider(
                new AspxCodeAssistProvider());

            codeAssistManager.RegisterProvider(
                new AscxCodeAssistProvider());

            codeAssistManager.RegisterProvider(
                new AshxCodeAssistProvider());

            codeAssistManager.RegisterProvider(
                new AsmxCodeAssistProvider());

            codeAssistManager.RegisterProvider(
                new MasterCodeAssistProvider());
        }
    }
}
