using System;
using MSMQ.Messaging;

namespace SimpleDB
{
    class Program
    {
        const string arquivoPath = "simpledb.db"; // banco de dados
        const string queuePath = ".\\Private$\\SimpleDBQueue"; // fila de mensagens
        const string cliQueuePath = ".\\Private$\\SimpleDBCliQueue"; // fila de mensagens do cliente 


        static void Main(string[] args)
        {
            BancoDeDados bancoDeDados = new BancoDeDados(arquivoPath);

            if (args.Length > 0) {
                GetLineCommands(args, bancoDeDados);
                return;
            }

            CreateQueue();

            MessageQueue messageQueue = new MessageQueue(queuePath);
            messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Comando) });

            MessageQueue cliMessageQueue = new MessageQueue(cliQueuePath);
            cliMessageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            while (true) {
                try {
                    Message message = messageQueue.Receive();
                    Comando comando = (Comando)message.Body;
                    messageQueue.Close();

                    string resposta = bancoDeDados.Executar(comando);
                    Message cliMessage = new Message(resposta);
                    cliMessageQueue.Send(cliMessage);
                    cliMessageQueue.Close();
                }
                catch (MessageQueueException e) {
                    Console.WriteLine("error: " + e.Message);
                }
                catch (InvalidOperationException e) {
                    Console.WriteLine("error: " + e.Message);
                }
            }
            // DeleteQueue();
        }

        static void CreateQueue() {
            if (!MessageQueue.Exists(queuePath)) {
                MessageQueue.Create(queuePath);
            }
        }

        static void DeleteQueue() {
            if (MessageQueue.Exists(queuePath)) {
                MessageQueue.Delete(queuePath);
                Console.WriteLine("Queue deleted");
            }
            else {
                Console.WriteLine("Queue does not exist");
            }
        }

        static void GetLineCommands(string[] args, BancoDeDados bancoDeDados) {
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

            string metodo = separado[0];
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

            Comando comando = new Comando();
            comando.chave = chave;

            switch (metodo) {
                case "--insert":
                    if (!CheckValueInput(valores)) return;
                    comando.valor = valores[1];
                    comando.op = Operacao.Inserir;
                    break;
                case "--remove":
                    if (!CheckKeyOnlyInput(valores)) return;
                    comando.op = Operacao.Remover;
                    break;
                case "--search":
                    if (!CheckKeyOnlyInput(valores)) return;
                    comando.op = Operacao.Procurar;
                    break;
                case "--update":
                    if (!CheckValueInput(valores)) return;
                    comando.valor = valores[1];
                    comando.op = Operacao.Atualizar;
                    break;
                default:
                    Console.WriteLine("invalid command: unknown command");
                    break;
            }

            try {
                string resposta = bancoDeDados.Executar(comando);
                Console.WriteLine(resposta);
            }
            catch (Exception e) {
                Console.WriteLine("error: " + e.Message);
            }
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