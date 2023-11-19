# 🤔 Como rodar o Cliente
Ao rodar o cliente, precisamos saber quais são os comandos suportados. Todos estão listados no arquivo [comandos_cliente.md](./comandos_cliente.md)!

## ✉️ Habilitando o MSMQ
Antes de mais nada, precisamos habilitar a comunicação entre processos, que neste caso é feita através do MSMQ! Só conseguimos ter uma comunicação entre o Banco de Dados e o Cliente se o MSMQ estiver habilitado. 

Para habilitar, siga os passos presentes no arquivo [como_habilitar_msmq.md](./como_habilitar_msmq.md)!

## 📂 Rodando o projeto
Para rodar o projeto primeiramente entramos na pasta `/SimpleDBClient/`, localizada no diretório principal do repositório. Dentro da pasta, executamos a seguinte ação em linha de comando:

    dotnet run

Ao rodar, o cliente aceitará os comandos pelo dispositivo de entrada padrão. Assim que recebido, é feita uma requisição para o Banco de Dados que o retorna uma resposta a ser impressa na saída padrão. Quando a resposta for exibida, volta a aceitar novas entradas do usuário.