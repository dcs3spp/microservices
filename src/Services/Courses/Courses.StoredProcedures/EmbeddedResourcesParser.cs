using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace dcs3spp.courseManagementContainers.Services.Courses.StoredProcedures
{
    public class EmbeddedResourceParser
    {
        private readonly List<Match> _createScripts;
        private readonly List<Match> _dropScripts;
        private Regex _regEx;
        public Assembly Assembly { get; private set; }
        public IReadOnlyCollection<Match> CreateScripts => _createScripts;
        public IReadOnlyCollection<Match> DropScripts => _dropScripts;
        public string RegularExpression { get; private set; }

        protected EmbeddedResourceParser()
        {
            _createScripts = new List<Match>();
            _dropScripts = new List<Match>();
        }

        public EmbeddedResourceParser (Assembly assembly, string regularExpression) : this()
        {
            this.Assembly = assembly;
            this.RegularExpression = regularExpression;
            this._regEx = new Regex(regularExpression, RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public void Parse()
        {
            if(_createScripts.Count > 0 || _dropScripts.Count > 0)
                ClearScripts();
            
            string[] resNames = this.Assembly.GetManifestResourceNames();
            string text = string.Join("\n",resNames);
            
            MatchCollection matches = this._regEx.Matches(text);
            foreach(Match match in matches)
            {
                GroupCollection col = match.Groups;
                if(col["create"].Captures.Count > 0)
                    _createScripts.Add(match);
                else if (col["drop"].Captures.Count > 0)
                    _dropScripts.Add(match);
            }
        }

        private void ClearScripts()
        {
            this._createScripts.Clear();
            this._dropScripts.Clear();
        }
    }
}
