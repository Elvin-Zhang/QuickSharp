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

namespace QuickSharp.CodeAssist.CSharp
{
    public class LookupTarget
    {
        public string Name { get; set; }
        public string Entity { get; set; }
        public string FullEntity { get; set; }
        public string LookAhead { get; set; }
        public bool IsIndexed { get; set; }
        public Type Type { get; set; }

        public LookupTarget(string entity, string lookAhead)
        {
            Name = entity;
            Entity = Name;
            LookAhead = lookAhead;
        }

        public LookupTarget(string text)
        {
            text = text.Replace("[]", "¬");

            string[] split =
                text.Split(new char[] { ' ', ';', '(', ')', '{', '}', '<', '!', '|', '&', '=', '[', ']' });
            
            if (split.Length == 0) return;

            Name = split[split.Length - 1];
            Entity = Name;
            LookAhead = String.Empty;
            IsIndexed = false;

            int i = Entity.LastIndexOf('.');

            if (i != -1)
            {
                LookAhead = Entity.Substring(i + 1);
                Entity = Entity.Substring(0, i);

                /*
                 * Set name to type name. If its a variable
                 * it will be overwritten later with the variable
                 * name; if not it's a static and the type name is
                 * the correct name.
                 */

                Name = Entity;

                if (Entity.EndsWith("¬"))
                {
                    IsIndexed = true;
                    Entity = Entity.Remove(Entity.Length - 1);
                }
            }
            else
            {
                LookAhead = Entity;
                Entity = String.Empty;
            }
        }
    }
}
