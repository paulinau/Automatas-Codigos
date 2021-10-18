using System;
using System.IO;
namespace Ensamblador
{
    public class Stack
    {
        int maxElementos;
        int ultimo;
        float[] elementos;

        public Stack(int maxElementos)
        {
            this.maxElementos = maxElementos;
            ultimo = 0;
            elementos = new float[maxElementos];
        }

        public void push(float element, StreamWriter bitacora, int linea, int caracter)
        {
            if (ultimo < maxElementos)
            {
                bitacora.WriteLine("Push =" + element);
                elementos[ultimo++] = element;
            }
            else
            {
                throw new StackOverflowException("Error StackOverflow: No se pueden agregar mas elementos a la pila llena. Linea: " + linea + ", caracter: " + caracter, new Error(bitacora, "Error StackOverflow: No se pueden agregar mas elementos a la pila. Linea: " + linea + ", caracter: " + caracter));
            }
        }

        public float pop(StreamWriter bitacora, int linea, int caracter)
        {
            if (ultimo > 0)
            {
                bitacora.WriteLine("Pop =" + elementos[ultimo - 1]);
                return elementos[--ultimo];
            }
            else
            {
                //else levantar excepcion de stackUnderFlow
                throw new Error(bitacora, "Error StackUnderflow: No se pueden sacar elementos de una pila vacía. Linea: " + linea + ", caracter: " + caracter);
            }
        }

        public void Display(StreamWriter bitacora)
        {
            bitacora.WriteLine("Contenido del stack  ");
            for (int i = 0; i < ultimo; i++)
            {
                bitacora.Write(elementos[i] + " ");
            }
            bitacora.WriteLine("");
        }
    }
}