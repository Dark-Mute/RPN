using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine( "podaj: 1. równanie 2. wartość x 3. minimum zakresu 4. maximum zakresu 5. ilość próbek");

                string row = Console.ReadLine();
                double x = 1;
                double min = 2;
                double max = 4;
                int ammo = 5;
                /*
                double x = double.Parse(Console.ReadLine());
                double min = double.Parse(Console.ReadLine());
                double max = double.Parse(Console.ReadLine());
                int ammo = int.Parse(Console.ReadLine());
                 */

                for (int i = 0;i<row.Length;i++)
                {
                    if(row[i]==' ')
                        row = row.Remove(i, 1);  
                }

                if(!row.EndsWith("="))
                {
                    row = row + "=";
                }
                RPN rPN = new RPN(row, x,min,max,ammo);
                Console.ReadLine();
            }
        }
    }

    enum typy
    {
        LICZBA,      
        ZNAK,
        X,
        WYRAZENIE,
        POTENGA,
        NAWIAS
    };

    class typ
    {
        public typ(typy t,string v)
        {
            typ_of = t;
            value = v;
        }
        public string value { get; private set; }
        public typy typ_of { get; private set; }
    }

    class InvalidEquasion : Exception
    {
        public InvalidEquasion(string message) : base(message)
        {}
    }

    class RPN
    {
        char[] znaki = {'+','/','*' ,'^'};        
        char[] liczby = { '1', '2', '3', '4', '5', '6', '7', '8','9','0','.','x'};       
  
        public RPN(string s, double nie, double min,double max,int ammount)
        {
            try
            {
                List<typ> elementynieznane = FindType(s,nie);
                wyswietl_gotowe(elementynieznane);
                calculate_unnown(nie, elementynieznane);
                calculate_unnown_range(min, max, ammount, elementynieznane);
                odwroc_rownanie(elementynieznane);
            }
            catch (InvalidEquasion iq)
            {
                Console.WriteLine(iq.Message);
            }
        }

        List<typ> deepcopy(List<typ> elementy)
        {
            List<typ> temp = new List<typ>();
            for (int j = 0; j < elementy.Count; j++)
            {
                temp.Add(new typ(elementy[j].typ_of, elementy[j].value));
            }
            return temp;
        }

        List<typ> FindType(string row,double nie)
        {
            List<typ> elements = new List<typ>();
            List<Stack<typ>> characters = new List<Stack<typ>>();
            Stack<typ> expressions = new Stack<typ>();

            characters.Add(new Stack<typ>());
            int eqcounter = 0;
            string number = "";
            typ ischaracter = null;
            bool divide = false, endof = false,numbertaken=false, signtaken=true, bracketopen =true;
            int expression = 0;
            while (row[0] != '=')
            {              
                if ((signtaken || bracketopen) && row[0] == '-')
                {
                    number += '-';
                    row = row.Remove(0, 1);
                    signtaken = false;
                    bracketopen = false;
                    continue;
                }
               

                if (row.IndexOfAny(liczby, 0) == 0)
                {
                    if (number == "--") throw new InvalidEquasion("Coś jest źle z równaniem");
                    if (row[0] == 'x')
                    {
                        if (divide && nie == 0)
                        {
                            throw new InvalidEquasion("Coś jest źle z równaniem");
                        }
                        number += 'x';
                        row = row.Remove(0, 1);
                        signtaken = false;
                        bracketopen = false;
                        continue;

                    }
                    else 
                    {
                        number += row[0];
                        row = row.Remove(0, 1);
                        signtaken = false;
                        bracketopen = false;
                        continue;
                    }
                   

                }
                else
                {                                       
                    if (number != "")
                    {
                        if (number.Contains("x"))
                        {
                            elements.Add(new typ(typy.X, number));
                        }
                        else
                        {
                            if (divide && double.Parse(number) == 0)
                            {
                                throw new InvalidEquasion("Coś jest źle z równaniem");
                            }

                            elements.Add(new typ(typy.LICZBA, number));
                        }
                        number = "";
                        divide = false;
                        endof = false;
                        numbertaken = true;
                       
                    }

                }


                if (row[0] == '-')
                {
                    number += '-';
                    row = row.Remove(0, 1);
                    row = '+' + row;
                    signtaken = false;
                    bracketopen = false;

                }

                ///////////////////////////////////////////

                if (row.IndexOfAny(znaki, 0) == 0)
                {
                    
                    if (!numbertaken  || signtaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem");
                    }


                    if (row[0] == '^')
                    {
                        ischaracter = new typ(typy.POTENGA, row[0].ToString());
                    }
                    else
                    {
                        ischaracter = new typ(typy.ZNAK, row[0].ToString());
                        if (row[0].ToString() == "/")
                            divide = true;
                    }             
                    endof = true;
                    numbertaken = false;
                    signtaken = true;
                    row = row.Remove(0, 1);
                }

                ////////////////////////////////////////////

                if (ischaracter != null)
                {
                    typ z = null; ;

                    if (characters[eqcounter].Count > 0)
                        z = characters[eqcounter].Pop();

                    if (z != null)
                    {
                        if (ischaracter.value == "+" || ischaracter.value == "-")
                        {
                            elements.Add(z);
                            z = null;
                        }
                        if (ischaracter.value == "*" || ischaracter.value == "/")
                        {
                            if (z.value == "*" || z.value == "/" || z.value == "^")
                            {
                                elements.Add(z);
                                z = null;
                            }
                        }
                        if (ischaracter.value == "^")
                        {
                            if (z.value == "^")
                            {
                                elements.Add(z);
                                z = null;
                            }
                        }
                    }

                    if (z != null)
                    {
                        characters[eqcounter].Push(z);
                    }

                    characters[eqcounter].Push(ischaracter);
                    ischaracter = null;
                    continue;
                }

                ///////////////////////////////////////////

                string ss;
                if (row.Length >= 4)
                {
                    ss = row.Substring(0, 4);
                    if (ss == "abs(" || ss == "cox(" || ss == "sin(" || ss == "tan(" || ss == "exp(" || ss == "log(")
                    {
                        ss = ss.Substring(0, 3);
                        expressions.Push(new typ(typy.WYRAZENIE, ss));
                        row = row.Remove(0, 4);
                        expression++;
                        signtaken = false;
                        bracketopen = true;
                        eqcounter++;
                        if (characters.Count <= eqcounter)
                        {
                            characters.Add(new Stack<typ>());
                        }
                    }
                }

                else if (row.Length >= 5 )
                {
                    ss = row.Substring(0, 5);
                    if (ss == "sqrt(" || ss == "cosh(" || ss == "sinh(" || ss == "tanh(" || ss == "asin(" || ss == "acos(" || ss == "atan(")
                    {
                        ss = ss.Substring(0, 4);
                        expressions.Push(new typ(typy.WYRAZENIE, ss));
                        row = row.Remove(0, 5);
                        expression++;
                        signtaken = false;
                        bracketopen = true;
                        eqcounter++;
                        if (characters.Count <= eqcounter)
                        {
                            characters.Add(new Stack<typ>());
                        }
                    }
                }

                

                ///////////////////////////////////////////

                if (row[0] == '(')
                {
                    eqcounter++;
                    if (characters.Count <= eqcounter)
                    {
                        characters.Add(new Stack<typ>());
                    }
                    expressions.Push(new typ(typy.NAWIAS, ")"));
                    row = row.Remove(0, 1);
                    bracketopen = true;
                    continue;
                }

                ///////////////////////////////////////////

                if (row[0] == ')')
                {
                    if(signtaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem");
                    }
                    typ t = expressions.Pop();
                    if (t.typ_of == typy.WYRAZENIE)
                    {
                        
                        while (characters[eqcounter].Count != 0)
                        {
                            elements.Add(characters[eqcounter].Pop());
                        }
                        expression--;
                        eqcounter--;
                        row = row.Remove(0, 1);
                        elements.Add(t);
                        
                    }
                    else
                    {
                       
                        while (characters[eqcounter].Count != 0)
                        {
                            elements.Add(characters[eqcounter].Pop());
                        }
                        row = row.Remove(0, 1);
                        eqcounter--;
                    }
                    numbertaken = true;
                }   
            }
            if (number != "")
            {
                elements.Add(new typ(typy.LICZBA, number));
                divide = false;
                endof = false;
                numbertaken = true;
            }

            while (characters[eqcounter].Count != 0)
            {
                elements.Add(characters[eqcounter].Pop());
            }

            if (eqcounter!=0 || endof || divide)
            {
                throw new InvalidEquasion("Coś jest źle z równaniem");   
            }

         

            for(int i=0;i< elements.Count;i++)
            {
                if (elements[i].value == "")
                {
                    elements.RemoveAt(i);
                }
            }
            return elements;
        }

        void calculate_unnown(double nie, List<typ> elements)
        {
            if (elements != null)
            {
                List<typ> backup = deepcopy(elements);
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].typ_of == typy.X)
                    {
                        if(elements[i].value=="-x")
                            backup[i] = new typ(typy.LICZBA, '-' + nie.ToString());
                        else
                            backup[i] = new typ(typy.LICZBA, nie.ToString());
                    }
                }
                Console.WriteLine(Calculate(backup));
            }
        }

        void calculate_unnown_range(double min, double max, int ammount,List<typ> elements )
        {
            if(max<min)
            {
                double m = max;
                max = min;
                min = m;
            }
            if (elements != null)
            {
                List<typ> backup = deepcopy(elements);
                double am = (max - min) / (ammount-1);
               
                for (double i = 0; i < ammount; i++)
                {
                    
                    for (int j = 0; j < elements.Count; j++)
                    {
                        if (elements[j].typ_of == typy.X)
                        {
                            if (elements[j].value == "-x")
                                backup[j] = new typ(typy.LICZBA, '-' + min.ToString());
                            else
                                backup[j] = new typ(typy.LICZBA, min.ToString());
                        }
                    }
                    Console.WriteLine( "{0} => {1}" ,min, Calculate(backup));
                    min = min + am;
                }
            }
        }

        string Calculate(List<typ> elements)
        {
            if (elements != null)
            {
                double typ1, typ2;            
                Stack<typ> equasion = new Stack<typ>();

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].typ_of == typy.LICZBA || elements[i].typ_of == typy.X)
                    {
                        equasion.Push(elements[i]);
                    }
                    else if (elements[i].typ_of == typy.ZNAK)
                    {                      
                        if (equasion.Count() >= 2 && double.TryParse(equasion.Pop().value, out typ1) && double.TryParse(equasion.Pop().value, out typ2))
                            equasion.Push(new typ(typy.LICZBA, calculatesigns(typ2, typ1, elements[i].value)));     
                    }
                    else if (elements[i].typ_of == typy.WYRAZENIE)
                    {
                       
                        if (equasion.Count() >= 1 && double.TryParse(equasion.Pop().value, out typ1))
                            equasion.Push(new typ(typy.LICZBA, calculateexpressions(typ1, elements[i].value)));
    
                    }
                    else if (elements[i].typ_of == typy.POTENGA)
                    {
                      
                        if (equasion.Count() >= 2 && double.TryParse(equasion.Pop().value, out typ1) && double.TryParse(equasion.Pop().value, out typ2))
                            equasion.Push(new typ(typy.LICZBA, Math.Pow(typ2, typ1).ToString()));                                           
                    } 
                }
                return equasion.Pop().value;            
            }
            return "null";
        }

        void wyswietl_gotowe(List<typ> elements)
        {
            if (elements != null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < elements.Count; i++)
                {
                    stringBuilder.Append(elements[i].value);
                    stringBuilder.Append(" ");
                }
                Console.WriteLine(stringBuilder.ToString());
            }
        }

        void odwroc_rownanie(List<typ> elements)
        {
            if (elements != null)
            {
                string typ1, typ2;
                StringBuilder sb = new StringBuilder();
                  
                Stack<string> store = new Stack<string>();

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].typ_of == typy.LICZBA || elements[i].typ_of == typy.X)
                    {
                        store.Push(elements[i].value);
                    }
                    else if (elements[i].typ_of == typy.ZNAK   )
                    {                     
                        typ1 = store.Pop();
                        typ2 = store.Pop();

                        switch (elements[i].value)
                        {                           
                            case "+":
                                if (typ1.Contains("-"))
                                    store.Push(sb.AppendFormat("({0}{1})", typ2, typ1).ToString());
                                else
                                    store.Push(sb.AppendFormat("({0}+{1})", typ2, typ1).ToString());
                                break;
                            case "*":
                                if (typ2.Contains("+") || typ2.Contains("-"))
                                {
                                    typ2 = sb.AppendFormat("({0})", typ2).ToString();
                                    sb.Clear();
                                }
                                if (typ1.Contains("+") || typ1.Contains("-"))
                                {
                                    typ1 = sb.AppendFormat("({0})", typ1).ToString();
                                    sb.Clear();
                                }
                                store.Push(sb.AppendFormat("({0}*{1})", typ2, typ1).ToString());
                                break;
                            case "/":
                                if (typ2.Contains("+") || typ2.Contains("-"))
                                {
                                    typ2 = sb.AppendFormat("({0})", typ2).ToString();
                                    sb.Clear();
                                }
                                if (typ1.Contains("+") || typ1.Contains("-"))
                                {
                                    typ1 = sb.AppendFormat("({0})", typ1).ToString();
                                    sb.Clear();
                                }
                                store.Push(sb.AppendFormat("({0}/{1})", typ2,typ1).ToString());
                                break;
                        }
                        sb.Clear();
                    }
                    else if (elements[i].typ_of == typy.WYRAZENIE )
                    {                       
                        typ1 = store.Pop();
                        switch (elements[i].value)
                        {
                           
                            case "cos": store.Push(sb.AppendFormat("cos({0})", typ1).ToString()); break;
                            case "sin": store.Push(sb.AppendFormat("sin({0})", typ1).ToString()); break;
                            case "abs": store.Push(sb.AppendFormat("abs({0})", typ1).ToString()); break;
                            case "tan": store.Push(sb.AppendFormat("tan({0})", typ1).ToString()); break;
                            case "exp": store.Push(sb.AppendFormat("exp({0})", typ1).ToString()); break;
                            case "log": store.Push(sb.AppendFormat("log({0})", typ1).ToString()); break;
                            case "sqrt": store.Push(sb.AppendFormat("sqrt({0})", typ1).ToString()); break;
                            case "cosh": store.Push(sb.AppendFormat("cosh({0})", typ1).ToString()); break;
                            case "sinh": store.Push(sb.AppendFormat("sinh({0})", typ1).ToString()); break;
                            case "tanh": store.Push(sb.AppendFormat("tanh({0})", typ1).ToString()); break;
                            case "asin": store.Push(sb.AppendFormat("asin({0})", typ1).ToString()); break;
                            case "acos": store.Push(sb.AppendFormat("acos({0})", typ1).ToString()); break;
                            case "atan": store.Push(sb.AppendFormat("atan({0})", typ1).ToString()); break;
                        }
                        sb.Clear();
                    }
                    else if (elements[i].typ_of == typy.POTENGA )
                    {                      
                        typ1 = store.Pop();
                        typ2 = store.Pop();
                        if (typ1.Length >= 3)
                        {
                            typ1 = sb.AppendFormat("({0})", typ1).ToString();
                            sb.Clear();
                        }
                        if (typ2.Length >= 3)
                        {
                            typ2 = sb.AppendFormat("({0})", typ2).ToString();
                            sb.Clear();
                        }
                        store.Push(sb.AppendFormat("({0}^{1})", typ2, typ1).ToString());
                        sb.Clear();
                    }                 
                }
                Console.WriteLine(store.Pop());
            }
        }
        
        string calculatesigns(double o,double t,string znak)
        {
            switch(znak)
            {
                case "-": return (o - t).ToString();
                case "+": return (o + t).ToString();
                case "*": return (o * t).ToString();
                case "/": return (o / t).ToString();                   
            }
            return "";
        }

        string calculateexpressions(double o,  string wyrazenie)
        {
            switch (wyrazenie)
            {
                case "cos": return Math.Cos(o).ToString();
                case "sin": return Math.Sin(o).ToString();
                case "abs": return Math.Abs(o).ToString();
                case "tan": return Math.Tan(o).ToString();
                case "exp": return Math.Exp(o).ToString();
                case "log": return Math.Log(o).ToString();
                case "sqrt": return Math.Sqrt(o).ToString();
                case "cosh": return Math.Cosh(o).ToString();
                case "sinh": return Math.Sinh(o).ToString();
                case "tanh": return Math.Tanh(o).ToString();
                case "asin": return Math.Asin(o).ToString();
                case "acos": return Math.Acos(o).ToString();
                case "atan": return Math.Atan(o).ToString();
              
            }
            return "";
        }
    }
}
