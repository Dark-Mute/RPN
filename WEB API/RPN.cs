using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace serwer
{
    enum typy
    {
        LICZBA,
        ZNAK,
        X,
        WYRAZENIE,
        POTENGA,
        NAWIAS,
        PI
    };

    class typ
    {
        public typ(typy t, string v)
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
        { }
    }

    class RPN
    {
        char[] znaki = { '+', '/', '*', '^' };
        char[] liczby = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', 'x', ',' };

        public RPN()
        { }

       
        public Models.Imodel formula(string row)
        {
            List<string> infix;

            try
            {
                row = row.Replace('.', ',');

                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] == ' ')
                        row = row.Remove(i, 1);
                }

                if (!row.EndsWith("="))
                {
                    row = row + "=";
                }

                List<typ> elementynieznane = FindType2(row, double.NaN, out infix);
                List<string> s = new List<string>();
                foreach(typ t in elementynieznane)
                {
                    s.Add(t.value);
                }
                
                Models.tokentsresult result = new Models.tokentsresult();
                result.infix = infix.ToArray();
                result.rpn = s.ToArray();

                Models.results model = new Models.results();
                model.status = "ok";
                model.result = result;
                return model;
            }
            catch (InvalidEquasion iq)
            {
                Models.error model = new Models.error();
                model.status = "error";
                model.message = iq.Message;
                return model;

            }
            catch (Exception ex)
            {
                Models.error model = new Models.error();
                model.status = "error";
                model.message = ex.Message;
                return model;

            }

        }

        public Models.Imodel formula(string row, double x)
        {
            StringBuilder sb = new StringBuilder();
            List<string> infix;

            try
            {
               
                row = row.Replace('.', ',');

                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] == ' ')
                        row = row.Remove(i, 1);
                }

                if (!row.EndsWith("="))
                {
                    row = row + "=";
                }

                List<typ> elementynieznane = FindType2(row, x,out infix);
                string result = calculate_unnown(x, elementynieznane);

                Models.results model = new Models.results();
                model.status = "ok";
                model.result = double.Parse(result);
                return model;

            }
            catch (InvalidEquasion iq)
            {
                Models.error model = new Models.error();
                model.status = "error";
                model.message = iq.Message;
                return model;

            }
            catch (Exception ex)
            {
                Models.error model = new Models.error();
                model.status = "error";
                model.message = ex.Message;
                return model;

            }

        }

        public Models.Imodel formula(string row, double min, double max, int ammount)
        {
            List<string> infix;

           
            StringBuilder sb = new StringBuilder();
            try
            {
                
                row = row.Replace('.', ',');

                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] == ' ')
                        row = row.Remove(i, 1);
                }

                if (!row.EndsWith("="))
                {
                    row = row + "=";
                }

                List<typ> elementynieznane = FindType2(row, double.NaN,out infix);
                List<Models.range> result = calculate_unnown_range(min, max, ammount, elementynieznane);

                Models.results model = new Models.results();
                model.status = "ok";
                model.result = result.ToArray();
                return model;

            }
            catch (InvalidEquasion iq)
            {
                Models.error model = new Models.error();
                model.status = "error";
                model.message = iq.Message;
                return model;

            }
            catch (Exception ex)
            {
                Models.error model = new Models.error();
                model.status = "error";
                model.message = ex.Message;
                return model;

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

        char[] znaki4 = { '-', '+', '/', '*', '^' };
        char[] liczby4 = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ',' };

        List<typ> FindType2(string row, double nie, out List<string> infix)
        {
            List<typ> elements = new List<typ>();
            List<Stack<typ>> characters = new List<Stack<typ>>();
            Stack<typ> expressions = new Stack<typ>();
            infix = new List<string>();
            characters.Add(new Stack<typ>());
            int eqcounter = 0;
            string number = "";
            typ ischaracter = null;
            bool divide = false, endof = false, numbertaken = false, equasionopen = true, minus = false;
            int expression = 0;
            while (row[0] != '=')
            {

                if (row[0] == '-' && equasionopen)
                {
                    minus = true;
                    row = row.Remove(0, 1);
                    equasionopen = false;
                    continue;
                }

                string ss;
                if (row.Length >= 2)
                {
                   
                    ss = row.Substring(0, 2);
                    if (ss == "PI")
                    {
                        if (numbertaken)
                        {
                            throw new InvalidEquasion("Coś jest źle z równaniem pi");
                        }
                        row = row.Remove(0, 2);
                        if (minus)
                        {
                            elements.Add(new typ(typy.PI, "-PI"));
                            infix.Add("-PI");
                        }
                        else
                        {
                            elements.Add(new typ(typy.PI, "PI"));
                            infix.Add("PI");
                        }
                        minus = false;
                        divide = false;
                        endof = false;
                        numbertaken = true;
                        equasionopen = false;
                        ischaracter = null;
                        continue;
                    }
                }

                if (row[0] == 'x')
                {
                    if (divide && nie == 0 || numbertaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem x");
                    }
                    row = row.Remove(0, 1);
                    if (minus)
                    {
                        elements.Add(new typ(typy.X, "-x"));
                        infix.Add("-x");
                    }
                    else
                    {
                        elements.Add(new typ(typy.X, "x"));
                        infix.Add("x");
                    }
                    minus = false;
                    divide = false;
                    endof = false;
                    numbertaken = true;
                    equasionopen = false;
                    ischaracter = null;
                    continue;
                }


                if (row.IndexOfAny(liczby4, 0) == 0)
                {
                    number += row[0];
                    row = row.Remove(0, 1);
                
                    equasionopen = false;
                    while (row.IndexOfAny(liczby4, 0) == 0)
                    {
                        number += row[0];
                        row = row.Remove(0, 1);
                    }

                    if (divide && double.Parse(number) == 0 || numbertaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem liczba");
                    }
                    if (minus)
                    {
                        elements.Add(new typ(typy.LICZBA, '-' + number));
                        infix.Add('-' + number);
                    }
                    else
                    {
                        elements.Add(new typ(typy.LICZBA, number));
                        infix.Add(number);
                    }
                    
                    number = "";
                    minus = false;
                    divide = false;
                    endof = false;
                    numbertaken = true;
                    equasionopen = false;
                    ischaracter = null;
                    continue;

                }
              
                //////////////////////////////////////////////////


                if (row.IndexOfAny(znaki4, 0) == 0)
                {
                    if (ischaracter != null || minus)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem znak");
                    }

                    infix.Add(row[0].ToString());

                    if (row[0] == '^')
                    {
                        ischaracter = new typ(typy.POTENGA, row[0].ToString());
                    }
                    else
                    {
                        if (row[0].ToString() == "/")
                            divide = true;
                        ischaracter = new typ(typy.ZNAK, row[0].ToString());

                    }
                    endof = true;

                    row = row.Remove(0, 1);

                    typ z = null;

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
                    numbertaken = false;

                    continue;
                }
                ///////////////////////////////////////////





                if (row.Length >= 4)
                {
                    ss = row.Substring(0, 4);
                    if (ss == "abs(" || ss == "cos(" || ss == "sin(" || ss == "tan(" || ss == "exp(" || ss == "log(")
                    {
                        ss = ss.Substring(0, 3);
                        expressions.Push(new typ(typy.WYRAZENIE, ss));
                        infix.Add(ss);
                        row = row.Remove(0, 4);

                        numbertaken = false;
                        ischaracter = null;
                        equasionopen = true;
                        eqcounter++;
                        expression++;
                        if (characters.Count <= eqcounter)
                        {
                            characters.Add(new Stack<typ>());
                        }
                        continue;
                    }
                }

                else if (row.Length >= 5)
                {
                    ss = row.Substring(0, 5);
                    if (ss == "sqrt(" || ss == "cosh(" || ss == "sinh(" || ss == "tanh(" || ss == "asin(" || ss == "acos(" || ss == "atan(")
                    {
                        ss = ss.Substring(0, 4);
                        expressions.Push(new typ(typy.WYRAZENIE, ss));
                        infix.Add(ss);
                        row = row.Remove(0, 5);

                        numbertaken = false;
                        ischaracter = null;
                        equasionopen = true;
                        eqcounter++;
                        expression++;
                        if (characters.Count <= eqcounter)
                        {
                            characters.Add(new Stack<typ>());
                        }
                        continue;
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
                    expressions.Push(new typ(typy.NAWIAS, "("));
                    row = row.Remove(0, 1);
                    equasionopen = true;
                    continue;
                }

                ///////////////////////////////////////////

                if (row[0] == ')')
                {
                    if (!numbertaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem nawias zam");
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

                    continue;
                }
                throw new InvalidEquasion("równanie ma niepoprawne znaki koniec" + row);
            }

            while (characters[eqcounter].Count != 0)
            {
                elements.Add(characters[eqcounter].Pop());
            }

            if (eqcounter != 0 || endof || divide || !numbertaken)
            {
                throw new InvalidEquasion("Coś jest źle z równaniem po koncu");
            }



            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].value == "")
                {
                    elements.RemoveAt(i);
                }
                if (elements[i].value == "PI")
                {
                    elements[i] = new typ(typy.PI, Math.PI.ToString());
                }
                if (elements[i].value == "-PI")
                {
                    elements[i] = new typ(typy.PI, "-" + Math.PI.ToString());
                }
            }
            return elements;
        }

        string calculate_unnown(double nie, List<typ> elements)
        {
            if (elements != null)
            {
                List<typ> backup = deepcopy(elements);
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].typ_of == typy.X)
                    {
                        if (elements[i].value == "-x")
                            backup[i] = new typ(typy.LICZBA, '-' + nie.ToString());
                        else
                            backup[i] = new typ(typy.LICZBA, nie.ToString());
                    }
                }
                return Calculate(backup);
            }
            return string.Empty;             
        }

      
        List<Models.range> calculate_unnown_range(double min, double max, int ammount, List<typ> elements)
        {
            if (max < min)
            {
                double m = max;
                max = min;
                min = m;
            }

            List<Models.range> results = new List<Models.range>();
            if (elements != null)
            {
                List<typ> backup = deepcopy(elements);
                double am = (max - min) / (ammount - 1);

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
                    results.Add(new Models.range(min, double.Parse(Calculate(backup))));
                    min = min + am;
                }
               
            }
            return results;
        }

        string Calculate(List<typ> elements)
        {
            if (elements != null)
            {
                double typ1, typ2;
                Stack<typ> equasion = new Stack<typ>();

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].typ_of == typy.LICZBA || elements[i].typ_of == typy.X || elements[i].typ_of == typy.PI)
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
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-CA");

                return string.Format("{0:0.0#########################################}", double.Parse(equasion.Pop().value));

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

        char[] znaki2 = { '+', '/', '*', '^', '-' };
        char[] liczby2 = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'x' };
        char[] liczby3 = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        void odwroc_rownanie2(string onp)
        {
            if (onp != null)
            {
                string[] elementy = onp.Split(' ');
                string typ1, typ2;
                StringBuilder sb = new StringBuilder();

                Stack<string> store = new Stack<string>();

                for (int i = 0; i < elementy.Length; i++)
                {
                    if (elementy[i] == "PI" || elementy[i].IndexOfAny(liczby2) == 0 || elementy[i].IndexOfAny(liczby3) == 1)
                    {
                        store.Push(elementy[i]);
                    }
                    else if (elementy[i].IndexOfAny(znaki2) == 0)
                    {
                        typ1 = store.Pop();
                        typ2 = store.Pop();

                        switch (elementy[i])
                        {
                            case "-":
                                store.Push(sb.AppendFormat("({0}-{1})", typ2, typ1).ToString());
                                break;
                            case "+":                              
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
                                store.Push(sb.AppendFormat("({0}/{1})", typ2, typ1).ToString());
                                break;
                            case "^":
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
                                break;
                        }
                        sb.Clear();
                    }
                    else if (elementy[i] != "")
                    {
                        typ1 = store.Pop();
                        switch (elementy[i])
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
                }
                Console.WriteLine(store.Pop().Replace(',', '.'));
            }
        }

        string calculatesigns(double o, double t, string znak)
        {
            switch (znak)
            {
                case "-": return (o - t).ToString();
                case "+": return (o + t).ToString();
                case "*": return (o * t).ToString();
                case "/": if (t == 0) throw new InvalidEquasion("Coś jest źle z równaniem dziel przez 0"); return (o / t).ToString();
            }
            return "";
        }

        string calculateexpressions(double o, string wyrazenie)
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
