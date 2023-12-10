## Parte A 
O banco de dados deve oferecer os seguintes requisitos funcionais: 
-  [x] **RF-1:** Suportar a inserção de objetos no banco de dados. 
-  [x] **RF-2:** Suportar a remoção objetos do banco de dados. 
-  [x] **RF-3:** Suportar atualização objetos no banco de dados. 
-  [x] **RF-4:** Suportar a pesquisa objetos no banco de dados. 
-  [x] **RF-5:** Ser capaz de persistir os objetos do banco de dados em um arquivo. 
 
Além dos requisitos funcionais listados anteriormente, o seu banco de dados deve apresentar os seguintes requisitos não funcionais: 
-  [x] **RFN-1:** Suportar uma interface por linha de comandos para realizar operações diretas no banco de dados. 
 
## Parte B 
O programa cliente deve oferecer os seguintes requisitos funcionais:  
- [x] **RF-6:** Suportar um comando de inserção de objetos no banco de dados. 
- [x] **RF-7:** Suportar um comando de remoção de objetos no banco de dados.  
- [x] **RF-8:** Suportar um comando de atualização objetos no banco de dados.  
- [x] **RF-9:** Suportar um comando de pesquisa objetos no banco de dados. 
 
Além dos requisitos funcionais listados anteriormente, o seu banco de dados deve apresentar os seguintes requisitos não funcionais 
 
- [x] **RFN-2:** Suportar a comunicação bidirecional com o programa cliente usando algum mecanismo de comunicação entre processos.  
- [x] **RFN-3:** Suportar o processamento concorrente de requisições no banco de dados, usando threads. 
 
Quanto ao programa cliente do banco de dados, os seguintes requisitos não funcionais devem ser oferecidos: 
 
- [x] **RFN-7:** Ler as requisições do dispositivo de entrada padrão.  
- [x] **RFN-8:** Escrever a resposta recebida das requisições feitas ao banco de dados no dispositivo de saída padrão. 
 
## Parte C 
 
- [x] **RFN-4:** Manter em memória principal um número máximo de registros do banco de dados. Operações no banco de dados devem ocorrer necessariamente em objetos carregados na memória principal. Devem ser suportadas as seguintes estratégias de substituição de objetos: FIFO, Aging e LRU.