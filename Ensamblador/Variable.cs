namespace Ensamblador
{
    public class Variable
    {
        public enum tipo
        {
            CHAR, INT, FLOAT, STRING
        };
        string nombre;
        string valor;
        tipo tipoDato;
        bool esConstante;

        public Variable(string nombre, tipo tipoDato, bool esConstante)
        {
            this.nombre = nombre;
            this.tipoDato = tipoDato;
            valor = "";
            this.esConstante = esConstante;
        }

        public bool getEsConstante()
        {
            return esConstante;
        }

        public string getNombre()
        {
            return nombre;
        }

        public void setValor(string nuevoValor)
        {
            valor = nuevoValor;
        }

        public string getValor()
        {
            return valor;
        }

        public tipo getTipoDato()
        {
            return tipoDato;
        }
    }
}