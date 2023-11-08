using System;
using System.IO;

namespace SimpleDBClient 
{
    class Program 
    {
        static void Main(string[] args) 
        {
            while(true) 
            {
                string? entrada = Console.ReadLine();

                if (entrada == null) continue;

                string[] entradaSeparada = entrada.Split(" ", 2);
                string metodo = entradaSeparada[0];
                string retorno = "command does not exist";

                switch(metodo) 
                {
                    case "insert":
                        retorno = Inserir();
                        break;
                    case "remove":
                        retorno = Remover();
                        break;
                    case "search":
                        retorno = Buscar();
                        break;
                    case "update":
                        retorno = Atualizar();
                        break;
                    case "quit":
                        return;
                }

                Console.WriteLine(retorno);
            }
        }

        static string Inserir()
        {
            return "inserted";
        }

        static string Remover()
        {
            return "removed";
        }

        static string Buscar()
        {
            return "valeu here";
        }

        static string Atualizar()
        {
            return "updated";
        }
    }
}
