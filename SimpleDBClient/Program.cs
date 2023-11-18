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

            // No sinal de interrupção, deletar a fila
            Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) {
                DeleteQueue(path);
            };

            while (true) {
                string? entrada = Console.ReadLine();

                if (entrada == null) continue;

                string[] entradaSeparada = entrada.Split(" ", 2);
                string metodo = entradaSeparada[0];

                if (metodo == "quit") {
                    DeleteQueue(path);
                    return;
                }

                if (entradaSeparada.Length < 2) {
                    Console.WriteLine("invalid command");
                    continue;
                }

                string[] valores = entradaSeparada[1].Split(",", 2);

                int chave;

                if (!int.TryParse(valores[0], out chave)) {
                    Console.WriteLine("invalid command");
                    continue;
                }

                string? valor = null;

                if (valores.Length > 1) {
                    valor = valores[1];
                }

                Requisicao? comando = null;

                switch(metodo) {
                    case "insert":
                        if (valor == null) {
                            Console.WriteLine("invalid command");
                            continue;
                        }

                        comando = Insert(chave, valor);
                        break;
                    case "remove":
                        comando = Remove(chave);
                        break;
                    case "search":
                        comando = Search(chave);
                        break;
                    case "update":
                        if (valor == null) {
                            Console.WriteLine("invalid command");
                            continue;
                        }

                        comando = Update(chave, valor);
                        break;
                    default:
                        Console.WriteLine("invalid command");
                        continue;
                }

                comando.path = path;

                MessageQueue messageQueue = new MessageQueue(servidorQueuePath);
                messageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(Requisicao)});

                MessageQueue cliMessageQueue = new MessageQueue(path);
                cliMessageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                cliMessageQueue.Purge();

                try {
                    messageQueue.Send(new Message(comando));
                    messageQueue.Close();

                    Message cliMessage = cliMessageQueue.Receive();
                    string resposta = (string) cliMessage.Body;
                    cliMessageQueue.Close();

                    Console.WriteLine(resposta);

                } catch (MessageQueueException e) {
                    Console.WriteLine("error: " + e.Message);
                } catch (InvalidOperationException e) {
                    Console.WriteLine("error: " + e.Message);
                }
            }
        }

        public static Requisicao Insert(int chave, string valor) {
            Requisicao comando = new Requisicao();
            comando.op = Operacao.Inserir;
            comando.chave = chave;
            comando.valor = valor;
            return comando;
        }

        public static Requisicao Remove(int chave) {
            Requisicao comando = new Requisicao();
            comando.op = Operacao.Remover;
            comando.chave = chave;
            return comando;
        }

        public static Requisicao Search(int chave) {
            Requisicao comando = new Requisicao();
            comando.op = Operacao.Procurar;
            comando.chave = chave;
            return comando;
        }

        public static Requisicao Update(int chave, string valor) {
            Requisicao comando = new Requisicao();
            comando.op = Operacao.Atualizar;
            comando.chave = chave;
            comando.valor = valor;
            return comando;
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
}
