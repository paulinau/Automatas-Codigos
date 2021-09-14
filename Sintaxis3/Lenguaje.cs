using System;
using System.Collections.Generic;
using System.Text;

// ✎ Requerimiento 1: Implementar las secuencias de escape: \n, \t cuando se imprime una cadena y 
//                  eliminar las dobles comillas.
// ✎ Requerimiento 2: Levantar excepciones en la clase Stack.
// ✎ Requerimiento 3: Agregar el tipo de dato en el Inserta de ListaVariables.
//                  salvar el tipo de dato en variable y pasarlo por argumento el tipo de dato a listaID's
// ✎ Requerimiento 4: Validar existencia o duplicidad de variables. Mensaje de error: 
//                  "Error de sintaxis: La variable (x26) no ha sido declarada."
//                  "Error de sintaxis: La variables (x26) está duplicada." 
// ✎ Requerimiento 5: Modificar el valor de la variable o constante al momento de su declaración.

namespace Sintaxis3{
    class Lenguaje: Sintaxis{
        Stack s;
        ListaVariables l;
        public Lenguaje(){            
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        public Lenguaje(string nombre): base(nombre){
            s = new Stack(5);
            l = new ListaVariables();
            Console.WriteLine("Iniciando analisis gramatical.");
        }

        // Programa -> Libreria Main
        public void Programa(){
            Libreria();
            Main();
            l.Imprime(bitacora);
        }

        // Libreria -> (#include <identificador(.h)?> Libreria) ?
        private void Libreria(){            
            if (getContenido() == "#"){
                match("#");
                match("include");
                match("<");
                match(clasificaciones.identificador);

                if (getContenido() == "."){
                    match(".");
                    match("h");
                }

                match(">");

                Libreria();
            }
        }

        // Main -> tipo_dato main() BloqueInstrucciones 
        private void Main(){
            match(clasificaciones.tipo_dato);
            match("main");
            match("(");
            match(")");

            BloqueInstrucciones();            
        }

        // BloqueInstrucciones -> { Instrucciones }
        private void BloqueInstrucciones(){
            match(clasificaciones.inicio_bloque);

            Instrucciones();

            match(clasificaciones.fin_bloque);
        }

        // Lista_IDs -> identificador (= Expresion)? (,Lista_IDs)? 
        private void Lista_IDs(Variable.tipo tipoDato){          
            string nombre = getContenido();

            if (!l.Existe(nombre)){
                l.Inserta(nombre, tipoDato);
                match(clasificaciones.identificador); // Validar duplicidad :D
            }else{
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
            }                

            if (getClasificacion() == clasificaciones.asignacion){
                match(clasificaciones.asignacion);
                Expresion();
                l.setValor(nombre, s.pop(bitacora, linea, caracter).ToString());
            }

            if (getContenido() == ","){
                match(",");
                Lista_IDs(tipoDato);
            }
        }

        // Variables -> tipo_dato Lista_IDs; 
        private void Variables(){
            string tipo = getContenido();
            match(clasificaciones.tipo_dato);

            Variable.tipo tipoDato;
            switch (tipo) {
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
            Lista_IDs(tipoDato);
            match(clasificaciones.fin_sentencia);           
        }

        // Instruccion -> (If | cin | cout | const | Variables | asignacion) ;
        private void Instruccion(){
            if (getContenido() == "do"){
                DoWhile();
            }else if (getContenido() == "while"){
                While();
            }else if (getContenido() == "for"){
                For();
            }else if (getContenido() == "if"){
                If();
            }else if (getContenido() == "cin"){
                // Requerimiento 5
                match("cin");
                match(clasificaciones.flujo_entrada);

                string nombre = getContenido();
                
                if (!l.Existe(nombre)){
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }else{
                    string valor = Console.ReadLine();
                    match(clasificaciones.identificador); // Validar existencia :D
                    l.setValor(nombre, valor);
                }

                match(clasificaciones.fin_sentencia);
            }else if (getContenido() == "cout"){
                match("cout");
                ListaFlujoSalida();
                match(clasificaciones.fin_sentencia);
            }else if (getContenido() == "const"){
                Constante();
            }else if (getClasificacion() == clasificaciones.tipo_dato){
                Variables();
            }else{
                string nombre = getContenido();
                
                if (!l.Existe(nombre)){
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }else{
                    match(clasificaciones.identificador); // Validar existencia :D
                }
                match(clasificaciones.asignacion);

                string valor;

                if (getClasificacion() == clasificaciones.cadena){           
                    valor = getContenido();         
                    match(clasificaciones.cadena);                    
                }else{                    
                    Expresion();
                    valor = s.pop(bitacora, linea, caracter).ToString();                  
                }                
                l.setValor(nombre, valor);
                match(clasificaciones.fin_sentencia);
            }
        }

        // Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones(){
            Instruccion();

            if (getClasificacion() != clasificaciones.fin_bloque){
                Instrucciones();
            }
        }

        // Constante -> const tipo_dato identificador = numero | cadena;
        private void Constante(){
            match("const");
            string tipo = getContenido();
            match(clasificaciones.tipo_dato);

            Variable.tipo tipoDato;
            switch (tipo) {
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

            if (!l.Existe(nombre)){
                l.Inserta(nombre, tipoDato, true);
                match(clasificaciones.identificador); // Validar duplicidad :D
            }else{
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") esta duplicada en la linea: " + linea + ", caracter: " + caracter);
            } 
            match(clasificaciones.asignacion);

            string valor = getContenido();

            if (getClasificacion() == clasificaciones.numero){
                match(clasificaciones.numero);
            }else{
                match(clasificaciones.cadena);
            }
            l.setValor(nombre, valor);
            match(clasificaciones.fin_sentencia);
        }

        // ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida(){
            match(clasificaciones.flujo_salida);

            if (getClasificacion() == clasificaciones.numero){
                Console.Write(getContenido());
                match(clasificaciones.numero); 
            }else if (getClasificacion() == clasificaciones.cadena){    
                string contenido = getContenido();

                if (contenido.Contains("\""))
                    contenido = contenido.Replace("\"", "");
                if (contenido.Contains("\\n"))
                    contenido = contenido.Replace("\\n", "\n");
                if (contenido.Contains("\\t"))
                    contenido = contenido.Replace("\\t", "\t");

                Console.Write(contenido);
                match(clasificaciones.cadena);
            }else{
                string nombre = getContenido();
                if (l.Existe(nombre)){
                    Console.Write(l.getValor(nombre));
                    match(clasificaciones.identificador); // Validar existencia :D
                }
                else{
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }             
            }

            if (getClasificacion() == clasificaciones.flujo_salida){
                ListaFlujoSalida();
            }
        }

        // If -> if (Condicion) { BloqueInstrucciones } (else BloqueInstrucciones)?
        private void If(){
            match("if");
            match("(");
            Condicion();
            match(")");
            BloqueInstrucciones();

            if (getContenido() == "else"){
                match("else");
                BloqueInstrucciones();
            }
        }

        // Condicion -> Expresion operador_relacional Expresion
        private void Condicion(){
            Expresion();
            match(clasificaciones.operador_relacional);
            Expresion();
        }

        // x26 = (3+5)*8-(10-4)/2;
        // Expresion -> Termino MasTermino 
        private void Expresion(){
            Termino();
            MasTermino();
        }
        // MasTermino -> (operador_termino Termino)?
        private void MasTermino(){
            if (getClasificacion() == clasificaciones.operador_termino){
                string operador = getContenido();                              
                match(clasificaciones.operador_termino);
                Termino();
                float e1 = s.pop(bitacora, linea, caracter), e2 = s.pop(bitacora, linea, caracter);  
                // Console.Write(operador + " ");

                switch(operador){
                    case "+":
                        s.push(e2+e1, bitacora, linea, caracter);
                        break;
                    case "-":
                        s.push(e2-e1, bitacora, linea, caracter);
                        break;                    
                }
                s.Display(bitacora);
            }
        }
        // Termino -> Factor PorFactor
        private void Termino(){
            Factor();
            PorFactor();
        }
        // PorFactor -> (operador_factor Factor)?
        private void PorFactor(){
            if (getClasificacion() == clasificaciones.operador_factor){
                string operador = getContenido();                
                match(clasificaciones.operador_factor);
                Factor();
                float e1 = s.pop(bitacora, linea, caracter), e2 = s.pop(bitacora, linea, caracter); 
                // Console.Write(operador + " ");

                switch(operador){
                    case "*":
                        s.push(e2*e1, bitacora, linea, caracter);                        
                        break;
                    case "/":
                        s.push(e2/e1, bitacora, linea, caracter);
                        break;                    
                }

                s.Display(bitacora);
            }
        }
        // Factor -> identificador | numero | ( Expresion )
        private void Factor(){
            if (getClasificacion() == clasificaciones.identificador){
                //Console.Write(getContenido() + " ");

                s.push(float.Parse(l.getValor(getContenido())), bitacora, linea, caracter);
                s.Display(bitacora);

                string nombre = getContenido();
                
                if (!l.Existe(nombre)){
                    throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
                }else{
                    //s.push(float.Parse(l.getValor(getContenido())), bitacora);
                    //s.Display(bitacora);
                    match(clasificaciones.identificador); // Validar existencia :D
                }

            }else if (getClasificacion() == clasificaciones.numero){
                // Console.Write(getContenido() + " ");
                s.push(float.Parse(getContenido()), bitacora, linea, caracter);
                s.Display(bitacora);
                match(clasificaciones.numero);
            }else{
                match("(");
                Expresion();
                match(")");
            }
        }

        // For -> for (identificador = Expresion; Condicion; identificador incremento_termino) BloqueInstrucciones
        private void For(){
            match("for");

            match("(");

            string nombre = getContenido();
                
            if (!l.Existe(nombre)){
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
            }else{
                match(clasificaciones.identificador); // Validar existencia :D
            }
            match(clasificaciones.asignacion);
            Expresion();
            match(clasificaciones.fin_sentencia);

            Condicion();
            match(clasificaciones.fin_sentencia);

            if (!l.Existe(nombre)){
                throw new Error(bitacora, "Error de sintaxis: La variable (" + nombre + ") no ha sido declarada. Linea: " + linea + ", caracter: " + caracter);
            }else{
                match(clasificaciones.identificador); // Validar existencia :D
            }
            match(clasificaciones.incremento_termino);

            match(")");

            BloqueInstrucciones();
        }

        // While -> while (Condicion) BloqueInstrucciones
        private void While(){
            match("while");

            match("(");
            Condicion();
            match(")");

            BloqueInstrucciones();
        }
        
        // DoWhile -> do BloqueInstrucciones while (Condicion);
        private void DoWhile(){
            match("do");

            BloqueInstrucciones();

            match("while");

            match("(");
            Condicion();
            match(")");
            match(clasificaciones.fin_sentencia);
        }

        // x26 = (3 + 5) * 8 - (10 - 4) / 2
        // x26 = 3 + 5 * 8 - 10 - 4 / 2
        // x26 = 3 5 + 8 * 10 4 - 2 / -
    }
}