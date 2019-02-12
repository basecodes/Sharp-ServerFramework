﻿using System.Collections.Generic;

namespace Ssc.SscConfiguration {


    public enum Language {
        CSharp,
        Lua,
        Python
    }

    public class SinglePath {
        public Language Language { get; set; }
        public string File { get; set; }
        public string Entry { get; set; }
        public string Search { get; set; }
    }

    public class PathConfig {
        public List<SinglePath> Paths { get; set; }
    }
}