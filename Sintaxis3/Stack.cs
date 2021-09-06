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

        public void push(float element){
            if(ultimo < maxElementos){
                elementos[ultimo++] = element;
            }
        }

        public float pop(){
            if(ultimo >0){
                return elementos[--ultimo];
            }
            return 0;
        }
    }
}