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

using QuickSharp.Core; 
using QuickSharp.Editor;
using QuickSharp.CodeAssist.CSharp;

namespace QuickSharp.CodeAssist.AspNet
{
    public class AscxCodeAssistProvider :
        CSharpWebCodeAssistProvider, ICodeAssistProvider
    {
        #region ICodeAssistProvider

        public DocumentType DocumentType
        {
            get { return new DocumentType(Constants.DOCUMENT_TYPE_ASCX); }
        }

        public bool IsAvailable
        {
            get { return true; }
        }

        public override void DocumentActivated(ScintillaEditForm document)
        {
            base.DocumentActivated(document);
        }

        public override LookupList GetLookupList(ScintillaEditForm document)
        {
            return base.GetLookupList(document);
        }

        #endregion
    }
}
