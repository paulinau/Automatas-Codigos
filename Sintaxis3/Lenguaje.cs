using System;
using System.Collections.Generic;
using System.Text;

// ✎ Requerimiento 1: Implementar el not en el if.
//    Requerimiento 2: Validar la asignación de strings en instrucción.
//    Requerimiento 3: Implementar la comparacion de tipos de datos en Lista_IDs
//    Requerimiento 4: Validar los tipos de datos en la asignacion del cin
//    Requerimiento 5: Implementar el cast

namespace Sintaxis3
{
    class Lenguaje : Sintaxis
    {
        Stack s;
        ListaVariables l;
        Variable.tipo maxBytes;
        public Lenguaje()
        {
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        // Programa -> Libreria Main
        public void Programa()
        {
            Libreria();
            Main();
            l.Imprime(bitacora);
        }

        // Libreria -> (#include <identificador(.h)?> Libreria) ?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(clasificaciones.identificador);

                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }

                match(">");

                Libreria();
            }
        }

        // Main -> tipo_dato main() BloqueInstrucciones 
        private void Main()
        {
            match(clasificaciones.tipo_dato);
            match("main");
            match("(");
            match(")");

            BloqueInstrucciones(true);
        }

        // BloqueInstrucciones -> { Instrucciones }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match(clasificaciones.inicio_bloque);

            Instrucciones(ejecuta);

            match(clasificaciones.fin_bloque);
        }

        // Lista_IDs -> identificador (= Expresion)? (,Lista_IDs)? 
        private void Lista_IDs(Variable.tipo tipoDato, bool ejecuta)
        {
            string nombre = getContenido();
            match(clasificaciones.identificador); // Validar duplicidad :D

            if (!l.Existe(nombre))
            {
                l.Inserta(nombre, tipoDato);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
            }

            if (getClasificacion() == clasificaciones.asignacion)
            {
                match(clasificaciones.asignacion);

                if (getClasificacion() == clasificaciones.cadena)
                {
                    string valor = getContenido();
                    if(tipoDato == Variable.tipo.STRING)
                    {
                        match(clasificaciones.cadena);
                        if(ejecuta){
                            l.setValor(nombre, valor);
                        }
                    }
                    else
                    {
                        throw new Error(bitacora, "Error semántico: No se puede asignar un STRING a un ("+tipoDato+") en la linea: " + linea + ", caracter: " + caracter);
                    }
                    
                }
                else
                {
                    // Requerimiento 3
                    maxBytes = Variable.tipo.CHAR;
                    Expresion();
                    if(ejecuta)
                    {
                        l.setValor(nombre, s.pop(bitacora, linea, caracter).ToString());
                    }
                }
            }

            if (getContenido() == ",")
            {
                match(",");
                Lista_IDs(tipoDato, ejecuta);
            }
        }

        // Variables -> tipo_dato Lista_IDs; 
        private void Variables(bool ejecuta)
        {
            string tipo = getContenido();
            match(clasificaciones.tipo_dato);

            Variable.tipo tipoDato;
            switch (tipo)
            {
                case "int":
                    tipoDato = Variable.tipo.INT;
                    break;
                case "float":
                    tipoDato = Variable.tipo.FLOAT;
                    break;
                case "string":
                    tipoDato = Variable.tipo.STRING;
                    break;
                case "char":
                    tipoDato = Variable.tipo.CHAR;
                    break;
                default:
                    tipoDato = Variable.tipo.CHAR;
                    break;
            }
            Lista_IDs(tipoDato, ejecuta);
            match(clasificaciones.fin_sentencia);
        }

        // Instruccion -> (If | cin | cout | const | Variables | asignacion) ;
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "do")
            {
                DoWhile(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "cin")
            {
                // Requerimiento 4
                match("cin");
                match(clasificaciones.flujo_entrada);

                string nombre = getContenido();

                if (!l.Existe(nombre))
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }
                else
                {
                    match(clasificaciones.identificador); // Validar existencia :D

                    if(ejecuta)
                    {
                        string valor = Console.ReadLine();
                        l.setValor(nombre, valor);
                    }   
                }

                match(clasificaciones.fin_sentencia);
            }
            else if (getContenido() == "cout")
            {
                match("cout");
                ListaFlujoSalida(ejecuta);
                match(clasificaciones.fin_sentencia);
            }
            else if (getContenido() == "const")
            {
                Constante(ejecuta);
            }
            else if (getClasificacion() == clasificaciones.tipo_dato)
            {
                Variables(ejecuta);
            }
            else
            {
                string nombre = getContenido();

                if (!l.Existe(nombre))
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }
                else
                {
                    match(clasificaciones.identificador); // Validar existencia :D
                }
                match(clasificaciones.asignacion);

                string valor;
                
                // Requerimiento 2
                if (getClasificacion() == clasificaciones.cadena)
                {
                    valor = getContenido();
                    match(clasificaciones.cadena);
                }
                else
                {
                    //Requerimiento 3
                    maxBytes = Variable.tipo.CHAR;
                    Expresion();
                    valor = s.pop(bitacora, linea, caracter).ToString();

                    if(tipoDatoExpresion(float.Parse(valor)) > maxBytes)
                    {
                        maxBytes = tipoDatoExpresion(float.Parse(valor));
                    }

                    if(maxBytes > l.getTipoDato(nombre))
                    {
                        throw new Error(bitacora, "Error semántico: No se puede asignar un " + maxBytes + " a un ("+l.getTipoDato(nombre)+") Linea: " + linea + ", caracter: " + caracter);
                    }
                }
                if(ejecuta)
                {
                    l.setValor(nombre, valor);
                }
                match(clasificaciones.fin_sentencia);
            }
        }

        // Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);

            if (getClasificacion() != clasificaciones.fin_bloque)
            {
                Instrucciones(ejecuta);
            }
        }

        // Constante -> const tipo_dato identificador = numero | cadena;
        private void Constante(bool ejecuta)
        {
            match("const");
            string tipo = getContenido();
            match(clasificaciones.tipo_dato);

            Variable.tipo tipoDato;
            switch (tipo)
            {
                case "int":
                    tipoDato = Variable.tipo.INT;
                    break;
                case "float":
                    tipoDato = Variable.tipo.FLOAT;
                    break;
                case "string":
                    tipoDato = Variable.tipo.STRING;
                    break;
                case "char":
                    tipoDato = Variable.tipo.CHAR;
                    break;
                default:
                    tipoDato = Variable.tipo.CHAR;
                    break;
            }

            string nombre = getContenido();
            match(clasificaciones.identificador); // Validar duplicidad :D

            if (!l.Existe(nombre) && ejecuta)
            {
                l.Inserta(nombre, tipoDato, true);
                
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
            }
            match(clasificaciones.asignacion);

            string valor = getContenido();

            if (getClasificacion() == clasificaciones.numero)
            {
                match(clasificaciones.numero);
            }
            else
            {
                match(clasificaciones.cadena);
            }
            if(ejecuta){
                l.setValor(nombre, valor);
            }
            
            match(clasificaciones.fin_sentencia);
        }

        // ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida(bool ejecuta)
        {
            match(clasificaciones.flujo_salida);

            if (getClasificacion() == clasificaciones.numero)
            {
                if(ejecuta)
                {
                    Console.Write(getContenido());
                }
                
                match(clasificaciones.numero);
            }
            else if (getClasificacion() == clasificaciones.cadena)
            {
                string contenido = getContenido();

                if (contenido.Contains("\""))
                    contenido = contenido.Replace("\"", "");
                if (contenido.Contains("\\n"))
                    contenido = contenido.Replace("\\n", "\n");
                if (contenido.Contains("\\t"))
                    contenido = contenido.Replace("\\t", "\t");

                if(ejecuta)
                {
                    Console.Write(contenido);
                }
                match(clasificaciones.cadena);
            }
            else
            {
                string nombre = getContenido();
                if (l.Existe(nombre))
                {
                    if(ejecuta)
                    {
                        Console.Write(l.getValor(nombre));
                    }
                    match(clasificaciones.identificador); // Validar existencia :D
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }
            }

            if (getClasificacion() == clasificaciones.flujo_salida)
            {
                ListaFlujoSalida(ejecuta);
            }
        }

        // If -> if (Condicion) { BloqueInstrucciones } (else BloqueInstrucciones)?
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta = Condicion();
            
            match(")");
            BloqueInstrucciones(ejecuta && ejecuta2);
            

            if (getContenido() == "else")
            {
                match("else");
                BloqueInstrucciones(!ejecuta && ejecuta2);
            }
        }

        // Condicion -> Expresion operador_relacional Expresion
        private bool Condicion()
        {
            maxBytes = Variable.tipo.CHAR;
            Expresion();
            float n1 = s.pop(bitacora, linea, caracter);
            string operador = getContenido();
            match(clasificaciones.operador_relacional);
            maxBytes = Variable.tipo.CHAR;
            Expresion();
            float n2 = s.pop(bitacora, linea, caracter);

            switch(operador){
                case ">":
                    return n1 > n2;
                case ">=":
                    return n1 >= n2;
                case "<":
                    return n1 < n2;
                case "<=":
                    return n1 <= n2;
                case "==":
                    return n1 == n2;
                default:
                    return n1 != n2;    
            }
        }

        // x26 = (3+5)*8-(10-4)/2;
        // Expresion -> Termino MasTermino 
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (operador_termino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == clasificaciones.operador_termino)
            {
                string operador = getContenido();
                match(clasificaciones.operador_termino);
                Termino();
                float e1 = s.pop(bitacora, linea, caracter), e2 = s.pop(bitacora, linea, caracter);
                // Console.Write(operador + " ");

                switch (operador)
                {
                    case "+":
                        s.push(e2 + e1, bitacora, linea, caracter);
                        break;
                    case "-":
                        s.push(e2 - e1, bitacora, linea, caracter);
                        break;
                }
                s.Display(bitacora);
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (operador_factor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == clasificaciones.operador_factor)
            {
                string operador = getContenido();
                match(clasificaciones.operador_factor);
                Factor();
                float e1 = s.pop(bitacora, linea, caracter), e2 = s.pop(bitacora, linea, caracter);
                // Console.Write(operador + " ");

                switch (operador)
                {
                    case "*":
                        s.push(e2 * e1, bitacora, linea, caracter);
                        break;
                    case "/":
                        s.push(e2 / e1, bitacora, linea, caracter);
                        break;
                }

                s.Display(bitacora);
            }
        }
        // Factor -> identificador | numero | ( Expresion )
        private void Factor()
        {
            if (getClasificacion() == clasificaciones.identificador)
            {
                //Console.Write(getContenido() + " ");

                string nombre = getContenido();

                if (!l.Existe(nombre))
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }
                else
                {
                    s.push(float.Parse(l.getValor(getContenido())), bitacora, linea, caracter);
                    s.Display(bitacora);
                    match(clasificaciones.identificador); // Validar existencia :D

                    if(l.getTipoDato(nombre) > maxBytes)
                    {
                        maxBytes = l.getTipoDato(nombre);
                    }
                }

            }
            else if (getClasificacion() == clasificaciones.numero)
            {
                // Console.Write(getContenido() + " ");
                s.push(float.Parse(getContenido()), bitacora, linea, caracter);
                s.Display(bitacora);
                
                if(tipoDatoExpresion(float.Parse(getContenido())) > maxBytes)
                {
                    maxBytes = tipoDatoExpresion(float.Parse(getContenido()));
                }
                match(clasificaciones.numero);
            }
            else
            {
                match("(");
                bool huboCast = false;

                Variable.tipo tipoDato = Variable.tipo.CHAR;
                if(getClasificacion() == clasificaciones.tipo_dato)
                {
                    huboCast = true;
                    tipoDato = determinarTipoDato(getContenido());
                    match(clasificaciones.tipo_dato);
                    match(")");
                    match("(");
                }
                
                Expresion();
                match(")");
                if(huboCast)
                {
                    // si hubo un cast hacer un pop y convertir ese numero a tipo dato y meterlo al stack
                    float n1 = s.pop(bitacora, linea, caracter);
                    // para convertir un entero a char necesitamos dividir entre 256 y el residuo 
                    // es el resultado del cast. 256 = 0, 257 = 1, 258 = 2, ... 
                    // para convertir un flot a int necesitamos dividir entre 65536 y el residuo 
                    // es el resultado del cast. 
                    // para convertir un float a otro tipo de dato redondear el numero para eliminar
                    // la parte fraccional.
                    // para convertir un flot a chat necesitamos dividir 65536/256 y el residuo 
                    // es el resultado del cast.
                    // para convertir a flotante n1 = n1
                    //n1 = cast(n1, tipoDato);
                    s.push(n1, bitacora, linea, caracter);
                    maxBytes = tipoDato;
                }
            }
        }

        // For -> for (identificador = Expresion; Condicion; identificador incremento_termino) BloqueInstrucciones
        private void For(bool ejecuta)
        {
            match("for");

            match("(");

            string nombre = getContenido();

            if (!l.Existe(nombre))
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
            }
            else
            {
                match(clasificaciones.identificador); // Validar existencia :D
            }
            match(clasificaciones.asignacion);
            Expresion();
            match(clasificaciones.fin_sentencia);

            Condicion();
            match(clasificaciones.fin_sentencia);

            if (!l.Existe(nombre))
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
            }
            else
            {
                match(clasificaciones.identificador); // Validar existencia :D
            }
            match(clasificaciones.incremento_termino);

            match(")");

            BloqueInstrucciones(ejecuta);
        }

        // While -> while (Condicion) BloqueInstrucciones
        private void While(bool ejecuta)
        {
            match("while");

            match("(");
            Condicion();
            match(")");

            BloqueInstrucciones(ejecuta);
        }

        // DoWhile -> do BloqueInstrucciones while (Condicion);
        private void DoWhile(bool ejecuta)
        {
            match("do");

            BloqueInstrucciones(ejecuta);

            match("while");

            match("(");
            Condicion();
            match(")");
            match(clasificaciones.fin_sentencia);
        }

        private Variable.tipo determinarTipoDato(string tipoDato)
        {
            Variable.tipo tipoVar;

            switch(tipoDato)
            {
                case "int":
                    tipoVar = Variable.tipo.INT;
                    break;
                
                case "float":
                    tipoVar = Variable.tipo.FLOAT;
                    break;

                case "string":
                    tipoVar = Variable.tipo.STRING;
                    break;

                default:
                    tipoVar = Variable.tipo.CHAR;  
                    break;                  
            }

            return tipoVar;
        }

        private Variable.tipo tipoDatoExpresion(float valor)
        {
            if(valor % 1 != 0)
            {
                return Variable.tipo.FLOAT;
            }
            else if (valor < 256)
            {
                return Variable.tipo.CHAR;
            }
            else if (valor < 65535)
            {
                return Variable.tipo.INT;
            }
            return Variable.tipo.FLOAT;
        }
    }
}