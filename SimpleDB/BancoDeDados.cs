using System;
using System.IO;

namespace SimpleDB 
{
    public class BancoDeDados
    {
        string arquivoPath; // banco de dados
        string tempPrefix; // prefixo para arquivos temporários

        public BancoDeDados(string arquivoPath, string tempPrefix = "temp_") 
        {
            this.arquivoPath = arquivoPath;
            this.tempPrefix = tempPrefix;
        }

        public string Executar(Comando comando) {
            switch (comando.op) {
                case Operacao.Inserir:
                    if (comando.valor == null) return "invalid command: invalid value";

                    if (Inserir(comando.chave, comando.valor)) return comando.chave.ToString();
                    return "already exists";
                case Operacao.Remover:
                    if (Remover(comando.chave)) return "removed";
                    return "not found";
                case Operacao.Procurar:
                    string? valor = Buscar(comando.chave);

                    if (valor != null) return valor;
                    return "not found";
                case Operacao.Atualizar:
                    if (comando.valor == null) return "invalid command: invalid value";

                    if (Atualizar(comando.chave, comando.valor)) return comando.chave.ToString();
                    return "not found";
            }

            return "invalid command: unknown command";
        }

        public bool Inserir(int chave, string valor)
        {
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

            StreamWriter arquivo = new StreamWriter(arquivoPath, true);

            arquivo.BaseStream.Seek(0, SeekOrigin.End);
            arquivo.WriteLine(chave + "," + valor);

            arquivo.Close();

            return true;
        }

        public bool Remover(int chave) {
            return Remover(chave.ToString());
        }


        public bool Remover(string chave)
        {
            if (!File.Exists(arquivoPath)) {
                return false;
            }

            string tempPath = tempPrefix + arquivoPath;
            bool removeu = false;

            // Cria um arquivo temporário
            StreamWriter temp_arquivo = new StreamWriter(tempPath, true);
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream || temp_arquivo == null) {
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

            return removeu;
        }

        public string? Buscar(int chave) {
            return Buscar(chave.ToString());
        }


        public string? Buscar(string chave)
        {
            if (!File.Exists(arquivoPath)) {
                return null;
            }

            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream) {
                return null;
            }

            string? linha = arquivo.ReadLine();

            while (linha != null) {
                string[] separado = linha.Split(',', 2);

                if (separado[0] == chave) {
                    arquivo.Close();
                    return separado[1];
                }

                linha = arquivo.ReadLine();
            }

            arquivo.Close();

            return null;
        }

        public bool Atualizar(int chave, string novoValor) {
            return Atualizar(chave.ToString(), novoValor);
        }


        public bool Atualizar(string chave, string novoValor)
        {
            string tempPath = tempPrefix + arquivoPath;
            bool editou = false;

            // Cria um arquivo temporário
            StreamWriter temp_arquivo = new StreamWriter(tempPath, true);
            StreamReader arquivo = new StreamReader(arquivoPath);

            if (arquivo == null || arquivo.EndOfStream || temp_arquivo == null) {
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

            return editou;
        }
    }
}