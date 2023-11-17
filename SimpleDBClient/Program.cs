using System;
using MSMQ.Messaging;
using System.Diagnostics;

namespace SimpleDBClient {
    class Program {

        const string queuePath = ".\\Private$\\SimpleDBCliQueue";
        const string servidorQueuePath = ".\\Private$\\SimpleDBQueue";

        static void Main(string[] args) {
            Process currentProcess = Process.GetCurrentProcess();
            string path = CreateQueue(currentProcess.Id);

            while (true) {
                string? entrada = Console.ReadLine();

                if (entrada == null) continue;

                string[] entradaSeparada = entrada.Split(" ", 2);
                string metodo = entradaSeparada[0];

                Requisicao comando = new Requisicao();
                comando.path = path;

                switch (metodo) {
                    case "insert":
                        comando.op = Operacao.Inserir;
                        break;
                    case "remove":
                        comando.op = Operacao.Remover;
                        break;
                    case "search":
                        comando.op = Operacao.Procurar;
                        break;
                    case "update":
                        comando.op = Operacao.Atualizar;
                        break;
                    case "quit":
                        DeleteQueue(path);
                        return;
                }

                string[] valores = entradaSeparada[1].Split(",", 2);

                int chave = int.Parse(valores[0]);
                comando.chave = chave;

                if (valores.Length > 1) {
                    comando.valor = valores[1];
                }


                MessageQueue messageQueue = new MessageQueue(servidorQueuePath);
                messageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(Requisicao)});

                MessageQueue cliMessageQueue = new MessageQueue(path);
                cliMessageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                cliMessageQueue.Purge();

                try {
                    messageQueue.Send(new Message(comando));
                    messageQueue.Close();

                    Message cliMessage = cliMessageQueue.Receive();
                    string resposta = (string)cliMessage.Body;
                    cliMessageQueue.Close();

                    Console.WriteLine(resposta);

                }
                catch (MessageQueueException e) {
                    Console.WriteLine("error: " + e.Message);
                } catch (InvalidOperationException e) {
                    Console.WriteLine("error: " + e.Message);
                }
            }
        }

        static string CreateQueue(int id) {
            string path = queuePath + "_" + id.ToString();
            if (!MessageQueue.Exists(path)) {
                MessageQueue.Create(path);
            }
            return path;
        }

        static void DeleteQueue(string path) {
            if (MessageQueue.Exists(path)) {
                MessageQueue.Delete(path);
            }
    }
}
