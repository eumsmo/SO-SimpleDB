# 🤔 Como funciona?
O projeto possui dois projetos diferentes: o Cliente e o Banco de Dados. O Cliente recebe o input do usuário na entrada padrão e envia requisições para o Banco de Dados utilizando o *Message Queue da Microsoft (MSMQ)*. Então, o Banco de Dados recebe a requisição e executa a operação desejada em uma Thread separada, ou seja, ele permite múltiplos leitores simultâneos. Após executar a operação, retorna o resultado para o Message Queue do Cliente.


## ✉️ O que é o Message Queue (MSMQ)?
O Message Queue é um sistema de comunicação entre processos desenvolvido pela Microsoft.  Uma `queue` é um armazenamento temporário onde mensagens podem ser enviadas e recebidas. Assim, um processo pode ouvir uma `queue` e qualquer outro processo pode enviar uma mensagem para essa `queue`, permitindo então que se comuniquem.  

Atualmente o MSMQ não vem habilitado como padrão, o que é uma pena, então devemos habilitar por conta própria. Para habilitar, siga os passos do arquivo [como_habilitar_msmq.md](./como_habilitar_msmq.md)!

## 💾 Como funciona o Banco de Dados?
É possível rodar múltiplos processos Cliente ao mesmo tempo, porém todos irão fazer requisições a apenas um Banco de Dados. Sendo assim, devemos rodar o Banco de Dados antes de rodar qualquer Cliente.

Ao iniciar, o Banco de Dados fica esperando por requisições em seu Message Queue. Assim que uma chega, cria uma nova Thread responsável para executar a operação e responder o cliente, e na Thread principal, volta a esperar por outras requisições.

Os dados ficam salvos em um arquivo de texto `simpledb.db`. Cada registro é salvo em sua própria linha com o formato `id,valor`. As inserções ocorrem no final do arquivo, porém as alterações de valores não mudam a posição do registro no arquivo. 

Para rodar o Banco de Dados, siga os passos do arquivo [como_rodar_bd.md](./como_rodar_bd.md)!

## 👤 Como funciona o Cliente?
Com o Banco de Dados rodando, você poderá rodar quantos Clientes você quiser. Cada cliente cria seu próprio Message Queue, que utiliza o `id` do processo como identificador. Dessa maneira, a resposta irá diretamente para o cliente que fez a requisição.

Ao iniciar, o Cliente aguarda a entrada do usuário. Assim que uma entrada valida for informada, envia uma requisição ao Message Queue do Banco de Dados e fica aguardando a resposta em seu Message Queue. Enquanto aguarda, nenhuma outra entrada do usuário é lida. Assim que é respondido, imprime a resposta na saída padrão e volta a escutar até que o comando de sair (ou o sinal de cancelar) seja chamado.

Para rodar o Cliente, siga os passos do arquivo [como_rodar_cliente.md](./como_rodar_cliente.md)!


