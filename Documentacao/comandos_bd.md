# 👨‍💻 Comandos do Banco de Dados
Ao iniciar o Banco de Dados, você pode passar um comando opcional que será executado ao invés de iniciar a comunicação entre processos. Todos os comandos disponíveis estão listados neste arquivo.

|Comando|Parâmetros|Descrição
|--|--|--|
|[`--insert`](#inserir-um-objeto)| `chave`, `valor` | Insere um objeto no banco de dados |
|[`--remove`](#remover-um-objeto)| `chave` | Remove do banco de dados o objeto que é identificado pela `chave` |
|[`--search`](#buscar-por-um-objeto)| `chave` | Busca no banco de dados um objeto que é identificado pela `chave` |
|[`--update`](#alterar-um-objeto)| `chave`, `novo-valor` | Atualiza o objeto que é identificado pela `chave` |
|[`-cache-size` ou `--cache-size`](#usar-a-cache)| `tamanho`, `poltiica` | Inicia o banco rodando uma cache como o tamanho definido por `tamanho` e politica de substituição de registros pela `politica`, podendo ser: `FIFO`, `LRU` ou `Aging`.

## Inserir um Objeto
**Comando:** `--insert=chave,valor`

Retorna a chave referente ao objeto inserido no banco de dados. Caso já exista um objeto com a mesma chave no banco de dados, retorna `key already exists` e não executa a inserção.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor inserido no banco de dados |
| `valor` | Valor a ser inserido no banco de dados, podendo ser de qualquer tipo, uma vez que será salvo como texto

## Remover um Objeto
**Comando:** `--remove=chave`

Retorna `removed` caso o valor tenha sido removido. Caso o objeto não tenha sido encontrado, ou seja, se a chave não bate com nenhuma outra chave armazenada no banco de dados, retorna `not found`.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor a ser removido do banco de dados |

## Buscar por um Objeto
**Comando:** `--search=chave`

Retorna o valor referenciado pela chave. Caso não encontre nenhum objeto com esta chave, retorna `not found`.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor buscado no banco de dados |

## Alterar um Objeto
**Comando:** `--update=chave,novo-valor`

Retorna a chave referente ao objeto alterado no banco de dados. Caso este objeto não exista, retorna `not found`.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor a ser alterado no banco de dados |
| `novo-valor` | Novo valor a ser armazenado no banco de dados

## Usar a Cache
**Comando:** `-cache-size=tamanho,politica` ou `--cache-size=tamanho,politica`

Inicia o banco de dados com uma cache intermediária, mantendo **N** dados (definido pelo parâmetro `tamanho`) na memória principal antes de mandar para o banco de dados. Quando o numero de instâncias carregadas passa o tamanho da cache, utiliza a política de substituição definida pelo parâmetro `politica` para dar espaço para a nova instância. Quando uma instância é removida, ou a aplicação é fechada, seus dados são efetivamente executados no banco de dados.

|Parâmetro|Descrição|
|--|--|
| `tamanho` | Um número inteiro e positivo que determina a quantidade de dados carregados em memória principal |
| `politica` | Uma string que define a politica de substituição utilizada pela cache, podendo ser: `FIFO`, `LRU` ou `Aging`