# 🤔 Como habilitar o MSMQ
O *Message Queue da Microsoft (MSMQ)* é o sistema de comunicação entre processos utilizado no projeto. Ele que possibilita que o Cliente e o Banco de Dados comuniquem entre si, mesmo sendo processos diferentes. 

Atualmente o MSMQ não vem habilitado como padrão, então temos que habilitar manualmente. Para isso, siga os seguintes passos:

1. Abra o  **Painel de Controle**.
2. Vá para **Programas e Recursos**.
3. Clique em **Ativar e desativar Recursos do Windows**.
4. Procure por **Servidor do MSMQ (Microsoft Message Queue)** e o habilite.
5. Clique em OK.

Após concluir estes passos o Message Queue estará disponível, portanto a comunicação entre processos do projeto estará funcionando!
