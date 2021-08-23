using System;
using System.IO;

namespace Sintaxis3
{
    public class Sintaxis : Lexico
    {
        public Sintaxis(){
            Console.WriteLine("Iniciando analisis sintactico");
            nextToken();
        }

        public Sintaxis(string nombre) : base(nombre){
            Console.WriteLine("Iniciando analisis sintactico");
            nextToken();
        }
        protected void match(string espera){
            //Console.WriteLine(getContenido() + " "+espera);
            if(espera == getContenido()){
                //sacamos un token
                nextToken();
            }else{
                bitacora.WriteLine("Error de sintaxis en la linea: {0} caracter: {1} se espera un: {2} ", linea, caracter,espera);
                throw new Exception("Error de sintaxis: se espera un "+espera+" en la linea: "+linea+" caracter: "+caracter);
            }
        }

        protected void match(clasificaciones espera){
            //Console.WriteLine(getContenido() + " "+espera);
            if(espera == getClasificacion()){
                nextToken();
            }else{
                bitacora.WriteLine("Error de sintaxis en la linea: {0} caracter: {1} se espera un: {2} ", linea, caracter,espera);
                throw new Exception("Error de sintaxis: se espera un "+espera+" en la linea: "+linea+" caracter: "+caracter);
            }
        }
    }
}