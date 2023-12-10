# 🤔 Como rodar o Banco de Dados
Além de rodar o banco de dados, podemos rodar comandos em linha de comando. Estes comando são executados uma única vez e não iniciam o sistema de comunicação do Banco de Dados, tendo como propósito debugar o banco de dados. Eles estão listados no arquivo [comandos_bd.md](./comandos_bd.md)!

## ✉️ Habilitando o MSMQ
Antes de mais nada, precisamos habilitar a comunicação entre processos, que neste caso é feita através do MSMQ! Só conseguimos ter uma comunicação entre o Banco de Dados e o Cliente se o MSMQ estiver habilitado. 

Para habilitar, siga os passos presentes no arquivo [como_habilitar_msmq.md](./como_habilitar_msmq.md)!

## 📂 Rodando o projeto
Para rodar o projeto primeiramente entramos na pasta `/SimpleDB/`, localizada no diretório principal do repositório. Dentro da pasta, executamos a seguinte ação em linha de comando:

    dotnet run [comando]

Se o comando for passado, executa o comando e encerra o processo. Se o comando for emitido, inicia a comunicação utilizando *MSMQ*.

## 📂 Rodando o projeto com Cache
Para rodar o projeto utilizando uma cache intermediária é necessário executar os mesmos passos de rodar o projeto normalmente, porém passando um comando que habilita a cache.

    dotnet run --cache-size=tamanho,politica

Ao rodar com uma cache, os valores serão guardados em memória principal antes de serem salvos na memória secundária. 

Os possíveis valores dos parâmetros `tamanho` e `politica` estão descritos no arquivo [comandos_bd.md](#./comandos_bd.md).