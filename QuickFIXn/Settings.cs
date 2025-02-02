﻿#nullable enable
using System.Collections.Generic;

namespace QuickFix
{
    public class Settings
    {
        private readonly LinkedList<QuickFix.Dictionary> _sections = new();

        public Settings(System.IO.TextReader conf)
        {
            QuickFix.Dictionary? currentSection = null;

            string? line;
            while ((line = conf.ReadLine()) != null)
            {
                line = line.Trim();
                if (IsComment(line))
                {
                    continue;
                }

                if (IsSection(line))
                {
                    currentSection = Add(new Dictionary(SplitSection(line)));
                }
                else if (IsKeyValue(line) && currentSection != null)
                {
                    string[] kv = line.Split(new char[] { '=' }, 2);
                    currentSection.SetString(kv[0].Trim(), kv[1].Trim());
                }
            }
        }

        /// <summary>
        /// Strip the outer '[' and ']' from the section name, e.g. '[DEFAULT]' becomes 'DEFAULT'
        /// </summary>
        /// <param name="s">the section name</param>
        /// <returns></returns>
        public static string SplitSection(string s)
        {
            return s.Trim('[', ']').Trim();
        }

        public static bool IsComment(string s)
        {
            if (s.Length < 1)
                return false;
            return '#' == s[0];
        }

        public static bool IsKeyValue(string s)
        {
            return s.IndexOf('=') != -1;
        }

        public static bool IsSection(string s)
        {
            if (s.Length < 2)
                return false;
            return s[0] == '[' && s[^1] == ']';
        }

        public QuickFix.Dictionary Add(QuickFix.Dictionary section)
        {
            _sections.AddLast(section);
            return section;
        }

        /// <summary>
        /// Retrieve dictionaries by section name
        /// (e.g. 'SESSION' would return a list containing each "[SESSION]" Dictionary)
        /// </summary>
        /// <param name="sectionName">(case is ignored)</param>
        /// <returns></returns>
        public LinkedList<QuickFix.Dictionary> Get(string sectionName)
        {
            LinkedList<QuickFix.Dictionary> result = new();
            foreach (QuickFix.Dictionary dict in _sections)
                if (sectionName.ToUpperInvariant() == dict.Name.ToUpperInvariant())
                    result.AddLast(dict);
            return result;
        }
    }
}
