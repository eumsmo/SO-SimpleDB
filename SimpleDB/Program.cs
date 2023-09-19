using System;

namespace SimpleDB {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) return;
            
            string[] separado = args[0].Split('='); // [0] = comando, [1] = argumento
            string[] valores = separado[1].Split(',');


            switch (separado[0]) {
                case "--insert":
                    Inserir(valores[0], valores[1]);
                    break;
                case "--remove":
                    Remover(valores[0]);
                    break;
                case "--search":
                    Buscar(valores[0]);
                    break;
                case "--update":
                    Atualizar(valores[0], valores[1]);
                    break;
            }
        }

        static void Inserir(string chave, string valor) {
            Console.WriteLine("Inserir [" + chave + "] = <" + valor + ">");
        }

        static void Remover(string chave) {
            Console.WriteLine("Remover [" + chave + "]");
        }

        static void Buscar(string chave) {
            Console.WriteLine("Buscar <" + chave + ">");
        }

        static void Atualizar(string chave, string novoValor) {
            Console.WriteLine("Atualizar [" + chave + "] = <" + novoValor + ">");
        }
    }
}