using System;
using MSMQ.Messaging;

namespace SimpleDBClient {
    class Program {

        const string servidorQueuePath = ".\\Private$\\SimpleDBQueue";

        static void Main(string[] args) {
            while(true) {
                string? entrada = Console.ReadLine();

                if (entrada == null) continue;

                string[] entradaSeparada = entrada.Split(" ", 2);
                string metodo = entradaSeparada[0];
                string[] valores = entradaSeparada[1].Split(",", 2);

                int chave = int.Parse(valores[0]);
                string valor = valores[1];

                Comando comando = new Comando();
                comando.chave = chave;
                comando.valor = valor;
                
                switch(metodo) {
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
                        comando.op = Operacao.Substituir;
                        break;
                    case "quit":
                        return;
                }

                MessageQueue messageQueue = new MessageQueue(servidorQueuePath);
                messageQueue.Formatter = new XmlMessageFormatter(new Type[]{typeof(Comando)});

                try {
                    Message message = new Message(comando);
                    messageQueue.Send(message);
                    Console.WriteLine("Message sent");
                    messageQueue.Close();
                } catch (MessageQueueException e) {
                    Console.WriteLine("error: " + e.Message);
                } catch (InvalidOperationException e) {
                    Console.WriteLine("error: " + e.Message);
                }
            }
        }

        static string Inserir() {
            return "inserted";
        }

        static string Remover() {
            return "removed";
        }

        static string Buscar() {
            return "value here";
        }

        static string Atualizar() {
            return "updated";
        }
    }
}
