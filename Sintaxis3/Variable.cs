namespace Sintaxis3
{
    public class Variable
    {
        public enum tipo{
            CHAR, INT, FLOAT, STRING
        };
        string nombre;
        string valor;
        tipo tipoDato;

        public Variable(string nombre, tipo tipoDato){
            this.nombre = nombre;
            this.tipoDato = tipoDato;
            valor = "";
        }

        public string getNombre(){
            return nombre;
        }


        public void setValor(string nuevoValor){
            valor = nuevoValor;
        }

        public string getValor(){
            return valor;
        }

        public tipo getTipoDato(){
            return tipoDato;
        }
    }
}