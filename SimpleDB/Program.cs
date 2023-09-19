using System;
using System.IO;

namespace SimpleDB {
    class Program {
        static string arquivoPath = "dados.txt"; // banco de dados

        static void Main(string[] args) {
            if (args.Length == 0) return;
            
            string[] separado = args[0].Split('=');

            string comando = separado[0];
            string[] valores = separado[1].Split(','); // [0] = chave, [1] = valor

            switch (comando) {
                case "--insert":
                    if (Inserir(valores[0], valores[1])) Console.WriteLine("inserted");
                    else Console.WriteLine("key already exists");

                    break;
                case "--remove":
                    Remover(valores[0]);
                    break;
                case "--search":
                    string? valor = Buscar(valores[0]);

                    if (valor != null) Console.WriteLine(valor);
                    else Console.WriteLine("not found");

                    break;
                case "--update":
                    Atualizar(valores[0], valores[1]);
                    break;
            }
        }

        static bool Inserir(string chave, string valor) {
            if (Buscar(chave) != null) {
                return false;
            }

            StreamWriter arquivo = new StreamWriter(arquivoPath, true);

            arquivo.BaseStream.Seek(0, SeekOrigin.End);
            arquivo.WriteLine(chave + "," + valor);

            arquivo.Close();

            return true;
        }

        static void Remover(string chave) {
            Console.WriteLine("Remover [" + chave + "]");
        }

        static string? Buscar(string chave) {
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream) {
                return null;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null) {
                string[] separado = linha.Split(',');

                if (separado[0] == chave) {
                    return separado[1];
                }

                linha = arquivo.ReadLine();
            }

            arquivo.Close();

            return null;
        }

        static void Atualizar(string chave, string novoValor) {
            Console.WriteLine("Atualizar [" + chave + "] = <" + novoValor + ">");
        }
    }
}