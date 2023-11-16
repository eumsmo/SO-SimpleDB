using System;
using MSMQ.Messaging;

namespace SimpleDBClient {
    class Program {

        const string queuePath = ".\\Private$\\SimpleDBCliQueue";
        const string servidorQueuePath = ".\\Private$\\SimpleDBQueue";

        static void Main(string[] args) {
            CreateQueue();

            while (true) {
                string? entrada = Console.ReadLine();

                if (entrada == null) continue;

                string[] entradaSeparada = entrada.Split(" ", 2);
                string metodo = entradaSeparada[0];

                Comando comando = new Comando();

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
                        return;
                }

                string[] valores = entradaSeparada[1].Split(",", 2);

                int chave = int.Parse(valores[0]);
                comando.chave = chave;

                if (valores.Length > 1) {
                    comando.valor = valores[1];
                }


                MessageQueue messageQueue = new MessageQueue(servidorQueuePath);
                messageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(Comando)});

                MessageQueue cliMessageQueue = new MessageQueue(queuePath);
                cliMessageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                try 
                {
                    Message message = new Message(comando);
                    for(int i = 0; i < 3; i++)
                    {
                        messageQueue.Send(message);
                    }
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

            DeleteQueue();
        }

        static void CreateQueue() {
            if (!MessageQueue.Exists(queuePath))
            {
                MessageQueue.Create(queuePath);
            }
        }

        static void DeleteQueue() {
            if (MessageQueue.Exists(queuePath))
            {
                MessageQueue.Delete(queuePath);
            }
    }
}
