using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
         static void Main(string[] args)
        {
            //Console.WriteLine("<FORMULA> <X> <FROM> <TO> <N>");

            //string all = Console.ReadLine();
            
            Console.Clear();
            RPN rPN = new RPN(args);           
            Console.Read();
        }        
    }

    enum elementTypes
    {
        NUMBER,
        SIGN,
        X,
        EXPRESSION,
        POWER,
        BRACKET
    };

    class elementType
    {
        public elementType(elementTypes t, string v)
        {
            elementtype = t;
            value = v;
        }
        public string value { get; private set; }
        public elementTypes elementtype { get; private set; }
    }

    class EquasionException : Exception
    {
        public EquasionException(string message) : base(message)
        {}
    }

    class RPN
    {
        public RPN(string[] args)
        {
            try
            {
                if(args==null || args.Length!=5)
                   throw new EquasionException("Nie podałeś odpowiedniej ilości parametrów");

                string infix = "";
                List<elementType> onp = CreateONP(args[0].Replace('.', ','), ref infix);             
                Console.WriteLine(infix);

                ShowReady(onp);
                
                Console.WriteLine(CalculateUnknown(double.Parse(args[1].Replace('.', ',')), onp));

                List<string> results = CalculateUnknownRange(double.Parse(args[2].Replace('.', ',')), double.Parse(args[3].Replace('.', ',')), int.Parse(args[4].Replace('.', ',')), onp);
                foreach(string r in results)
                {
                    Console.WriteLine(r);
                }
                
          
            }
            catch (EquasionException exe)
            {
                Console.WriteLine(exe.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        
        Dictionary<string, int> priority = new Dictionary<string, int>()
        {
            {"-",1},
            {"+",1},
            {"*",2},
            {"/",2},
            {"^",3}
        };

        List<elementType> Deepcopy(List<elementType> elements)
        {
            List<elementType> temp = new List<elementType>();
            for (int j = 0; j < elements.Count; j++)
            {
                temp.Add(new elementType(elements[j].elementtype, elements[j].value));
            }
            return temp;
        }

        char[] signs = { '-', '+', '/', '*', '^' };
        char[] numbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ',' };

        List<elementType> CreateONP(string row, ref string infix)
        {
            List<elementType> elements = new List<elementType>();
            List<Stack<elementType>> characters = new List<Stack<elementType>>();
            Stack<elementType> sincosStack = new Stack<elementType>();
            characters.Add(new Stack<elementType>());           
            int bracketCounter = 0, sincosCounter = 0;
            string number = "", minus = "", tempString;
            elementType characterTemp = null;
            bool divide = false, umberIsTaken = false, bracketIsOpen = true;

            void numbers()
            {
                minus = "";
                divide = false;
                umberIsTaken = true;
                bracketIsOpen = false;
            }

            void equasions()
            {
                umberIsTaken = false;
                bracketIsOpen = true;
                minus = "";
                bracketCounter++;
                sincosCounter++;
                if (characters.Count <= bracketCounter)
                {
                    characters.Add(new Stack<elementType>());
                }
            }

            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == '-' && bracketIsOpen)
                {
                    minus = "-";
                    bracketIsOpen = false;
                    continue;
                }

                if (row.Length >= i + 2 && row.Substring(i, 2) == "PI")
                {
                    if (umberIsTaken)
                        throw new EquasionException("Coś jest źle z równaniem pi");

                    i += 1;
                    elements.Add(new elementType(elementTypes.NUMBER, minus + Math.PI.ToString()));
                    infix += minus + "PI ";
                    numbers();
                    continue;
                }

                if (row[i] == 'x')
                {
                    if (umberIsTaken)
                        throw new EquasionException("Coś jest źle z równaniem lub /0");

                    elements.Add(new elementType(elementTypes.X, minus + "x"));
                    infix += minus + "x ";
                    numbers();
                    continue;
                }

                if (row.IndexOfAny(this.numbers, i) == i)
                {
                    while (row.IndexOfAny(this.numbers, i) == i)
                    {
                        number += row[i];
                        i++;
                    }
                    i--;
                    if (divide && double.Parse(number) == 0 || umberIsTaken || number == "," || number.IndexOf(",") != number.LastIndexOf(","))
                        throw new EquasionException("Coś jest źle z równaniem  ");

                    elements.Add(new elementType(elementTypes.NUMBER, minus + number));
                    infix += minus + number + " ";
                    number = "";
                    numbers();
                    continue;
                }

                if (row.IndexOfAny(signs, i) == i)
                {
                    if (!umberIsTaken || minus == "-" || bracketIsOpen)
                        throw new EquasionException("Coś jest źle z równaniem znak");

                    if (row[i] == '^')
                        characterTemp = new elementType(elementTypes.POWER, row[i].ToString());
                    else
                    {
                        if (row[i].ToString() == "/")
                            divide = true;
                        characterTemp = new elementType(elementTypes.SIGN, row[i].ToString());
                    }
                    infix += row[i].ToString() + " ";
                    elementType z = null;

                    int p1, p2;
                    priority.TryGetValue(characterTemp.value, out p1);
                    while (characters[bracketCounter].Count > 0)
                    {
                        z = characters[bracketCounter].Pop();
                        priority.TryGetValue(z.value, out p2);
                        if (p1 <= p2)
                        {
                            elements.Add(z);
                        }
                        else
                        {
                            characters[bracketCounter].Push(z);
                            break;
                        }
                    }

                    characters[bracketCounter].Push(characterTemp);
                    umberIsTaken = false;
                    continue;
                }

                if (row.Length >= i + 5)
                {
                    tempString = row.Substring(i, 4);
                    if (tempString == "abs(" || tempString == "cos(" || tempString == "sin(" || tempString == "tan(" || tempString == "exp(" || tempString == "log(")
                    {
                        tempString = minus + tempString.Substring(0, 3);
                        sincosStack.Push(new elementType(elementTypes.EXPRESSION, tempString));
                        infix += tempString + " ( ";
                        i += 3;
                        equasions();
                        continue;
                    }
                    tempString = row.Substring(i, 5);
                    if (tempString == "sqrt(" || tempString == "cosh(" || tempString == "sinh(" || tempString == "tanh(" || tempString == "asin(" || tempString == "acos(" || tempString == "atan(")
                    {
                        tempString = minus + tempString.Substring(0, 4);
                        sincosStack.Push(new elementType(elementTypes.EXPRESSION, tempString));
                        infix += tempString + " ( ";
                        i += 4;
                        equasions();
                        continue;
                    }
                }

                if (row[i] == '(')
                {
                    infix += "( ";
                    bracketCounter++;
                    if (characters.Count <= bracketCounter)
                    {
                        characters.Add(new Stack<elementType>());
                    }
                    sincosStack.Push(new elementType(elementTypes.BRACKET, "("));
                    bracketIsOpen = true;
                    continue;
                }

                if (row[i] == ')')
                {
                    infix += ") ";
                    if (!umberIsTaken || sincosStack.Count <= 0)
                        throw new EquasionException("Coś jest źle z równaniem nawias zamykającym");

                    elementType t = sincosStack.Pop();
                    while (characters[bracketCounter].Count != 0)
                    {
                        elements.Add(characters[bracketCounter].Pop());
                    }
                    bracketCounter--;
                    if (t.elementtype == elementTypes.EXPRESSION)
                    {
                        sincosCounter--;
                        elements.Add(t);
                    }
                    continue;

                }
                throw new EquasionException("równanie ma niepoprawne znaki w miejscu zaznaczonym < błąd >: " + row.Substring(0, i) + "< " + row[i] + " >" + row.Substring(i + 1));
            }

            if (bracketCounter != 0 || !umberIsTaken || divide)
            {
                throw new EquasionException("Coś jest źle z równaniem brak nawiasu lub za durza ilość, na końcu +,-,*,/,^");
            }

            while (characters[bracketCounter].Count != 0)
            {
                elements.Add(characters[bracketCounter].Pop());
            }

            return elements;
        }

        double CalculateUnknown(double x, List<elementType> elements)
        {
            List<elementType> backup = Deepcopy(elements);
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].elementtype == elementTypes.X)
                {
                    if (elements[i].value == "-x")
                        backup[i] = new elementType(elementTypes.NUMBER, '-' + x.ToString());
                    else
                        backup[i] = new elementType(elementTypes.NUMBER, x.ToString());
                }
            }
            double temp = Calculate(backup);
            if (double.IsNaN(temp))
                throw new EquasionException("Wynik jest zbyt wysoki");
            return temp;
        }

        List<string> CalculateUnknownRange(double min, double max, int ammount, List<elementType> elements)
        {
            List<string> results = new List<string>();
            List<elementType> backup = Deepcopy(elements);
            double am = (max - min) / (ammount - 1);
            am = Math.Round(am, 15);
            for (double i = 0; i < ammount; i++)
            {

                for (int j = 0; j < elements.Count; j++)
                {
                    if (elements[j].elementtype == elementTypes.X)
                    {
                        if (elements[j].value == "-x")
                            backup[j] = new elementType(elementTypes.NUMBER, '-' + min.ToString());
                        else
                            backup[j] = new elementType(elementTypes.NUMBER, min.ToString());
                    }
                }
                double temp = Calculate(backup);
                if (double.IsNaN(temp))
                {
                    results.Add(string.Format("{0} => {1}", min, "Wynik jest zbyt wysoki"));
                }
                else
                {
                    results.Add(string.Format("{0} => {1}", min, Calculate(backup)));
                }
                
                   
                min += am;
            }
            return results;
        }

        double Calculate(List<elementType> elements)
        {
            double typ1, typ2;
            Stack<elementType> equasion = new Stack<elementType>();

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].elementtype == elementTypes.NUMBER || elements[i].elementtype == elementTypes.X)
                {
                    equasion.Push(elements[i]);
                }
                else if (elements[i].elementtype == elementTypes.SIGN)
                {
                    if (equasion.Count() >= 2 && double.TryParse(equasion.Pop().value, out typ1) && double.TryParse(equasion.Pop().value, out typ2))
                        equasion.Push(new elementType(elementTypes.NUMBER, CalculateSigns(typ2, typ1, elements[i].value)));
                }
                else if (elements[i].elementtype == elementTypes.EXPRESSION)
                {
                    if (equasion.Count() >= 1 && double.TryParse(equasion.Pop().value, out typ1))
                    {
                        equasion.Push(new elementType(elementTypes.NUMBER, CalculateSinCos(typ1, elements[i].value)));
                    }
                }
                else if (elements[i].elementtype == elementTypes.POWER)
                {
                    if (equasion.Count() >= 2 && double.TryParse(equasion.Pop().value, out typ1) && double.TryParse(equasion.Pop().value, out typ2))
                        equasion.Push(new elementType(elementTypes.NUMBER, Math.Pow(typ2, typ1).ToString()));
                }
            }
            return double.Parse(equasion.Pop().value);
        }

        void ShowReady(List<elementType> elements)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < elements.Count; i++)
                stringBuilder.Append(elements[i].value + " ");

            Console.WriteLine(stringBuilder.ToString());
        }

        char[] signs2 = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        void ReversONP(string onp)
        {
            if (onp != null)
            {
                string[] elements = onp.Split(' ');
                string one, two;
                StringBuilder stringBuilder = new StringBuilder();

                Stack<string> store = new Stack<string>();

                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] == "PI" || elements[i] == "x" || elements[i].IndexOfAny(signs2) == 0 && elements[i].Length > 0)
                    {
                        store.Push(elements[i]);
                    }
                    else if (elements[i].IndexOfAny(signs) == 0)
                    {
                        one = store.Pop();
                        two = store.Pop();

                        switch (elements[i])
                        {
                            case "-":
                                store.Push(stringBuilder.AppendFormat("({0}-{1})", two, one).ToString());
                                break;
                            case "+":
                                store.Push(stringBuilder.AppendFormat("({0}+{1})", two, one).ToString());
                                break;
                            case "*":
                                if (two.Contains("+") || two.Contains("-"))
                                {
                                    two = stringBuilder.AppendFormat("({0})", two).ToString();
                                    stringBuilder.Clear();
                                }
                                if (one.Contains("+") || one.Contains("-"))
                                {
                                    one = stringBuilder.AppendFormat("({0})", one).ToString();
                                    stringBuilder.Clear();
                                }
                                store.Push(stringBuilder.AppendFormat("({0}*{1})", two, one).ToString());
                                break;
                            case "/":
                                if (two.Contains("+") || two.Contains("-"))
                                {
                                    two = stringBuilder.AppendFormat("({0})", two).ToString();
                                    stringBuilder.Clear();
                                }
                                if (one.Contains("+") || one.Contains("-"))
                                {
                                    one = stringBuilder.AppendFormat("({0})", one).ToString();
                                    stringBuilder.Clear();
                                }
                                store.Push(stringBuilder.AppendFormat("({0}/{1})", two, one).ToString());
                                break;
                            case "^":
                                if (one.Length >= 3)
                                {
                                    one = stringBuilder.AppendFormat("({0})", one).ToString();
                                    stringBuilder.Clear();
                                }
                                if (two.Length >= 3)
                                {
                                    two = stringBuilder.AppendFormat("({0})", two).ToString();
                                    stringBuilder.Clear();
                                }
                                store.Push(stringBuilder.AppendFormat("({0}^{1})", two, one).ToString());
                                stringBuilder.Clear();
                                break;
                        }
                        stringBuilder.Clear();
                    }
                    else if (elements[i] != "")
                    {
                        one = store.Pop();
                        switch (elements[i])
                        {
                            case "cos": store.Push(stringBuilder.AppendFormat("cos({0})", one).ToString()); break;
                            case "sin": store.Push(stringBuilder.AppendFormat("sin({0})", one).ToString()); break;
                            case "abs": store.Push(stringBuilder.AppendFormat("abs({0})", one).ToString()); break;
                            case "tan": store.Push(stringBuilder.AppendFormat("tan({0})", one).ToString()); break;
                            case "exp": store.Push(stringBuilder.AppendFormat("exp({0})", one).ToString()); break;
                            case "log": store.Push(stringBuilder.AppendFormat("log({0})", one).ToString()); break;
                            case "sqrt": store.Push(stringBuilder.AppendFormat("sqrt({0})", one).ToString()); break;
                            case "cosh": store.Push(stringBuilder.AppendFormat("cosh({0})", one).ToString()); break;
                            case "sinh": store.Push(stringBuilder.AppendFormat("sinh({0})", one).ToString()); break;
                            case "tanh": store.Push(stringBuilder.AppendFormat("tanh({0})", one).ToString()); break;
                            case "asin": store.Push(stringBuilder.AppendFormat("asin({0})", one).ToString()); break;
                            case "acos": store.Push(stringBuilder.AppendFormat("acos({0})", one).ToString()); break;
                            case "atan": store.Push(stringBuilder.AppendFormat("atan({0})", one).ToString()); break;

                        }
                        stringBuilder.Clear();
                    }
                }
                Console.WriteLine(store.Pop().Replace(',', '.'));
            }
        }

        string CalculateSigns(double one, double two, string signs)
        {
            switch (signs)
            {
                case "-":
                    return (one - two).ToString();
                case "+":
                    return (one + two).ToString();
                case "*":
                    return (one * two).ToString();
                case "/":
                    if (two == 0)
                        throw new EquasionException("Coś jest źle z równaniem dziel przez 0");
                    return (one / two).ToString();
            }
            return "";
        }

        string CalculateSinCos(double one, string word)
        {
            if (word.Contains("-"))
            {
                one *= -1;
                word = word.Substring(1, word.Length - 1);
            }
            switch (word)
            {
                case "cos":
                    return (Math.Cos(one)).ToString();
                case "sin":
                    return (Math.Sin(one)).ToString();
                case "abs":
                    return (Math.Abs(one)).ToString();
                case "tan":
                    return (Math.Tan(one)).ToString();
                case "exp":
                    return (Math.Exp(one)).ToString();
                case "log":
                    return (Math.Log(one)).ToString();
                case "sqrt":
                    return (Math.Sqrt(one)).ToString();
                case "cosh":
                    return (Math.Cosh(one)).ToString();
                case "sinh":
                    return (Math.Sinh(one)).ToString();
                case "tanh":
                    return (Math.Tanh(one)).ToString();
                case "asin":
                    if (one > 1 || one < -1)
                        throw new EquasionException("wartość w asin() przekracza dziedzinę funkcji");
                    return (Math.Asin(one)).ToString();
                case "acos":
                    if (one > 1 || one < -1)
                        throw new EquasionException("wartość w acos() przekracza dziedzinę  funkcji");
                    return (Math.Acos(one)).ToString();
                case "atan":
                    if (one > 1 || one < -1)
                        throw new EquasionException("wartość w atan() przekracza dziedzinę  funkcji");
                    return (Math.Atan(one)).ToString();
            }
            return "";
        }
    }

}
