using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderGenerator
{
    class Token
    {
        public string Text;
        public int Value;

        public Token(string Text, int Value)
        {
            this.Text = Text;
            this.Value = Value;
        }

        public static bool operator ==(Token lhs, Token rhs)
        {
            return lhs.Value == rhs.Value; 
        }
        public static bool operator !=(Token lhs, Token rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator ==(Token lhs, int rhs)
        {
            return lhs.Value == rhs;
        }
        public static bool operator !=(Token lhs, int rhs)
        {
            return lhs.Value != rhs;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    class Struct
    {
        public string Name;
        public string Data;

        public Struct(string N, string D)
        {
            Name = N;
            Data = D;
        }
    }
    class Pair<T1, T2> where T1 : IEquatable<T1>
    {
        public T1 first;
        public T2 second;

        public Pair(T1 _1, T2 _2)
        {
            first = _1;
            second = _2;
        }

        public static bool operator ==(Pair<T1, T2> lhs, Pair<T1, T2> rhs)
        {
            return lhs.first.Equals(rhs.first);
        }
        public static bool operator !=(Pair<T1, T2> lhs, Pair<T1, T2> rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() { }
        public InvalidTokenException(string message) : base(message) { }
        public InvalidTokenException(string message, Exception inner) : base(message, inner) { }
        protected InvalidTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    class Lexer
    {
        public static readonly int EOF = -1;
        public static readonly int Comment = 1;
        public static readonly int MultiLineComment = 2;
        public static readonly int WhiteSpace = 3;
        public static readonly int Preprocessor = 4;
        public static readonly int Void = 5;
        public static readonly int DataType = 6;
        public static readonly int LayoutModifier = 7;
        public static readonly int OutputModifier = 8;
        public static readonly int Layout = 9;
        public static readonly int FuncBody = 10;
        public static readonly int Brackets = 11;
        public static readonly int LineEnd = 12;
        public static readonly int Struct = 13;
        public static readonly int Identifer = 14;
        public static readonly int Number = 15;

        #region Arrays
        private static readonly string[] DataTypes = 
        {
            "vec2",
            "vec3",
            "vec4",
            "dvec2",
            "dvec3",
            "dvec4",
            "ivec2", 
            "ivec3",
            "ivec4",
            "uvec2",
            "uvec3",
            "uvec4",
            "bvec2",
            "bvec3",
            "bvec4",
            "float",
            "double",
            "int",
            "uint",
            "bool",
            "mat2",
            "mat3",
            "mat4", 
            "dmat2",
            "dmat3",
            "dmat4",
            "mat2x3",
            "mat2x4",
            "mat3X2",
            "mat3x4",
            "mat4x2",
            "mat4x3",
            "dmat2x3",
            "dmat2x4",
            "dmat3X2",
            "dmat3x4",
            "dmat4x2",
            "dmat4x3",
            "atomic_uint",
            "sampler1D",
            "sampler2D",
            "sampler3D", 
            "samplerCube",
            "sampler2DRect",
            "sampler1DArray",
            "sampler2DArray",
            "samplerCubeArray", 
            "samplerBuffer",
            "sampler2DMS",
            "sampler2DMSArray",
            "usampler",
            "usampler1D",
            "usampler2D",
            "usampler3D", 
            "usamplerCube",
            "usampler2DRect",
            "usampler1DArray",
            "usampler2DArray",
            "usamplerCubeArray", 
            "usamplerBuffer",
            "usampler2DMS",
            "usampler2DMSArray",
            "isampler",
            "isampler1D",
            "isampler2D",
            "isampler3D", 
            "isamplerCube",
            "isampler2DRect",
            "isampler1DArray",
            "isampler2DArray",
            "isamplerCubeArray", 
            "isamplerBuffer",
            "isampler2DMS",
            "isampler2DMSArray",
            "sampler1DShadow",
            "sampler2DShadow", 
            "samplerCubeShadow",
            "sampler2DRectShadow",
            "sampler1DArrayShadow",
            "sampler2DArrayShadow",
            "samplerCubeArrayShadow"
        };
        private static readonly string[] Modifiers =
        {
            "uniform",
            "in",
            "out",
            "inout"
        };
        private static readonly string[] Letters = GetLetters();
        private static readonly string[] Numbers = GetNumbers();
        private static readonly string[] Alphanumerics = GetAlphanumeric();
        private static readonly string[] Hex = GetHex();
        #endregion

        private string InputString;

        public Token CurrentToken;
        public Token NextToken;
        public Token NextNextToken;

        private bool StartsWith(params string[] str)
        {
            for (int i = 0; i < str.Length; i++ )
            {
                if (InputString.StartsWith(str[i]))
                    return true;
            }
            return false;
        }
        private bool StartsWith(out int i, params string[] str)
        {
            for (i = 0; i < str.Length; i++)
            {
                if (InputString.StartsWith(str[i]))
                    return true;
            }
            return false;
        }
        private void Advance(int i)
        {
            InputString = InputString.Substring(i);
        }
        private static string[] GetLetters()
        {
            List<string> Strings = new List<string>();
            for (int i = 'A'; i < 'Z'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            for (int i = 'a'; i < 'z'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            Strings.Add("_");
            return Strings.ToArray();
        }
        private static string[] GetAlphanumeric()
        {
            List<string> Strings = new List<string>();
            for (int i = '0'; i < '9'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            for (int i = 'A'; i < 'Z'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            for (int i = 'a'; i < 'z'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            Strings.Add("_");
            return Strings.ToArray();
        }
        private static string[] GetNumbers()
        {
            List<string> Strings = new List<string>();
            for (int i = '0'; i < '9'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            Strings.Add("_");
            return Strings.ToArray();
        }
        private static string[] GetHex()
        {
            List<string> Strings = new List<string>();
            for (int i = '0'; i < 'f'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            for (int i = 'A'; i < 'F'; i++)
                Strings.Add(new string(Convert.ToChar(i), 1));
            return Strings.ToArray();
        }

        private bool CompareAt(int p, params string[] str)
        {
            for(int i = 0; i < str.Length; i++)
            {
                if (str[i][0] == InputString[p])
                    return true;
            }
            return false;
        }

        public Lexer(string Str)
        {
            InputString = Str;
            EmitToken();
            EmitToken();
            EmitToken();
        }

        public void EmitToken()
        {
            CurrentToken = NextToken;
            NextToken = NextNextToken;

            EmitTokenInternal();
        }

        private void EmitTokenInternal()
        {
            int i;
            //EOF
            if (InputString == "")
            {
                NextNextToken = new Token("", EOF);
                return;
            }

            //Comment
            if (StartsWith("//"))
            {
                int Position = 2;
                while (InputString[Position] != '\n' && InputString[Position] != '\r')
                {
                    Position++;
                }
                Advance(Position);
                EmitTokenInternal();
                return;
            }

            //MultiLineComment
            if (StartsWith("/*"))
            {
                int Position = 2;
                while (InputString[Position] != '*' && InputString[Position + 1] != '/')
                {
                    Position++;
                }
                Position++;
                Advance(Position);
                EmitTokenInternal();
                return;
            }

            //Preprocessor
            if (StartsWith("#"))
            {
                string val = "#";
                int Position = 1;
                while (InputString[Position] != '\n' && InputString[Position] != '\r')
                {
                    val += InputString[Position];
                    Position++;
                }
                NextNextToken = new Token(val, Preprocessor);
                Advance(Position);
                return;
            }

            //WhiteSpace
            if (StartsWith(" ", "\t", "\n", "\r"))
            {
                int Position = 1;
                while (InputString[Position] == ' ' || InputString[Position] == '\t' || InputString[Position] == '\n' || InputString[Position] == '\r')
                {
                    Position++;
                }
                Advance(Position);
                EmitTokenInternal();
                return;
            }

            //FuncBody
            if (StartsWith("{"))
            {
                int Position = 1;
                string output = "{";
                while (InputString[Position] != '}')
                {
                    output += InputString[Position];
                    Position++;
                }
                output += "}";
                Advance(Position + 1);
                NextNextToken = new Token(output, FuncBody);
                return;
            }

            //Brackets
            if (StartsWith("("))
            {
                int Position = 1;
                string output = "(";
                while (InputString[Position] != ')')
                {
                    output += InputString[Position];
                    Position++;
                }
                output += "}";
                Position++;
                Advance(Position);
                NextNextToken = new Token(output, Brackets);
                return;
            }

            //Void
            if (StartsWith("void"))
            {
                Advance(4);
                NextNextToken = new Token("void", Void);
                return;
            }

            //DataType
            if (StartsWith(out i, DataTypes))
            {
                int Position = DataTypes[i].Length;
                NextNextToken = new Token(DataTypes[i], DataType);
                Advance(Position);
                return;
            }

            //MDataTypes
            if (StartsWith(out i, "lowp", "mediump", "highp"))
            {
                i = i == 0 ? 4 : i == 1 ? 7 : 5;
                string OldString = InputString;
                Advance(i);
                if (StartsWith(out i, DataTypes))
                {
                    int Position = DataTypes[i].Length;
                    NextNextToken = new Token(DataTypes[i], DataType);
                    Advance(Position);
                    return;
                }
                InputString = OldString;
            }

            //FlowModifier
            if (StartsWith(out i, Modifiers))
            {
                NextNextToken = new Token(Modifiers[i], LayoutModifier);
                Advance(Modifiers[i].Length);
                return;
            }


            //OutputModifier
            if (StartsWith(out i, "smooth", "flat"))
            {
                if (i == 0)
                {
                    NextNextToken = new Token("smooth", OutputModifier);
                    Advance("smooth".Length);
                }
                else
                {
                    NextNextToken = new Token("flat", OutputModifier);
                    Advance("flat".Length);
                }
                return;
            }

            //Layout
            if (StartsWith("layout"))
            {
                NextNextToken = new Token("layout", Layout);
                Advance(6);
                return;
            }

            //LineEnd
            if (StartsWith(";"))
            {
                NextNextToken = new Token(";", LineEnd);
                Advance(1);
                return;
            }

            //Struct
            if (StartsWith("struct"))
            {
                Advance("struct".Length);
                NextNextToken = new Token("struct", Struct);
                return;
            }

            //Identifier
            if (StartsWith(Letters))
            {
                string output = "";
                output += InputString[0];
                i = 1;
                while (CompareAt(i, Alphanumerics))
                {
                    output += InputString[i];
                    i++;
                }
                Advance(i);
                NextNextToken = new Token(output, Identifer);
                return;
            }

            //Number
            if (StartsWith("0x"))
            {
                string output = "0x";
                i = 2;
                while (CompareAt(i, Hex))
                {
                    output += InputString[i];
                    i++;
                }
                Advance(i);
                NextNextToken = new Token(output, Number);
                return;
            }

            if (StartsWith("0.", "1.", "2.", "3.", "4.", "5.", "6.", "7.", "8.", "9."))
            {
                string output = "";
                output += InputString[0];
                output += '.';
                i = 2;
                while (CompareAt(i, Numbers))
                {
                    output += InputString[i];
                    i++;
                }
                if (CompareAt(i, "f"))
                {
                    i++;
                    output += "f";
                }
                Advance(i);
                NextNextToken = new Token(output, Number);
                return;
            }

            if (StartsWith(Numbers))
            {
                string output = "";
                output += InputString[0];
                i = 1;
                while (CompareAt(i, Numbers))
                {
                    output += InputString[i];
                    i++;
                }
                if (CompareAt(i, "f"))
                {
                    i++;
                    output += "f";
                }
                else if (CompareAt(i, "u"))
                {
                    i++;
                    output += "u";
                }
                Advance(i);
                NextNextToken = new Token(output, Number);
                return;
            }

            throw new InvalidTokenException("Unexpected character found");
        }
    }

    class Parser
    {
        public Lexer Lexer;
        public List<Pair<string, string>> In = new List<Pair<string, string>>();
        public List<Pair<string, string>> Uniform = new List<Pair<string, string>>();
        public List<string> Preprocessor = new List<string>();
        public List<Struct> Structs = new List<Struct>();
        
        private Token CurrentToken
        {
            get
            {
                return Lexer.CurrentToken;
            }
        }
        private Token NextToken
        {
            get
            {
                return Lexer.NextToken;
            }
        }

        public Parser(Lexer TokenSource)
        {
            Lexer = TokenSource;
        }

        public void Parse()
        {
            while(Lexer.NextToken != Lexer.EOF)
            {
                line_st();
            }
        }

        private void line_st()
        {
            if(is_sh_var_st())
            {
                string Mod = CurrentToken.Text;
                Lexer.EmitToken();
                string Type = CurrentToken.Text;
                string Id = NextToken.Text;

                if(Mod == "uniform")
                {
                    Uniform.Add(new Pair<string, string>(Id, Type));
                }
                else if(Mod == "in")
                {
                    In.Add(new Pair<string, string>(Id, Type));
                }
                Lexer.EmitToken();
                Lexer.EmitToken();
            }
            else if(CurrentToken == Lexer.Preprocessor)
            {
                Preprocessor.Add(CurrentToken.Text);
                Lexer.EmitToken();
            }
            else if(is_func_decl())
            {
                Lexer.EmitToken();
                Lexer.EmitToken();
                Lexer.EmitToken();
                Lexer.EmitToken();
            }
            else if(is_var_st())
            {
                Lexer.EmitToken();
                Lexer.EmitToken();
                Lexer.EmitToken();
            }
            else if(is_struct())
            {
                Lexer.EmitToken();
                Structs.Add(new Struct(CurrentToken.Text, NextToken.Text));
                Lexer.EmitToken();
                Lexer.EmitToken();
                Lexer.EmitToken();
            }
            else
            {
                Lexer.EmitToken();
            }
        }

        private bool is_sh_var_st()
        {
            if (CurrentToken == Lexer.Layout && NextToken == Lexer.Brackets)
            {
                Lexer.EmitToken();
                Lexer.EmitToken();
                return true;
            }
            if(CurrentToken == Lexer.LayoutModifier && NextToken == Lexer.DataType)
            {
                return true;
            }
            if(CurrentToken == Lexer.OutputModifier && NextToken == Lexer.LayoutModifier && Lexer.NextNextToken == Lexer.DataType)
            {
                Lexer.EmitToken();
                return true;
            }
            return false;
        }
        private bool is_func_decl()
        {
            if((CurrentToken == Lexer.Void || CurrentToken == Lexer.DataType) && NextToken == Lexer.Identifer && Lexer.NextNextToken == Lexer.Brackets)
            {
                return true;
            }
            return false;
        }
        private bool is_var_st()
        {
            return (CurrentToken == Lexer.DataType && NextToken == Lexer.Identifer);
        }
        private bool is_struct()
        {
            return CurrentToken == Lexer.Struct;
        }
    }
}
