using System;
using System.IO;
namespace Sintaxis3
{
    public class Stack
    {
        int maxElementos;
        int ultimo;
        float[] elementos;

        public Stack(int maxElementos){
            this.maxElementos = maxElementos;
            ultimo = 0;
            elementos = new float[maxElementos];
        }

        public void push(float element, StreamWriter bitacora){
            if(ultimo < maxElementos){
                bitacora.WriteLine("Push ="+element);
                elementos[ultimo++] = element;
            }
            throw new StackOverflowException("Error StackOverflow: No se pueden agregar mas elementos a la pila llena.", new Error(bitacora, "Error StackOverflow: No se pueden agregar mas elementos a la pila."));
        }

        public float pop(StreamWriter bitacora){
            if(ultimo >0){
                bitacora.WriteLine("Pop ="+elementos[ultimo-1]);
                return elementos[--ultimo];
            }else{
                //else levantar excepcion de stackUnderFlow
                throw new Error(bitacora, "Error StackUnderflow: No se pueden sacar elementos de una pila vacía.");
            }
        }

        public void Display(StreamWriter bitacora){
            bitacora.WriteLine("Contenido del stack  ");
            for(int i = 0; i<ultimo; i++){
                bitacora.Write(elementos[i]+" ");
            }
            bitacora.WriteLine("");
        }
    }
}