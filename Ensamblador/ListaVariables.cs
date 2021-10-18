using System.Collections.Generic;
using System.IO;
using System;

namespace Ensamblador
{
    public class ListaVariables
    {
        List<Variable> lista;
        public ListaVariables()
        {
            lista = new List<Variable>();
        }

        public void Inserta(string nombre, Variable.tipo tipoDato, bool esConstante = false)
        {
            lista.Add(new Variable(nombre, tipoDato, esConstante));
        }

        public bool Existe(string nombre)
        {
            return lista.Exists(x => x.getNombre() == nombre);
        }

        public void setValor(string nombre, string valor)
        {
            foreach (Variable x in lista)
            {
                if (x.getNombre() == nombre)
                {
                    x.setValor(valor);
                    break;
                }
            }
        }

        public string getValor(string nombre)
        {
            foreach (Variable x in lista)
            {
                if (x.getNombre() == nombre)
                {
                    return x.getValor();
                }
            }
            return "";
        }

        public Variable.tipo getTipoDato(string nombre)
        {
            foreach (Variable x in lista)
            {
                if (x.getNombre() == nombre)
                {
                    return x.getTipoDato();
                }
            }
            return Variable.tipo.CHAR;
        }

        public void Imprime(StreamWriter bitacora, StreamWriter asm)
        {
            bitacora.WriteLine("Lista de Variables: ");
            foreach (Variable x in lista)
            {
                bitacora.WriteLine(x.getNombre() + " " + x.getValor() + " " + x.getTipoDato() + " " + (x.getEsConstante() ? "Constante" : "Variable"));
                asm.Write(x.getNombre() + " ");
                if(x.getTipoDato() == Variable.tipo.CHAR)
                    asm.WriteLine("db ");
                else
                    asm.WriteLine("dw ");
            }
        }
    }
}