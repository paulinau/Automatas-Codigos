using System;
using System.Collections.Generic;
using System.Text;

// ✎ 
//  ✎ Requerimiento 1: Programar el residuo de la division en PorFactor
//                   (para c++ y para ensamblador) y hacer un salto de linea cuando se imprima un \n
//  ✎ Requerimiento 2: Programar (en ensamblador) el else. Sera necesario agregar etiquetas.
//                   (asi como en el if, el contador sería prácticamente el mismo)
//  Requerimiento 3: Agregar (en ensamblador) la negacion de la condicion
//  ✎ Requerimiento 4: Declarar variables en el for (int i)
//  ✎ Requerimiento 5: Actualizar la variable del for con += y -=
namespace Ensamblador
{
    class Lenguaje : Sintaxis
    {
        Stack s;
        ListaVariables l;
        Variable.tipo maxBytes;
        int numero_if;
        int numero_for;
        int numero_else;
        public Lenguaje()
        {
            s = new Stack(5);
            l = new ListaVariables();
            numero_if = numero_for = 0;
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack(5);
            l = new ListaVariables();
            numero_if = numero_for = 0;
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        // Programa -> Libreria Main
        public void Programa()
        {
            asm.WriteLine("include \"emu8086.inc\"");
            asm.WriteLine("org 100h");
            Libreria();
            Main();
            asm.WriteLine("ret");
            asm.WriteLine("define_print_string");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num_uns");
            asm.WriteLine("define_scan_num");
            asm.WriteLine("; variables");
            l.Imprime(bitacora, asm);
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

            if (!l.Existe(nombre))
            {
                match(clasificaciones.identificador);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
            }
            l.Inserta(nombre, tipoDato);

            if (getClasificacion() == clasificaciones.asignacion)
            {
                match(clasificaciones.asignacion);

                if (getClasificacion() == clasificaciones.cadena)
                {
                    if (tipoDato == Variable.tipo.STRING)
                    {
                        match(clasificaciones.cadena);
                        if (ejecuta)
                        {
                            l.setValor(nombre, getContenido());
                        }
                    }
                    else
                    {
                        throw new Error(bitacora, "Error semántico: No se puede asignar un STRING a un (" + tipoDato + ") en la linea: " + linea + ", caracter: " + caracter);
                    }
                }
                else
                {
                    Expresion();
                    maxBytes = Variable.tipo.CHAR;

                    string valor = s.pop(bitacora, linea, caracter).ToString();
                    asm.WriteLine("\tPOP CX");

                    if (tipoDatoExpresion(float.Parse(valor)) > maxBytes)
                    {
                        maxBytes = tipoDatoExpresion(float.Parse(valor));
                    }

                    if (maxBytes > l.getTipoDato(nombre))
                    {
                        throw new Error(bitacora, "Error semántico: No se puede asignar un " + maxBytes + " a un (" + l.getTipoDato(nombre) + ") Linea: " + linea + ", caracter: " + caracter);
                    }

                    asm.WriteLine("\tMOV "+nombre+", CX");

                    if (ejecuta)
                    {
                        l.setValor(nombre, valor);
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

            Lista_IDs(determinarTipoDato(tipo), ejecuta);
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
                match("cin");
                match(clasificaciones.flujo_entrada);

                string nombre = getContenido();

                if (!l.Existe(nombre))
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }
                else
                {
                    asm.WriteLine("\tcall scan_num");
                    asm.WriteLine("\tMOV " + nombre + ", CX");
                    asm.WriteLine("\tprintn \"\"");
                    if (ejecuta)
                    {
                        match(clasificaciones.identificador);
                        string valor = Console.ReadLine();
                        //asm.WriteLine("\tprintn \"\"");

                        if (tipoDatoExpresion(float.Parse(valor)) > maxBytes)
                        {
                            maxBytes = tipoDatoExpresion(float.Parse(valor));
                        }
                        if (maxBytes > l.getTipoDato(nombre))
                        {
                            throw new Error(bitacora, "Error semántico: No se puede asignar un " + maxBytes + " a un (" + l.getTipoDato(nombre) + ") Linea: " + linea + ", caracter: " + caracter);
                        }
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
                    match(clasificaciones.identificador);
                }
                match(clasificaciones.asignacion);

                string valor;

                if (getClasificacion() == clasificaciones.cadena)
                {
                    valor = getContenido();

                    if (l.getTipoDato(nombre) == Variable.tipo.STRING)
                    {
                        match(clasificaciones.cadena);
                        if (ejecuta)
                        {
                            l.setValor(nombre, valor);
                        }
                    }
                    else
                    {
                        throw new Error(bitacora, "Error semántico: No se puede asignar un STRING a un (" + l.getTipoDato(nombre) + ") en la linea: " + linea + ", caracter: " + caracter);
                    }
                }
                else
                {
                    maxBytes = Variable.tipo.CHAR;
                    Expresion();
                    valor = s.pop(bitacora, linea, caracter).ToString();
                    asm.WriteLine("\tPOP CX");

                    if (tipoDatoExpresion(float.Parse(valor)) > maxBytes)
                    {
                        maxBytes = tipoDatoExpresion(float.Parse(valor));
                    }

                    if (maxBytes > l.getTipoDato(nombre))
                    {
                        throw new Error(bitacora, "Error semántico: No se puede asignar un " + maxBytes + " a un (" + l.getTipoDato(nombre) + ") Linea: " + linea + ", caracter: " + caracter);
                    }
                }
                asm.WriteLine("\tMOV "+nombre+", CX");

                if (ejecuta)
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

            string nombre = getContenido();

            if (!l.Existe(nombre) && ejecuta)
            {
                match(clasificaciones.identificador);
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
            }
            l.Inserta(nombre, determinarTipoDato(tipo), true);
            match(clasificaciones.asignacion);

            string valor = getContenido();

            if (getClasificacion() == clasificaciones.numero)
            {
                if (ejecuta)
                {
                    l.setValor(nombre, valor);
                }
                match(clasificaciones.numero);
            }
            else
            {
                if (ejecuta)
                {
                    l.setValor(nombre, valor);
                }
                match(clasificaciones.cadena);
            }
            match(clasificaciones.fin_sentencia);
        }

        // ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida(bool ejecuta)
        {
            match(clasificaciones.flujo_salida);

            if (getClasificacion() == clasificaciones.numero)
            {
                asm.WriteLine("MOV AX, "+getContenido());
                asm.WriteLine("call print_num");
                if (ejecuta)
                {
                    Console.Write(getContenido());
                }
                match(clasificaciones.numero);
            }
            else if (getClasificacion() == clasificaciones.cadena)
            {
                string contenido = getContenido();
                string contenido2 = "\tprint" + contenido;
                
                if (contenido.Contains("\""))
                    contenido = contenido.Replace("\"", "");
                if (contenido.Contains("\\n"))
                {
                    contenido = contenido.Replace("\\n", "\n");
                    contenido2 = contenido2.Replace("\\n", "\"\n\t printn \"");
                }
                if (contenido.Contains("\\t"))
                    contenido = contenido.Replace("\\t", "\t");

                asm.WriteLine(contenido2);
                if (ejecuta)
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
                    asm.WriteLine("MOV AX, "+nombre);
                    asm.WriteLine("call print_num");

                    if (ejecuta)
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
            bool ejecuta;
            string etiqueta = "if"+numero_if++;
            string etiqueta2 = "else"+numero_else++;

            if (getContenido() == "!")
            {
                match(clasificaciones.operador_logico);
                match("(");
                ejecuta = !Condicion(etiqueta);
                match(")");
            }
            else
            {
                ejecuta = Condicion(etiqueta);
            }

            match(")");
            BloqueInstrucciones(ejecuta && ejecuta2);
            asm.WriteLine(etiqueta+":");

            if (getContenido() == "else")
            {
                if(ejecuta)
                    asm.WriteLine("\tJMP "+etiqueta2);
                match("else");
                BloqueInstrucciones(!ejecuta && ejecuta2);
                asm.WriteLine(etiqueta2+":");
            }
        }

        // Condicion -> Expresion operador_relacional Expresion
        private bool Condicion(string etiqueta)
        {
            maxBytes = Variable.tipo.CHAR;
            Expresion();
            float n1 = s.pop(bitacora, linea, caracter);
            asm.WriteLine("\tPOP CX");
            string operador = getContenido();
            match(clasificaciones.operador_relacional);
            maxBytes = Variable.tipo.CHAR;
            Expresion();
            float n2 = s.pop(bitacora, linea, caracter);
            asm.WriteLine("\tPOP BX");

            asm.WriteLine("\tCMP CX, BX");

            switch (operador)
            {
                case ">":
                    asm.WriteLine("\tJLE "+etiqueta);
                    return n1 > n2;
                case ">=":
                    asm.WriteLine("\tJL "+etiqueta);
                    return n1 >= n2;
                case "<":
                    asm.WriteLine("\tJGE "+etiqueta);
                    return n1 < n2;
                case "<=":
                    asm.WriteLine("\tJG "+etiqueta);
                    return n1 <= n2;
                case "==":
                    asm.WriteLine("\tJNE "+etiqueta);
                    return n1 == n2;
                default:
                    asm.WriteLine("\tJE "+etiqueta);
                    return n1 != n2;
            }
        }

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
                asm.WriteLine("\tPOP BX");
                asm.WriteLine("\tPOP AX");
                // Console.Write(operador + " ");

                switch (operador)
                {
                    case "+":
                        asm.WriteLine("\tADD AX, BX");
                        s.push(e2 + e1, bitacora, linea, caracter);
                        asm.WriteLine("\tPUSH AX");
                        break;
                    case "-":
                        asm.WriteLine("\tSUB AX, BX");
                        s.push(e2 - e1, bitacora, linea, caracter);
                        asm.WriteLine("\tPUSH AX");
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
                asm.WriteLine("\tPOP BX");
                asm.WriteLine("\tPOP AX");
                // Console.Write(operador + " ");

                switch (operador)
                {
                    case "*":
                        asm.WriteLine("\tMUL BX");
                        s.push(e2 * e1, bitacora, linea, caracter);
                        asm.WriteLine("\tPUSH AX");
                        break;
                    case "/":
                        asm.WriteLine("\tDIV BX");
                        s.push(e2 / e1, bitacora, linea, caracter);
                        asm.WriteLine("\tPUSH AX");
                        break;
                    case "%":
                        asm.WriteLine("\tDIV BX");
                        s.push(e2 % e1, bitacora, linea, caracter);
                        asm.WriteLine("\tPUSH DX");
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
                    asm.WriteLine("\tMOV AX, "+nombre);
                    asm.WriteLine("\tPUSH AX");
                    s.Display(bitacora);
                    match(clasificaciones.identificador); // Validar existencia :D

                    if (l.getTipoDato(nombre) > maxBytes)
                    {
                        maxBytes = l.getTipoDato(nombre);
                    }
                }

            }
            else if (getClasificacion() == clasificaciones.numero)
            {
                // Console.Write(getContenido() + " ");
                s.push(float.Parse(getContenido()), bitacora, linea, caracter);
                asm.WriteLine("\tMOV AX, "+getContenido());
                asm.WriteLine("\tPUSH AX");
                s.Display(bitacora);

                if (tipoDatoExpresion(float.Parse(getContenido())) > maxBytes)
                {
                    maxBytes = tipoDatoExpresion(float.Parse(getContenido()));
                }
                match(clasificaciones.numero);
            }
            else
            {
                match("(");
                Variable.tipo tipoDato = Variable.tipo.CHAR;
                bool huboCast = false;

                if (getClasificacion() == clasificaciones.tipo_dato)
                {
                    huboCast = true;
                    tipoDato = determinarTipoDato(getContenido());
                    match(clasificaciones.tipo_dato);
                    match(")");
                    match("(");
                }

                Expresion();
                match(")");
                if (huboCast)
                {
                    // si hubo un cast hacer un pop y convertir ese numero a tipo dato y meterlo al stack
                    float n1 = s.pop(bitacora, linea, caracter);
                    asm.WriteLine("\tPOP BX");
                    n1 = cast(n1, tipoDato);
                    s.push(n1, bitacora, linea, caracter);
                    asm.WriteLine("\tMOV AX, "+n1);
                    asm.WriteLine("\tPUSH AX");
                    maxBytes = tipoDato;
                }
            }
        }

        // For -> for (identificador = Expresion; Condicion; identificador incremento_termino) BloqueInstrucciones
        private void For(bool ejecuta2)
        {
            match("for");
            match("(");

            string nombre;
            bool ejecuta;

            if(getClasificacion() == clasificaciones.tipo_dato)
            {
                string tipo = getContenido();
                match(clasificaciones.tipo_dato);

                nombre = getContenido();

                if (!l.Existe(nombre))
                {
                   match(clasificaciones.identificador);
                }
                else
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
                }
                l.Inserta(nombre, determinarTipoDato(tipo));
            }
            else
            {
                nombre = getContenido();
                match(clasificaciones.identificador); 

                if (!l.Existe(nombre))
                {
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }
            }

            match(clasificaciones.asignacion);
            Expresion();

            maxBytes = Variable.tipo.CHAR;
            string valor = s.pop(bitacora, linea, caracter).ToString();
            asm.WriteLine("\tPOP CX");

            if (tipoDatoExpresion(float.Parse(valor)) > maxBytes)
            {
                maxBytes = tipoDatoExpresion(float.Parse(valor));
            }

            if (maxBytes > l.getTipoDato(nombre))
            {
                throw new Error(bitacora, "Error semántico: No se puede asignar un " + maxBytes + " a un (" + l.getTipoDato(nombre) + ") Linea: " + linea + ", caracter: " + caracter);
            }

            asm.WriteLine("\tMOV "+nombre+", CX");
            l.setValor(nombre, valor);
            match(clasificaciones.fin_sentencia);

            string etiquetaFin = "endFor"+numero_for++;
            string etiquetaInicio = "beginFor"+numero_for++;

            asm.WriteLine(etiquetaInicio + ":");
            ejecuta = Condicion(etiquetaFin);
            match(clasificaciones.fin_sentencia);

            nombre = getContenido();
            match(clasificaciones.identificador);

            if (!l.Existe(nombre))
            {
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
            }
            
            string operador = getContenido();
            match(clasificaciones.incremento_termino);

            if(operador == "++")
            {
                l.setValor(nombre, (float.Parse(l.getValor(nombre)) + 1).ToString());
                asm.WriteLine("\tINC "+nombre);
            }
            else if(operador == "--")
            {
                l.setValor(nombre, (float.Parse(l.getValor(nombre)) - 1).ToString());
                asm.WriteLine("\tDEC "+nombre);
            }
            else if(operador == "+=")
            {
                string numero = getContenido();
                match(clasificaciones.numero);

                l.setValor(nombre, (float.Parse(l.getValor(nombre)) + float.Parse(numero)).ToString());
                //Requerimiento 5
                asm.WriteLine("\tADD "+nombre+", "+numero);
            }
            else if(operador == "-=")
            {
                string numero = getContenido();
                match(clasificaciones.numero);

                l.setValor(nombre, (float.Parse(l.getValor(nombre)) - float.Parse(numero)).ToString());
                // Requerimiento 5
                asm.WriteLine("\tSUB "+nombre+", "+numero);
            }
            match(")");

            BloqueInstrucciones(ejecuta && ejecuta2);
            asm.WriteLine("\tJMP "+etiquetaInicio);
            asm.WriteLine(etiquetaFin + ":");
        }

        // While -> while (Condicion) BloqueInstrucciones
        private void While(bool ejecuta)
        {
            match("while");

            match("(");
            Condicion("");
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
            Condicion("");
            match(")");
            match(clasificaciones.fin_sentencia);
        }

        private Variable.tipo determinarTipoDato(string tipoDato)
        {
            Variable.tipo tipoVar;

            switch (tipoDato)
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
            if (valor % 1 != 0)
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

        private float cast(float n1, Variable.tipo tipoDato)
        {
            switch (tipoDato)
            {
                case Variable.tipo.CHAR:
                    if (tipoDatoExpresion(n1) == Variable.tipo.INT)
                    {
                        // Para convertir un entero a char necesitamos dividir entre 256 y el residuo 
                        // es el resultado del cast. 256 = 0, 257 = 1, 258 = 2, ... 
                        n1 = n1 % 256;
                    }
                    if (tipoDatoExpresion(n1) == Variable.tipo.FLOAT)
                    {
                        // Para convertir un float a otro tipo de dato redondear el numero para eliminar
                        // la parte fraccional.
                        n1 = (int)Math.Round(n1);
                        // Para convertir un float a char necesitamos dividir entre 65536/256 y el residuo 
                        // es el resultado del cast.
                        n1 = n1 % (65536 / 256);
                    }
                    break;
                case Variable.tipo.INT:
                    if (tipoDatoExpresion(n1) == Variable.tipo.FLOAT)
                    {
                        // Para convertir un float a otro tipo de dato redondear el numero para eliminar
                        // la parte fraccional.
                        n1 = (int)Math.Round(n1);
                        // Para convertir un flot a int necesitamos dividir entre 65536 y el residuo 
                        // es el resultado del cast.
                        n1 = n1 % 65536;
                    }
                    break;
            }
            return n1;
        }
    }
}