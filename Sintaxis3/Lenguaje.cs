using System;
using System.IO;

// REQUERIMIENTO 1: Implementar las secuencias de escape: \n, \t cuando se imprime una cadena 
//                  y eliminar las dobles comillas
// REQUERIMIENTO 2: 


namespace Sintaxis3
{
    class Lenguaje : Sintaxis
    {
        public Lenguaje(){
            Console.WriteLine("Iniciando analisis gramatical");
        }

        public Lenguaje(string nombre) : base(nombre){
            Console.WriteLine("Iniciando analisis gramatical");
        }

        // Programa -> Liberia Main
        public void Programa(){
            Libreria();
            Main();
        }
        // Libreria -> (#include <identificador(.h)?> Libreria) ?
        private void Libreria(){
            //caso base de la recursividad
            if(getContenido() == "#"){
                match("#");
                match("include");
                match("<");
                match(clasificaciones.identificador);
                if(getContenido() == "."){
                    match(".");
                    match("h");
                }
                match(">");

                Libreria();
            }
        }

        // Main -> void main() { (Variables)? Instrucciones }
        private void Main(){
            match(clasificaciones.tipo_dato);
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
        }

        //BloqueInstrucciones -> {Instrucciones}
        private void BloqueInstrucciones(){
            match(clasificaciones.inicio_bloque);
            Instrucciones();
            match(clasificaciones.fin_bloque);
        }

        //Lista_IDs -> identificador (,Lista_IDs)?
        private void Lista_IDs(){
            match(clasificaciones.identificador); //validar duplicidad

            if(getClasificacion() == clasificaciones.asignacion){
                match(clasificaciones.asignacion);
                Expresion();
            }
            if(getContenido() == ","){
                match(",");
                Lista_IDs();
            }
        }

        //Variables -> tipo_dato Lista_IDs; 
        private void Variables(){
            match(clasificaciones.tipo_dato);
            Lista_IDs();
            match(clasificaciones.fin_sentencia);
        }

        //Instruccion -> (asignacion | printf(identificador | cadena | numero)) ;
        private void Instruccion(){
            if(getContenido() == "for"){
                For();
            }else if(getContenido() == "if"){
                If();
            }else if(getContenido() == "cout"){
                match("cout");
                ListaFlujoSalida();
                match(clasificaciones.fin_sentencia);
            }else if(getContenido() == "cin"){
                match("cin");
                match(clasificaciones.flujo_entrada);
                match(clasificaciones.identificador);   //validar existencia
                match(clasificaciones.fin_sentencia);
            }else if(getContenido() == "const"){
                Constante();
            }else if(getClasificacion() == clasificaciones.tipo_dato){
                Variables();
            }else if(getContenido() == "while"){
                While();
            }else if(getContenido() == "do"){
                DoWhile();
            }else{
                match(clasificaciones.identificador);   //validar existencia
                match(clasificaciones.asignacion);

                if(getClasificacion() == clasificaciones.cadena){
                    match(clasificaciones.cadena);
                }else{
                    Expresion();
                }
                match(clasificaciones.fin_sentencia);
            }
        }

        //Instrucciones -> Instruccion Instrucciones?
        private void Instrucciones(){
            Instruccion();
            if(getClasificacion() != clasificaciones.fin_bloque){
                Instrucciones();
            }
        }

        //Constante -> const tipo_dato identificador = numero | cadena;
        private void Constante(){
            match("const");
            match(clasificaciones.tipo_dato);
            match(clasificaciones.identificador);   //validar duplicidad
            match(clasificaciones.asignacion);
            if(getClasificacion() == clasificaciones.numero){
                match(clasificaciones.numero);
            }else{
                match(clasificaciones.cadena);
            }
            match(clasificaciones.fin_sentencia);
        }

        //ListaFlujoSalida -> << cadena | identificador | numero (ListaFlujoSalida)?
        private void ListaFlujoSalida(){
            match(clasificaciones.flujo_salida); //<<
            if(getClasificacion() == clasificaciones.numero){
                Console.Write(getContenido());
                match(clasificaciones.numero);
            }else if(getClasificacion() == clasificaciones.cadena){
                Console.Write(getContenido());
                match(clasificaciones.cadena);
            }else{
                Console.Write(getContenido());
                match(clasificaciones.identificador);   //validar existencia
            }

            if(getClasificacion() == clasificaciones.flujo_salida){
                ListaFlujoSalida();
            }
        }

        //if -> if (condicion) BloqueInstrucciones (else BloqueInstrucciones)?
        private void If(){
            match("if");
            match("(");
            Condicion();
            match(")");
            BloqueInstrucciones();

            if(getContenido() == "else"){
                match("else");
                BloqueInstrucciones();
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
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
            if(getClasificacion() == clasificaciones.operador_termino){
                Console.Write(getContenido()+" ");
                match(clasificaciones.operador_termino);
                Termino();
            }
        }
        // Termino -> Factor PorFactor
        private void Termino(){
            Factor();
            PorFactor();
        }
        // PorFactor -> (operador_factor Factor)?
        private void PorFactor(){
            if(getClasificacion() == clasificaciones.operador_factor){
                Console.Write(getContenido()+" ");
                match(clasificaciones.operador_factor);
                Factor();
            }
        }
        // Factor -> identificador | numero | (Expresion)
        private void Factor(){
            if(getClasificacion() == clasificaciones.identificador){
                Console.Write(getContenido()+" ");
                match(clasificaciones.identificador);   //validar existencia
            }else if(getClasificacion() == clasificaciones.numero){
                Console.Write(getContenido()+" ");
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
            match(clasificaciones.identificador);   //validar existencia
            match(clasificaciones.asignacion);
            Expresion();

            match(clasificaciones.fin_sentencia);
            Condicion();
            match(clasificaciones.fin_sentencia);

            match(clasificaciones.identificador);   //validar existencia
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
        // DoWhile -> do BloqueInstrucciones while(Condicion);
        private void DoWhile(){
            match("do");
            BloqueInstrucciones();
            match("while");
            match("(");
            Condicion();
            match(")");
            match(clasificaciones.fin_sentencia);
        }

        //x26 = (3+5)*8-(10-4)/2
        //x26 = 3+5*8-10-4/2
        //x26 = 3 5 + 8 * 10 4 - 2 / -
    }
}