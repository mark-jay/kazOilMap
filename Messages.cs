using System;
using System.Collections.Generic;
using System.Text;

namespace kazOilMap
{
    /// <summary>
    /// class that contains messages for interaction with user
    /// </summary>
    class Messages
    {
        public static readonly string projectCreated = "Проект создан";
        public static readonly string kazOilMapProject = "newOilMalProject";
        public static readonly string kazOilMapExtension = "kom";
        public static readonly string kazOilMapMainXml = "project.xml";
        public static readonly string kazOilMapFilter = "Oil map project documents (." + 
            kazOilMapExtension + ")|*." + kazOilMapExtension;
    }
}
