using System;
using MSMQ.Messaging;

namespace SimpleDB
{
    class Program
    {
        const string arquivoPath = "simpledb.db"; // banco de dados
        const string queuePath = ".\\Private$\\SimpleDBQueue"; // fila de mensagens
        
        static void Main(string[] args)
        {
            BancoDeDados bancoDeDados = new BancoDeDados(arquivoPath);

            if (args.Length > 0) {
                GetLineCommands(args, bancoDeDados);
                return;
            }

            CreateQueue();

            Comando teste = new Comando();
            teste.op = Operacao.Inserir;
            teste.chave = 1;
            teste.valor = "teste";

            MessageQueue messageQueue = new MessageQueue(queuePath);
            messageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(Comando)});

            try {
                messageQueue.Send(teste);
                Console.WriteLine("Message sent");

                Message message = messageQueue.Receive();
                Console.WriteLine("Message received");

                Comando comando = (Comando) message.Body;
                Console.WriteLine(comando.op);
                Console.WriteLine(comando.chave);
                Console.WriteLine(comando.valor);
                messageQueue.Close();

            } catch (MessageQueueException e) {
                Console.WriteLine("error: " + e.Message);
            } catch (InvalidOperationException e) {
                Console.WriteLine("error: " + e.Message);
            }

            DeleteQueue();
        }

        static void CreateQueue() {
            if (!MessageQueue.Exists(queuePath)) {
                MessageQueue.Create(queuePath);
                Console.WriteLine("Queue created");
            } else {
                Console.WriteLine("Queue already exists");
            }
        }

        static void DeleteQueue() {
            if (MessageQueue.Exists(queuePath)) {
                MessageQueue.Delete(queuePath);
                Console.WriteLine("Queue deleted");
            } else {
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

                        if (bancoDeDados.Inserir(valores[0], valores[1])) Console.WriteLine(valores[0]);
                        else Console.WriteLine("key already exists");

                        break;
                    case "--remove":
                        if (!CheckKeyOnlyInput(valores)) return;

                        if (bancoDeDados.Remover(valores[0])) Console.WriteLine("removed");
                        else Console.WriteLine("not found");

                        break;
                    case "--search":
                        if (!CheckKeyOnlyInput(valores)) return;

                        string? valor = bancoDeDados.Buscar(valores[0]);

                        if (valor != null) Console.WriteLine(valor);
                        else Console.WriteLine("not found");

                        break;
                    case "--update":
                        if (!CheckValueInput(valores)) return;

                        if (bancoDeDados.Atualizar(valores[0], valores[1])) Console.WriteLine(valores[0]);
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