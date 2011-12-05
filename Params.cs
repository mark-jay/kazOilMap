using System;
using System.Xml.Serialization;
using System.IO;

namespace kazOilMap
{
    public class Params
    {
        #region parameters declaraion and initialization

        public string myString = "Hello World";
        public int myInt = 1234;
        public string[] myArray = new string[4];
        private int myPrivateInt = 4321;

        #endregion

        public Params()
        {
        }

        public static void ParamsTest()
        {
            string path = "c:/myTestClass.xml";
            Params p = MakeDefaultProject();

            // serializeTest(p, path);

            p = deserializeTest(path);
            Console.WriteLine(p.myInt + " " + p.myArray + " " + p.myString);
        }

        private static void serializeTest(Params p, string path)
        {
            // these lines do the actual serialization
            XmlSerializer serializer = new XmlSerializer(typeof(Params));
            StreamWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, p);
            writer.Close();
        }

        private static Params deserializeTest(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Params));
            StreamReader reader = new StreamReader(path);
            Params p = (Params)serializer.Deserialize(reader);
            reader.Close();
            return p;
        }

        internal static Params MakeDefaultProject()
        {
            Params p = new Params();
            p.myInt = 0;
            p.myArray = new string[] { "This ", "is ", "default ", "string-array" };
            p.myString = "default string";
            return p;
        }
    }
}