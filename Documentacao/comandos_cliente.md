# 👨‍💻 Comandos do Cliente

|Comando|Parâmetros|Descrição
|--|--|--|
|[`insert`](#inserir-um-objeto)| `chave`, `valor` | Insere um objeto no banco de dados |
|[`remove`](#remover-um-objeto)| `chave` | Remove do banco de dados o objeto que é identificado pela `chave` |
|[`search`](#buscar-por-um-objeto)| `chave` | Busca no banco de dados um objeto que é identificado pela `chave` |
|[`update`](#alterar-um-objeto)| `chave`, `novo-valor` | Atualiza o objeto que é identificado pela `chave` |
|[`quit`](#encerrar-o-processo)|  | Encerra o programa cliente atual |

## Inserir um Objeto
**Comando:** `insert chave,valor`

Retorna a chave referente ao objeto inserido no banco de dados. Caso já exista um objeto com a mesma chave no banco de dados, retorna `key already exists` e não executa a inserção.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor inserido no banco de dados |
| `valor` | Valor a ser inserido no banco de dados, podendo ser de qualquer tipo, uma vez que será salvo como texto

## Remover um Objeto
**Comando:** `remove chave`

Retorna `removed` caso o valor tenha sido removido. Caso o objeto não tenha sido encontrado, ou seja, se a chave não bate com nenhuma outra chave armazenada no banco de dados, retorna `not found`.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor a ser removido do banco de dados |

## Buscar por um Objeto
**Comando:** `search chave`

Retorna o valor referenciado pela chave. Caso não encontre nenhum objeto com esta chave, retorna `not found`.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor buscado no banco de dados |

## Alterar um Objeto
**Comando:** `update chave,novo-valor`

Retorna a chave referente ao objeto alterado no banco de dados. Caso este objeto não exista, retorna `not found`.

|Parâmetro|Descrição|
|--|--|
| `chave` | Um número inteiro e positivo que serve de identificador para o valor a ser alterado no banco de dados |
| `novo-valor` | Novo valor a ser armazenado no banco de dados

## Encerrar o Processo
**Comando:** `quit`
**Comando alternativo** `ctrl + c`

Deleta o Message Queue criado pelo cliente atual e encerra o programa.
