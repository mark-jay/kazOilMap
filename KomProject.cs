using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;

namespace kazOilMap
{
    class KomProject
    {
        private Params innerParamz;
        private Point[][] innerResult = null;

        public KomProject(Params paramz)
        {
            this.innerParamz = paramz;
        }

        public KomProject(Params paramz, Point[][] result)
        {
            this.innerParamz = paramz;
            this.innerResult = result;
        }

        public static KomProject makeKomProject(string filepath)
        {
            string xmlPath = filepath + Path.DirectorySeparatorChar + Messages.kazOilMapMainXml;
            if (File.Exists(xmlPath))
            {
                // FIXME if we have output we should use it, but now we don't
                return new KomProject(Params.Deserialize(xmlPath));
            }
            return null;
        }

        public Params paramz
        {
            get
            {
                return innerParamz;
            }
        }

        public Point[][] result
        {
            get
            {
                if (innerResult == null)
                {
                    innerResult = Utils.toIsolines(Calc.RunCalc(innerParamz));
                }
                return innerResult;
            }
        }
    }
}
