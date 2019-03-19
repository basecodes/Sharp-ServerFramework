using System.Collections.Generic;

namespace Ssc.SscConfiguration {


    public enum Language {
        CSharp,
        Lua,
        Python
    }

    public class SinglePath {
        public string ModuleName { get; set; }
        public Language Language { get; set; }
        public string File { get; set; }
        public string Entry { get; set; }
        public string Search { get; set; }
    }

    public class ProjectConfig {
        public string ProjectName { get; set; }
        public List<SinglePath> Paths { get; set; }
    }
}
