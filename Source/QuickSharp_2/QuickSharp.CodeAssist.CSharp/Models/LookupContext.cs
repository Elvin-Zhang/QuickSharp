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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.CSharp
{
    public class LookupContext
    {
        private string _fullSource;
        private string _preSource;
        private string _line;
        private int _lineStartPos;
        private int _currentPos;
        private bool _beforeClass;
        private LookupTarget _target;
        private LocalMethods _localMethods;
        private LocalProperties _localProperties;

        public DeclarationContext DeclarationContext { get; set; }
        public DeclaredVariables DeclaredVariables { get; set; }
        public InheritedVariablesBase InheritedVariables { get; set; }

        #region Properties

        public string FullSource
        {
            get { return _fullSource; }
        }
        
        public string PreSource
        { 
            get { return _preSource; }
        }
        
        public string Line
        {
            get { return _line; }
        }
        
        public int LineStartPos
        {
            get { return _lineStartPos; }
        }

        public int CurrentPos
        {
            get { return _currentPos; }
        }

        public bool BeforeClass
        {
            get { return _beforeClass; }
        }

        public LookupTarget Target
        {
            get { return _target; }
        }

        public LocalMethods Methods
        {
            get { return _localMethods; }
        }

        public LocalProperties Properties
        {
            get { return _localProperties; }
        }

        #endregion

        #region Constructor

        public LookupContext(string fullSource, string preSource,
            string line, int lineStartPos, int currentPos, bool insideClass)
        {
            _fullSource = fullSource;
            _preSource = preSource;
            _line = line;
            _lineStartPos = lineStartPos;
            _currentPos = currentPos;
            _beforeClass = insideClass;

            /*
             * Cleanup the source code.
             */

            _preSource = CSharpFormattingTools.RemoveUnwantedText(_preSource);
            _preSource = CSharpFormattingTools.RemoveUnwantedBracketText(_preSource);
            _line = CSharpFormattingTools.RemoveUnwantedText(_line);
            _line = CSharpFormattingTools.RemoveUnwantedBracketText(_line);

            /*
             * Get rid of 'global::' - we don't do anything with them
             * so we might as well not have them.
             */

            _line = _line.Replace("global::", String.Empty);

            /*
             * We remove the content of any balanced brackets to
             * allow indexed variables to identified and classified.
             */
            
            _line = CSharpFormattingTools.RemoveUnwantedParentheses(Line);

            /*
             * Create the target.
             */

            _target = new LookupTarget(_line);

            /*
             * Find the visible methods and properties.
             */

            _localMethods = new LocalMethods(_fullSource);
            _localProperties = new LocalProperties(_fullSource);
        }

        #endregion
    }
}
