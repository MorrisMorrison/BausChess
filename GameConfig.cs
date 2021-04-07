using System;
using System.IO;
using System.Net;
using SimpleConfigAccess.Config;

namespace BausChess
{
    public class GameConfig:Config
    {
        public GameConfig(string p_pathToConfigFile, string p_separator = ":") : base(p_pathToConfigFile, p_separator)
        {
        }
        
        
    }
}