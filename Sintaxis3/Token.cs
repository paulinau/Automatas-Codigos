using System;
using System.IO;

namespace Sintaxis3
{
    class Token: Error
    {
        public enum clasificaciones{
            identificador, numero, asignacion, inicializacion, fin_sentencia,
            operador_logico, operador_relacional, operador_termino, operador_factor,
            incremento_termino, incremento_factor, cadena, operador_ternario, caracter,
            tipo_dato, zona, condicion, ciclo, inicio_bloque, fin_bloque, flujo_entrada,
            flujo_salida,
        }
        private string contenido;
        private clasificaciones clasificacion;
        
        public void setContenido(string contenido){
            this.contenido = contenido;
        }

        public void setClasificacion(clasificaciones clasificacion){
            this.clasificacion = clasificacion;
        }

        public string getContenido(){
            return contenido;
        }

        public clasificaciones getClasificacion(){
            return clasificacion;
        }
    }
}