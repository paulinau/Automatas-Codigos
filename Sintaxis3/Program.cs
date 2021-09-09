using System;

namespace Sintaxis3
{
    class Program
    {
        static void Main(string[] args)
        {
            try{
                //ejecutar dispose cuando se destruye el objeto
                //using (Lenguaje l = new Lenguaje("C:\\archivos\\suma.cpp")){
                using (Lenguaje l = new Lenguaje("C:\\archivos\\suma.cpp")){          
                    //instanciamos nuestra clase
                    /*
                    while(!l.finArchivo()){
                        l.nextToken();
                    }*/
                    l.Programa();
                }
            }catch(Exception e){
                Console.Write(e.Message);
            }
            Console.ReadKey();
        }
    }
}
