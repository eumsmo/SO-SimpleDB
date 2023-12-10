using System;
using System.Threading;
using MSMQ.Messaging;

namespace SimpleDB
{
    class Program
    {
        const string arquivoPath = "simpledb.db"; // banco de dados
        const string queuePath = ".\\Private$\\SimpleDBQueue"; // fila de mensagens
        const int cacheUpdateInterval = 1000; // intervalo de tempo para atualizar o cache

        static bool running = true;
        
        static void Main(string[] args)
        {
            CRUD bancoDeDados = new BancoDeDados(arquivoPath);

            if (args.Length > 0) {
                if (GetLineCommands(args, ref bancoDeDados)) {
                    return;
                }
            }

            CreateQueue();

            MessageQueue messageQueue = new MessageQueue(queuePath);
            messageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(Requisicao)});

            // No sinal de interrupção, deletar a fila
            Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) {
                bancoDeDados.Fechar();
                DeleteQueue();
                running = false;
            };

            if (bancoDeDados is LRUCache || bancoDeDados is AgingCache) {
                Thread thread = new Thread(() => UpdateCache(bancoDeDados));
                thread.Start();
            }

            while(running) {
                try {
                    Message message = messageQueue.Receive();
                    Requisicao requisicao = (Requisicao) message.Body;

                    // Criando thread para responder o cliente
                    Thread thread = new Thread(() => AnswerClient(bancoDeDados, requisicao));
                    thread.Start();
                } catch (MessageQueueException e) {
                    Console.WriteLine("error: " + e.Message);
                } 
            }
        }

        static void UpdateCache(CRUD bancoDeDados) {
            while (running) {
                Thread.Sleep(cacheUpdateInterval);
                bancoDeDados.Update();
            }
        }

        static void AnswerClient(CRUD bancoDeDados, Requisicao requisicao) {
            if (requisicao.path == null) return;
            if (!MessageQueue.Exists(requisicao.path)) return;

            // Processando resposta
            string resposta = bancoDeDados.Executar(requisicao);

            // Enviando resposta
            MessageQueue cliMessageQueue = new MessageQueue(requisicao.path);
            cliMessageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(string)});

            try {
                cliMessageQueue.Send(new Message(resposta));
            } catch (MessageQueueException e) {
                Console.WriteLine("error: " + e.Message);
            }

            cliMessageQueue.Close();
        }

        static void CreateQueue() {
            if (!MessageQueue.Exists(queuePath)) {
                MessageQueue.Create(queuePath);
            }
        }

        static void DeleteQueue() {
            if (MessageQueue.Exists(queuePath)) {
                MessageQueue.Delete(queuePath);
            }
        }

        static bool GetLineCommands(string[] args, ref CRUD bancoDeDados) {
            /*Args serão os argumentos do programa, o qual o usuário terá os seguintes comandos:
              . Inserir  . Remover  . Procurar  . Substituir
              
              Além disso, teremos as chaves e os valores:
              . Chave - Guarda referências do programa, sempre um inteiro positivo;
              . Valor - É uma informação qualquer oferecida pelo usuário.*/
            string[] separado = args[0].Split('=', 2);

            if (separado.Length < 2) {
                Console.WriteLine("invalid command: missing '='");
                return true;
            }

            string metodo = separado[0];
            string[] valores = separado[1].Split(',', 2); // [0] = chave, [1] = valor

            if (valores[0] == "") {
                Console.WriteLine("invalid command: key missing");
                return true;
            }

            int chave;
            if (!int.TryParse(valores[0], out chave)) {
                Console.WriteLine("invalid command: key must be an integer");
                return true;
            }

            if (chave < 0) {
                Console.WriteLine("invalid command: key must be positive");
                return true;
            }

            Comando comando = new Comando();
            comando.chave = chave;

            switch (metodo) {
                case "--insert":
                    if (!CheckValueInput(valores)) return true;
                    comando.valor = valores[1];
                    comando.op = Operacao.Inserir;
                    break;
                case "--remove":
                    if (!CheckKeyOnlyInput(valores)) return true;
                    comando.op = Operacao.Remover;
                    break;
                case "--search":
                    if (!CheckKeyOnlyInput(valores)) return true;
                    comando.op = Operacao.Procurar;
                    break;
                case "--update":
                    if (!CheckValueInput(valores)) return true;
                    comando.valor = valores[1];
                    comando.op = Operacao.Atualizar;
                    break;
                case "-cache-size":
                case "--cache-size": // Por algum motivo as vezes o windows não aceita ',' se tiver apenas um '-' no argumento
                    if (!CheckValueInput(valores)) return true;
                    CRUD? bd = BDCache.GetCache(bancoDeDados, chave, valores[1]);
                    if (bd == null) {
                        Console.WriteLine("invalid command: invalid cache values");
                        return true;
                    }
                    bancoDeDados = bd;
                    return false;
                default:
                    Console.WriteLine("invalid command: unknown command");
                    break;
            }
            
            try {
                string resposta = bancoDeDados.Executar(comando);
                Console.WriteLine(resposta);
            } catch (Exception e) {
                Console.WriteLine("error: " + e.Message);
            }

            return true;
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