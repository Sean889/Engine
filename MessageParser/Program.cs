using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace MessageParser
{
    class Program
    {
#pragma warning disable 660
#pragma warning disable 661
        struct DataType
        {
            public string Type;
            public string ID;

            public DataType(string ID, string type)
            {
                this.ID = ID;
                this.Type = type;
            }

            public static bool operator ==(DataType lhs, DataType rhs)
            {
                return lhs.ID == rhs.ID && lhs.Type != rhs.Type;
            }
            public static bool operator !=(DataType lhs, DataType rhs)
            {
                return lhs.ID != rhs.ID;
            }
        }

        struct Pair<T1, T2>
        {
            public T1 First;
            public T2 Second;

            public Pair(T1 _1, T2 _2)
            {
                First = _1;
                Second = _2;
            }
        }

        static void Main(string[] args)
        {
            args = new string[2];

            args[0] = "D:\\Projects\\Projects\\C#\\Engine\\MessageIDs.xml";
            args[1] = "D:\\Projects\\Projects\\C#\\Engine\\Engine\\Messages.cs";

            if(args.Length < 2)
            {
                Console.WriteLine("Need at least 2 console arguments.");
                return;
            }

            List<String> Lines = new List<string>();

            string Path = "";//Environment.CurrentDirectory + "\\";

            XmlDocument Doc = new XmlDocument();
            Doc.Load(Path + args[0]);

            foreach(XmlNode Node in Doc.SelectNodes("//usings//using"))
            {
                Lines.Add("using " + Node.InnerText + ";");
            }

            Lines.Add("");
            Lines.Add("namespace EngineSystem.Messaging");
            Lines.Add("{");

            List<DataType> IDs = new List<DataType>();

            foreach(XmlNode Node in Doc.SelectNodes("//events//event"))
            {
                try
                {
                    XmlNode ID = Node.ChildNodes[0];
                    XmlNode Name = Node.ChildNodes[1];
                    XmlNode Desc = Node.ChildNodes[2];

                    List<Pair<string, string>> Fields = new List<Pair<string, string>>();

                    foreach (XmlNode Field in Node.ChildNodes[3].ChildNodes)
                    {
                        Fields.Add(new Pair<string, string>(Field.ChildNodes[0].InnerText, Field.ChildNodes[1].InnerText));
                    }


                    IDs.Add(new DataType(ID.InnerText, Name.InnerText));

                    Lines.Add("");
                    Lines.Add("\t/// <summary> ");
                    Lines.Add("\t/// " + Desc.InnerText);
                    Lines.Add("\t/// </summary>");
                    Lines.Add("\tpublic class " + Name.InnerText + " : IEvent");
                    Lines.Add("\t{");

                    Lines.Add("\t\tpublic static readonly uint _Id = " + ID.InnerText + ";");

                    foreach (Pair<string, string> Field in Fields)
                    {
                        Lines.Add("\t\tpublic " + Field.First + " " + Field.Second + ";");
                    }

                    Lines.Add("\t\tpublic uint GetID()");
                    Lines.Add("\t\t{");
                    Lines.Add("\t\t\treturn _Id;");
                    Lines.Add("\t\t}");

                    string Args = "\t\tpublic " + Name.InnerText + "(";

                    for(int i = 0; i < Fields.Count; i++)
                    {
                        if ((i < Fields.Count - 1))
                        {
                            Args += (Fields[i].First + " " + Fields[i].Second + ",");
                        }
                        else
                        {
                            Args += (" " + Fields[i].First + " " + Fields[i].Second);
  
                        }
                    }

                    Lines.Add(Args + ")");
                    Lines.Add("\t\t{");

                    foreach (Pair<string, string> Field in Fields)
                    {
                        Lines.Add("\t\t\tthis." + Field.Second + " = " + Field.Second + ";");
                    }

                    Lines.Add("\t\t}");

                    Lines.Add("\t}");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Lines.Add("}");

            

            foreach(DataType D1 in IDs)
            {
                foreach(DataType D2 in IDs)
                {
                    if(D1 == D2)
                    {
                        Console.WriteLine("Duplicate IDs detected: " + D1.Type + " and " + D2.Type + " have the same ID.");
                    }
                }
            }

            StreamWriter Writer = new StreamWriter(Path + args[1]);

            foreach(string str in Lines)
            {
                Writer.WriteLine(str);
            }

            Writer.Close();
        }
    }
}
