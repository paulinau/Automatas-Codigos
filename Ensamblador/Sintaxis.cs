using System;
using System.IO;

namespace Ensamblador
{
    class Sintaxis : Lexico
    {
        public Sintaxis()
        {
            Console.WriteLine("Iniciando analisis sintactico");
            nextToken();
        }

        public Sintaxis(string nombre) : base(nombre)
        {
            Console.WriteLine("Iniciando analisis sintactico");
            nextToken();
        }
        protected void match(string espera)
        {
            //Console.WriteLine(getContenido() + " "+espera);
            if (espera == getContenido())
            {
                //sacamos un token
                nextToken();
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Se espera un(a) " + espera + " en la linea: " + linea + ", caracter: " + caracter);
            }
        }

        protected void match(clasificaciones espera)
        {
            //Console.WriteLine(getContenido() + " "+espera);
            if (espera == getClasificacion())
            {
                nextToken();
            }
            else
            {
                throw new Error(bitacora, "Error de sintaxis: Se espera un(a) " + espera + " en la linea: " + linea + ", caracter: " + caracter);
            }
        }
    }
}