using System;
using System.IO;

namespace SimpleDB
{
    class Program
    {
        const string arquivoPath = "simpledb.db"; // banco de dados
        const string tempPrefix = "temp_"; // prefixo para arquivos temporários

        static void Main(string[] args)
        {
            if (args.Length == 0) return;

            string[] separado = args[0].Split('=');

            string comando = separado[0];
            string[] valores = separado[1].Split(','); // [0] = chave, [1] = valor

            switch (comando)
            {
                case "--insert":
                    if (Inserir(valores[0], valores[1])) Console.WriteLine("inserted");
                    else Console.WriteLine("key already exists");

                    break;
                case "--remove":
                    if (Remover(valores[0])) Console.WriteLine("removed");
                    else Console.WriteLine("not found");
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

        static bool Inserir(string chave, string valor)
        {
            if (Buscar(chave) != null)
            {
                return false;
            }

            StreamWriter arquivo = new StreamWriter(arquivoPath, true);

            arquivo.BaseStream.Seek(0, SeekOrigin.End);
            arquivo.WriteLine(chave + "," + valor);

            arquivo.Close();

            return true;
        }

        static bool Remover(string chave)
        {
            string tempPath = tempPrefix + arquivoPath;
            bool removeu = false;

            // Cria um arquivo temporário
            StreamWriter temp_arquivo = new StreamWriter(tempPath, true);
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream || temp_arquivo == null)
            {
                return false;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null)
            {
                string[] separado = linha.Split(',');

                // Passa linha por linha para o arquivo temporário e se a chave for igual, ignora
                if (separado[0] == chave) removeu = true;
                else temp_arquivo.WriteLine(linha);

                linha = arquivo.ReadLine();
            }

            arquivo.Close();
            temp_arquivo.Close();

            if (removeu)
            {
                // Substitui o arquivo original pelo temporário
                File.Delete(arquivoPath);
                File.Move(tempPath, arquivoPath);
            }
            else
            {
                // Remove o arquivo temporário
                File.Delete(tempPath);
            }

            return removeu;
        }

        static string? Buscar(string chave)
        {
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream)
            {
                return null;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null)
            {
                string[] separado = linha.Split(',');

                if (separado[0] == chave)
                {
                    return separado[1];
                }

                linha = arquivo.ReadLine();
            }

            arquivo.Close();

            return null;
        }

        static void Atualizar(string chave, string novoValor)
        {
            Console.WriteLine("Atualizar [" + chave + "] = <" + novoValor + ">");
        }
    }
}