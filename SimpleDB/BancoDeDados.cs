using System;
using System.IO;
using System.Threading;

namespace SimpleDB 
{
    public class BancoDeDados: CRUD {
        string arquivoPath; // banco de dados
        string tempPrefix; // prefixo para arquivos temporários

        Semaphore semaphore;
        Mutex mutex;
        int leitores;

        public BancoDeDados(string arquivoPath, string tempPrefix = "temp_") 
        {
            this.arquivoPath = arquivoPath;
            this.tempPrefix = tempPrefix;
            mutex = new Mutex();
            semaphore = new Semaphore(1, 1);
            leitores = 0;
        }

        public override bool Inserir(int chave, string valor){
            return Inserir(chave.ToString(), valor);
        }

        public bool Inserir(string chave, string valor)
        {
            if (Buscar(chave) != null) {
                return false;
            }

            /* O comando StreamWrite poderá receber um arquivo ou até mesmo cria-lo se o mesmo não existir.
            Logo depois, ele vai abrir o arquivo e lê-lo, após inserir as informações o mesmo será fechado, desse
            modo, evitando que o arquivo fique aberto e impeça as outras pessoas de acessarem. */

            semaphore.WaitOne(); // Down no semáforo

            StreamWriter arquivo = new StreamWriter(arquivoPath, true);

            arquivo.BaseStream.Seek(0, SeekOrigin.End);
            arquivo.WriteLine(chave + "," + valor);

            arquivo.Close();


            semaphore.Release(); // Up no semáforo

            return true;
        }

        public override bool Remover(int chave) {
            return Remover(chave.ToString());
        }

        public bool Remover(string chave)
        {
            if (!File.Exists(arquivoPath)) {
                return false;
            }

            string tempPath = tempPrefix + arquivoPath;
            bool removeu = false;

            semaphore.WaitOne(); // Down no semáforo

            // Cria um arquivo temporário
            StreamWriter temp_arquivo = new StreamWriter(tempPath, true);
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream || temp_arquivo == null) {
                semaphore.Release(); // Up no semáforo
                return false;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null) {
                string[] separado = linha.Split(',', 2);

                // Passa linha por linha para o arquivo temporário e se a chave for igual, ignora
                if (separado[0] == chave) removeu = true;
                else temp_arquivo.WriteLine(linha);

                linha = arquivo.ReadLine();
            }

            arquivo.Close();
            temp_arquivo.Close();

            if (removeu) {
                // Substitui o arquivo original pelo temporário
                File.Delete(arquivoPath);
                File.Move(tempPath, arquivoPath);
            }
            else {
                // Remove o arquivo temporário
                File.Delete(tempPath);
            }

            semaphore.Release(); // Up no semáforo

            return removeu;
        }

        public override string? Buscar(int chave) {
            return Buscar(chave.ToString());
        }

        public string? Buscar(string chave)
        {
            if (!File.Exists(arquivoPath)) {
                return null;
            }

            mutex.WaitOne(); // Lock no mutex
            leitores++;

            if (leitores == 1) {
                semaphore.WaitOne(); // Down no semáforo
            }

            mutex.ReleaseMutex(); // Unlock no mutex

            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream) {
                semaphore.Release(); // Up no semáforo
                return null;
            }

            string? linha = arquivo.ReadLine();
            string? resultado = null;

            while (linha != null) {
                string[] separado = linha.Split(',', 2);

                if (separado[0] == chave) {
                    resultado = separado[1];
                    break;
                }

                linha = arquivo.ReadLine();
            }

            arquivo.Close();

            mutex.WaitOne(); // Lock no mutex
            leitores--;

            if (leitores == 0) {
                semaphore.Release(); // Up no semáforo
            }

            mutex.ReleaseMutex(); // Unlock no mutex

            return resultado;
        }

        public override bool Atualizar(int chave, string novoValor) {
            return Atualizar(chave.ToString(), novoValor);
        }

        public bool Atualizar(string chave, string novoValor)
        {
            string tempPath = tempPrefix + arquivoPath;
            bool editou = false;

            semaphore.WaitOne(); // Down no semáforo

            // Cria um arquivo temporário
            StreamWriter temp_arquivo = new StreamWriter(tempPath, true);
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream || temp_arquivo == null) {
                semaphore.Release(); // Up no semáforo
                return false;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null) {
                string[] separado = linha.Split(',', 2);

                // Passa linha por linha para o arquivo temporário e se a chave for igual, insere o novo valor
                if (!editou && separado[0] == chave) {
                    temp_arquivo.WriteLine(chave + "," + novoValor);
                    editou = true;
                }
                else temp_arquivo.WriteLine(linha);

                linha = arquivo.ReadLine();
            }

            arquivo.Close();
            temp_arquivo.Close();

            if (editou) {
                // Substitui o arquivo original pelo temporário
                File.Delete(arquivoPath);
                File.Move(tempPath, arquivoPath);
            }
            else {
                // Remove o arquivo temporário
                File.Delete(tempPath);
            }

            semaphore.Release(); // Up no semáforo

            return editou;
        }
    }
}