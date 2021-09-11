using System;
using System.IO;

namespace Sintaxis3
{
    class Lexico : Token, IDisposable
    {
        protected StreamReader archivo;
        protected StreamWriter bitacora;
        protected int linea, caracter;
        const int F = -1; 
        const int E = -2;
        string nombre_archivo;
        int[,] trand   = { // WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La, {, }, #10
                            {  0, F, 1, 2,29,17,18, 1, 8, 9,11,12,13,15,26,27,20,32,20,22,24,28,29,30,31, 0},//0
                            {  F, F, 1, 1, F, F, F, 1, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//1
                            {  F, F, F, 2, 3, F, F, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//2
                            {  E, E, E, 4, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},//3
                            {  F, F, F, 4, F, F, F, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//4
                            {  E, E, E, 7, E, 6, 6, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},//5
                            {  E, E, E, 7, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},//6
                            {  F, F, F, 7, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//7
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//8
                            {  F, F, F, F, F, F, F, F,10, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//9
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//10
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//11
                            {  F, F, F, F, F, F, F, F, F, F, F,14, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//12
                            {  F, F, F, F, F, F, F, F, F, F, F, F,14, F, F, F, F, F, F, F, F, F, F, F, F, F},//13
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//14
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//15
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//16
                            {  F, F, F, F, F,19, F, F,19, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//17
                            {  F, F, F, F, F, F,19, F,19, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//18
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//19
                            {  F, F, F, F, F, F, F, F,21, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//20
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//21
                            { 22, E,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,23,22,22,22,22,22,22},//22
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//23
                            { 24, E,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,25,24,24,24,24,24},//24
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//25
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F,36, F, F, F, F, F, F, F, F, F, F, F},//26
                            {  F, F, F, F, F, F, F, F,16, F, F, F, F, F,16,37, F, F, F, F, F, F, F, F, F, F},//27
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//28
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//29
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//30
                            {  F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//31
                            {  F, F, F, F, F, F, F, F,21, F, F, F, F, F, F, F,34,33, F, F, F, F, F, F, F, F},//32
                            { 33, 0,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33,33, 0},//33
                            { 34, E,34,34,34,34,34,34,34,34,34,34,34,34,34,34,35,34,34,34,34,34,34,34,34,34},//34
                            { 34, E,34,34,34,34,34,34,34,34,34,34,34,34,34,34,35, 0,34,34,34,34,34,34,34,34},//35
                            {  F, E, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//36
                            {  F, E, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},//36
                            //WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La, {, }, #10
                            };

        //Declaramos nuestro constructor
        public Lexico(){
            linea = caracter = 1;
            Console.WriteLine("Compilando prueba.cpp");
            Console.WriteLine("Iniciando analisis lexico");
            
            //checar si el archivo existe
            if(File.Exists("C:\\archivos\\prueba.cpp")){
                archivo = new StreamReader("C:\\archivos\\prueba.cpp");
                bitacora = new StreamWriter("C:\\archivos\\prueba.log");
                bitacora.AutoFlush = true; //recibe falso o verdadero

                DateTime fechaActual = DateTime.Now;

                bitacora.WriteLine("Archivo: prueba.cpp");  //grabamos algo en el archivo
                bitacora.WriteLine("Directorio: C:\\archivos");
                bitacora.WriteLine("Fecha: "+fechaActual.ToString("D")+" Hora: "+fechaActual.ToString("t"));
            }else{
                throw new Exception("El archivo prueba.cpp no existe");
            }
        }

        //sobrecarga de constructores
        public Lexico(string nombre){
            linea = caracter = 1;

            nombre_archivo = Path.GetFileName(nombre);
            Console.WriteLine("Compilando "+nombre_archivo);
            Console.WriteLine("Iniciando analisis lexico");

            string extension = Path.GetExtension(nombre);
            if(extension == ".cpp"){
            //checar si el archivo existe
                if(File.Exists(nombre)){
                    archivo = new StreamReader(nombre);

                    string log = Path.ChangeExtension(nombre, ".log");
                    bitacora = new StreamWriter(log);
                    bitacora.AutoFlush = true; //recibe falso o verdadero

                    DateTime fechaActual = DateTime.Now;
                    string directorio = Path.GetDirectoryName(nombre);
                        
                    bitacora.WriteLine("Archivo: "+nombre_archivo);  //grabamos algo en el archivo
                    bitacora.WriteLine("Directorio: "+directorio);
                    bitacora.WriteLine("Fecha: "+fechaActual.ToString("D")+" Hora: "+fechaActual.ToString("t"));
                }else{
                    throw new Exception("El archivo "+nombre_archivo+" no existe");
                }
            }else{
                throw new Exception("No se puede compilar "+nombre_archivo+" puesto que la extension no es .cpp");
            }
        }

        //Destructor
        //~ Lexico(){
        public void Dispose(){
            Console.WriteLine("\nFinaliza la compilación de "+nombre_archivo);
            CerrarArchivos();   //invoca el metodo para cerrar los archivos
        }

        //Cerramos los archivos
        private void CerrarArchivos(){
            archivo.Close();
            bitacora.Close();
        }

        protected void nextToken(){
            char transicion;
            string palabra = "";
            int estado = 0;    
            int estado_anterior = 0;

            //mientras esté en estados positivos, permanezco en el automata
            while(estado >= 0){  
                estado_anterior = estado;

                transicion = (char) archivo.Peek();
                estado = trand[estado, columna(transicion)];
                clasificar(estado);
                
                //son caracteres validos
                if(estado >= 0){
                    archivo.Read();
                    caracter++;
                    //Contador de linea y caracteres
                    if(transicion == 10){
                        linea++;
                        caracter = 1;
                    }
                    if(estado >0){
                        palabra += transicion;
                    }else{
                        //se limpia todo lo que se concateno
                        palabra = "";
                    }
                }
            }
            setContenido(palabra);
            //se produjo una excepcion
            if (estado == E){
                if(getClasificacion() == clasificaciones.cadena){
                    throw new Error(bitacora, "Error lexico: Se esperaba comillas (\") o comilla (') de cierre. (" + linea + ", " + caracter + ")");
                }
                else if (getClasificacion() == clasificaciones.numero){
                    throw new Error(bitacora, "Error lexico: Se esperaba un dígito. (" + linea + ", " + caracter + ")");
                }
                else{
                    throw new Error(bitacora, "Error lexico: Se esperaba un cierre de comentario (*/). (" + linea + ", " + caracter + ")");
                }      
            }else if (getClasificacion() == clasificaciones.identificador){
                switch(palabra){
                    //en caso de que sea char, int o float
                    case "char":
                    case "int":
                    case "float":
                    case "string":
                        setClasificacion(clasificaciones.tipo_dato);
                        break;
                    case "private":
                    case "public":
                    case "protected":
                        setClasificacion(clasificaciones.zona);
                        break;
                    case "if":
                    case "else":
                    case "switch":
                        setClasificacion(clasificaciones.condicion);
                        break;
                    case "while":
                    case "for":
                    case "do":
                        setClasificacion(clasificaciones.ciclo);
                        break;
                }
            }
            if(getContenido() != ""){
                bitacora.WriteLine("Token = " + getContenido());
                bitacora.WriteLine("Clasificacion = " + getClasificacion());
            }
        }

        private void clasificar(int estado){
            switch(estado){
                case 1:
                    setClasificacion(clasificaciones.identificador);
                    break;
                case 2:
                    setClasificacion(clasificaciones.numero);
                    break;
                case 8:
                    setClasificacion(clasificaciones.asignacion);
                    break;
                case 9:
                case 12:
                case 13:
                case 29:
                    setClasificacion(clasificaciones.caracter);
                    break;
                case 10:
                    setClasificacion(clasificaciones.inicializacion);
                    break;
                case 11:
                    setClasificacion(clasificaciones.fin_sentencia);
                    break;
                case 14:
                case 15:
                    setClasificacion(clasificaciones.operador_logico);
                    break;
                case 16:
                case 26:
                case 27:
                    setClasificacion(clasificaciones.operador_relacional);
                    break;
                case 17:
                case 18:
                    setClasificacion(clasificaciones.operador_termino);
                    break;
                 case 19:
                    setClasificacion(clasificaciones.incremento_termino);
                    break;
                case 20:
                case 32:
                    setClasificacion(clasificaciones.operador_factor);
                    break;
                case 21:
                    setClasificacion(clasificaciones.incremento_factor);
                    break;
                case 22:
                case 24:
                    setClasificacion(clasificaciones.cadena);
                    break;
                case 28:
                    setClasificacion(clasificaciones.operador_ternario);
                    break;
                case 30:
                    setClasificacion(clasificaciones.inicio_bloque);
                    break;
                case 31:
                    setClasificacion(clasificaciones.fin_bloque);
                    break;
                case 33:
                case 34:
                case 35:
                    break;
                case 36:
                    setClasificacion(clasificaciones.flujo_entrada);
                    break;
                case 37:
                    setClasificacion(clasificaciones.flujo_salida);
                    break;
            }
        }

        private int columna(char t){
            // WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La, {, },#10
            if (finArchivo()){
                return 1;
            }else if(t == 10){
                return 25;
            }else if (char.IsWhiteSpace(t)){
                return 0;
            }else if (char.ToLower(t) == 'e'){
                return 7;
            }else if (char.IsLetter(t)){
                return 2;
            }else if (char.IsDigit(t)){
                return 3;
            }else if (t == '.'){
                return 4;
            }else if (t == '+'){
                return 5;
            }else if (t == '-'){
                return 6;
            }else if (t == '='){
                return 8;
            }else if (t == ':'){
                return 9;
            }else if (t == ';'){
                return 10;
            }else if (t == '&'){
                return 11;
            }else if (t == '|'){
                return 12;
            }else if (t == '!'){
                return 13;
            }else if (t == '>'){
                return 14;
            }else if (t == '<'){
                return 15;
            }else if (t == '*'){
                return 16;
            }else if (t == '/'){
                return 17;
            }else if (t == '%'){
                return 18;
            }else if (t == '"'){
                return 19;
            }else if (t == '\''){
                return 20;
            }else if (t == '?'){
                return 21;
            }else if(t == '{'){
                return 23;
            }else if(t == '}'){
                return 24;
            }else {
                return 22;
            }
            // WS,EF, L, D, ., +, -, E, =, :, ;, &, |, !, >, <, *, /, %, ", ', ?,La, {, },#10
        }

        public bool finArchivo(){
            return archivo.EndOfStream;
        }
    }
}