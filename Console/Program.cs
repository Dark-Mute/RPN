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
            try
            {
               // Console.WriteLine("podaj: 1. równanie 2. wartość x 3. minimum zakresu 4. maximum zakresu 5. ilość próbek");

                string all = Console.ReadLine();
                Console.Clear();
                RPN rPN = new RPN(all);           
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.Read();
        }        
}

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
        char[] liczby = { '1', '2', '3', '4', '5', '6', '7', '8','9','0','.','x',','};       
  
        public RPN(string all)
        {
            try
            {
                for (int i = 0; i < all.Length; i++)
                  {
                      if (all[i] == '"')
                        all = all.Remove(i, 1);
                  }
                string[] array = all.Split(' ');

                string row = "1 + 1";
                double x = 1;
                double min = 1;
                double max = 1;
                int ammount = 1;

                if (array.Length == 5)
                {
                    row = array[0];
                    x = double.Parse(array[1]);
                    min = double.Parse(array[2]);
                    max = double.Parse(array[3]);
                    ammount = int.Parse(array[4]);
                }
                else
                {
                    throw new InvalidEquasion("Nie podałeś odpowiedniej ilości parametrów");
                }
                row = row.Replace('.', ',');

              

                if (!row.EndsWith("="))
                {
                    row = row + "=";
                }
                string r;
                List<typ> elementynieznane = FindType3(row, double.NaN,out r);
                Console.WriteLine(r);
              
                wyswietl_gotowe(elementynieznane);
                calculate_unnown(x, elementynieznane);
                calculate_unnown_range(min, max, ammount, elementynieznane);

               /* string onw = "";
                for (int o = 0; o < elementynieznane.Count; o++)
                {
                    onw += elementynieznane[o].value + " ";
                }
                    odwroc_rownanie2(onw);*/
            }
            catch (InvalidEquasion iq)
            {
                Console.WriteLine(iq.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

      

        List<typ> FindType2(string row, double nie)
        {
            List<typ> elements = new List<typ>();
            List<Stack<typ>> characters = new List<Stack<typ>>();
            Stack<typ> expressions = new Stack<typ>();
           
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
                            
                        }
                        else
                        {
                            elements.Add(new typ(typy.PI, "PI"));
                            
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
                        
                    }
                    else
                    {
                        elements.Add(new typ(typy.X, "x"));
                       
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
                       
                    }
                    else
                    {
                        elements.Add(new typ(typy.LICZBA, number));
                        
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

        List<typ> FindType3(string row, double nie, out string r)
        {
            List<typ> elements = new List<typ>();
            List<Stack<typ>> characters = new List<Stack<typ>>();
            Stack<typ> expressions = new Stack<typ>();
            r = "";
            characters.Add(new Stack<typ>());
            int eqcounter = 0;
            string number = "";
            typ ischaracter = null;
            bool divide = false, endof = false, numbertaken = false, equasionopen = true, minus = false;
           
            int expression = 0;
            for(int i =0;i<row.Length-2;i++)           
            {

                if (row[i] == '-' && equasionopen)
                {
                    minus = true;
                    row = row.Remove(0, 1);
                    equasionopen = false;
                    continue;
                }

                string ss;
                if (row.Length >= 2)
                {

                    ss = row.Substring(i, 2);
                    if (ss == "PI")
                    {
                        if (numbertaken)
                        {
                            throw new InvalidEquasion("Coś jest źle z równaniem pi");
                        }
                        i += 1;
                        if (minus)
                        {
                            elements.Add(new typ(typy.PI, "-PI"));
                            r += "-PI ";
                        }
                        else
                        {
                            elements.Add(new typ(typy.PI, "PI"));
                            r += "PI ";
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

                if (row[i] == 'x')
                {
                    if (divide && nie == 0 || numbertaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem x");
                    }                    
                    if (minus)
                    {
                        elements.Add(new typ(typy.X, "-x"));
                        r += "-x ";
                    }
                    else
                    {
                        elements.Add(new typ(typy.X, "x"));
                        r += "x ";
                    }
                  
                    minus = false;
                    divide = false;
                    endof = false;
                    numbertaken = true;
                    equasionopen = false;
                    ischaracter = null;
                    continue;
                }


                if (row.IndexOfAny(liczby4, i) == i)
                {
                    number += row[i];
                    i++;
                    equasionopen = false;
                    while (row.IndexOfAny(liczby4, i) == i)
                    {
                        number += row[i];
                        i += 1;
                    }
                    i--;
                    if (divide && double.Parse(number) == 0 || numbertaken)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem liczba");
                    }
                    if (minus)
                    {
                        elements.Add(new typ(typy.LICZBA, '-' + number));
                        r += "-" + number + " ";
                    }
                    else
                    {
                        elements.Add(new typ(typy.LICZBA, number));
                        r += number + " ";
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


                if (row.IndexOfAny(znaki4, i) == i)
                {
                    if (ischaracter != null || minus)
                    {
                        throw new InvalidEquasion("Coś jest źle z równaniem znak");
                    }



                    if (row[i] == '^')
                    {
                        ischaracter = new typ(typy.POTENGA, row[0].ToString());
                    }
                    else
                    {
                        if (row[i].ToString() == "/")
                            divide = true;
                        ischaracter = new typ(typy.ZNAK, row[i].ToString());

                    }
                    r += row[i] + " ";
                    endof = true;

                  

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
                    ss = row.Substring(i, 4);
                    if (ss == "abs(" || ss == "cos(" || ss == "sin(" || ss == "tan(" || ss == "exp(" || ss == "log(")
                    {
                        ss = ss.Substring(0, 3);
                        expressions.Push(new typ(typy.WYRAZENIE, ss));
                       
                        i += 3;

                        r += ss + " ( ";

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
                    ss = row.Substring(i, 5);
                    if (ss == "sqrt(" || ss == "cosh(" || ss == "sinh(" || ss == "tanh(" || ss == "asin(" || ss == "acos(" || ss == "atan(")
                    {
                        ss = ss.Substring(0, 4);
                        expressions.Push(new typ(typy.WYRAZENIE, ss));

                        i += 4;
                        r += ss + " ( ";
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

                if (row[i] == '(')
                {
                    r +="( ";
                    eqcounter++;
                    if (characters.Count <= eqcounter)
                    {
                        characters.Add(new Stack<typ>());
                    }
                    expressions.Push(new typ(typy.NAWIAS, "("));
                  

                    equasionopen = true;
                    continue;
                }

                ///////////////////////////////////////////

                if (row[i] == ')')
                {
                    r += ") ";
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
                        elements.Add(t);

                    }
                    else
                    {

                        while (characters[eqcounter].Count != 0)
                        {
                            elements.Add(characters[eqcounter].Pop());
                        }
                       
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




        void calculate_unnown(double nie, List<typ> elements)
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
                Console.WriteLine(Calculate(backup));

            }
            else
            {
                throw new InvalidEquasion("Coś jest źle z obliczaniem x");
            }
        }

        void calculate_unnown_range(double min, double max, int ammount, List<typ> elements)
        {
            if (max < min)
            {
                double m = max;
                max = min;
                min = m;
            }

           
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
                    Console.WriteLine( "{0} => {1}",min, Calculate(backup));

                    //string x = string.Format("\"x\" : {0}", string.Format("{0:0.0#########################################}", min)).Replace(',', '.');
                    //string y = string.Format(" \"y\" : {0} ", Calculate(backup)).Replace(',', '.');
                    
                    min = min + am;
                }
                
            }
            else
            {
                throw new InvalidEquasion("Coś jest źle z obliczaniem x");
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
