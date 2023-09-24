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

            /*Args serão os argumentos do programa, o qual o usuário terá os seguintes comandos:
              . Inserir  . Remover  . Procurar  . Substituir
              
              Além disso, teremos as chaves e os valores:
              . Chave - Guarda referências do programa, sempre um inteiro positivo;
              . Valor - É uma informação qualquer oferecida pelo usuário.*/
            string[] separado = args[0].Split('=', 2);

            if (separado.Length < 2) {
                Console.WriteLine("invalid command: missing '='");
                return;
            }

            string comando = separado[0];
            string[] valores = separado[1].Split(',', 2); // [0] = chave, [1] = valor

            if (valores[0] == "") {
                Console.WriteLine("invalid command: key missing");
                return;
            }

            int chave;
            if (!int.TryParse(valores[0], out chave)) {
                Console.WriteLine("invalid command: key must be an integer");
                return;
            }

            if (chave < 0) {
                Console.WriteLine("invalid command: key must be positive");
                return;
            }

            try {

                switch (comando) {
                    case "--insert":
                        if (!CheckValueInput(valores)) return;

                        if (Inserir(valores[0], valores[1])) Console.WriteLine(valores[0]);
                        else Console.WriteLine("key already exists");

                        break;
                    case "--remove":
                        if (!CheckKeyOnlyInput(valores)) return;

                        if (Remover(valores[0])) Console.WriteLine("removed");
                        else Console.WriteLine("not found");

                        break;
                    case "--search":
                        if (!CheckKeyOnlyInput(valores)) return;

                        string? valor = Buscar(valores[0]);

                        if (valor != null) Console.WriteLine(valor);
                        else Console.WriteLine("not found");

                        break;
                    case "--update":
                        if (!CheckValueInput(valores)) return;

                        if (Atualizar(valores[0], valores[1])) Console.WriteLine(valores[0]);
                        else Console.WriteLine("not found");

                        break;
                    default:
                        Console.WriteLine("invalid command: unknown command");
                        break;
                }

            } catch (Exception e) {
                Console.WriteLine("error: " + e.Message);
            }
        }

        static bool Inserir(string chave, string valor)
        {
            if (Buscar(chave) != null)
            {
                return false;
            }

            /* O comando StreamWrite poderá receber um arquivo ou até mesmo cria-lo se o mesmo não existir.
            Logo depois, ele vai abrir o arquivo e lê-lo, após inserir as informações o mesmo será fechado, desse
            modo, evitando que o arquivo fique aberto e impeça as outras pessoas de acessarem. */

            StreamWriter arquivo = new StreamWriter(arquivoPath, true);

            arquivo.BaseStream.Seek(0, SeekOrigin.End);
            arquivo.WriteLine(chave + "," + valor);

            arquivo.Close();

            return true;
        }

        static bool Remover(string chave)
        {
            if (!File.Exists(arquivoPath)) {
                return false;
            }

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
                string[] separado = linha.Split(',', 2);

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
            if (!File.Exists(arquivoPath)) {
                return null;
            }

            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream)
            {
                return null;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null)
            {
                string[] separado = linha.Split(',', 2);

                if (separado[0] == chave)
                {
                    return separado[1];
                }

                linha = arquivo.ReadLine();
            }

            arquivo.Close();

            return null;
        }

        static bool Atualizar(string chave, string novoValor)
        {
            string tempPath = tempPrefix + arquivoPath;
            bool editou = false;

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
                string[] separado = linha.Split(',', 2);

                // Passa linha por linha para o arquivo temporário e se a chave for igual, insere o novo valor
                if (!editou && separado[0] == chave)
                {
                    temp_arquivo.WriteLine(chave + "," + novoValor);
                    editou = true;
                }
                else temp_arquivo.WriteLine(linha);

                linha = arquivo.ReadLine();
            }

            arquivo.Close();
            temp_arquivo.Close();

            if (editou)
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

            return editou;
        }

        static bool CheckValueInput(string[] valores) {
            if (valores.Length < 2) {
                Console.WriteLine("invalid command: value missing");
                return false;
            }

            return true;
        }

        static bool CheckKeyOnlyInput(string[] valores) {
            if (valores.Length == 1) return true;


            Console.WriteLine("invalid command: too many values");
            return false;
        }
    }
}